import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from '../environments/enviroment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, catchError, map } from 'rxjs';
import { NavigationService } from './navigation.service';

@Injectable({
  providedIn: 'root'
})
export class AuthServiceService {

  constructor(
    private http: HttpClient,
    private router: Router,
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
        
        const parsedObject = JSON.parse(JSON.stringify(data));
        const username = parsedObject.username;

        localStorage.setItem('isAuthenticated', 'true');  
        localStorage.setItem('username', username);
        localStorage.setItem('userid', response);
        
      }), 
      catchError(error => {
        this.clearStorageVariables();
        console.error('login() | ', error);
        return error;
      })
    ); 
  }

  logout(): void {
    this.clearStorageVariables();
    this.router.navigate(['/login']);
  }

  checkAuthentication(): boolean {
    return localStorage.getItem('isAuthenticated') === 'true';
  }

  clearStorageVariables()
  {
    localStorage.removeItem('isAuthenticated');  
    localStorage.removeItem('username');
    localStorage.removeItem('userid');
  }
}
