import { Component } from '@angular/core';
import { SaleService } from '../services/sale.service';
import { CommonModule } from '@angular/common';
import { OfflineDbStore } from '../services/offline-db-store.service';
import { UserStateService } from '../services/user-state.service';
import { first } from 'rxjs';

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
    private offlineDbStore: OfflineDbStore,
    private userState: UserStateService
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
  
  resetListaVenta()
  {
    this.ventaService.saleProducts = [];
    this.ventaService.saleProductsGrouped = [];
  }
  
  removeProduct(productId: number): void {
    this.ventaService.removeProductFromList(productId);
  }

  async finalizarVenta()
  {
    this.isHidden = false;
    let userState = this.userState.getUserStateLocalStorage();
    this.ventaService.finishSale(userState.userId, userState.companyId).pipe(first())
    .subscribe(
      {
        next: (response) => 
        {
          this.message = `Venta registrada: ${JSON.stringify(response)}`;
          this.messageClass = "alert  alert-success mt-2";
          this.resetListaVenta();
        },
        error: async (error) => {
          this.messageClass = "alert  alert-warning mt-2";
          
          try {
            const id = await this.offlineDbStore.AddSale(this.ventaService.getCurrentSale());
            console.log('offlineID: ' + id);
            this.message = `Venta registrada (en proceso de sincronización...)`;
            this.resetListaVenta();
          } catch (error) {
            console.error('Error adding product:', error);
          }
        }
    });

    setTimeout(() => {
      this.isHidden = true; // Oculta el div después de X segundos
    }, 20000);
  }

 

}
