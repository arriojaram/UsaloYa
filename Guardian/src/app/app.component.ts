import { Component, OnDestroy, OnInit } from '@angular/core';
import { RouterLink, RouterModule, RouterOutlet } from '@angular/router';
import { ConnectionService } from 'ngx-connection-service';
import { Observable, Subject, debounceTime, takeUntil } from 'rxjs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { LoadingService } from './services/loading.service';
import { CommonModule, NgIf } from '@angular/common';
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
  loading$: Observable<boolean>;

  constructor(
    private connectionService: ConnectionService,
    private loadingService: LoadingService
  ) {
    this.loading$ = this.loadingService.loading$;

    this.connectionService.monitor()
      .pipe(debounceTime(10000), takeUntil(this.unsubscribe$))
      .subscribe(currentState => {
        this.hasNetworkConnection = currentState.hasNetworkConnection;
        this.hasInternetAccess = currentState.hasInternetAccess;
        this.status = this.hasNetworkConnection && this.hasInternetAccess ? 'ONLINE' : 'OFFLINE';
        console.log(`${this.status} - ${new Date()}`);
      });
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
    console.log(`Unsuscribe - ${this.status} - ${new Date()}`);
  }

  title = 'Guardian';
  
}

