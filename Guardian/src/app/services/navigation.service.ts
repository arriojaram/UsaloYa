import { Injectable } from '@angular/core';
import { environment } from '../environments/enviroment';
import { MatSnackBar } from '@angular/material/snack-bar'
import { userDto } from '../dto/userDto';
import { BehaviorSubject } from 'rxjs';
import { CompanyStatus } from '../Enums/enums';


@Injectable({
  providedIn: 'root'
})
export class NavigationService {
  
  apiBaseUrl: string = '';
  private userStateSource = new BehaviorSubject<userDto>({userId:0, userName:'', roleId:0, companyId:0, groupId:0, statusId:0, companyName:""});
  // Crea un observable público para exponer el BehaviorSubject
  public userState$ = this.userStateSource.asObservable();
  isSearchPanelHidden: boolean = false;
  searchItem: string = '';
  companyStatus: CompanyStatus  = CompanyStatus.Inactive;

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

  checkScreenSize() {
    if (window.innerWidth < 768) {
      this.isSearchPanelHidden = true;  // Ocultar búsqueda en pantallas pequeñas por defecto
      this.searchItem = '';
    }
  }

  toggleSearchPanel(): void {
    this.isSearchPanelHidden = !this.isSearchPanelHidden;
  }

  setItemWithExpiry(key: string, value: string): void {
    const now = new Date();
    const item = {
        value: value,
        expiry: now.getTime() + (environment.sessionDurationMinutes * 60 * 1000) // as miliseconds
    };

    localStorage.setItem(key, JSON.stringify(item));
  }

  setUserState(userDto: userDto)
  {
    this.userStateSource.next(userDto);
    this.companyStatus = userDto.companyStatusId as CompanyStatus;
  }

  getItemWithExpiry(key: string, isUserState: boolean = false): string | null {
    const itemStr = localStorage.getItem(key);
   
    if (!itemStr) {
        return null;
    }

    const item = JSON.parse(itemStr);
    const now = new Date();
    if(isUserState)
    {
      const parsedState: userDto = item;
      this.userStateSource.next(parsedState);
    }
    
    if (now.getTime() > item.expiry) {
        // El ítem ha expirado, removemos del localStorage
        localStorage.removeItem(key);
        return null;
    }

    return item.value;
  }

  showUIMessage(message: string)
  {
    this._snackBar.open(message, 'cerrar', {
      duration: environment.notificationsDisplayTimeSeconds
     
    });
  }
}
