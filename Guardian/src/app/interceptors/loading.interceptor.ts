import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { LoadingService } from '../services/loading.service';
import { UserStateService } from '../services/user-state.service';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
  constructor(
    private loadingService: LoadingService,
    private userService: UserStateService,
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

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
     
    const clonedRequest = req.clone({
        setHeaders: {
          RequestorId: this.getUserId().toString() || '' // Asegúrate de manejar posibles valores nulos o indefinidos
        }
      });

      this.loadingService.show();
      
      return next.handle(clonedRequest).pipe(
        finalize(() => {
          this.loadingService.hide();
        })
      );
   
  }
}
