import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../environments/enviroment';
import { customerDto } from '../dto/customerDto';
import { catchError, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {

  private baseUrl = environment.apiUrlBase + '/api/Customer';

  constructor(
    private http: HttpClient
  ) 
  {

  }

  saveCustomer(customer: customerDto): Observable<customerDto> {
    const apiUrl = `${this.baseUrl}/SaveCustomer`;

    return this.http.post<customerDto>(apiUrl, customer).pipe(
      catchError(error => {
        console.error('saveCustomer() | ', error);
        throw error;
      })
    );
  }

  getCustomer(customerId: number): Observable<customerDto> {
    const apiUrl = `${this.baseUrl}/GetCustomer?customerId=${customerId}`;
    
    return this.http.get<customerDto>(apiUrl).pipe(
      catchError(error => {
        console.error('getCustomer() | ', error);
        throw error;
      })
    );
  }

  getAllCustomer(companyId: number, nameOrEmailOrPhone:string): Observable<customerDto[]> {
    
    const apiUrl =`${this.baseUrl}/GetAll?nameOrPhoneorEmail=${nameOrEmailOrPhone}&companyId=${companyId}`;
    
    return this.http.get<customerDto[]>(apiUrl).pipe(
      catchError(error => {
        console.error('getAllCustomer() | ', error);
        throw error;
      })
    );
  }
}
