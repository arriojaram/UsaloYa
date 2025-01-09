import { Injectable, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, catchError, first, from, tap } from 'rxjs';
import { Producto } from '../dto/producto';
import { ProductService } from './product.service';
import { UserStateService } from './user-state.service';
import { NavigationService } from './navigation.service';
import { environment } from '../environments/enviroment';
import { Sale, SaleDetail } from '../dto/sale';
import Dexie from 'dexie';
import { UpdateSaleStatus } from '../dto/update-sale-status';
import { AlertLevel, PriceLevel } from '../Enums/enums';

@Injectable({
  providedIn: 'root'
})
export class SaleService extends Dexie implements OnInit{
  productCatalogTable: Dexie.Table<Producto, number>;
  migrationsTable: Dexie.Table<Date, number>;
  totalVenta: number = 0;

  private baseUrl = environment.apiUrlBase + '/api/Sale';
 
  private currentSale: Sale;
  public customerId: number = 0;
  audioCtx: AudioContext | null = null;

  constructor(
    private httpClient: HttpClient,
    private productService: ProductService,
    private userService: UserStateService,
    private navigationService: NavigationService
  ) 
  {
    super('catalog' +  environment.databaseName);
    this.version(environment.databaseVersion).stores({
      productCatalogTable: 'productId, name, barcode, sku',
      migrationsTable: '++id, migrationDate'
  });

  // Create relationships
  this.productCatalogTable = this.table('productCatalogTable');
  this.migrationsTable = this.table('migrationsTable');

  this.currentSale = {
    id: undefined,
    saleId: 0,
    customerId: this.customerId,
    paymentMethod: '',
    companyId: 0,
    tax: 0,
    notes: '',
    userId: 0,
    saleDetailsList: []
  };

    
  }

  ngOnInit():void{
    
    this.currentSale = {
      id: undefined,
      saleId: 0,
      customerId: this.customerId,
      paymentMethod: '',
      companyId: 0,
      tax: 0,
      notes: '',
      userId: 0,
      saleDetailsList: []
    };
  }

  saleProductsGrouped: Producto [] = []
  productCatalog: Producto[] | any[] = []

