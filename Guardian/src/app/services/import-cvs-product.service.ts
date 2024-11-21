import { Injectable } from '@angular/core';
import { Producto } from '../dto/producto';
import * as Papa from 'papaparse';
import { from, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ImportCvsProductService {

  constructor() { }

  parseCsv(file: File): Observable<Producto[]> {
    return from(new Promise<Producto[]>((resolve, reject) => {
      Papa.parse(file, {
        header: true,
        skipEmptyLines: true,
        complete: (results) => {
          const productos: Producto[] = results.data.map((row: any) => this.toProducto(row));
          resolve(productos);
        },
        error: (error) => reject(error),
      });
    }));
  }

  private toProducto(row: any): Producto {
    return {
      productId: 0, 
      name: row.Nombre || '',
      description: row.Descripcion || '',
      buyPrice: parseFloat(row.PrecioCompra) || 0,
      unitPrice: parseFloat(row.PrecioUnitario) || 0,
      unitPrice1: parseFloat(row.Precio1) || 0,
      unitPrice2: parseFloat(row.Precio2) || 0,
      unitPrice3: parseFloat(row.Precio3) || 0,
      unitsInStock: parseInt(row.UnidadesEnStock) || 0,
      discontinued: false, // Assuming a default value
      sku: row.Sku || '',
      barcode: row.CodigoBarras || '',
      companyId: 0,
      categoryId:0,
      supplierId:0,
      imgUrl:'',
      brand:'',
      color:'',
      size:'',
      count:0,
      total:0,
      dateAdded: new Date(),
      dateModified: new Date()

    };
  }
}
