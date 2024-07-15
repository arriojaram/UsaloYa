import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, catchError } from 'rxjs';
import { Producto } from '../dto/producto';
import { ProductService } from './product.service';
import { UserStateService } from './userState.service';
import { userStateDto } from '../dto/userDto';
import { NavigationService } from './navigation.service';
import { environment } from '../environments/enviroment';
import { Sale, SaleDetail } from '../dto/sale';

@Injectable({
  providedIn: 'root'
})
export class SaleService {

  userState: userStateDto;
  private baseUrl = environment.apiUrlBase + '/api/Sale';
  private httpOptions;
  private currentSale: Sale;
  constructor(
    private httpClient: HttpClient,
    private productService: ProductService,
    private userService: UserStateService,
    private navigationService: NavigationService
  ) 
  {
    this.currentSale = {
      SaleId: 0,
      CustomerId: 0,
      PaymentMethod: '',
      CompanyId: 0,
      Tax: 0,
      Notes: '',
      UserId: 0,
      SaleDetailsList: []
    };

    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': environment.apiToken
      })
    };
    
    this.userState = userService.getUserState();
    this.cacheProductCatalog();
  }

  ngOnInit():void{}

  saleProducts: Producto [] = []
  productCatalog: any [] = []

  getCurrentSale(): Sale
  {
    return this.currentSale;
  }

  addProduct(barcode: string)
  {
    const productForSale: Producto = this.productCatalog.find(
      p => p.barcode == barcode
              && p.companyId == this.userState.companyId
    );

    if(productForSale === undefined)
    {
      this.navigationService.showUIMessage("El producto no se encuentra registrado");
      return false;
    }
    else
    {
      this.saleProducts.push(productForSale);
      return true;
    }
  }
  
  private buildSale(): Sale
  {
    let saleDetail: SaleDetail[] = new Array(this.saleProducts.length-1);
    for (let index = 0; index < this.saleProducts.length; index++) {
      const p = this.saleProducts[index];
      const sd: SaleDetail = {SaleId:0 ,ProductId: p.productId, Quantity:1, TotalPrice:p.unitPrice, UnitPrice:p.unitPrice};
      saleDetail[index] = sd; 
    }

    const sale: Sale = {
      SaleId: 0,
      CustomerId: 0,
      PaymentMethod: "Efectivo",
      Tax: 0.00,
      Notes: "",
      UserId: this.userState.userId,
      CompanyId: this.userState.companyId,
      SaleDetailsList: saleDetail
    };

    this.currentSale = sale;
    return sale;
  }

   finishSale(): Observable<Sale> {
    const apiUrl = `${this.baseUrl}/AddSale`;
    const sale = this.buildSale();
    return this.httpClient.post<Sale>(apiUrl, sale, this.httpOptions).pipe(
      catchError(error => {
        console.error('finishSale() | ', error);
        throw error;
      })
    );
  }

  completeTemporalSale(sale: Sale): Observable<Sale> {
    const apiUrl = `${this.baseUrl}/AddSale`;
    return this.httpClient.post<Sale>(apiUrl, sale, this.httpOptions).pipe(
      catchError(error => {
        console.error('finishSale() | ', error);
        throw error;
      })
    );
  }

  private cacheProductCatalog()
  {
    this.productService.searchProducts(this.userState.companyId, "-1").subscribe({
      next: (products) => {
        this.productCatalog = products;
      },
      complete: () => {
        
      },
      error:(err) => {
        this.navigationService.showUIMessage(err);
      },
    });
  }
}
