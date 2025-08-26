import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../environments/enviroment';
import { catchError, Observable } from 'rxjs';
import { ProductSaleDetailReport, SaleDetailReport } from '../dto/sale-detail-report';
import { RefundReportDto } from '../dto/refundReportDto';
import {CompanyReportDto} from '../dto/CompanyReportDto';

@Injectable({
  providedIn: 'root'
})
export class ReportsService {
  private baseUrl = environment.apiUrlBase + '/api/Report';

  constructor(private httpClient: HttpClient) {}

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

  getRefundsReport(fromDate: string, toDate: string, companyId: number): Observable<RefundReportDto[]> {
    const apiUrl = `${this.baseUrl}/GetRefundsReport?fromDate=${fromDate}&toDate=${toDate}&companyId=${companyId}`;
    return this.httpClient.get<RefundReportDto[]>(apiUrl).pipe(
      catchError(error => {
        console.error('getRefundsReport() | ', error);
        throw error;
      })
    );
  }

  getRefundDetails(saleId: number, companyId: number): Observable<RefundReportDto> {
    const apiUrl = `${this.baseUrl}/GetRefundDetails?saleId=${saleId}&companyId=${companyId}`;
    return this.httpClient.get<RefundReportDto>(apiUrl).pipe(
      catchError(error => {
        console.error('getRefundDetails() | ', error);
        throw error;
      })
    );
  }

  getCompanyReport( companyId: number, inactiveDays: number, status: number, company: string,): Observable<CompanyReportDto[]> {
    const apiUrl = `${this.baseUrl}/GetCompaniesReport?companyId=${companyId}&InactiveDays=${inactiveDays}&status=${status}&company=${company}`;
    return this.httpClient.get<CompanyReportDto[]>(apiUrl).pipe(
      catchError(error => {
        console.error('getCompanyReport() | ', error);
        throw error;
      })
    );
  }
}
