import { HttpClient, HttpHeaders } from '@angular/common/http'; // Faltaba importar HttpHeaders
import { Injectable } from '@angular/core';
import { environment } from '../environments/enviroment';
import { catchError, Observable, throwError } from 'rxjs';
import { RequestRefundDto } from '../dto/requestRefundDto';

@Injectable({
  providedIn: 'root'
})
export class RefundService {
  private baseUrl = environment.apiUrlBase + '/api/Refund'; // Cambié Refunds por Refund (singular)
  constructor(private http: HttpClient) { }

  manageRefund(refundDto: RequestRefundDto, companyId: number, userId: number): Observable<boolean> {
    const headers = new HttpHeaders({
      'RequestorId': userId.toString() // userId es un número, convertir a string
    });

    return this.http.post<boolean>(
      `${this.baseUrl}/ManageRefund?companyId=${companyId}`,
      refundDto,
      { headers }
    ).pipe(
      catchError(err => {
        console.error('Error en manageRefund:', err);
        return throwError(() => err);
      })
    );
  }
}
