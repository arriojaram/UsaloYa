import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../environments/enviroment';
import { Observable, catchError } from 'rxjs';
import { companyDto } from '../dto/companyDto';
import { rentRequestDto } from '../dto/rentRequestDto';
import { setStatusDto } from '../dto/setStatusDto';

@Injectable({
  providedIn: 'root'
})
export class CompanyService {

  private baseUrl = environment.apiUrlBase + '/api/Company';
  private httpOptions;

  constructor(
    private http: HttpClient
  ) 
  {
    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': environment.apiToken,
      })
    };
  }


  addCompanyRent(c: rentRequestDto): Observable<number> {
    const apiUrl = `${this.baseUrl}/AddRent`;

    return this.http.post<number>(apiUrl, c, this.httpOptions).pipe(
      catchError(error => {
        console.error('addCompanyRent() | ', error);
        throw error;
      })
    );
  }

  setCompanyStatus(c: setStatusDto): Observable<number> {
    const apiUrl = `${this.baseUrl}/SetCompanyStatus`;

    return this.http.post<number>(apiUrl, c, this.httpOptions).pipe(
      catchError(error => {
        console.error('setCompanyStatus() | ', error);
        throw error;
      })
    );
  }

  saveCompany(c: companyDto): Observable<companyDto> {
    const apiUrl = `${this.baseUrl}/SaveCompany`;

    return this.http.post<companyDto>(apiUrl, c, this.httpOptions).pipe(
      catchError(error => {
        console.error('SaveCompany() | ', error);
        throw error;
      })
    );
  }

  getCompany(companyId: number): Observable<companyDto> {
    const apiUrl = `${this.baseUrl}/GetCompany?companyId=${companyId}`;
    
    return this.http.get<companyDto>(apiUrl, this.httpOptions).pipe(
      catchError(error => {
        console.error('getCompany() | ', error);
        throw error;
      })
    );
  }

  getPaymentHistory(companyId: number): Observable<rentRequestDto[]> {
    const apiUrl =`${this.baseUrl}/GetPaymentHistory?companyId=${companyId}`;
    
    return this.http.get<rentRequestDto[]>(apiUrl, this.httpOptions).pipe(
      catchError(error => {
        console.error('getPaymentHistory() | ', error);
        throw error;
      })
    );
  }

  getAll4List(): Observable<companyDto[]> {
    const apiUrl =`${this.baseUrl}/GetAll4List`;
    return this.http.get<companyDto[]>(apiUrl, this.httpOptions).pipe(
      catchError(error => {
        console.error('GetAll4List() | ', error);
        throw error;
      })
    );
  }
}
