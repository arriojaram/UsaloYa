import { Injectable } from '@angular/core';
import { environment } from '../environments/enviroment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, catchError, map } from 'rxjs';
import { NavigationService } from './navigation.service';
import { TokenDto } from '../dto/set-token';
@Injectable({
  providedIn: 'root'
})
export class AuthorizationService {

  constructor(
    private http: HttpClient,
    private navigation: NavigationService
  ) {}

  login(data: string) : Observable<any> {
    const url = this.navigation.apiBaseUrl + '/api/User/Validate';
    const httpOptions = {
      headers: new HttpHeaders({
        'Authorization': environment.apiToken
      })
    };

    return this.http.post<any>(url, data, httpOptions).pipe(
      map(response => { 
        this.navigation.setItemWithExpiry('isAuthenticated', 'true'); 
        return response; 
      }), 
      catchError(error => {
        this.clearStorageVariables();
        console.error('login(E) | ', error);
        throw error;
      })
    ); 
  }

  logout(userName: string): Observable<any> {
    const url = this.navigation.apiBaseUrl + '/api/User/LogOut';
    const httpOptions = {
      headers: new HttpHeaders({
        'Authorization': environment.apiToken
      })
    };
    const setTokenData : TokenDto = {userName:userName, token:'', userId:0};
    return this.http.post<any>(url, setTokenData, httpOptions).pipe(
      map(response => { 
        this.navigation.setItemWithExpiry('isAuthenticated', 'false'); 
        return response;
      }), 
      catchError(error => {
        console.error('logout() | ', error);
        this.clearStorageVariables();
        throw error;
      })
    ); 
  }

  checkAuthentication(): boolean {
    let authenticationValue = this.navigation.getItemWithExpiry('isAuthenticated');
    let isAuthenticated = false;

    if(authenticationValue)
      isAuthenticated = this.navigation.getItemWithExpiry('isAuthenticated') === 'true';
    
    if(!isAuthenticated)
      this.navigation.showUIMessage("Sesión expirada, vuelve a iniciar sesión en la aplicación.");
    
    return isAuthenticated;
  }

  clearStorageVariables()
  {
    localStorage.removeItem('isAuthenticated');  
    localStorage.removeItem('userState');
  }
}
