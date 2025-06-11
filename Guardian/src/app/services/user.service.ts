import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError,  map, Observable } from 'rxjs';
import { userDto } from '../dto/userDto';
import { environment } from '../environments/enviroment';
import { TokenDto } from '../dto/authenticateDto';
import { adminGroupDto } from '../dto/adminGroupDto';
import { NavigationService } from './navigation.service';
import { RegisterUserAndCompanyDto } from '../dto/RegisterUserAndCompanyDto ';
import { HttpHeaders } from '@angular/common/http';
import { VerificationResponseDto } from '../dto/VerificationResponseDto';

export interface RequestVerificationCodeDto {
  Email: string;
  Code: string;
}

export interface VerificationResponse {
  isValid: boolean;
  message: string;
  userId: number;
}

export interface LoginResponseDto {
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
    private navigationService: NavigationService,
    
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
checkUsernameUnique(username: string): Observable<boolean> {
  const apiUrl = `${this.baseUrl}/IsUsernameUnique`;
  return this.http.post<boolean>(apiUrl, JSON.stringify(username), {
    headers: { 'Content-Type': 'application/json' }
  }).pipe(
    catchError(error => {
      console.error('checkUsernameUnique() | ', error);
      throw error;
    })
  );
}

checkEmailUnique(email: string): Observable<boolean> {
  const apiUrl = `${this.baseUrl}/IsEmailUnique`;
  return this.http.post<boolean>(apiUrl, JSON.stringify(email), {
    headers: { 'Content-Type': 'application/json' }
  }).pipe(
    catchError(error => {
      console.error('checkEmailUnique() | ', error);
      throw error;
    })
  );
}



  

  requestVerificationCode(request: RequestVerificationCodeDto, deviceId: string): Observable<VerificationResponseDto> {
    const apiUrl = `${this.baseUrl}/RequestVerificationCode`;

    const headers = new HttpHeaders({
      'DeviceId': deviceId
    });

    return this.http.post<VerificationResponseDto>(apiUrl, request, { headers }).pipe(
      catchError(error => {
        console.error('requestVerificationCode() | ', error);
        throw error;
      })
    );
  }

 requestVerificationCodeEmail(request: { Code: string; Email: string }, deviceId: string): Observable<VerificationResponseDto> {
  const apiUrl = `${this.baseUrl}/RequestVerificationCodeEmail`;

  const headers = new HttpHeaders({
    'DeviceId': deviceId
  });

  return this.http.post<VerificationResponseDto>(apiUrl, request, { headers }).pipe(
    catchError(error => {
      console.error('requestVerificationCodeEmail() | ', error);
      throw error;
    })
  );
}


}

