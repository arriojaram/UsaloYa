import { Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
import { SaleService } from '../services/sale.service';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { OfflineDbStore } from '../services/offline-db-store.service';
import { UserStateService } from '../services/user-state.service';
import { first } from 'rxjs';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { environment } from '../environments/enviroment';
import { userDto } from '../dto/userDto';
import { NavigationService } from '../services/navigation.service';
import { Producto } from '../dto/producto';

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
    private userState: UserStateService,
    private navigationService: NavigationService
  )
  {
    this.isHidden = true;
    this.metodoPago = "";
    this.numVenta = "-1";
  }

  metodoPago: string;
  pagoRecibido: number | undefined;
  notaVenta: string = "";
  isHidden?: boolean;
  message?: string;
  numVenta: string;
  messageClass: string = "alert  alert-success mt-2";
 
  @ViewChild('inputNumber') inputNumber?: ElementRef;
  tempProductCounter: number | undefined;
  /****************************************************/
  ticketVisible = false;
  fechaHora = new Date().toLocaleString();
 
  showTicket() {
    this.ticketVisible = true;
    
    setTimeout(() => {
      this.generatePrintableTicket();
    }, 2);

    setTimeout(() => {
      this.ticketVisible = false;
      this.resetListaVenta();
    }, environment.notificationsDisplayTimeSeconds);
  }

  generatePrintableTicket() {
    const companyName = this.getUserState().companyName;
    const ventaNumber = this.numVenta;
    const fechaHora = new Date().toLocaleString();  // Asumiendo que `fechaHora` se calcula así
    const products = this.ventaService.saleProductsGrouped;
    const totalVenta = this.ventaService.getTotalVenta();
    const cambio = this.ventaService.getCambio(this.pagoRecibido?? 0);
    const cashierName = `${this.getUserState().firstName} ${this.getUserState().lastName}`;

    let productList: string = '';
    
    products.forEach(product => {
      const count = product.count.toString().padEnd(3, ' '); 
      const name = product.name.length > 13 ? product.name.substring(0, 13) : product.name.padEnd(12, ' ');
      const precio = product.unitPrice.toFixed(2).padEnd(6, ' '); 
      const total = product.total.toFixed(2); 
      productList += `${count}${name} ${precio} ${total}
`;
    });
    

    let ticket: string = `     *** ${companyName} ***
${ventaNumber} 
Fecha: ${fechaHora}
Cant. Nombre   Precio   Importe
${productList}
Total: $${totalVenta.toFixed(2)}
Recibido: $${this.pagoRecibido?.toFixed(2)}
Cambio: $${cambio.toFixed(2)}    
Cajero: ${cashierName}
    ¡Gracias por su compra!`;
    
    this.sendToPrinter(ticket);
  }

  sendToPrinter(ticket: string)
  {
    try
    {

      const encodedText = encodeURI(ticket); // Codificar en Base64
      console.log(encodedText);
      var S = "#Intent;scheme=rawbt;";
      var P =  "package=ru.a402d.rawbtprinter;end;";
     
      window.location.href="intent:"+encodedText+S+P;
    }
    catch(e)
    {
      console.error('Error al imprimir', e);
    }
  }

  /******************************************************** */
  ngOnInit(): void {
    this.metodoPago = "Efectivo";
  }

  onInputChange(value: string): void {
    const parsedValue = parseFloat(value);
    if (!isNaN(parsedValue) && parsedValue >= 0) {
      this.pagoRecibido = parsedValue;
    } else {
      this.pagoRecibido = 0; // O maneja el error de otra manera
    }
  }
  
  canFinishSale(): boolean
  {
    var terminarVenta = false;
    if(this.ventaService.saleProductsGrouped.length > 0 && 
        (this.pagoRecibido?? 0) >= this.ventaService.getTotalVenta())
    {
      terminarVenta = true;
    }
    return terminarVenta;
  }
  
  resetListaVenta()
  {
    this.ventaService.saleProductsGrouped = [];
    this.ventaService.totalVenta = 0;
    this.pagoRecibido = undefined;
    this.notaVenta = '';
    this.metodoPago = 'Efectivo';
  }
 
  enableEditing(item: any) {
    item.editing = true;
    this.tempProductCounter = item.count;
    setTimeout(() => {
      this.inputNumber?.nativeElement.focus();
      this.inputNumber?.nativeElement.select();
    }, 0);
  }

  disableEditing(item: Producto, event: any) {
    const newValue = this.inputNumber?.nativeElement.value;
    if (newValue > 0 && newValue != this.tempProductCounter) 
    {
      item.count = newValue; // Acepta el nuevo valor si es mayor que cero
      item.editing = false;  // Desactiva el modo de edición
      
      this.ventaService.updateNumOfProductos(item.productId, newValue);
    } 
    else 
    {
      if(newValue <= 0)
      {
        this.navigationService.showUIMessage('El valor debe ser mayor que cero.');
        item.count = this.tempProductCounter?? 0;
      } 
      item.editing = false;
      
    }
    event.preventDefault();
  }
  
  removeProduct(productId: number): void {
    this.ventaService.removeProductFromList(productId);
  }

  getUserState(): userDto
  {
    let userState = this.userState.getUserStateLocalStorage();
    return userState;
  }

  async finalizarVenta(): Promise<void>
  {
    const finVenta: boolean = this.canFinishSale();

    if(finVenta === true)
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
    else
    {
      this.navigationService.showUIMessage('No has agregado la cantidad de dinero recibida');
    }
  }
}
