import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { RequestRegisterNewUserDto } from '../dto/RequestRegisterNewUserDto ';

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
}
