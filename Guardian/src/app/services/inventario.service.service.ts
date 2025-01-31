import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError } from 'rxjs';
import { environment } from '../environments/enviroment';
import { BarcodeDto, IdDto } from '../dto/idDto';
import { Inventory, InventoryProduct } from '../dto/inventoryDto';
import { setUnitsInStockDto } from '../dto/setUnitsInStockDto';

@Injectable({
  providedIn: 'root'
})
export class InventarioService {

   private baseUrl = environment.apiUrlBase + '/api/Product';
  
    constructor(private httpClient: HttpClient) 
    { 
  
    }
   
    setAllStockEqualsInventory(companyId:number): Observable<void> {
      const id: IdDto = {id: companyId};
      const apiUrl = `${this.baseUrl}/SetAllUnitsStock`;
      
      return this.httpClient.post<void>(apiUrl, id).pipe(
        catchError(error => {
          console.error('setAllStockEqualsInventory | ', error);
          throw error;
        })
      );
    }

    setToZeroAllInventory(companyId:number): Observable<void> {
      const id: IdDto = {id: companyId};
      const apiUrl = `${this.baseUrl}/ResetAllInVentario`;
      
      return this.httpClient.post<void>(apiUrl, id).pipe(
        catchError(error => {
          console.error('setToZeroAllInventory() | ', error);
          throw error;
        })
      );
    }

    setUnitsInStockToProduct(stockInfo: setUnitsInStockDto, companyId:number): Observable<number> {
      
      const apiUrl = `${this.baseUrl}/SetUnitsInStock?companyId=${companyId}`;
      
      return this.httpClient.post<number>(apiUrl, stockInfo).pipe(
        catchError(error => {
          console.error('setUnitsInStockToProduct() | ', error);
          throw error;
        })
      );
    }

    setProductInventarioValue(setInVentario: BarcodeDto, companyId:number): Observable<InventoryProduct> {
      
      const apiUrl = `${this.baseUrl}/SetInVentario?companyId=${companyId}`;
      
      return this.httpClient.post<InventoryProduct>(apiUrl, setInVentario).pipe(
        catchError(error => {
          console.error('setProductInventarioValue() | ', error);
          throw error;
        })
      );
    }

    getInventoryByAlertId(pageNumber:number, alertId:number, companyId: number): Observable<InventoryProduct[]> {
      const apiUrl =`${this.baseUrl}/GetInventarioByAlertId?alertLevel=${alertId}&companyId=${companyId}&pageNumber=${pageNumber}`;
      
      return this.httpClient.get<InventoryProduct[]>(apiUrl).pipe(
        catchError(error => {
          console.error('getInventoryByAlertId() | ', error);
          throw error;
        })
      );
    }

    getInventoryAll(pageNumber:number, companyId: number): Observable<Inventory> {
      const apiUrl =`${this.baseUrl}/GetInventarioAll?pageNumber${pageNumber}&companyId=${companyId}`;
      
      return this.httpClient.get<Inventory>(apiUrl).pipe(
        catchError(error => {
          console.error('getInventoryAll() | ', error);
          throw error;
        })
      );
    }

    getInventoryTop50(companyId: number, keyword:string, pageNumber: number): Observable<Inventory> {
      const apiUrl =`${this.baseUrl}/GetInventarioTop50?pageNumber=${pageNumber}&keyword=${keyword}&companyId=${companyId}`;
      
      return this.httpClient.get<Inventory>(apiUrl).pipe(
        catchError(error => {
          console.error('getInventory() | ', error);
          throw error;
        })
      );
    }
}
