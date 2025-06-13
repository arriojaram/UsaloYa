 import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SharedDataService {
  private phone: string = '';
  private address: string = '';
  private email: string = '';
   private userId: number | null = null;

  setUserData(phone: string, address: string, email: string): void {
    this.phone = phone;
    this.address = address;
    this.email = email;

  }

  setUserId(userId: number): void {
    this.userId = userId;
  }
  getUserId():number | null{
    return this.userId;
  }

  getPhone(): string {
    return this.phone;
  }

  getAddress(): string {
    return this.address;
  }

  getEmail(): string {
    return this.email;
  }

  clear(): void {
    this.phone = '';
    this.address = '';
    this.email = '';
    this.userId = null;
  }
}
