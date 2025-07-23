import { HttpClient, HttpHeaders ,HttpBackend} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../environments/enviroment';
import { Observable, catchError } from 'rxjs';
import { companyDto } from '../dto/companyDto';
import { rentRequestDto } from '../dto/rentRequestDto';
import { setStatusDto } from '../dto/setStatusDto';
import { AdminCompanyDto } from '../dto/adminCompanyDto';
import { companySettingsDto, pairSettingsDto } from '../dto/companySettingsDto';
import { setValueDto } from '../dto/setValueDto';

@Injectable({
  providedIn: 'root'
})
export class CompanyService {
  
  private rawHttp: HttpClient;

  private baseUrl = environment.apiUrlBase + '/api/Company';
  selectedCompanyId: number = 0;
  
  constructor(
    private http: HttpClient,
    private httpBackend: HttpBackend
  ) 
  { 
    this.rawHttp = new HttpClient(httpBackend);
  }

  addCompanyRent(c: rentRequestDto): Observable<number> {
    const apiUrl = `${this.baseUrl}/AddRent`;

    return this.http.post<number>(apiUrl, c).pipe(
      catchError(error => {
        console.error('addCompanyRent() | ', error);
        throw error;
      })
    );
  }

  setCompanySettings(s: companySettingsDto): Observable<number> {
    const apiUrl = `${this.baseUrl}/SetSettings`;

    return this.http.post<number>(apiUrl, s).pipe(
      catchError(error => {
        console.error('setCompanyStatus() | ', error);
        throw error;
      })
    );
  }
  
  getCompanySettings(companyId: number): Observable<pairSettingsDto[]> {
    const apiUrl = `${this.baseUrl}/GetSettings?companyId=${companyId}`;

    return this.http.get<pairSettingsDto[]>(apiUrl).pipe(
      catchError(error => {
        console.error('getCompanySettins() | ', error);
        throw error;
      })
    );
  }

  setCompanyStatus(c: setStatusDto): Observable<number> {
    const apiUrl = `${this.baseUrl}/SetCompanyStatus`;

    return this.http.post<number>(apiUrl, c).pipe(
      catchError(error => {
        console.error('setCompanyStatus() | ', error);
        throw error;
      })
    );
  }

  setCompanyLicense(c: setValueDto): Observable<number> {
    const apiUrl = `${this.baseUrl}/SetCompanyLicense`;

    return this.http.post<number>(apiUrl, c).pipe(
      catchError(error => {
        console.error('setCompanyLicense() | ', error);
        throw error;
      })
    );
  }

  saveCompany(c: companyDto): Observable<companyDto> {
    const apiUrl = `${this.baseUrl}/SaveCompany`;

    return this.http.post<companyDto>(apiUrl, c).pipe(
      catchError(error => {
        console.error('SaveCompany() | ', error);
        throw error;
      })
    );
  }

  getCompany(companyId: number): Observable<companyDto> {
    const apiUrl = `${this.baseUrl}/GetCompany?companyId=${companyId}`;
    
    return this.http.get<companyDto>(apiUrl).pipe(
      catchError(error => {
        console.error('getCompany() | ', error);
        throw error;
      })
    );
  }

  getPaymentHistory(companyId: number): Observable<rentRequestDto[]> {
    const apiUrl =`${this.baseUrl}/GetPaymentHistory?companyId=${companyId}`;
    
    return this.http.get<rentRequestDto[]>(apiUrl).pipe(
      catchError(error => {
        console.error('getPaymentHistory() | ', error);
        throw error;
      })
    );
  }

  getAll4List(companyId: number, name:string): Observable<AdminCompanyDto[]> {
      const apiUrl =`${this.baseUrl}/GetAll4List?name=${name}&companyId=${companyId}`;
      
      return this.http.get<AdminCompanyDto[]>(apiUrl).pipe(
        catchError(error => {
          console.error('getCompanies() | ', error);
          throw error;
        })
      );
    }
  
 checkCompanyUnique(name: string): Observable<boolean> {
  const apiUrl = `${this.baseUrl}/IsCompanyUnique`;
  return this.rawHttp.post<boolean>(apiUrl, JSON.stringify(name), {
    headers: { 'Content-Type': 'application/json' }
  }).pipe(
    catchError(error => {
      console.error('checkCompanyUnique() | ', error);
      throw error;
    })
  );
}


}
