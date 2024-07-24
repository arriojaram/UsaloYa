import { Injectable } from '@angular/core';
import Dexie from 'dexie';
import { from, Observable, tap } from 'rxjs';
import { Sale } from '../dto/sale';
import { environment } from '../environments/enviroment';

@Injectable({
  providedIn: 'root',
})
export class OfflineDbStore extends Dexie {
  sales: Dexie.Table<Sale, number>;

  constructor() {
    super('sales' + environment.databaseName);
    this.version(environment.databaseVersion).stores({
      sales: '++id'
    });
    this.sales = this.table('sales');
  }

  AddSale(sale: Sale): Observable<number> {
    return from(this.transaction('rw', this.sales, () => {
      return this.sales.add(sale);
    }));
  }

  DeleteSale(id: number): Observable<void> {
    console.log("Delete record-id: " + id);
    return from(this.transaction('rw', this.sales, () => {
      return this.sales.delete(id);
    }));
  }

  GetSale(saleId: number): Observable<Sale | undefined> {
    return from(this.sales.where({ id: saleId }).first());
  }

  GetSales(): Observable<Sale[]> {
    return from(this.sales.toArray()).pipe();
  }
}
