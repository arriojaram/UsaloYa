import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../environments/enviroment';
import { Observable, catchError } from 'rxjs';
import { companyDto } from '../dto/companyDto';

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

  getAllFull(): Observable<companyDto[]> {
    const apiUrl =`${this.baseUrl}/GetAll`;
    return this.http.get<companyDto[]>(apiUrl, this.httpOptions).pipe(
      catchError(error => {
        console.error('GetAll() | ', error);
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