  generateGUID(): string {
    return 'xxxxx4xyxx'.replace(/[xy]/g, function(c) {
      const r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }
  
  getCurrentSale(): Sale
  {
    return this.currentSale;
  }

  playBeep(added: boolean): void {
    // Reutilizar o crear el AudioContext
    if (!this.audioCtx) {
      this.audioCtx = new (window.AudioContext || (window as any).webkitAudioContext)();
    }
  
    // Crear el oscilador
    const oscillator = this.audioCtx.createOscillator();
    oscillator.type = added ? 'sine' : 'sawtooth';
    oscillator.frequency.setValueAtTime(440, this.audioCtx.currentTime);
  
    // Conectar y configurar
    oscillator.connect(this.audioCtx.destination);
    oscillator.start();
  
    // Detener y cerrar el oscilador
    oscillator.stop(this.audioCtx.currentTime + 0.1);
    oscillator.onended = () => {
      oscillator.disconnect();
      if (!added && this.audioCtx) {
        this.audioCtx.close();
        this.audioCtx = null;
      }
    };
  }
  
  getCambio(recibido: number)
  {
    if(recibido > 0.1)
    {
      return recibido - this.getTotalVenta();
    }
    return 0;
  }

  getTotalVenta(): number {
    return this.totalVenta;
  }

  removeProductFromList(productId: number): void {
    const productIndex = this.saleProductsGrouped.findIndex(product => product.productId === productId);
    
    if (productIndex !== -1) {
        // Si el producto existe en la lista, ajustar el count o eliminar si necesario.
        if (this.saleProductsGrouped[productIndex].count > 1) {
            // Disminuir el count si es mayor que 1.
            this.saleProductsGrouped[productIndex].count -= 1;
            this.saleProductsGrouped[productIndex].total = 
              this.saleProductsGrouped[productIndex].count * this.getPrice(this.saleProductsGrouped[productIndex]);
        } else {
            // Si el count es 1, eliminar el producto completamente.
            this.saleProductsGrouped.splice(productIndex, 1);
        }
        this.updateTotal();
      } 
  }

  async addProduct(barcode: string, companyId: number)
  {
    let productForSale: Producto | any = undefined;
    
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
      let barcodeLength = barcode.length;
      if(barcodeLength > 10)
      {
        productForSale = this.productCatalog.find(
          p => p.barcode.toLowerCase() === barcode.toLowerCase()
                  && p.companyId == companyId
        );
      }
      else
      {
        // Search barcode first
        productForSale = this.productCatalog.find(
          p => p.barcode.toLowerCase() === barcode.toLowerCase()
                  && p.companyId == companyId
        );

        // If the barcode search didn't return the product, try the SKU
        if(productForSale === undefined)
        {
          productForSale = this.productCatalog.find(
            p => (p.sku??'_$').toLowerCase() === barcode.toLowerCase()
                    && p.companyId == companyId
          );
        }
      }
    }

    if(productForSale === undefined)
    {
      this.navigationService.showUIMessage("El producto no se encuentra registrado");
      return false;
    }
    else
    {
      this.groupProducts(productForSale);
      return true;
    }
  }

  resetProductPrice(): void {
    this.totalVenta = 0;
    let index = 0;
    this.saleProductsGrouped.forEach(p => {
      let totalProds = this.saleProductsGrouped[index].count;
      this.saleProductsGrouped[index].total = totalProds * p.unitPrice;
      index++;
    });

    
    this.updateTotal();
  }

  getPrice(producto: Producto) : number
  {
    let prodPrice = producto.unitPrice;
    switch (producto.priceLevel) {
      case PriceLevel.UnitPrice1:
        prodPrice = producto.unitPrice1;
        break;
      case PriceLevel.UnitPrice2:
          prodPrice = producto.unitPrice2;
          break;
      case PriceLevel.UnitPrice3:
        prodPrice = producto.unitPrice3;
        break;
      default:
        break;
    }
    return prodPrice;
  }

  updateProductPrice(productId: number, priceLevel: number): void {
    this.totalVenta = 0;
    const productIndex = this.saleProductsGrouped.findIndex(p => p.productId === productId);
    if (productIndex !== -1) 
    {
      let existingProduct = this.saleProductsGrouped[productIndex];
      let totalProds = this.saleProductsGrouped[productIndex].count;
      this.saleProductsGrouped[productIndex].priceLevel = priceLevel;
      existingProduct.priceLevel = priceLevel;
      this.saleProductsGrouped[productIndex].total = totalProds * this.getPrice(existingProduct);
    }
    else
    {
      this.navigationService.showUIMessage("Producto no encontrado, reinicia la venta ha ocurrido un errror.");
    }
    
    this.updateTotal();
  }

  updateNumOfProductos(productId: number, totalProducts: number): void {
    const productMap = new Map();
    this.totalVenta = 0;
    const productIndex = this.saleProductsGrouped.findIndex(p => p.productId === productId);
    if (productIndex !== -1) 
    {
      let existingProduct = this.saleProductsGrouped[productIndex];
      this.saleProductsGrouped[productIndex].count = totalProducts;
      this.saleProductsGrouped[productIndex].total = totalProducts * this.getPrice(existingProduct);
    }
    else
    {
      this.navigationService.showUIMessage("Producto no encontrado, reinicia la venta ha ocurrido un errror.");
    }
    this.updateTotal();
  }

  private updateTotal()
  {
    this.totalVenta = 0;
    this.saleProductsGrouped.forEach((product) => {
      this.totalVenta += product.total;
    });
  }

  private groupProducts(newProduct: Producto): void {
    const productMap = new Map();
   
    const productIndex = this.saleProductsGrouped.findIndex(p => p.productId === newProduct.productId);
    if (productIndex !== -1) 
    {
      let existingProduct = this.saleProductsGrouped[productIndex];
      this.saleProductsGrouped[productIndex].count += 1;
      this.saleProductsGrouped[productIndex].total = (existingProduct.count) * this.getPrice(existingProduct);
    }
    else
    {
      newProduct.count = 1;
      newProduct.priceLevel = PriceLevel.UnitPrice;
      newProduct.total = 1 * newProduct.unitPrice;
      this.saleProductsGrouped.push(newProduct);
    }
    
    if(newProduct.unitsInStock <= 0)
    {
      this.navigationService.showUIMessage('Producto agotado en almacén ('+ newProduct.name +')', AlertLevel.Info);
    }
    
    this.updateTotal();
  }

  private buildSale(userId: number, companyId: number, notes: string, metodoPago: string): Sale
  {
    let saleDetail: SaleDetail[] = new Array(this.saleProductsGrouped.length-1);
    for (let index = 0; index < this.saleProductsGrouped.length; index++) {
      const p = this.saleProductsGrouped[index];
      const sd: SaleDetail = {SaleId:0 ,ProductId: p.productId, Quantity:p.count, TotalPrice:p.total, UnitPrice:p.unitPrice, PriceLevel:p.priceLevel?? 0};
      saleDetail[index] = sd; 
    }

    const sale: Sale = {
      id: undefined,
      saleId: 0,
      paymentMethod: metodoPago,
      tax: 0.00,
      notes: notes,
      userId: userId,
      companyId: companyId,
      saleDetailsList: saleDetail,
      customerId: this.customerId
    };

    this.currentSale = sale;
    return sale;
  }

   finishSale(userId: number, companyId: number, notas: string, metodoPago: string): Observable<number> {
    const apiUrl = `${this.baseUrl}/AddSale`;
    const sale = this.buildSale(userId, companyId, notas, metodoPago);
    return this.httpClient.post<number>(apiUrl, sale).pipe(
      tap(() => {
        //this.totalVenta = 0;
      }),
      catchError(error => {
        console.error('finishSale() | ', error);
        throw error;
      })
    );
  }

  updateSaleStatus(saleId: number, status: string): Observable<void> {
    const saleStatus: UpdateSaleStatus = {SaleId: saleId, Status: status};
    const apiUrl = `${this.baseUrl}/UpdateSaleStatus`;
    
    return this.httpClient.post<void>(apiUrl, saleStatus).pipe(
      catchError(error => {
        console.error('updateSaleStatus() | ', error);
        throw error;
      })
    );
  }

  completeTemporalSale(sale: Sale): Observable<Sale> {
    const apiUrl = `${this.baseUrl}/AddSale`;
    return this.httpClient.post<Sale>(apiUrl, sale).pipe(
      catchError(error => {
        console.error('finishSale() | ', error);
        throw error;
      })
    );
  }

  public cacheProductCatalog(companyId: number)
  {
    this.productService.searchProductsFull(-1, companyId, "-1").pipe(first())
    .subscribe({
      next: async (products: Producto[]) => {
        this.productCatalog = products;
        
        // Remove any existing record
        this.productCatalogTable.clear();
        // Update products variable
        await this.transaction('rw', this.productCatalogTable, async () => {
          await this.productCatalogTable.bulkAdd(products);
        })
      },
      error:(err) => {
        if (err.status === 404) {
          this.navigationService.showUIMessage("No hay productos registrados");
        } else {
          this.navigationService.showUIMessage(err.message);
        }
      },
    });
  }
}
