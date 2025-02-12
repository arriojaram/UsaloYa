import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError } from 'rxjs';
import { customerDto } from '../dto/customerDto';
import { environment } from '../environments/enviroment';
import { productCategoryDto } from '../dto/productCategoryDto';

@Injectable({
  providedIn: 'root'
})
export class ProductCategoryService {

   private baseUrl = environment.apiUrlBase + '/api/Category';
  
    constructor(
      private http: HttpClient
    ) 
    {
  
    }
  
    delete(c: productCategoryDto): Observable<productCategoryDto> {
      const apiUrl = `${this.baseUrl}/DeleteCategory?companyId=${c.companyId}`;
  
      return this.http.post<productCategoryDto>(apiUrl, c).pipe(
        catchError(error => {
          console.error('deleteCategory() | ', error);
          throw error;
        })
      );
    }

    save(c: productCategoryDto): Observable<productCategoryDto> {
      const apiUrl = `${this.baseUrl}/SaveCategory`;
  
      return this.http.post<productCategoryDto>(apiUrl, c).pipe(
        catchError(error => {
          console.error('saveCategory() | ', error);
          throw error;
        })
      );
    }
  
    get(id: number, companyId: number): Observable<productCategoryDto> {
      const apiUrl = `${this.baseUrl}/GetCategory?categoryId=${id}&companyId=${companyId}`;
      
      return this.http.get<productCategoryDto>(apiUrl).pipe(
        catchError(error => {
          console.error('getCategory() | ', error);
          throw error;
        })
      );
    }
  
    getAll(companyId: number, keyword:string): Observable<productCategoryDto[]> {
      
      const apiUrl =`${this.baseUrl}/GetAll4List?keyword=${keyword}&companyId=${companyId}`;
      
      return this.http.get<productCategoryDto[]>(apiUrl).pipe(
        catchError(error => {
          console.error('getAll() | ', error);
          throw error;
        })
      );
    }
  
  
}
