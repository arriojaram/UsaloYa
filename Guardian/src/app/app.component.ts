import { Component, OnDestroy, OnInit } from '@angular/core';
import { RouterLink, RouterModule, RouterOutlet } from '@angular/router';
import { ConnectionService } from 'ngx-connection-service';
import { BehaviorSubject, Observable, Subject, catchError, debounceTime, from, mergeMap, takeUntil, throwError } from 'rxjs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { LoadingService } from './services/loading.service';
import { CommonModule, NgIf } from '@angular/common';
import { SaleService } from './services/sale.service';
import { OfflineDbStore } from './services/offline-db-store.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterModule, RouterOutlet, RouterLink, CommonModule, MatProgressSpinnerModule, NgIf ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnDestroy {

  hasNetworkConnection: boolean = true;
  hasInternetAccess: boolean = true;
  status: string = "";

  private unsubscribe$: Subject<void> = new Subject();
  loading_i$: Observable<boolean>;
  
  
  private migrating: boolean;

  constructor(
    private connectionService: ConnectionService,
    private loadingService: LoadingService,
    private salesService: SaleService,
    private offlineDbService: OfflineDbStore
  ) 
  {
    this.loading_i$ = this.loadingService.loading$;
    this.migrating = false;

    this.connectionService.monitor()
      .pipe(debounceTime(5000), takeUntil(this.unsubscribe$))
      .subscribe(currentState => {
        this.hasNetworkConnection = currentState.hasNetworkConnection;
        this.hasInternetAccess = currentState.hasInternetAccess;
        let isOnline = this.hasNetworkConnection && this.hasInternetAccess; 
        this.status = isOnline ? 'ONLINE' : 'OFFLINE';
        console.log(`${this.status} - ${new Date()}`);

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

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
    console.log(`Unsuscribe - ${this.status} - ${new Date()}`);
  }

  title = 'Guardian';
  
  migrateSales() {
    this.migrating = true;
    this.offlineDbService.GetSales().subscribe({
      next: (sales) => {
        console.log("Init Migration");
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
        console.log("Finish Migration");
      },
      error: (err) => 
      {
        this.migrating = false;
        console.error('Failed migration:', err)
      }
    });
  }

  migrateSales22(): Observable<any> {
    console.log("Init Migration");
    return from(this.offlineDbService.GetSales()).pipe(
      mergeMap(sales => from(sales)), 
      mergeMap(sale => this.salesService.completeTemporalSale(sale).pipe(
          mergeMap(response => {
            console.log("Migrated " + JSON.stringify(response));
            if (response && response.saleId) {
              return from(this.offlineDbService.DeleteSale(response.saleId));
            } else {
              return throwError(() => new Error('Sale migration failed: No response SaleId'));
            }
          })
        )
      ),
      catchError(error => {
        console.error('Error en la migración de ventas:', error);
        return throwError(() => error); // Propagamos el error
      })
    );
  }
}

