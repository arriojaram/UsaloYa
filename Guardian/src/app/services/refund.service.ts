import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../environments/enviroment';
import { catchError, Observable, throwError } from 'rxjs';
import { RequestRefundDto } from '../dto/requestRefundDto';

@Injectable({
  providedIn: 'root'
})
export class RefundService {
  private baseUrl = environment.apiUrlBase + '/api/Refund'; 

  constructor(private http: HttpClient) {}

  manageRefund(refundDto: RequestRefundDto, companyId: number, userId: number): Observable<boolean> {
    return this.http.post<boolean>(
      `${this.baseUrl}/ManageRefund?companyId=${companyId}`,
      refundDto
    ).pipe(
      catchError(err => {
        console.error('Error en manageRefund:', err);
        return throwError(() => err);
      })
    );
  }
}

