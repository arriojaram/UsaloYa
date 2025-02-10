import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, catchError, of, timeInterval } from 'rxjs';
import { Producto } from '../dto/producto';
import { environment } from '../environments/enviroment';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private baseUrl = environment.apiUrlBase + '/api/Product';
 
  constructor(
    private http: HttpClient
  ) 
  {

  }

  filterProducts(pageNumber: number, companyId: number, categoryId: number): Observable<Producto[]> {
    const apiUrl =`${this.baseUrl}/FilterProducts?pageNumber=${pageNumber}&companyId=${companyId}&categoryId=${categoryId}`;
    
    return this.http.get<Producto[]>(apiUrl).pipe(
      catchError(error => {
        
        if (error.status === 404) {
          return of([]);  // Retorna null si el error es 404 Not Found
        } else {
          throw error;  // Lanza la excepción para otros tipos de errores
        }
      })
    );
  }

  searchProducts4List(pageNumber: number, companyId: number, keyword: string): Observable<Producto[]> {
    const apiUrl =`${this.baseUrl}/SearchProduct4List?pageNumber=${pageNumber}&companyId=${companyId}&keyword=${keyword}`;
    
    return this.http.get<Producto[]>(apiUrl).pipe(
      catchError(error => {
        console.error('SearchProduct4List() | ', error);
        if (error.status === 404) {
          return of([]);  // Retorna null si el error es 404 Not Found
        } else {
          throw error;  // Lanza la excepción para otros tipos de errores
        }
      })
    );
  }

  searchProductsFull(pageNumber: number, companyId: number, keyword: string): Observable<Producto[]> {
    const apiUrl =`${this.baseUrl}/SearchProductFull?pageNumber=${pageNumber}&companyId=${companyId}&keyword=${keyword}`;
    
    return this.http.get<Producto[]>(apiUrl).pipe(
      catchError(error => {
        console.error('searchProductsFull() | ', error);
        throw error;
      })
    );
  }

  getProduct(companyId: number, productId: number): Observable<Producto> {
    const apiUrl = `${this.baseUrl}/GetProduct?companyid=${companyId}&productid=${productId}`;
    
    return this.http.get<Producto>(apiUrl).pipe(
      catchError(error => {
        console.error('getProduct() | ', error);
        throw error;
      })
    );
  }

  saveProduct(companyId: number, product: Producto): Observable<Producto> {
    const apiUrl = `${this.baseUrl}/AddProduct?companyId=${companyId}`;
    return this.http.post<Producto>(apiUrl, product).pipe(
      catchError(error => {
        console.error('saveProduct() | ', error);
        throw error;
      })
    );
  }
}
