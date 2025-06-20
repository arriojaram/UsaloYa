import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { Producto } from '../../dto/producto';
import { NgClass, NgFor, NgIf } from '@angular/common';
import { UserStateService } from '../../services/user-state.service';
import { userDto } from '../../dto/userDto';
import { NavigationService } from '../../services/navigation.service';
import { first } from 'rxjs';
import { AlertLevel, CompanyStatus, Roles } from '../../Enums/enums';
import { InventarioService } from '../../services/inventario.service.service';
import { setUnitsInStockDto } from '../../dto/setUnitsInStockDto';
import { productCategoryDto } from '../../dto/productCategoryDto';
import { ProductCategoryService } from '../../services/product-category.service';

@Component({
    selector: 'app-product-management',
    templateUrl: './product-management.component.html',
    styleUrls: ['./product-management.component.css'],
    imports: [ReactiveFormsModule, FormsModule, NgFor, NgIf, NgClass]
})
export class ProductManagementComponent implements OnInit {

  categoryList: productCategoryDto[] = [];
  categoryListFilter: productCategoryDto[] = [];
  productForm: FormGroup;
  products: Producto[] = [];
  selectedProduct: Producto | null = null;
  userState: userDto;
  showAddInventarioBox: boolean = false;

  pageNumber: number = 1;
  rol = Roles;
  cStatus = CompanyStatus;
  selectedCategoryId: number = 0;
  moreItems: boolean | undefined;
  

  constructor(
    private fb: FormBuilder, 
    private productService: ProductService,
    private userService: UserStateService,
    public navigationService: NavigationService,
    private inventoryService: InventarioService,
    private categoryService: ProductCategoryService
  ) 
  {
    this.userState = userService.getUserStateLocalStorage();
    this.productForm = this.initProductForm();
  }

  initProductForm() : FormGroup
  {
    return this.fb.group({
      productId: [0],
      name: ['', Validators.required],
      description: [''],
      categoryId: [0],
      supplierId: [null],
      buyPrice: [0, Validators.required],
      unitPrice: [0, Validators.required],
      unitPrice1: [null],
      unitPrice2: [null],
      unitPrice3: [null],
      unitsInStock: [0, Validators.required],
      discontinued: [false, Validators.required],
      imgUrl: [''],
      weight: [null],
      sku: [null],
      barcode: ['', Validators.required],
      brand: [''],
      color: [''],
      size: [''],
      companyId: [1, Validators.required],
      lowInventoryStart: [0],
      addToInventoryVal: [0]
    });
  }

  ngOnInit(): void {
    this.pageNumber = 1;
    this.moreItems = true;
    this.searchProductsInternal('-1');
    this.navigationService.checkScreenSize();
    this.navigationService.showFreeLicenseMsg(this.userState.companyStatusId?? 0);
    
    this.getCategories();
  }

  openCaptureInventory() {
    this.showAddInventarioBox=!this.showAddInventarioBox;
    if(!this.showAddInventarioBox && this.productForm.get('addToInventoryVal'))
    {
      let addVal = this.productForm.get('addToInventoryVal')?.value;
      let productId = this.productForm.get('productId')?.value;
      //Si esta abierto y se hace click entonces sumar o restar el valor proporcionado
      if(addVal !== 0)
      {
        let stockInfo:setUnitsInStockDto = {isHardReset:false, productId, unitsInStock:addVal};

        this.inventoryService.setUnitsInStockToProduct(stockInfo, this.userState.companyId).pipe(first())
          .subscribe({
            next: (newStock) => {
              this.navigationService.showFreeLicenseMsg(this.userState.companyStatusId?? 0);
              this.navigationService.showUIMessage("Se agregaron las nuevas configuraciones a las existencias del producto.", AlertLevel.Sucess);      
              this.productForm.get('addToInventoryVal')?.setValue(0);
              this.productForm.get('unitsInStock')?.setValue(newStock);
            },
            error:(err) => {
              this.navigationService.showUIMessage("No se pudo actualizar el inventario.", AlertLevel.Error);      
              this.productForm.get('addToInventoryVal')?.setValue(0);
              console.log("No se pudo actualizar el inventario " + err);
            },
          });
      }
    }
  }

  filterProducts(): void {
    this.navigationService.showFreeLicenseMsg(this.userState.companyStatusId?? 0);
    this.pageNumber = 1;
    this.moreItems = true;
    let categoryId = this.selectedCategoryId;
    let companyId = this.userState.companyId;
    this.productService.filterProducts(this.pageNumber, companyId, categoryId).pipe(first())
      .subscribe({
        next:(products) => {
          if(products.length == 0)
            this.navigationService.showUIMessage('No hay productos en la categoria seleccionada.');
          
          this.products = products.sort((a,b) => a.name.localeCompare(b.name));
          }
      });
  }

