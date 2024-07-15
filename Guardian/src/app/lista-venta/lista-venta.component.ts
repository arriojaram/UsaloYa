import { Component } from '@angular/core';
import { SaleService } from '../services/sale.service';
import { CommonModule } from '@angular/common';
import { OfflineDbStore } from '../services/offline-db-store.service';

@Component({
  selector: 'app-lista-venta',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './lista-venta.component.html',
  styleUrl: './lista-venta.component.css'
})
export class ListaVentaComponent {
  constructor(
    public ventaService: SaleService,
    public offlineDbStore: OfflineDbStore
  )
  {
    this.isHidden = true;
  }

  isHidden?: boolean;
  message?: string;
  messageClass: string = "alert  alert-success mt-2";

  canFinishSale()
  {
    var terminarVenta = false;
    if(this.ventaService.saleProducts.length > 0)
      terminarVenta = true;
    
    return !terminarVenta;
  }
  
  async finalizarVenta()
  {
    this.isHidden = false;
         
    this.ventaService.finishSale().subscribe(
      {
        next: (response) => 
        {
          this.message = `Venta registrada: ${JSON.stringify(response)}`;
          this.messageClass = "alert  alert-success mt-2";
          this.ventaService.saleProducts = [];
        },
        error: async (error) => {
          this.messageClass = "alert  alert-warning mt-2";
          
          try {
            const id = await this.offlineDbStore.AddSale(this.ventaService.getCurrentSale());
            console.log('Added product with id:', id);
            this.message = `Venta registrada temporalmente: ${JSON.stringify(error)}`;
          } catch (error) {
            console.error('Error adding product:', error);
          }
        }
    });
    this.ventaService.saleProducts = [];
    setTimeout(() => {
      this.isHidden = true; // Oculta el div después de X segundos
  }, 15000);
  }

}
