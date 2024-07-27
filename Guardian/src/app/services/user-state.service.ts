import { Injectable, OnInit } from '@angular/core';
import { NavigationService } from './navigation.service';
import { BehaviorSubject, Observable, catchError } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../environments/enviroment';
import { userDto } from '../dto/userDto';
import { AuthorizationService } from './authorization.service';

@Injectable({
  providedIn: 'root'
})
export class UserStateService {
  private httpOptions;

  constructor(
    private navigation: NavigationService,
    private http: HttpClient,
    private authorizationService: AuthorizationService
  ) 
  { 
    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': environment.apiToken
      })
    };    
  }

  setUserState(userInfo: userDto)
  {
    this.navigation.setItemWithExpiry('userState', JSON.stringify(userInfo));  
  }

  getUserState() : userDto
  {
    const userInfo = this.navigation.getItemWithExpiry('userState');
    if(userInfo)
    {
      const userState: userDto = JSON.parse(userInfo);
      return userState;
    }
    else
    {
      console.error('No se puede cargar la información del usuario. Usuario no encontrado');
      this.authorizationService.logout();

      throw new Error("Usuario invalido");
    }
  }

  loadUser(userId: number) : Observable<any> {
    if(userId > 0)
    {
      const url = this.navigation.apiBaseUrl + '/api/User/GetUser?userId=' + userId;
    
      return this.http.get<any>(url, this.httpOptions).pipe(
        catchError(error => {
          console.error('getUser() | ', error);
          throw error;
        })
      ); 
    }
    else
    {
      throw new Error("Usuario no econtrado, revisa tus datos e intenta nuevamente.");
    }
  }

}
