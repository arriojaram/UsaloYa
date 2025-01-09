import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { Producto } from '../../dto/producto';
import { NgFor, NgIf } from '@angular/common';
import { UserStateService } from '../../services/user-state.service';
import { userDto } from '../../dto/userDto';
import { NavigationService } from '../../services/navigation.service';
import { first, Subscription } from 'rxjs';
import { BarcodeFormat } from "@zxing/library";
import { ZXingScannerModule } from "@zxing/ngx-scanner";
import { barcodeFormats } from '../../shared/barcode-s-formats';
import { AlertLevel, Roles } from '../../Enums/enums';

@Component({
    selector: 'app-product-management',
    templateUrl: './product-management.component.html',
    styleUrls: ['./product-management.component.css'],
    imports: [ReactiveFormsModule, FormsModule, NgFor, NgIf, ZXingScannerModule]
})
export class ProductManagementComponent implements OnInit {

  productForm: FormGroup;
  products: Producto[] = [];
  selectedProduct: Producto | null = null;
  userState: userDto;
  
  isScannerEnabled: boolean = false;
  allowedFormats: BarcodeFormat [];
  qrResultString: string = "init";
  scannerBtnLabel: string | undefined;
  pageNumber: number = 1;
  rol = Roles;

  constructor(
    private fb: FormBuilder, 
    private productService: ProductService,
    private userService: UserStateService,
    public navigationService: NavigationService
  ) 
  {
    this.userState = userService.getUserStateLocalStorage();
    this.productForm = this.initProductForm();

    this.allowedFormats = barcodeFormats.allowedFormats;    
  }

  setScannerStatus()
  {
      this.isScannerEnabled = !this.isScannerEnabled;
      if(this.isScannerEnabled)
          this.scannerBtnLabel = "Apagar escaner";
      else
          this.scannerBtnLabel = "Prender escaner";
  }

  onCodeResult(resultString: string) {
    this.productForm.patchValue({
      barcode: resultString
    });
    this.setScannerStatus();
  }

  initProductForm() : FormGroup
  {
    return this.fb.group({
      productId: [0],
      name: ['', Validators.required],
      description: [''],
      categoryId: [null],
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
      companyId: [1, Validators.required]
    });
  }

  ngOnInit(): void {
    this.searchProductsInternal('-1');
    this.scannerBtnLabel = "Abrir escaner";
    this.pageNumber = 1;
    this.navigationService.checkScreenSize();
  }

  searchProducts(): void {
    this.pageNumber = 1;
    let keyword = this.navigationService.searchItem;
    if (!keyword || keyword.trim() === "") {
      keyword="-1";
    }
    this.searchProductsInternal(keyword);
    
  }
  
  loadMore(): void {
    this.pageNumber++;
    let keyword = this.navigationService.searchItem;
    if (!keyword || keyword.trim() === "") {
      keyword="-1";
    }
    this.appendToSearchResults(keyword);
  }

  private appendToSearchResults(name: string): void {
    this.productService.searchProducts4List(this.pageNumber, this.userState.companyId, name).pipe(first())
    .subscribe(products => {
      this.products = this.products.concat(products.sort((a,b) => a.name.localeCompare(b.name)));
    });
  }

  private searchProductsInternal(name: string): void {
    this.productService.searchProducts4List(this.pageNumber, this.userState.companyId, name).pipe(first())
    .subscribe(products => {
      this.products = products.sort((a,b) => a.name.localeCompare(b.name));
    });
  }

  selectProduct(productId: number): void {
    this.productService.getProduct(this.userState.companyId, productId).pipe(first())
    .subscribe(product => {
      this.selectedProduct = product;
      this.productForm.patchValue(product);
      
      this.navigationService.checkScreenSize();
    });
  }

  saveProduct(): void {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }
    if (this.productForm.valid) {
      const product: Producto = this.productForm.value;
      product.companyId = this.userState.companyId;
      
      this.productService.saveProduct(this.userState.companyId, product).pipe(first())
      .subscribe({
        next: (savedProduct) => {
          this.searchProductsInternal('-1');
          this.selectProduct(savedProduct.productId);
          this.navigationService.showUIMessage("Producto guardado (" + savedProduct.productId + ")", AlertLevel.Sucess);
          //window.scrollTo(0, 0);
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
}
