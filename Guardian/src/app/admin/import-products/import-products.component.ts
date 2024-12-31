import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ImportCvsProductService } from '../../services/import-cvs-product.service';
import { ProductService } from '../../services/product.service';
import { EMPTY, from, Subject, switchMap, takeUntil } from 'rxjs';
import { userDto } from '../../dto/userDto';
import { UserStateService } from '../../services/user-state.service';
import { CommonModule } from '@angular/common';
import { catchError, concatMap, finalize } from 'rxjs/operators';
import { Roles } from '../../Enums/enums';
import { NavigationService } from '../../services/navigation.service';

@Component({
  selector: 'app-import-products',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './import-products.component.html',
  styleUrl: './import-products.component.css'
})
export class ImportProductsComponent implements OnInit, OnDestroy {
  @ViewChild('fileInput') fileInput!: ElementRef;

  userState: userDto;
  private destroy$ = new Subject<void>();
  productsProcessed = 0;
  productsFailed = 0;
  selectedFile: File | null = null;
  isRunning: Boolean | undefined;
  alertClass: string | undefined;
  user_message: string | undefined;
  messages: string[] = [];
  isAutorized: boolean = false;

  initialMessage: string= "El proceso de importación esta en ejecución, el proceso puede tardar varios minutos, no cambies de página mientras estoy trabajando con la importación de datos.";
  constructor(
    private csvReaderService: ImportCvsProductService,
    private productService: ProductService,
    private userService: UserStateService,
    private navigationService: NavigationService
  ) 
  {
    this.userState = this.userService.getUserStateLocalStorage();
  }

  ngOnInit(): void {
    this.userState = this.userService.getUserStateLocalStorage();
    this.isRunning = false;
    this.alertClass = "alert alert-warning";
    this.user_message = this.initialMessage;

    if(this.userState.roleId < Roles.Admin)
      this.navigationService.showUIMessage("Petición incorrecta.");
    else
      this.isAutorized = true;

  }

  ngOnDestroy() {
    this.destroy$.next();  // Emitir un valor para completar todas las suscripciones
    this.destroy$.complete();  // Cerrar el Subject
  }

  onFileSelected(event: any) {
    this.isRunning = false;
    this.selectedFile = event.target.files[0];
    this.user_message = this.initialMessage;
    this.messages = [];
    this.log('Archivo seleccionado.','', '');
    this.productsFailed = 0;
    this.productsProcessed = 0;
  }

  clearFileInput(): void {
    if (this.fileInput && this.fileInput.nativeElement) {
      this.fileInput.nativeElement.value = '';
    }
  }

  log(message: string, name:string, barcode: string): void {
    if (!this.messages.includes(message)) {
      this.messages.push(message); 
    }
    if(barcode != "")
    {
      this.messages.push(barcode + ' - ' + name); 
    }
  }

  importProducts() {
    if (this.selectedFile) {
      this.isRunning = true;
      this.user_message = this.initialMessage;
      this.alertClass = "alert alert-warning";

      this.csvReaderService.parseCsv(this.selectedFile).pipe(
        switchMap(productos => from(productos).pipe(
          concatMap(producto => 
            this.productService.saveProduct(this.userState.companyId, producto).pipe(
              catchError((error) => {
                this.productsFailed++;
                this.log(error.error.message, producto.name, producto.barcode);
                return EMPTY; // Continúa con el siguiente producto a pesar del error.
              })
            )
          ),
          takeUntil(this.destroy$)
        )),
        finalize(() => {
          this.user_message = "El proceso de importación ha finalizado.";
          this.alertClass = "alert alert-info";
          this.clearFileInput();
          console.log(`Procesados: ${this.productsProcessed}, Fallidos: ${this.productsFailed}`);
        })
      ).subscribe({
        next: (response) => {
          this.productsProcessed++;
        },
        error: (error) => {
          this.log(error, '', '');
        },
        complete: () => {
          this.log('Todos los productos han sido procesados.', '', '');
        }
      });
     
    }
  }

 
}

