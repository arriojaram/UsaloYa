import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError, Observable } from 'rxjs';
import { userDto as userDto } from '../dto/userDto';
import { environment } from '../environments/enviroment';
import { TokenDto } from '../dto/set-token';
import { adminGroupDto } from '../dto/adminGroupDto';
import { AdminCompanyDto } from '../dto/adminCompanyDto';


@Injectable({
  providedIn: 'root'
})
export class UserService {

  private baseUrl = environment.apiUrlBase + '/api/User';
  private httpOptions;
  constructor(
    private http: HttpClient
  ) 
  {
    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': environment.apiToken
      })
    };
  }

  saveUser(user: userDto): Observable<userDto> {
    const apiUrl = `${this.baseUrl}/SaveUser`;

    return this.http.post<userDto>(apiUrl, user, this.httpOptions).pipe(
      catchError(error => {
        console.error('saveUser() | ', error);
        throw error;
      })
    );
  }

  getUser(userId: number): Observable<userDto> {
    const apiUrl = `${this.baseUrl}/GetUser?i=0&userId=${userId}`;
    
    return this.http.get<userDto>(apiUrl, this.httpOptions).pipe(
      catchError(error => {
        console.error('getUser() | ', error);
        throw error;
      })
    );
  }

  getAllUser(companyId: number, name:string): Observable<userDto[]> {
    const apiUrl =`${this.baseUrl}/GetAll?name=${name}&companyId=${companyId}`;
    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': environment.apiToken
      })
    };
    return this.http.get<userDto[]>(apiUrl, this.httpOptions).pipe(
      catchError(error => {
        console.error('getAllUser() | ', error);
        throw error;
      })
    );
  }

  getCompanies(companyId: number): Observable<AdminCompanyDto[]> {
    const apiUrl =`${this.baseUrl}/GetCompanies?companyId=${companyId}`;
    
    return this.http.get<AdminCompanyDto[]>(apiUrl, this.httpOptions).pipe(
      catchError(error => {
        console.error('getCompanies() | ', error);
        throw error;
      })
    );
  }

  getGroups(): Observable<adminGroupDto[]> {
    const apiUrl =`${this.baseUrl}/GetGroups`;
    
    return this.http.get<adminGroupDto[]>(apiUrl, this.httpOptions).pipe(
      catchError(error => {
        console.error('getGroups() | ', error);
        throw error;
      })
    );
  }

  setPassword(userName: string, password: string): Observable<any> {
    const apiUrl = `${this.baseUrl}/SetToken`;
    let tokenbody: TokenDto = {
      userId: 0,
      userName: userName,
      token: password
    };
    return this.http.post<userDto>(apiUrl, tokenbody, this.httpOptions).pipe(
      catchError(error => {
        console.error('setPassword() | ', error);
        throw error;
      })
    );
  }
}
