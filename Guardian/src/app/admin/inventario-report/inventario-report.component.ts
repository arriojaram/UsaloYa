import { Component, OnDestroy, OnInit } from '@angular/core';
import { Inventory, InventoryProduct } from '../../dto/inventoryDto';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { first, Subject } from 'rxjs';
import { UserStateService } from '../../services/user-state.service';
import { NavigationService } from '../../services/navigation.service';
import { InventarioService } from '../../services/inventario.service.service';
import { userDto } from '../../dto/userDto';
import { AlertLevel, InventoryView, Roles } from '../../Enums/enums';
import { ProductService } from '../../services/product.service';
import { setUnitsInStockDto } from '../../dto/setUnitsInStockDto';
import { BarcodeDto as setInVentarioByBarcodeDto } from '../../dto/idDto';

@Component({
  selector: 'app-inventario-report',
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './inventario-report.component.html',
  styleUrl: './inventario-report.component.css'
})
export class InventarioReportComponent implements OnInit, OnDestroy{

  filteredProducts: InventoryProduct[] = [];
  products: InventoryProduct []= [];
  totalInventarioProds = 0;
  totalInventarioProdsUnits = 0;
  totalInventarioCash = 0;
  currentPage = 1;
  currentView: InventoryView = InventoryView.Other;
  moreItems: boolean | undefined;

  highlight = false;

  form = new FormGroup({
    codigo : new FormControl('', Validators.required)
  });

  userState: userDto;
  private unsubscribe$: Subject<void> = new Subject();
  filterBarcode: string | undefined;
  alertLevelCss: string = "";
  
  isCapturingStock: boolean = false;
  productIdEditing: number| undefined;
  editingStock: Record<number, boolean> = {};
  newProdStockVal: number | undefined;
  editingInventario: Record<number, boolean> = {};
  newProdInventoryVal: number | undefined;

