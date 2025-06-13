import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../environments/enviroment';
import { Observable, catchError } from 'rxjs';
import { licenseDto } from '../dto/licenseDto';

@Injectable({
  providedIn: 'root'
})
export class GeneralService {

  private baseUrl = environment.apiUrlBase + '/api/General';
    
  constructor(
    private http: HttpClient
  ) 
  { }

  getLicenses(): Observable<licenseDto[]> {
      const apiUrl =`${this.baseUrl}/GetLicenses`;
      
      return this.http.get<licenseDto[]>(apiUrl).pipe(
        catchError(error => {
          console.error('getLicenses() | ', error);
          throw error;
        })
      );
    }
  

}
