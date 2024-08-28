import { Component, OnInit } from '@angular/core';
import { SaleService } from '../services/sale.service';
import { CommonModule } from '@angular/common';
import { OfflineDbStore } from '../services/offline-db-store.service';
import { UserStateService } from '../services/user-state.service';
import { first } from 'rxjs';
import { Router } from '@angular/router';

import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-lista-venta',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './lista-venta.component.html',
  styleUrl: './lista-venta.component.css'
})
export class ListaVentaComponent implements OnInit {
  constructor(
    private router: Router,
    public ventaService: SaleService,
    private offlineDbStore: OfflineDbStore,
    private userState: UserStateService
  )
  {
    this.isHidden = true;
    this.metodoPago = "";
  }
  metodoPago: string;
  notaVenta: string = "";
  isHidden?: boolean;
  message?: string;
  messageClass: string = "alert  alert-success mt-2";
  
  ngOnInit(): void {
    this.metodoPago = "Efectivo";  
  }

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
    this.ventaService.totalVenta = 0;
    this.notaVenta = '';
    this.metodoPago = 'Efectivo';
  }
 
  removeProduct(productId: number): void {
    this.ventaService.removeProductFromList(productId);
  }

  async finalizarVenta()
  {
    this.isHidden = false;
    let userState = this.userState.getUserStateLocalStorage();
    
    this.ventaService.finishSale(userState.userId, userState.companyId, this.notaVenta?? '', this.metodoPago?? 'Efectivo').pipe(first())
    .subscribe(
      {
        next: (response) => 
        {
          this.message = `Venta registrada: ${JSON.stringify(response)}`;
          this.messageClass = "alert  alert-success mt-2";
          this.resetListaVenta();
          // Reload catalog
          this.ventaService.cacheProductCatalog(userState.companyId);
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
