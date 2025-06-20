import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router, RouterModule, RouterOutlet } from '@angular/router';
import { ConnectionService } from 'ngx-connection-service';
import { BehaviorSubject, Observable, Subject, catchError, debounceTime, filter, finalize, first, forkJoin, interval, of, switchMap, takeUntil } from 'rxjs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { LoadingService } from './services/loading.service';
import { CommonModule, NgIf } from '@angular/common';
import { SaleService } from './services/sale.service';
import { OfflineDbStore } from './services/offline-db-store.service';
import { UserStateService } from './services/user-state.service';
import { userDto } from './dto/userDto';
import { AuthorizationService } from './services/authorization.service';
import { NavigationService } from './services/navigation.service';
import { Sale } from './dto/sale';
import { AlertLevel, CompanyStatus, Roles, UserStatus } from './Enums/enums';
import { environment } from './environments/enviroment';

@Component({
  selector: 'app-root',
  imports: [RouterModule, RouterOutlet, CommonModule, MatProgressSpinnerModule, NgIf],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit, OnDestroy {

  hasNetworkConnection: boolean | undefined;
  hasInternetAccess: boolean | undefined;
  appOnlineStatus: string = "ONLINE";
  userStateUI: userDto | undefined;
  isOnline: boolean | undefined;
  title = 'Guardian';
  rol = Roles;
  cStatus = CompanyStatus;
  uStatus = UserStatus;
  userRol: string | undefined;
  whatsAppUrl = environment.whatsNumber
  paymentMsg: string = "";

  private unsubscribe$: Subject<void> = new Subject();
  loading_i$: Observable<boolean> = new BehaviorSubject<boolean>(false);
  currentPath: string = "/";
  environmentClass: string = environment.production ? "bg-success" : "bg-primary";

  public showPaymentAlert: boolean = false;

  constructor(
    private connectionService: ConnectionService,
    private loadingService: LoadingService,
    private salesService: SaleService,
    private offlineDbService: OfflineDbStore,
    private userStateService: UserStateService,
    private authService: AuthorizationService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    public navigationService: NavigationService
  ) {

  }

  ngOnInit(): void {

    setTimeout(() => {
      this.loading_i$ = this.loadingService.loading$;
    }, 10);

    this.userStateUI = { userId: 0, userName: '', roleId: 0, companyId: 0, groupId: 0, statusId: 0, companyName: "" };

    // Init network status monitor
    this.connectionService.monitor()
      .pipe(debounceTime(3000), takeUntil(this.unsubscribe$))
      .subscribe(currentState => {
        this.hasNetworkConnection = currentState.hasNetworkConnection;
        this.hasInternetAccess = currentState.hasInternetAccess;
        this.isOnline = this.hasNetworkConnection && this.hasInternetAccess;
        this.appOnlineStatus = this.isOnline ? 'ONLINE' : 'OFFLINE';
        this.salesService.isOnline = this.isOnline;
      });

    // Init the navigation end to manage the UI, introduced to check user inactivity
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd), takeUntil(this.unsubscribe$)
    ).subscribe(() => {
      this.currentPath = this.getCurrentRoute(this.activatedRoute);

      switch (this.currentPath) {
        case "/":
          this.currentPath = "Bienvenido";
          break;
        case "policy":
        case "agreements":
        case "register":
          case"verification":
          case"register-company":
          case"questions":
          case"forms-navigator":
          this.currentPath = "";
          break;
        default:
          this.setUserDetailsUI();
          this.currentPath = "";
          break;
      }
    });

    this.initMigrationService();
  }

  private getCurrentRoute(route: ActivatedRoute): string {
    while (route.firstChild) {
      route = route.firstChild;
    }
    return route.snapshot.routeConfig ? route.snapshot.routeConfig.path ?? '/' : '/';
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
    console.log(`Unsuscribe - ${this.appOnlineStatus} - ${new Date()}`);
  }

  public logoutApp(event: MouseEvent) {
    if (this.userStateUI) {
      event.preventDefault(); // Detiene la navegación

      this.authService.logout(this.userStateUI.userName).pipe(first()).subscribe({
        next: (value) => {
          this.manageLogoutUI();
        },
        error: (e) => {
          this.navigationService.showUIMessage(e.error);
          this.manageLogoutUI();
          console.log('logout error', e);
        }
      });
    }
  }

  manageLogoutUI() {
    if (this.userStateUI) {

      this.userStateUI.statusId = this.uStatus.Desconectado;
      this.userStateUI.userId = 0;
      this.authService.clearStorageVariables();
      this.router.navigate(['/login']);
    }
  }


  setUserDetailsUI() {
    try {

      var storedUserInfo = this.userStateService.getUserStateLocalStorage();
      this.userStateUI = storedUserInfo;
      this.showPaymentAlert = false;

      if (this.userStateUI.companyStatusId == this.cStatus.Expired) {
        this.paymentMsg = environment.paymentExpiredMsg;
        this.showPaymentAlert = true;
      }
      else if (this.userStateUI.companyStatusId == this.cStatus.PendingPayment) {
        this.paymentMsg = environment.paymentPendingMsg;
        this.showPaymentAlert = true;
      }

      this.userRol = Roles[this.userStateUI.roleId];
    }
    catch (e) {
      if ((e as Error).message === '$Invalid_User')
        console.log('/SigIn');
      else
        console.error('setUserDetailsUI()', e);
    }
  }

  closeAlert() {
    this.showPaymentAlert = false; // Función para cerrar la alerta
  }

  initMigrationService() {
    interval(1000 * 60) // every 1 minute
      .pipe(
        switchMap(() => this.offlineDbService.GetSales()),
        switchMap(sales => {
          console.log('Migration service running');

          if (sales.length > 0 && this.isOnline) {
            this.navigationService.showUIMessage("Sincronizando ventas con el servidor", AlertLevel.Info);
            return this.processSales(sales);
          } else {
            return of(null); // No sales to process, just a placeholder to keep the stream alive
          }

        }),
        catchError(err => {
          console.error('Migración fallida:', err);
          return of(null); // Handle error but continue the stream
        }),
        finalize(() => {
          console.log("El proceso de migración ha terminado.");
        }),
        takeUntil(this.unsubscribe$) // This will allow us to stop the migration when needed
      )
      .subscribe();
  }

  private processSales(sales: Sale[]) {
    const tasks = sales.map(sale => this.completeAndDeleteSale(sale));
    return forkJoin(tasks); // Execute all tasks concurrently and wait for all of them to complete
  }

  private completeAndDeleteSale(sale: Sale) {

    return this.salesService.completeTemporalSale(sale).pipe(
      switchMap(completed => this.offlineDbService.DeleteSale(sale.id ?? 0)),
      catchError(err => {
        console.error('Error al migrar la venta', sale, err);
        return of(null); // Continue processing other sales even if one fails
      })
    );
  }


}

