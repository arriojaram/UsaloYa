import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, catchError, from } from 'rxjs';
import { Producto } from '../dto/producto';
import { ProductService } from './product.service';
import { UserStateService } from './userState.service';
import { NavigationService } from './navigation.service';
import { environment } from '../environments/enviroment';
import { Sale, SaleDetail } from '../dto/sale';
import Dexie from 'dexie';

@Injectable({
  providedIn: 'root'
})
export class SaleService extends Dexie {
  productCatalogTable: Dexie.Table<Producto, number>;
  migrationsTable: Dexie.Table<Date, number>;

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
    super('catalog' +  environment.databaseName);
    this.version(environment.databaseVersion).stores({
      productCatalogTable: 'productId, name, description, barcode, companyId',
      migrationsTable: '++id, migrationDate'
  });

  // Create relationships
  this.productCatalogTable = this.table('productCatalogTable');
  this.migrationsTable = this.table('migrationsTable');

    this.currentSale = {
      id: 0,
      saleId: 0,
      customerId: 0,
      paymentMethod: '',
      companyId: 0,
      tax: 0,
      notes: '',
      userId: 0,
      saleDetailsList: []
    };

    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': environment.apiToken
      })
    };
    
  }

  ngOnInit():void{}

  saleProducts: Producto [] = []
  saleProductsGrouped: Producto [] = []
  productCatalog: Producto[] | any[] = []

  getCurrentSale(): Sale
  {
    return this.currentSale;
  }

 
  async addProduct(barcode: string, companyId: number)
  {
    let productForSale: any = undefined;
    console.log('longitud: ' + this.productCatalog.length);
    if(this.productCatalog.length == 0)
    {
      console.log('Buscar producto offline');
      const cachedProduct = await this.productCatalogTable.where('barcode').equals(barcode.toString()).toArray();
      if(cachedProduct && cachedProduct.length > 0)
      {
        productForSale = cachedProduct[0];
      }
    }
    else
    {
      productForSale = this.productCatalog.find(
        p => p.barcode == barcode
                && p.companyId == companyId
      );
    }

    if(productForSale === undefined)
    {
      this.navigationService.showUIMessage("El producto no se encuentra registrado");
      return false;
    }
    else
    {
      this.saleProducts.push(productForSale);
      this.groupProducts();
      return true;
    }
  }
  
  private groupProducts(): void {
    const productMap = new Map();
    
    this.saleProducts.forEach((product) => {
      if (productMap.has(product.barcode)) {
        let group = productMap.get(product.barcode);
        group.count += 1;
        group.total += product.unitPrice;
      } else {
        productMap.set(product.barcode, {
          ...product,
          count: 1,
          total: product.unitPrice
        });
      }
    });

    this.saleProductsGrouped = Array.from(productMap.values());
  }

  private buildSale(userId: number, companyId: number): Sale
  {
    let saleDetail: SaleDetail[] = new Array(this.saleProductsGrouped.length-1);
    for (let index = 0; index < this.saleProductsGrouped.length; index++) {
      const p = this.saleProductsGrouped[index];
      const sd: SaleDetail = {SaleId:0 ,ProductId: p.productId, Quantity:p.count, TotalPrice:p.total, UnitPrice:p.unitPrice};
      saleDetail[index] = sd; 
    }

    const sale: Sale = {
      id: 0,
      saleId: 0,
      customerId: 0,
      paymentMethod: "Efectivo",
      tax: 0.00,
      notes: "",
      userId: userId,
      companyId: companyId,
      saleDetailsList: saleDetail
    };

    this.currentSale = sale;
    return sale;
  }

   finishSale(userId: number, companyId: number): Observable<Sale> {
    const apiUrl = `${this.baseUrl}/AddSale`;
    const sale = this.buildSale(userId, companyId);
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

  public cacheProductCatalog(companyId: number)
  {
    console.log("Cached Catalog");
    this.productService.searchProducts(companyId, "-1").subscribe({
      next: async (products: Producto[]) => {
        this.productCatalog = products;
        
        // Remove any existing record
        this.productCatalogTable.clear();
        // Update products variable
        await this.transaction('rw', this.productCatalogTable, async () => {
          await this.productCatalogTable.bulkAdd(products);
        })
      },
      complete: () => {
        
      },
      error:(err) => {
        this.navigationService.showUIMessage(err.message);
      },
    });
  }
}
