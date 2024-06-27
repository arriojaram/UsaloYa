import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { Producto } from '../../dto/producto';
import { NgFor, NgIf } from '@angular/common';
import { UserStateService } from '../../services/userState.service';
import { userStateDto } from '../../dto/userDto';
import { NavigationService } from '../../services/navigation.service';

@Component({
  selector: 'app-product-management',
  templateUrl: './product-management.component.html',
  styleUrls: ['./product-management.component.css'],
  standalone: true,
  imports: [ReactiveFormsModule, NgFor, NgIf]
})
export class ProductManagementComponent implements OnInit {
  productForm: FormGroup;
  products: Producto[] = [];
  selectedProduct: Producto | null = null;
  userState: userStateDto;

  constructor(
    private fb: FormBuilder, 
    private productService: ProductService,
    private userService: UserStateService,
    private navigationService: NavigationService
  ) 
  {
    this.userState = userService.getUserState();
    this.productForm = this.initProductForm();
  }

  initProductForm() : FormGroup
  {
    return this.fb.group({
      productId: [0],
      name: ['', Validators.required],
      description: ['', Validators.required],
      categoryId: [null],
      supplierId: [null],
      unitPrice: [null],
      unitsInStock: [0, Validators.required],
      discontinued: [false, Validators.required],
      imgUrl: [''],
      weight: [null],
      sku: [''],
      barcode: ['', Validators.required],
      brand: [''],
      color: [''],
      size: [''],
      companyId: [1, Validators.required]
    });
  }

  ngOnInit(): void {
    this.searchProductsInternal('-1');
  }

  searchProducts(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    const keyword = inputElement.value || '-1'; // Default to an empty string if keyword is null or undefined
    this.searchProductsInternal(keyword);
  }
  
  private searchProductsInternal(name: string): void {
    this.productService.searchProducts(this.userState.companyId, name).subscribe(products => {
      this.products = products;
    });
  }

  selectProduct(productId: number): void {
    this.productService.getProduct(this.userState.companyId, productId).subscribe(product => {
      this.selectedProduct = product;
      this.productForm.patchValue(product);
    });
  }

  saveProduct(): void {
    if (this.productForm.valid) {
      const product: Producto = this.productForm.value;
      this.productService.saveProduct(this.userState.companyId, product).subscribe(savedProduct => {
        this.searchProductsInternal('-1');
        this.selectProduct(savedProduct.productId);
      });
    } else {
      this.navigationService.showUIMessage('Proporciona toda la información requerida');
    }
  }

  newProduct(): void {
    this.selectedProduct = null;
    this.productForm.reset();
    this.productForm.patchValue({ productId: 0, companyId: this.userState.companyId, unitsInStock: 0, discontinued: false });
  }
}
