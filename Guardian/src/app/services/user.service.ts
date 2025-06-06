import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable } from 'rxjs';
import { userDto } from '../dto/userDto';
import { environment } from '../environments/enviroment';
import { TokenDto } from '../dto/authenticateDto';
import { adminGroupDto } from '../dto/adminGroupDto';
import { NavigationService } from './navigation.service';
import { RegisterUserAndCompanyDto } from '../dto/RegisterUserAndCompanyDto ';

export interface RequestVerificationCodeDto {
  Email: string;
  Code: string;
}

export interface VerificationResponse {
  isValid: boolean;
  message: string;
  userId: number;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private baseUrl = environment.apiUrlBase + '/api/User';

  constructor(
    private http: HttpClient,
    private navigationService: NavigationService
  ) { }

  saveUser(user: userDto): Observable<userDto> {
    const apiUrl = `${this.baseUrl}/SaveUser`;

    return this.http.post<userDto>(apiUrl, user).pipe(
      catchError(error => {
        console.error('saveUser() | ', error);
        throw error;
      })
    );
  }

  getUser(userId: number): Observable<userDto> {
    const apiUrl = `${this.baseUrl}/GetUser?i=0&userId=${userId}`;

    return this.http.get<userDto>(apiUrl).pipe(
      catchError(error => {
        console.error('getUser() | ', error);
        throw error;
      })
    );
  }

  getAllUser(companyId: number, name: string): Observable<userDto[]> {
    const apiUrl = `${this.baseUrl}/GetAll?name=${name}&companyId=${companyId}`;

    return this.http.get<userDto[]>(apiUrl).pipe(
      catchError(error => {
        console.error('getAllUser() | ', error);
        throw error;
      })
    );
  }

  getGroups(): Observable<adminGroupDto[]> {
    const apiUrl = `${this.baseUrl}/GetGroups`;

    return this.http.get<adminGroupDto[]>(apiUrl).pipe(
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
    return this.http.post<userDto>(apiUrl, tokenbody).pipe(
      catchError(error => {
        console.error('setPassword() | ', error);
        throw error;
      })
    );
  }

  registerNewUser(data: RegisterUserAndCompanyDto): Observable<any> {
    const apiUrl = `${this.baseUrl}/RegisterNewUser`;
    return this.http.post(apiUrl, data).pipe(
      catchError(error => {
        console.error('registerNewUser() | ', error);
        throw error;
      })
    );
  }

requestVerificationCodeEmail(request: RequestVerificationCodeDto): Observable<VerificationResponse> {
  const apiUrl = `${this.baseUrl}/RequestVerificationCodeEmail`;

  return this.http.post<VerificationResponse>(apiUrl, request).pipe(
    catchError(error => {
      console.error('requestVerificationCodeEmail() | ', error);
      throw error;
    })
  );
}


}
