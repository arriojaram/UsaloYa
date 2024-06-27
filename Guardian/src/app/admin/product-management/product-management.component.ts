import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { Producto } from '../../dto/producto';

@Component({
  selector: 'app-product-management',
  templateUrl: './product-management.component.html',
  styleUrls: ['./product-management.component.css']
})
export class ProductManagementComponent implements OnInit {
  productForm: FormGroup;
  products: Producto[] = [];
  selectedProduct: Producto | null = null;

  constructor(
    private fb: FormBuilder, 
    private productService: ProductService
  ) {
    this.productForm = this.fb.group({
      productId: [0],
      name: ['', Validators.required],
      description: [''],
      categoryId: [null],
      supplierId: [null],
      unitPrice: [null],
      unitsInStock: [0, Validators.required],
      discontinued: [false],
      imgUrl: [''],
      weight: [null],
      sku: [''],
      barcode: [''],
      brand: [''],
      color: [''],
      size: [''],
      companyId: [1, Validators.required],
    });
  }

  ngOnInit(): void {
    this.searchProducts('');
  }

  searchProducts(keyword: string): void {
    this.productService.searchProducts(1, keyword).subscribe(products => {
      this.products = products;
    });
  }

  selectProduct(productId: number): void {
    this.productService.getProduct(1, productId).subscribe(product => {
      this.selectedProduct = product;
      this.productForm.patchValue(product);
    });
  }

  saveProduct(): void {
    const product: Producto = this.productForm.value;
    this.productService.saveProduct(1, product).subscribe(savedProduct => {
      this.searchProducts('-1');
      this.selectProduct(savedProduct.productId);
    });
  }

  newProduct(): void {
    this.selectedProduct = null;
    this.productForm.reset();
    this.productForm.patchValue({ productId: 0, companyId: 1, unitsInStock: 0, discontinued: false });
  }
}
