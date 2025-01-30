import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../environments/enviroment';
import { catchError, Observable } from 'rxjs';
import { ProductSaleDetailReport, SaleDetailReport } from '../dto/sale-detail-report';

@Injectable({
  providedIn: 'root'
})
export class ReportsService {
  private baseUrl = environment.apiUrlBase + '/api/Report';

  constructor(private httpClient: HttpClient) 
  { 

  }

  getSales(fromDate: string, toDate: string, companyId: number, userId: number): Observable<SaleDetailReport[]> {
    const apiUrl =`${this.baseUrl}/GetSalesReport?fromDate=${fromDate}&toDate=${toDate}&companyId=${companyId}&userId=${userId}`;

    return this.httpClient.get<SaleDetailReport[]>(apiUrl).pipe(
      catchError(error => {
        console.error('getSales() | ', error);
        throw error;
      })
    );
  }

  getProductSalesDetails(saleId: number, companyId: number): Observable<ProductSaleDetailReport[]> {
    const apiUrl =`${this.baseUrl}/GetSaleDetails?companyId=${companyId}&saleId=${saleId}`;

    return this.httpClient.get<ProductSaleDetailReport[]>(apiUrl).pipe(
      catchError(error => {
        console.error('getProductSalesDetails() | ', error);
        throw error;
      })
    );
  }
}
