import { Injectable } from '@angular/core';
import { environment } from '../environments/enviroment';
import {  MatSnackBar  } from '@angular/material/snack-bar'
import { userStateDto } from '../dto/userDto';


@Injectable({
  providedIn: 'root'
})
export class NavigationService {
  
  apiBaseUrl: string = '';
  userState?: userStateDto = undefined;
  
  constructor(
    private _snackBar: MatSnackBar
  ) 
  {
    this.apiBaseUrl = environment.apiUrlBase;
    if(this.apiBaseUrl.endsWith('/'))
    {
      this.apiBaseUrl = this.apiBaseUrl.substring(0, this.apiBaseUrl.length-1);
    }
  }

  setItemWithExpiry(key: string, value: string): void {
    const now = new Date();
    const item = {
        value: value,
        expiry: now.getTime() + (environment.sessionDurationMinutes * 60 * 1000) // as miliseconds
    };

    localStorage.setItem(key, JSON.stringify(item));
  }

  getItemWithExpiry(key: string): string | null {
    const itemStr = localStorage.getItem(key);

    if (!itemStr) {
        return null;
    }

    const item = JSON.parse(itemStr);
    const now = new Date();

    if (now.getTime() > item.expiry) {
        // El ítem ha expirado, removemos del localStorage
        localStorage.removeItem(key);
        return null;
    }

    return item.value;
  }

  showUIMessage(message: string)
  {
    this._snackBar.open(message, 'X', {
      duration: 3000
      //panelClass: ["custom-snackbar"]
    });
  }
}
