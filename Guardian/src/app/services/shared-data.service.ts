import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SharedDataService {
  private email: string = '';
 

  // Guarda solo el email
  setEmail(email: string): void {
    this.email = email;
  }

  getEmail(): string {
    return this.email;
  }


  clear(): void {
    this.email = '';
  }
}
