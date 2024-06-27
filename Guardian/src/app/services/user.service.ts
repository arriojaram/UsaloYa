import { Injectable } from '@angular/core';
import { NavigationService } from './navigation.service';
import { Observable, catchError } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../environments/enviroment';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private httpOptions;
  constructor(
    private navigation: NavigationService,
    private http: HttpClient
  ) 
  { 
    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': environment.apiToken
      })
    };
  }

  getUser() : Observable<any> {
    const userId = Number(localStorage.getItem('userid')?? 0);
    if(userId > 0)
    {
      
      const url = this.navigation.apiBaseUrl + 'api/User/GetUser?userId=' + userId;
    
      return this.http.get<any>(url, this.httpOptions).pipe(
        catchError(error => {
          console.error('getUser() | ', error);
          throw error;
        })
      ); 
    }
    else
    {
      throw new Error("Usuario no valido");
    }
  }

}
