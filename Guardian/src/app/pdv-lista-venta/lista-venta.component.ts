import { Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
import { SaleService } from '../services/sale.service';
import { CommonModule } from '@angular/common';
import { OfflineDbStore } from '../services/offline-db-store.service';
import { UserStateService } from '../services/user-state.service';
import { first } from 'rxjs';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { environment } from '../environments/enviroment';
import { userDto } from '../dto/userDto';
import { NavigationService } from '../services/navigation.service';
import { Producto } from '../dto/producto';
import { customerDto } from '../dto/customerDto';
import { CustomerService } from '../services/customer.service';

@Component({
  selector: 'app-lista-venta',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './lista-venta.component.html',
  styleUrl: './lista-venta.component.css'
})
export class ListaVentaComponent implements OnInit {


  userState: userDto;
  metodoPago: string;
  pagoRecibido: number | undefined;
  notaVenta: string = "";
  isHidden?: boolean;
  message?: string;
  numVenta: string;
  messageClass: string = "alert  alert-success mt-2";
 
  @ViewChild('inputNumber') inputNumber?: ElementRef;
  tempProductCounter: number | undefined;
  isSelectingCustomer: boolean = false;
  
  custButtonClass: string = 'btn btn-success';
  custButtonLabel: string = '+';

  customerName: string = '';  // Almacena el texto ingresado
  selectedCustomer: customerDto | undefined;  
  filteredCustomer: customerDto[] = [];  
new: any;

  constructor(
    private router: Router,
    public ventaService: SaleService,
    private offlineDbStore: OfflineDbStore,
    private userStateService: UserStateService,
    private navigationService: NavigationService,
    private customerService: CustomerService
  )
  {
    this.isHidden = true;
    this.metodoPago = "";
    this.numVenta = "-1";
    this.userState = this.userStateService.getUserStateLocalStorage();
  }

  ngOnInit(): void {
    this.metodoPago = "Efectivo";
    this.userState = this.userStateService.getUserStateLocalStorage();
  }

  onSelectPrice(event: Event, productId: number): void {
    const selectElement = event.target as HTMLSelectElement;
    const selectedPriceLevel = selectElement.value;
    var newPriceLevel: number = parseFloat(selectedPriceLevel);

    this.ventaService.updateProductPrice(productId, newPriceLevel);
  }

  searchCustomers() {
    this.filteredCustomer = [];
    let searchItem = this.customerName.trim() == '' ? '-1' : this.customerName.trim();
    this.customerService.getAllCustomer(this.userState.companyId, searchItem).pipe(first())
    .subscribe({
      next: (customerList) =>{
        this.filteredCustomer = customerList;
        if(customerList.length == 0)
          this.navigationService.showUIMessage(`No hay clientes con el nombre: ${this.customerName}`);
      },
      error: (err) => {
        this.navigationService.showUIMessage(err.error);
      },
    });
  }

  selectCustomer(customer: customerDto) {

    this.selectedCustomer = customer;
    this.ventaService.customerId = customer.customerId;
    this.custButtonClass = 'btn btn-danger';
    this.custButtonLabel = '-';
    this.isSelectingCustomer = false;
    this.filteredCustomer = [];

  }

  showSearchCustomerPanel(): void {
    this.isSelectingCustomer = !this.isSelectingCustomer;
    this.custButtonClass = this.isSelectingCustomer ? 'btn btn-danger' : 'btn btn-success';
    this.custButtonLabel = this.isSelectingCustomer ? '-' : '+';
    if(this.isSelectingCustomer)
    {
      this.searchCustomers();
      this.selectedCustomer = undefined;
      this.ventaService.customerId = 0;
      this.ventaService.resetProductPrice();
    }
   
  }

  /****************** TICKET *****************************/
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
    const companyName = this.userState.companyName;
    const ventaNumber = this.numVenta;
    const fechaHora = new Date().toLocaleString();  // Asumiendo que `fechaHora` se calcula así
    const products = this.ventaService.saleProductsGrouped;
    const totalVenta = this.ventaService.getTotalVenta();
    const cambio = this.ventaService.getCambio(this.pagoRecibido?? 0);
    const cashierName = `${this.userState.firstName} ${this.userState.lastName}`;

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
      
      var S = "#Intent;scheme=rawbt;";
      var P =  "package=ru.a402d.rawbtprinter;end;";
     
      window.location.href="intent:"+encodedText+S+P;
    }
    catch(e)
    {
      console.error('Error al imprimir', e);
    }
  }

  /****************** END TICKET *******************************/

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
    this.isSelectingCustomer = false;
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

  async finalizarVenta(): Promise<void>
  {
    const finVenta: boolean = this.canFinishSale();

    if(finVenta === true)
    {
      this.isHidden = false;
      
      this.ventaService.finishSale(this.userState.userId, this.userState.companyId, this.notaVenta?? '', this.metodoPago?? 'Efectivo').pipe(first())
      .subscribe(
        {
          next: (response) => 
          {
            
            this.message = `Venta registrada: ${response}`;
            this.numVenta = 'Num. Venta: ' + response;
            this.messageClass = "alert  alert-success mt-2";
            this.showTicket();
          
            // Reload catalog
            this.ventaService.cacheProductCatalog(this.userState.companyId);
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
