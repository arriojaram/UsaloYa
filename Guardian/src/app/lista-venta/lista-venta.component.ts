import { Component, OnInit } from '@angular/core';
import { SaleService } from '../services/sale.service';
import { CommonModule } from '@angular/common';
import { OfflineDbStore } from '../services/offline-db-store.service';
import { UserStateService } from '../services/user-state.service';
import { first } from 'rxjs';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { environment } from '../environments/enviroment';
import { userDto } from '../dto/userDto';

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
    this.numVenta = "-1";
  }
  metodoPago: string;
  notaVenta: string = "";
  isHidden?: boolean;
  message?: string;
  numVenta: string;
  messageClass: string = "alert  alert-success mt-2";
  
  /****************************************************/
  ticketVisible = false;
  fechaHora = new Date().toLocaleString();
 
  showTicket() {
    this.ticketVisible = true;
    setTimeout(() => {
      this.ticketVisible = false;
      this.resetListaVenta();
    }, environment.notificationsDisplayTimeSeconds);
  }
  /******************************************************** */
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

  getUserState(): userDto
  {
    let userState = this.userState.getUserStateLocalStorage();
    return userState;
  }

  async finalizarVenta()
  {
    this.isHidden = false;
    let userState = this.getUserState();
    
    this.ventaService.finishSale(userState.userId, userState.companyId, this.notaVenta?? '', this.metodoPago?? 'Efectivo').pipe(first())
    .subscribe(
      {
        next: (response) => 
        {
          console.log(response);
          this.message = `Venta registrada: ${response}`;
          this.numVenta = 'Num. Venta: ' + response;
          this.messageClass = "alert  alert-success mt-2";
          this.showTicket();
         
          // Reload catalog
          this.ventaService.cacheProductCatalog(userState.companyId);
        },
        error: async (error) => {
          this.messageClass = "alert  alert-warning mt-2";
          
          try {
            const newGuid = this.ventaService.generateGUID();
            
            const saleTmp = this.ventaService.getCurrentSale();
            saleTmp.notes = saleTmp.notes + ' -> TMP-:' + newGuid;
            const id = await this.offlineDbStore.AddSale(saleTmp);
            
            this.numVenta = 'Num. Venta: TMP-' +  newGuid;
            this.message = `Venta registrada (en proceso de sincronización...)`;
            this.resetListaVenta();
          } catch (error) {
            console.error('Error adding product:', error);
          }
        }
    });

    setTimeout(() => {
      this.isHidden = true; // Oculta el div después de X segundos
    }, environment.notificationsDisplayTimeSeconds);
  }

 

}
