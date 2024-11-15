import { Injectable } from '@angular/core';
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

@Injectable({
  providedIn: 'root'
})
export class SaleService extends Dexie {
  productCatalogTable: Dexie.Table<Producto, number>;
  migrationsTable: Dexie.Table<Date, number>;
  totalVenta: number = 0;

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
    id: undefined,
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
    const audioCtx = new (window.AudioContext || (window as any).webkitAudioContext)();

    // Crear un oscilador que genera el tono
    const oscillator = audioCtx.createOscillator();

    // Configurar el tipo de onda y frecuencia
    oscillator.type = 'sine'; // Puedes probar con 'square', 'sawtooth', 'triangle'
    if(added === false)
    {
      oscillator.type = 'sawtooth'; // Puedes probar con 'square', 'sawtooth', 'triangle'
    }

    oscillator.frequency.setValueAtTime(440, audioCtx.currentTime); // Frecuencia en Hz (440 Hz es la nota A4)

    // Conectar el oscilador al destino (altavoces)
    oscillator.connect(audioCtx.destination);

    // Iniciar y detener el oscilador después de un breve tiempo para crear un pitido
    oscillator.start();
    oscillator.stop(audioCtx.currentTime + 0.1); // Pitido de 0.1 segundos de duración
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
              this.saleProductsGrouped[productIndex].count * this.saleProductsGrouped[productIndex].unitPrice;
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
        productForSale = this.productCatalog.find(
          p => (p.sku??'_$').toLowerCase() === barcode.toLowerCase()
                  && p.companyId == companyId
        );
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

  updateNumOfProductos(productId: number, totalProducts: number): void {
    const productMap = new Map();
    this.totalVenta = 0;
    const productIndex = this.saleProductsGrouped.findIndex(p => p.productId === productId);
    if (productIndex !== -1) 
    {
      let existingProduct = this.saleProductsGrouped[productIndex];
      this.saleProductsGrouped[productIndex].count = totalProducts;
      this.saleProductsGrouped[productIndex].total = totalProducts * existingProduct.unitPrice;
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
      this.saleProductsGrouped[productIndex].total = (existingProduct.count) * existingProduct.unitPrice;
    }
    else
    {
      newProduct.count = 1;
      newProduct.total = 1 * newProduct.unitPrice;
      this.saleProductsGrouped.push(newProduct);
    }
    
    if(newProduct.unitsInStock <= 0)
    {
      this.navigationService.showUIMessage('Producto agotado en almacén ('+ newProduct.name +')');
    }
    
    this.updateTotal();
  }

  private buildSale(userId: number, companyId: number, notes: string, metodoPago: string): Sale
  {
    let saleDetail: SaleDetail[] = new Array(this.saleProductsGrouped.length-1);
    for (let index = 0; index < this.saleProductsGrouped.length; index++) {
      const p = this.saleProductsGrouped[index];
      const sd: SaleDetail = {SaleId:0 ,ProductId: p.productId, Quantity:p.count, TotalPrice:p.total, UnitPrice:p.unitPrice};
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
      customerId: undefined
    };

    this.currentSale = sale;
    return sale;
  }

   finishSale(userId: number, companyId: number, notas: string, metodoPago: string): Observable<number> {
    const apiUrl = `${this.baseUrl}/AddSale`;
    const sale = this.buildSale(userId, companyId, notas, metodoPago);
    return this.httpClient.post<number>(apiUrl, sale, this.httpOptions).pipe(
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
    
    return this.httpClient.post<void>(apiUrl, saleStatus, this.httpOptions).pipe(
      catchError(error => {
        console.error('updateSaleStatus() | ', error);
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
    this.productService.searchProducts(companyId, "-1").pipe(first())
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
        this.navigationService.showUIMessage(err.message);
      },
    });
  }
}
