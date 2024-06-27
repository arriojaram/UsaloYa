import { Injectable } from '@angular/core';
import { environment } from '../environments/enviroment';

@Injectable({
  providedIn: 'root'
})
export class NavigationService {
  
  apiBaseUrl: string = '';
  userId: number = 0;
  userName: string = '';

  constructor() {
    this.apiBaseUrl = environment.apiUrlBase;
    if(this.apiBaseUrl.endsWith('/'))
    {
      this.apiBaseUrl = this.apiBaseUrl.substring(0, this.apiBaseUrl.length-1);
    }
  }

  updateUserInfo()
  {
    this.userName = localStorage.getItem('username')?? "";
    this.userId = Number(localStorage.getItem('userid')?? 0);
  }
}
