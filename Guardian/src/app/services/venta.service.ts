import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Producto } from '../dto/producto';

@Injectable({
  providedIn: 'root'
})
export class VentaService {

  constructor(private httpClient: HttpClient) 
  {
    this.cacheProductCatalog();
  }

  ngOnInit():void{}

  private urlGetAPI = 'https://dummyjson.com/products';
  private urlSetAPI = 'https://dummyjson.com/products/add';

  ventaProductos: Producto [] = []
  productCatalog: any [] = []

  addProduct(code: number)
  {
    var prodId = Math.floor(Math.random() * 30) + 1;
    const product = this.productCatalog.find(p => p.id === prodId);
    if(product == undefined)
    {
      return false;
    }
    else
    {
      //var p = new Producto(1, code.toString(), product.title, product.price, product.price);
      //this.ventaProductos.push(p);
      return true;
    }
}
  
  saveVenta(product: any) : Observable<any>
  {
    return this.httpClient.post(this.urlSetAPI, product);
  }

  private getProductCatalog() : Observable<any>
  {
    return this.httpClient.get<any>(this.urlGetAPI).pipe(r => r);
  }

  private cacheProductCatalog()
  {
      this.getProductCatalog().subscribe({
          next: (result) => {
              this.productCatalog = result.products;
          },
          error: (err) =>{
              console.error(err);
          }
      });
  }
}
