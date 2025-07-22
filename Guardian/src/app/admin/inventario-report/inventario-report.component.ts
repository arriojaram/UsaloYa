import { Component, OnDestroy, OnInit } from '@angular/core';
import { Inventory, InventoryProduct } from '../../dto/inventoryDto';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { first, Subject } from 'rxjs';
import { UserStateService } from '../../services/user-state.service';
import { NavigationService } from '../../services/navigation.service';
import { InventarioService } from '../../services/inventario.service.service';
import { userDto } from '../../dto/userDto';
import { AlertLevel, CompanyStatus, InventoryView, Roles } from '../../Enums/enums';
import { ProductService } from '../../services/product.service';
import { setUnitsInStockDto } from '../../dto/setUnitsInStockDto';
import { IdDto, BarcodeDto as setInVentarioByBarcodeDto } from '../../dto/idDto';
import { productCategoryDto } from '../../dto/productCategoryDto';
import { ProductCategoryService } from '../../services/product-category.service';
import { environment } from '../../environments/enviroment';
import { TranslateService } from '@ngx-translate/core';


@Component({
  selector: 'app-inventario-report',
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './inventario-report.component.html',
  styleUrl: './inventario-report.component.css'
})
export class InventarioReportComponent implements OnInit, OnDestroy {

  filteredProducts: InventoryProduct[] = [];
  products: InventoryProduct[] = [];
  totalInventarioProds = 0;
  totalInventarioProdsUnits = 0;
  totalInventarioCash = 0;
  currentPage = 1;
  currentView: InventoryView = InventoryView.Other;
  moreItems: boolean | undefined;
  categoryList: productCategoryDto[] = [];
  cStatus = CompanyStatus;
  rol = Roles;



  highlight = false;
  selectedCategoryId: number = 0;

  form = new FormGroup({
    codigo: new FormControl('', Validators.required)
  });

  userState: userDto;
  private unsubscribe$: Subject<void> = new Subject();
  filterBarcode: string | undefined;
  alertLevelCss: string = "";

  isCapturingStock: boolean = false;
  productIdEditing: number | undefined;
  editingStock: Record<number, boolean> = {};
  newProdStockVal: number | undefined;
  editingInventario: Record<number, boolean> = {};
  newProdInventoryVal: number | undefined;

