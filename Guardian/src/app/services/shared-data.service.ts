import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SharedDataService {
  private email: string = '';
  private userId: number | null = null;

  // Email
  setEmail(email: string): void {
    this.email = email;
  }

  getEmail(): string {
    return this.email;
  }

  clearEmail(): void {
    this.email = '';
  }

  // User ID
  setUserId(id: number): void {
    this.userId = id;
  }

  getUserId(): number | null {
    return this.userId;
  }

  clearUserId(): void {
    this.userId = null;
  }

  // Limpia todo
  clear(): void {
    this.email = '';
    this.userId = null;
  }
}