  searchProducts(): void {
    this.pageNumber = 1;
    this.moreItems = true;
    let keyword = this.navigationService.searchItem;
    if (!keyword || keyword.trim() === "") {
      keyword="-1";
    }
    this.searchProductsInternal(keyword);
    
  }
  
  loadMore(): void {
    this.navigationService.showFreeLicenseMsg(this.userState.companyStatusId?? 0);
    this.pageNumber++;
    if(this.selectedCategoryId > 0)
    {
      this.appendToFilteredResults();
    }
    else
    {
      let keyword = this.navigationService.searchItem;
      if (!keyword || keyword.trim() === "") {
        keyword="-1";
      }
  
      this.appendToSearchResults(keyword);
    }
  }

  private appendToFilteredResults(): void {
    this.productService.filterProducts(this.pageNumber, this.userState.companyId, this.selectedCategoryId).pipe(first())
    .subscribe({
      next: (products) => {
        if(products.length == 0)
        {
          this.moreItems = false;
          return;
        }
        this.products = this.products.concat(products.sort((a,b) => a.name.localeCompare(b.name)));
      }
    });
  }

  private appendToSearchResults(name: string): void {
    this.productService.searchProducts4List(this.pageNumber, this.userState.companyId, name).pipe(first())
    .subscribe({
      next: (products) => {
        if(products.length == 0)
        {
          this.moreItems = false;
          return;
        }
        
        this.products = this.products.concat(products.sort((a,b) => a.name.localeCompare(b.name)));
      },
      error:(err) => {

        if (err.status === 404) {  
          if(this.pageNumber > 1)
            this.moreItems = false;
          else
            this.navigationService.showUIMessage("El producto no fue encontrado.");
        } else {
          this.navigationService.showUIMessage("Error al procesar la solicitud. Servidor no disponible" );
        }
      },
    });
  }

  private searchProductsInternal(name: string): void {
    this.productService.searchProducts4List(this.pageNumber, this.userState.companyId, name).pipe(first())
    .subscribe({
      next: (products) => {
        this.products = products.sort((a,b) => a.name.localeCompare(b.name));
        if(this.products.length > 0)
        {
          this.selectProduct(products[0].productId);
        }
      },
      error:(err) => {
        if (err.status === 404) {  
          if(this.pageNumber > 1)
            this.moreItems = false;
          else
            this.navigationService.showUIMessage("El producto no fue encontrado.");
        } else {
          this.navigationService.showUIMessage("Error al procesar la solicitud. Servidor no disponible" );
        }
      },
    });
  }

  selectProduct(productId: number): void {
    this.productService.getProduct(this.userState.companyId, productId).pipe(first())
    .subscribe(product => {
      this.selectedProduct = product;
      this.productForm.patchValue(product);
      if(this.userState.companyStatusId == this.cStatus.Free)
      {
        this.productForm.get('unitPrice1')?.disable();
        this.productForm.get('unitPrice2')?.disable();
        this.productForm.get('unitPrice3')?.disable();
      }
      this.navigationService.checkScreenSize();
    });
  }

  saveProduct(): void {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }
    if (this.productForm.valid) {
      this.navigationService.showFreeLicenseMsg(this.userState.companyStatusId?? 0);
      const product: Producto = this.productForm.value;
      product.companyId = this.userState.companyId;
      
      this.productService.saveProduct(this.userState.companyId, product).pipe(first())
      .subscribe({
        next: (savedProduct) => {
          this.pageNumber = 1;
          if(product.productId == 0)
            this.products.unshift(savedProduct);
          
          this.selectProduct(savedProduct.productId);
          this.navigationService.showUIMessage("Producto guardado (" + savedProduct.productId + ")", AlertLevel.Sucess);
        },
        error: (e) => 
        {
          this.navigationService.showUIMessage(e.error.message);
        }
      });
    } else {
      this.navigationService.showUIMessage('Proporciona toda la información requerida', AlertLevel.Warning);
    }
  }

  newProduct(): void {
    this.selectedProduct = null;
    this.productForm.reset();
    this.productForm.patchValue({ productId: 0, companyId: this.userState.companyId, unitsInStock: 0, discontinued: false });
    window.scrollTo(0, 0);
  }

  isFieldInvalid(field: string): boolean {
    const control = this.productForm.get(field);
    return control ? control.invalid && control.touched : false;
  }

  private getCategories(): void {
    if(this.userState != null)
    {
      this.categoryService.getAll(this.userState.companyId, '-1').pipe(first())
      .subscribe(users => {
        this.categoryList = users.sort((a,b) => (a.name?? '').localeCompare((b.name?? '')));
        if(this.userState.companyStatusId != this.cStatus.Free)
        {
          this.categoryListFilter = this.categoryList;
        }
        else
        {
          this.categoryListFilter = [];
        }
      });
    }
    else
    {
      console.error("Estado de usuario invalido.");
    }
  }
}
