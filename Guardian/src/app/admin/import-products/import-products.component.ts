import { Component, OnDestroy, OnInit } from '@angular/core';
import { ImportCvsProductService } from '../../services/import-cvs-product.service';
import { ProductService } from '../../services/product.service';
import { from, Subject, switchMap, takeUntil } from 'rxjs';
import { userDto } from '../../dto/userDto';
import { UserStateService } from '../../services/user-state.service';

@Component({
  selector: 'app-import-products',
  standalone: true,
  imports: [],
  templateUrl: './import-products.component.html',
  styleUrl: './import-products.component.css'
})
export class ImportProductsComponent implements OnInit, OnDestroy {
  userState: userDto;
  private destroy$ = new Subject<void>();
  productsProcessed = 0;
  productsFailed = 0;
  selectedFile: File | null = null;

  constructor(
    private csvReaderService: ImportCvsProductService,
    private productService: ProductService,
    private userService: UserStateService,
  ) 
  {
    this.userState = this.userService.getUserStateLocalStorage();
  }

  ngOnInit(): void {
    this.userState = this.userService.getUserStateLocalStorage();
  }

  ngOnDestroy() {
    this.destroy$.next();  // Emitir un valor para completar todas las suscripciones
    this.destroy$.complete();  // Cerrar el Subject
  }

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }

  importProducts() {
    if (this.selectedFile) {
      this.csvReaderService.parseCsv(this.selectedFile).pipe(
        switchMap(productos => from(productos)),
        takeUntil(this.destroy$)  
      ).subscribe(producto => {
        this.productService.saveProduct(this.userState.companyId, producto).pipe(
          takeUntil(this.destroy$) 
        ).subscribe({
          next: (response) => {
            this.productsProcessed ++;
          },
          error: (error) => 
          {
            this.productsFailed++;
          }
        });
      });
    }
  }

 
}

