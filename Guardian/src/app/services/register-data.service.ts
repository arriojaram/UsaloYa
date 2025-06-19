import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { RequestRegisterNewUserDto } from '../dto/RequestRegisterNewUserDto ';
import { companyDto } from '../dto/companyDto';

@Injectable({
  providedIn: 'root',
})
export class RegisterDataService {
  private userDataSource = new BehaviorSubject<RequestRegisterNewUserDto | null>(null);
  userData$ = this.userDataSource.asObservable();

  setUserData(data: RequestRegisterNewUserDto | null) {
    this.userDataSource.next(data);
  }

  getUserData(): RequestRegisterNewUserDto | null {
    return this.userDataSource.value;
  }

  private companyDataSource = new BehaviorSubject<companyDto | null>(null);

  setCompanyData(data: companyDto | null) {
    this.companyDataSource.next(data);
  }

  getCompanyData(): companyDto | null {
    return this.companyDataSource.value;
  }   
}