  constructor(
    private inventoryService: InventarioService,
    private productService: ProductService,
    private navigationService: NavigationService,
    private userService: UserStateService,
    private categoryService: ProductCategoryService,
    private translate: TranslateService
  ) {
    this.userState = userService.getUserStateLocalStorage();
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  ngOnInit(): void {
    this.userState = this.userService.getUserStateLocalStorage();
    this.currentPage = 1;
    if (this.userState.roleId < Roles.User)
      this.navigationService.showUIMessage(this.translate.instant('inventory_report.request_incorrect'));
    else
      this.getProductsTop50("-1");

    this.getCategories();
    this.navigationService.showFreeLicenseMsg(this.userState.companyStatusId ?? 0);
  }



  private getCategories(): void {
    if (this.userState != null) {
      if (this.userState.companyStatusId != this.cStatus.Free) {
        this.categoryService.getAll(this.userState.companyId, '-1').pipe(first())
          .subscribe(users => {
            this.categoryList = users.sort((a, b) => (a.name ?? '').localeCompare((b.name ?? '')));

          });
      }
    }
    else {
      console.error("Estado de usuario invalido.");
    }
  }

  setStockEqualInventory() {
    if (!confirm('Los productos modificados serán actualizados. El valor del "Inventario" será asignago al valor "Existencias" ¿Estás seguro de que quieres continuar?'))
      return;

    this.inventoryService.setAllStockEqualsInventory(this.userState.companyId).pipe(first())
      .subscribe({
        complete: () => {
          this.navigationService.showUIMessage(this.translate.instant('Inventary_report.inventory_stock_synced'), AlertLevel.Sucess);
          this.currentPage = 1;
          this.getProductsTop50("-1");
        },
        error: (err) => {
          if (err.status === 404) {
            this.navigationService.showUIMessage(this.translate.instant('product.not_found'));
          } else {
            this.navigationService.showUIMessage(this.translate.instant('inventory_report.server_error'));
          }
        },
      });
  }

  filterProductsByCategoryId(pageNumber: number) {
    this.navigationService.showFreeLicenseMsg(this.userState.companyStatusId ?? 0);
    let categoryId = this.selectedCategoryId;
    if (categoryId == 0) {
      this.currentPage = 1;
      this.getProductsTop50("-1");
      return;
    }

    if (pageNumber == 1)
      this.filteredProducts = [];

    this.inventoryService.getInventoryByCategoryId(pageNumber, categoryId, this.userState.companyId).pipe(first())
      .subscribe({
        next: (data: InventoryProduct[]) => {

          this.products = data;
          this.filteredProducts.push(...this.products);

          if ((!data || data.length == 0) && this.currentPage == 1) {
            this.navigationService.showUIMessage(this.translate.instant('inventory_report.no_products_by_category'));
          }
          this.moreItems = true;
          if ((!data || data.length == 0) && this.currentPage > 1) {
            this.moreItems = false;
          }
          this.filterBarcode = undefined;
          this.currentView = InventoryView.ByCategory;
        },
        error: (err) => {
          this.filterBarcode = undefined;
          if (err.status === 404) {
            if (this.currentPage > 1)
              this.moreItems = false;
            else
              this.navigationService.showUIMessage(this.translate.instant('inventory_report.product_not_found'));
          } else {
            this.navigationService.showUIMessage(this.translate.instant('inventory_report.server_error'));
          }
        },
      });
  }

  openCaptureNewInventory(productId: number, code: string) {
    if (!this.isCapturingStock) {
      this.isCapturingStock = true;
      this.productIdEditing = productId;
    }
    if (this.isCapturingStock && this.productIdEditing != productId)
      return;

    this.editingInventario[productId] = !this.editingInventario[productId];
    if (!this.editingInventario[productId])
      this.isCapturingStock = false;


    if (!this.editingInventario[productId] && this.newProdInventoryVal != null) {
      if (this.newProdInventoryVal >= 0) { 
         //Obtiene el producto actual
        const product = this.filteredProducts.find(p => p.productId === productId);

        //No permite cantidades negativas
        if (this.newProdInventoryVal < 0) {
          this.navigationService.showUIMessage(this.translate.instant('Inventary_report.negative_quantity_not_allowed'), AlertLevel.Error);
          return;
        }

        //Se valida si el tipo de medida es por unidad
        if (product?.measure === 1 && !Number.isInteger(this.newProdInventoryVal)) {
          this.navigationService.showUIMessage(this.translate.instant('Inventary_report.invalid_quantity_unit'), AlertLevel.Error);
          return;
        }

        let stockInfo: setInVentarioByBarcodeDto = {
          code: code,
          quantity: this.newProdInventoryVal
        };

        this.inventoryService.setProductInventarioValue(stockInfo, this.userState.companyId).pipe(first())
          .subscribe({
            next: (prodUpdate) => {
              this.navigationService.showFreeLicenseMsg(this.userState.companyStatusId ?? 0);
              this.navigationService.showUIMessage(this.translate.instant('Inventary_report.updated'), AlertLevel.Sucess);
              this.newProdInventoryVal = undefined;
              let p = this.filteredProducts.find(p => p.productId == productId);
              if (p) {
                p.unitsInVentario = prodUpdate.unitsInVentario;
                p.isInVentarioUpdated = true;
              }
            },
            error: (err) => {
              this.navigationService.showUIMessage(this.translate.instant('Inventary_report.updated_error'), AlertLevel.Error);
              this.newProdInventoryVal = undefined;
              console.log("No se pudo actualizar el stock " + err);
            },
          });

      }
    }
  }

  openCaptureNewStock(productId: number) {
    if (!this.isCapturingStock) {
      this.isCapturingStock = true;
      this.productIdEditing = productId;
    }

    if (this.isCapturingStock && this.productIdEditing != productId)
      return;

    this.editingStock[productId] = !this.editingStock[productId];

    if (!this.editingStock[productId])
      this.isCapturingStock = false;

    if (!this.editingStock[productId] && this.newProdStockVal) {
      if (this.newProdStockVal !== 0) {
        //Obtiene el producto actual
        const product = this.filteredProducts.find(p => p.productId === productId);

        //No permite cantidades negativas
        if (this.newProdStockVal < 0) {
          this.navigationService.showUIMessage(this.translate.instant('Inventary_report.negative_quantity_not_allowed'), AlertLevel.Error);
          return;
        }

        //Se valida si el tipo de medida es por unidad
        if (product?.measure === 1 && !Number.isInteger(this.newProdStockVal)) {
          this.navigationService.showUIMessage(this.translate.instant('Inventary_report.invalid_quantity_unit'), AlertLevel.Error);
          return;
        }

        this.setNewProdStockVal(productId, this.newProdStockVal, false);
      }
    }
  }


  updateTable(productId: number, newStock: number) {
    this.navigationService.showUIMessage(this.translate.instant('Inventary_report.updated'), AlertLevel.Sucess);
    this.newProdStockVal = undefined;
    let p = this.filteredProducts.find(p => p.productId == productId);
    if (p) {
      p.unitsInStock = newStock;
        p.unitsInVentario = 0;
        p.inVentarioAlertLevel = p.unitsInStock <= 0 ? InventoryView.Critical :
        p.unitsInStock <= p.alertaStockNumProducts ? InventoryView.Warning
          : InventoryView.Other;
      p.isInVentarioUpdated = false;
    }

  }

  setNewProdStockVal(productId: number, newProdStockVal: number, isHardReset: boolean) {
    let stockInfo: setUnitsInStockDto = { isHardReset, productId, unitsInStock: newProdStockVal };

    this.inventoryService.setUnitsInStockToProduct(stockInfo, this.userState.companyId).pipe(first())
      .subscribe({
        next: (newStock) => {
          this.updateTable(productId, newStock);
        },
        error: (err) => {
          this.navigationService.showUIMessage(this.translate.instant('Inventary_report.updated_error'), AlertLevel.Error);
          this.newProdStockVal = undefined;
          console.log("No se pudo actualizar el stock " + err);
        },
      });
  }

  syncStockWinventoryByProduct(productId: number) {
    let productInfo: IdDto = { id: productId };

    this.inventoryService.setUnitsInStockByProductId(productInfo, this.userState.companyId).pipe(first())
      .subscribe({
        next: (newStock) => {
          this.updateTable(productId, newStock);
        },
        error: (err) => {
          this.navigationService.showUIMessage(this.translate.instant('Inventary_report.updated_error'), AlertLevel.Error);
          this.newProdStockVal = undefined;
          console.log("No se pudo actualizar el stock " + err);
        },
      });
  }

  get codigo() {
    return this.form.get('codigo') as FormControl;
  }

  async addProductToInventory() {
    this.navigationService.showFreeLicenseMsg(this.userState.companyStatusId ?? 0);
    let code = this.codigo.value;
    if (code) {
      let stockInfo: setInVentarioByBarcodeDto = {
        code: code,
        quantity: -1
     };
      this.inventoryService.setProductInventarioValue(stockInfo, this.userState.companyId).pipe(first())
        .subscribe({
          next: (product) => {
            if (product) {
              this.highlight = true;
              setTimeout(() => this.highlight = false, 1000);

              this.products = [];
              this.products.push(product);
              this.filteredProducts = this.products;
              this.codigo.setValue(undefined);
            }
          },
          error: (err) => {
            if (err.status === 404) {
              this.navigationService.showUIMessage(this.translate.instant('inventory_report.product_not_found'));
            } else {
              this.navigationService.showUIMessage(this.translate.instant('inventory_report.server_error'));
            }
          },
        });
    }
  }

  setToZeroAllInventory() {
    if (!confirm('¿Estás seguro de que quieres reiniciar los valores del inventario a cero?'))
      return;

    this.inventoryService.setToZeroAllInventory(this.userState.companyId).pipe(first())
      .subscribe({
        complete: () => {
          this.navigationService.showUIMessage(this.translate.instant('Inventary_report.inventory_initialized_zero'), AlertLevel.Sucess);
          this.currentPage = 1;
          this.getProductsTop50("-1");
        },
        error: (err) => {
          if (err.status === 404) {
            this.navigationService.showUIMessage(this.translate.instant('inventory_report.product_not_found'));
          } else {
            this.navigationService.showUIMessage(this.translate.instant('inventory_report.server_error'));
          }
        },
      });
  }

  filtarProducto() {
    let keyword = this.filterBarcode ?? "-1";
    this.currentPage = 1;
    if (keyword != '-1')
      this.getProductsTop50(keyword);
  }


  loadMore() {
    this.navigationService.showFreeLicenseMsg(this.userState.companyStatusId ?? 0);
    this.currentPage++;
    switch (this.currentView) {
      case InventoryView.Critical:
        this.loadCriticalProducts(this.currentPage);
        break;
      case InventoryView.Warning:
        this.loadWarningProducts(this.currentPage);
        break;
      case InventoryView.ByCategory:
        this.filterProductsByCategoryId(this.currentPage);
        break;
      case InventoryView.WithDiscrepancia:
        this.loadWithDiscrepancias(this.currentPage);
        break;
      case InventoryView.ItemsUpdated:
        this.getInventarioItemsUpdated(this.currentPage);
        break;
      default:
        this.getProductsTop50('-1');
        break;
    }

  }

  loadProductsByAlertId(pageNumber: number, alertId: number) {
    this.navigationService.showFreeLicenseMsg(this.userState.companyStatusId ?? 0);
    if (pageNumber == 1)
      this.filteredProducts = [];

    this.inventoryService.getInventoryByAlertId(pageNumber, alertId, this.userState.companyId).pipe(first())
      .subscribe({
        next: (data: InventoryProduct[]) => {
          this.products = data;
          this.filteredProducts.push(...this.products);

          if ((!data || data.length == 0) && this.currentPage == 1) {
            let estadoDesc = "Crítico";
            if (alertId == 2)
              estadoDesc = "Bajo";
            this.navigationService.showUIMessage(this.translate.instant('Inventary_report.no_products_existing_by_status') + estadoDesc);
          }
          this.moreItems = true;
          if ((!data || data.length == 0) && this.currentPage > 1) {
            this.moreItems = false;
          }
          this.filterBarcode = undefined;

        },
        error: (err) => {
          this.filterBarcode = undefined;
          if (err.status === 404) {
            if (this.currentPage > 1)
              this.moreItems = false;
            else
              this.navigationService.showUIMessage(this.translate.instant('inventory_report.product_not_found'));
          } else {
            this.navigationService.showUIMessage(this.translate.instant('inventory_report.server_error'));
          }
        },
      });
  }

  loadCriticalProducts(pageNumber: number) {
    this.currentPage = pageNumber;
    this.currentView = InventoryView.Critical;
    this.loadProductsByAlertId(pageNumber, InventoryView.Critical);
  }

  loadWarningProducts(pageNumber: number) {
    this.currentPage = pageNumber;
    this.currentView = InventoryView.Warning;
    this.loadProductsByAlertId(pageNumber, InventoryView.Warning);
  }

  getInventarioItemsUpdated(pageNumber: number) {
    this.navigationService.showFreeLicenseMsg(this.userState.companyStatusId ?? 0);
    if (pageNumber == 1)
      this.filteredProducts = [];

    this.currentPage = pageNumber;
    this.currentView = InventoryView.ItemsUpdated;
    this.inventoryService.getInventarioItemsUpdated(pageNumber, this.userState.companyId).pipe(first())
      .subscribe({
        next: (data: InventoryProduct[]) => {
          if (data) {
            this.products = data;
            this.filteredProducts.push(...this.products);
            this.moreItems = true;
          }
          else {
            if (this.currentPage > 1)
              this.moreItems = false;
            else
              this.navigationService.showUIMessage(this.translate.instant('inventory_report.products_not_found'));
          }
          this.filterBarcode = undefined;

        },
        error: (err) => {
          this.filterBarcode = undefined;
          if (err.status === 404) {
            if (this.currentPage > 1)
              this.moreItems = false;
            else
              this.navigationService.showUIMessage(this.translate.instant('inventory_report.product_not_found'));
          } else {
            this.navigationService.showUIMessage(this.translate.instant('inventory_report.server_error'));
          }
        },
      });
  }

  loadWithDiscrepancias(pageNumber: number) {
    this.navigationService.showFreeLicenseMsg(this.userState.companyStatusId ?? 0);
    if (pageNumber == 1)
      this.filteredProducts = [];

    this.currentPage = pageNumber;
    this.currentView = InventoryView.WithDiscrepancia;
    this.inventoryService.getInventarioWithDiscrepancias(pageNumber, this.userState.companyId).pipe(first())
      .subscribe({
        next: (data: InventoryProduct[]) => {
          if (data) {
            this.products = data;
            this.filteredProducts.push(...this.products);
            this.moreItems = true;
          }
          else {
            if (this.currentPage > 1)
              this.moreItems = false;
            else
              this.navigationService.showUIMessage(this.translate.instant('inventory_report.products_not_found'));
          }
          this.filterBarcode = undefined;

        },
        error: (err) => {
          this.filterBarcode = undefined;
          if (err.status === 404) {
            if (this.currentPage > 1)
              this.moreItems = false;
            else
              this.navigationService.showUIMessage(this.translate.instant('inventory_report.product_not_found'));
          } else {
            this.navigationService.showUIMessage(this.translate.instant('inventory_report.server_error'));
          }
        },
      });
  }

  resetProdsTotal() {
    this.totalInventarioProds = 0;
    this.totalInventarioCash = 0;
    this.totalInventarioProdsUnits = 0;
  }

  setNewProdsTotal(data: Inventory) {
    this.totalInventarioProds = data.totalProducts;
    this.totalInventarioProdsUnits = data.totalProductUnits;
    this.totalInventarioCash = data.totalCash;
  }

  getProductsTop50(keyword: string): void {
    this.navigationService.showFreeLicenseMsg(this.userState.companyStatusId ?? 0);
    if (this.currentPage == 1) {
      this.products = [];
      this.filteredProducts = [];
      this.resetProdsTotal();
    }

    this.currentView = InventoryView.Other;
    this.inventoryService.getInventoryTop50(this.userState.companyId, keyword, this.currentPage)
      .pipe(first())
      .subscribe({
        next: (data: Inventory) => {
          if (data) {

            this.products = data.products;
            this.filteredProducts = this.products;
            this.setNewProdsTotal(data);
          }
          else {
            this.navigationService.showUIMessage(this.translate.instant('inventory_report.no_more_products'));
          }
          this.filterBarcode = undefined;
          this.moreItems = true;
        },
        error: (err) => {
          this.filterBarcode = undefined;
          this.moreItems = false;
          if (err.status === 404) {
            if (keyword != '-1')
              this.navigationService.showUIMessage(this.translate.instant('inventory_report.product_not_found'));
          }
          else {
            this.navigationService.showUIMessage(this.translate.instant('inventory_report.server_error'));
          }
        },
      });
  }

  restToDefaultValues() {
    this.currentView = InventoryView.Other;
    this.selectedCategoryId = 0;
    this.getProductsTop50('-1')
  }


}
