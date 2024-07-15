import { Injectable } from '@angular/core';
import Dexie from 'dexie';
import { Sale, SaleDetail } from '../dto/sale';
import { from, mergeMap } from 'rxjs';
import { SaleService } from './sale.service';

@Injectable({
  providedIn: 'root',
})
export class OfflineDbStore extends Dexie {
  sales: Dexie.Table<Sale, number>;
  
  constructor(
    public saleService: SaleService
  ) 
  {
    super('salesDbOfflinev1');

    this.version(1).stores({
        sales: '++id, sale'
    });

    // Create relationships
    this.sales = this.table('sales');
    
  
    // Abre la base de datos.
    this.open().catch((err) => {
      console.error(`Open failed: ${err.stack}`);
    });
  }

  async AddSale(sale: Sale): Promise<number> {
    const saleId = await this.transaction('rw', this.sales, async () => {
        const id = await this.sales.add(sale);
        return id;
    });
    return saleId;
  }

  async DeleteSale(saleId: number): Promise<void> {
      await this.transaction('rw', this.sales, async () => {
          await this.sales.delete(saleId);
      });
  }

  async GetSale(saleId: number): Promise<Sale | undefined> {
      const sale = await this.sales.where({ id: saleId }).first();
     
      return sale;
  }

  async GetSales(): Promise<Sale[]> {
      const sales = await this.sales.toArray();
     
      return sales;
  }

  public migrateSales() {
    from(this.GetSales()).pipe(
      mergeMap(sales => from(sales)),
      mergeMap(sale =>
        this.saleService.completeTemporalSale(sale).pipe(
          mergeMap(response => {
            if (response && response.SaleId) {
              return from(this.DeleteSale(response.SaleId));
            } else {
              throw new Error('Sale migration failed: No response SaleId');
            }
          })
        )
      )
    ).subscribe(
      () => console.log('Migration successful for a sale.'),
      error => console.error('Error in migrating sales:', error)
    );
  }
}
