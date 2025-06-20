import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpResponse } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { LoadingService } from '../services/loading.service';
import { UserStateService } from '../services/user-state.service';
import { NavigationService } from '../services/navigation.service';
import { environment } from '../environments/enviroment';
import { AuthorizationService } from '../services/authorization.service';
import { Router } from '@angular/router';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
  constructor(
     private router: Router,
    private loadingService: LoadingService,
    private userService: UserStateService,
    private navigationService: NavigationService,
    private authService: AuthorizationService
  ) {}


  getUserId(): number
  {
    try
    {
      let username = this.userService.getUserStateLocalStorage().userId;
      return  username
    }
    catch(e)
    {
      return 0;
    }
  }

  getDeviceId(): string
  {
    try
    {
      let deviceId = this.navigationService.getItemWithExpiry('deviceId')?? '';
      return  deviceId
    }
    catch(e)
    {
      return "";
    }
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.loadingService.show();
     
    if(this.shouldSkip(req))
    {
      return next.handle(req).pipe(
        finalize(() => {
          this.loadingService.hide();
        })
      );
    }
    else
    {
      const clonedRequest = req.clone({
          setHeaders: {
            RequestorId: this.getUserId().toString() || '', // Asegúrate de manejar posibles valores nulos o indefinidos
            DeviceId: this.getDeviceId(),
            Authorization: environment.apiToken,
           
          }
        });
        
        return next.handle(clonedRequest).pipe(
          tap((event) => {
            
          }),
          catchError((e) => {
           
            var msg = "Esta sesión ha sido cerrada porque ha caducado o se inicio sesión en otro dispositivo.";
            if (e.status === 401 && (e.error === '$_Duplicated_Session' || e.error === 'Dispositivo no reconocido.')) {
              this.authService.clearStorageVariables();
              this.router.navigate(['/login']);
              e.error = msg;
              e.message = msg;
              return throwError(() => e);
            }
            if(this.getUserId() === 0 || this.getDeviceId() === '' )
            {
              
              this.authService.clearStorageVariables();
              this.router.navigate(['/login']);
             
            }
            // Propagar otros errores
            return throwError(() => e);
          }),
          finalize(() => {
            this.loadingService.hide();
          })
        );
    }
  }

  private shouldSkip(request: HttpRequest<any>): boolean {
    
    let shouldSkip = false;
    if(request.url.includes('i=login'))
      shouldSkip = true;
    else if(request.url.includes('ping.json'))
      shouldSkip = true;

    return shouldSkip;
  }
}