import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router, RouterLink, RouterModule, RouterOutlet } from '@angular/router';
import { ConnectionService } from 'ngx-connection-service';
import { BehaviorSubject, Observable, Subject, catchError, debounceTime, filter, from, mergeMap, takeUntil, throwError } from 'rxjs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { LoadingService } from './services/loading.service';
import { CommonModule, NgIf } from '@angular/common';
import { SaleService } from './services/sale.service';
import { OfflineDbStore } from './services/offline-db-store.service';
import { UserStateService } from './services/userState.service';
import { userStateDto } from './dto/userDto';
import { AuthorizationService } from './services/authorization.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterModule, RouterOutlet, RouterLink, CommonModule, MatProgressSpinnerModule, NgIf ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit, OnDestroy {

  hasNetworkConnection: boolean = true;
  hasInternetAccess: boolean = true;
  appOnlineStatus: string = "OFFLINE";
  userStateUI: userStateDto;

  private unsubscribe$: Subject<void> = new Subject();
  loading_i$: Observable<boolean>;
  currentPath: string = "/";

  private migrating: boolean;
  private LOGGED_OUT: number = 0;
  LOGGED_IN: number = 1;

  constructor(
    private connectionService: ConnectionService,
    private loadingService: LoadingService,
    private salesService: SaleService,
    private offlineDbService: OfflineDbStore,
    private userStateService: UserStateService,
    private authService: AuthorizationService,
    private router: Router, 
    private activatedRoute: ActivatedRoute
  ) 
  {
    this.loading_i$ = this.loadingService.loading$;
    this.migrating = false;
    this.userStateUI = {userId:0, companyId:0, groupId:0, statusId:0};

    this.connectionService.monitor()
      .pipe(debounceTime(5000), takeUntil(this.unsubscribe$))
      .subscribe(currentState => {
        this.hasNetworkConnection = currentState.hasNetworkConnection;
        this.hasInternetAccess = currentState.hasInternetAccess;
        let isOnline = this.hasNetworkConnection && this.hasInternetAccess; 
        this.appOnlineStatus = isOnline ? 'ONLINE' : 'OFFLINE';
                
        if(isOnline && this.migrating == false){
          try{
            this.migrateSales();
          }
          catch(e)
          {
            console.error("Error en la migración de ventas al servidor.");
          }
        }          
      });
  }

  ngOnInit(): void {
    console.log("On App init");
   
    
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.currentPath = this.getCurrentRoute(this.activatedRoute);
      switch (this.currentPath) {
        case "/":
          this.currentPath = "Bienvenido";
          break;
        default:
          console.log("Set UserState");
          this.setUserDetailsUI();
          this.currentPath = "";
          break;
      }
    });
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

  title = 'Guardian';
  
  public logoutApp()
  {
    this.authService.logout();
    //Manage UI
    this.userStateUI.statusId = this.LOGGED_OUT;
  }

  setUserDetailsUI()
  {
    try
    {
      this.userStateUI = this.userStateService.getUserState();
      this.userStateUI.statusId = this.LOGGED_IN;
    }
    catch(e)
    {
      //TODO: not sure what to do here currently, the getUserState Exception is logged
    }
  }

  migrateSales() {
    this.migrating = true;
    this.offlineDbService.GetSales().subscribe({
      next: (sales) => {
        
        if (sales.length > 0) {
          for (let i = 0; i < sales.length; i++) {
            const sale1 = sales[i];
            this.salesService.completeTemporalSale(sale1).subscribe({
              next:(completed) => {
                this.offlineDbService.DeleteSale(sale1.id).subscribe({
                  next(value) {
                    console.log("Sale migrated " + sale1);
                  },
                  error(err) {
                    console.error("Error al eliminar la venta" + sale1);    
                  },
                });
              },
              error:(err) => {
                console.error("Error al migrar la venta" + sale1);
              },
            });
            console.log(sale1);
          }
        }
      },
      complete:() => {
        this.migrating = false;
      },
      error: (err) => 
      {
        this.migrating = false;
        console.error('Failed migration:', err)
      }
    });
  }

}