  constructor( 
      private inventoryService: InventarioService,
      private productService: ProductService,
      private navigationService: NavigationService,
      private userService: UserStateService,) 
  {
    this.userState = userService.getUserStateLocalStorage();
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  ngOnInit(): void {
    this.userState = this.userService.getUserStateLocalStorage();
 
    if(this.userState.roleId < Roles.Admin)
      this.navigationService.showUIMessage("Petición incorrecta.");
    else
      this.getProductsTop50("-1");
 
  }

  setStockEqualInventory() {
    if (!confirm('¿Estás seguro de que quieres asignar los valores del inventario a las existencias de todos los productos?')) 
      return;

    this.inventoryService.setAllStockEqualsInventory(this.userState.companyId).pipe(first())
    .subscribe({
      complete:() => {
        this.navigationService.showUIMessage("Se igualaron los valores de las existencias al de los capturados en el inventario.", AlertLevel.Sucess);
        this.getProductsTop50("-1");
      },
      error:(err) => {
        if (err.status === 404) {  
          this.navigationService.showUIMessage("El producto no fue encontrado.");
        } else {
          this.navigationService.showUIMessage("Error al procesar la solicitud. Servidor no disponible" );
        }
      },
    });
  }

  openCaptureNewInventory(productId: number, code: string) {
    if(!this.isCapturingStock)
    {
      this.isCapturingStock = true;
      this.productIdEditing = productId;
    }
    if(this.isCapturingStock && this.productIdEditing != productId)
      return;

    
    this.editingInventario[productId] = !this.editingInventario[productId];
    if(!this.editingStock[productId])
      this.isCapturingStock = false;

   
    if(!this.editingInventario[productId] && this.newProdInventoryVal)
    {
      if(this.newProdInventoryVal !== 0)
      {
        let stockInfo:setInVentarioByBarcodeDto = {
           code: code,
           quantity: this.newProdInventoryVal
        };

        this.inventoryService.setProductInventarioValue(stockInfo, this.userState.companyId).pipe(first())
          .subscribe({
            next: (prodUpdate) => {
              this.navigationService.showUIMessage("Actualizado.", AlertLevel.Sucess);      
              this.newProdInventoryVal = undefined;
              let p = this.filteredProducts.find(p=>p.productId == productId);
              if(p)
              {
                p.unitsInVentario = prodUpdate.unitsInVentario;
              }
            },
            error:(err) => {
              this.navigationService.showUIMessage("Error no se pudo actualizar.", AlertLevel.Error);      
              this.newProdInventoryVal = undefined;
              console.log("No se pudo actualizar el stock " + err);
            },
          });
              
      }
    }
  }

  openCaptureNewStock(productId: number) {
    
    if(!this.isCapturingStock)
    {
      this.isCapturingStock = true;
      this.productIdEditing = productId;
    }
    if(this.isCapturingStock && this.productIdEditing != productId)
      return;

   
    this.editingStock[productId] = !this.editingStock[productId];
    if(!this.editingStock[productId])
      this.isCapturingStock = false;

    if(!this.editingStock[productId] && this.newProdStockVal)
    {
      if(this.newProdStockVal !== 0)
      {
        let stockInfo:setUnitsInStockDto = {isHardReset:false, productId, unitsInStock:this.newProdStockVal};
        
        this.inventoryService.setUnitsInStockToProduct(stockInfo, this.userState.companyId).pipe(first())
          .subscribe({
            next: (newStock) => {
              this.navigationService.showUIMessage("Actualizado.", AlertLevel.Sucess);      
              this.newProdStockVal = undefined;
              let p = this.filteredProducts.find(p=>p.productId == productId);
              if(p)
              {
                p.unitsInStock = newStock;
                p.inVentarioAlertLevel = p.unitsInStock <= 0 ? InventoryView.Critical:
                                         p.unitsInStock <= p.alertaStockNumProducts ? InventoryView.Warning
                                        : InventoryView.Other;
              }
            },
            error:(err) => {
              this.navigationService.showUIMessage("Error no se pudo actualizar.", AlertLevel.Error);      
              this.newProdStockVal = undefined;
              console.log("No se pudo actualizar el stock " + err);
            },
          }); 
      }
    }
  }

  get codigo(){
    return this.form.get('codigo') as FormControl;
  }

  async addProductToInventory()
  {
    let code = this.codigo.value;
    if(code)
    {  
      this.inventoryService.setProductInventarioValue(code.trim(), this.userState.companyId).pipe(first())
      .subscribe({
        next:(product) => {
          if(product)
          {   
            this.highlight = true;
            setTimeout(() => this.highlight = false, 1000);

            this.products = [];
            this.products.push(product);
            this.filteredProducts = this.products;
            this.codigo.setValue(undefined);
          }
        },
        error:(err) => {
          if (err.status === 404) {  
            this.navigationService.showUIMessage("El producto no fue encontrado.");
          } else {
            this.navigationService.showUIMessage("Error al procesar la solicitud. Servidor no disponible" );
          }
        },
      });
    }
  }

  setToZeroAllInventory()
  {
    if (!confirm('¿Estás seguro de que quieres reiniciar los valores del inventario a cero?')) 
      return;

    this.inventoryService.setToZeroAllInventory(this.userState.companyId).pipe(first())
      .subscribe({
        complete:() => {
          this.navigationService.showUIMessage("Se inicializaron todos los valores del inventario a cero.", AlertLevel.Sucess);
          this.getProductsTop50("-1");
        },
        error:(err) => {
          if (err.status === 404) {  
            this.navigationService.showUIMessage("El producto no fue encontrado.");
          } else {
            this.navigationService.showUIMessage("Error al procesar la solicitud. Servidor no disponible" );
          }
        },
      });
  }

  filtarProducto()
  {
    let keyword = this.filterBarcode?? "-1";
    if(keyword != '-1')
      this.getProductsTop50(keyword);
  }


  loadMore()
  {
    this.currentPage++;
    switch (this.currentView) {
      case InventoryView.Critical:
        this.loadCriticalProducts(this.currentPage);
        break;
      case InventoryView.Warning:
        this.loadWarningProducts(this.currentPage);
        break;
      default:
        this.loadAll(this.currentPage);
        break;
    }
  }

  loadProductsByAlertId(pageNumber: number, alertId: number)
  {
    if(pageNumber == 1)
      this.filteredProducts = [];

    this.inventoryService.getInventoryByAlertId(pageNumber, alertId, this.userState.companyId).pipe(first())
    .subscribe({
      next:(data: InventoryProduct[]) => {
        this.products = data;
        this.filteredProducts.push(...this.products);

        if((!data || data.length == 0) && this.currentPage == 1)
        {
          let estadoDesc = "Crítico";
          if(alertId == 2)
              estadoDesc = "Bajo";
          this.navigationService.showUIMessage(`No se encontraron productos con estado "${estadoDesc}" en existencias.`);  
        }
        this.moreItems = true;
        if((!data || data.length == 0) && this.currentPage > 1)
        {
          this.moreItems = false;
        }
        this.filterBarcode= undefined;
        
      },
      error:(err) => {
        this.filterBarcode= undefined;
        if (err.status === 404) {  
          if(this.currentPage > 1)
            this.moreItems = false;
          else
            this.navigationService.showUIMessage("El producto no fue encontrado.");
        } else {
          this.navigationService.showUIMessage("Error al procesar la solicitud. Servidor no disponible" );
        }
      },
    });
  }

  loadCriticalProducts(pageNumber: number)
  {
    this.currentPage = pageNumber;
    this.currentView = InventoryView.Critical;
    this.loadProductsByAlertId(pageNumber, InventoryView.Critical);
  }

  loadWarningProducts(pageNumber: number)
  {
    this.currentPage = pageNumber;
    this.currentView = InventoryView.Warning;
    this.loadProductsByAlertId(pageNumber, InventoryView.Warning);
  }

  loadAll(pageNumber:number)
  {
    if(pageNumber == 1)
      this.filteredProducts = [];

    this.currentPage = pageNumber;
    this.currentView = InventoryView.Other;
    this.inventoryService.getInventoryAll(pageNumber, this.userState.companyId).pipe(first())
    .subscribe({
      next:(data: Inventory) => {
        if(data)
        {
          this.products = data.products;
          this.filteredProducts.push(...this.products);
          this.moreItems = true;
        }
        else
        {
          if(this.currentPage > 1)
            this.moreItems = false;
          else
            this.navigationService.showUIMessage('No se encontraron productos con estado critico en existencias.');  
        }
        this.filterBarcode= undefined;
        
      },
      error:(err) => {
        this.filterBarcode= undefined;
        if (err.status === 404) {  
          if(this.currentPage > 1)
            this.moreItems = false;
          else
            this.navigationService.showUIMessage("El producto no fue encontrado.");
        } else {
          this.navigationService.showUIMessage("Error al procesar la solicitud. Servidor no disponible" );
        }
      },
    });
  }

  resetProdsTotal()
  {
    this.totalInventarioProds = 0;
    this.totalInventarioCash = 0;
    this.totalInventarioProdsUnits = 0;
  }

  setNewProdsTotal(data: Inventory)
  {
    this.totalInventarioProds = data.totalProducts;
    this.totalInventarioProdsUnits = data.totalProductUnits;
    this.totalInventarioCash = data.totalCash;
  }

  getProductsTop50(keyword: string): void {
    this.products = [];
    this.filteredProducts = [];
    this.resetProdsTotal();
    
    let pageNumber = 1;
    
    this.inventoryService.getInventoryTop50(this.userState.companyId, keyword, pageNumber).pipe(first())
    .subscribe({
      next:(data: Inventory) => {
        if(data)
        {
          this.products = data.products;
          this.filteredProducts = this.products;
          this.setNewProdsTotal(data);
        }
        else
        {
          this.navigationService.showUIMessage('No se encontraron productos');  
        }
        this.filterBarcode= undefined;
        this.moreItems = true;
      },
      error:(err) => {
        this.filterBarcode= undefined;
        if (err.status === 404) {  
          this.navigationService.showUIMessage("El producto no fue encontrado.");
        } else {
          this.navigationService.showUIMessage("Error al procesar la solicitud. Servidor no disponible" );
        }
      },
    });
  }


}
