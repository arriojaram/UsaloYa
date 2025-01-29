import { Component, OnInit } from "@angular/core"; 
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { SaleService } from "../services/sale.service";  
import { CommonModule } from "@angular/common"; 
import { BarcodeFormat } from "@zxing/library";
import { ListaVentaComponent } from "../pdv-lista-venta/lista-venta.component";
import { UserStateService } from "../services/user-state.service";
import { barcodeFormats } from "../shared/barcode-s-formats";
import { Producto } from "../dto/producto";
import { AlertLevel } from "../Enums/enums";
import { NavigationService } from "../services/navigation.service";

@Component({
    selector: 'app-scanner',
    imports: [ReactiveFormsModule, ListaVentaComponent, CommonModule, FormsModule],
    templateUrl: './scanner.component.html',
    styleUrls: ['./scanner.component.css']
})

export class PuntoDeVentaComponent implements OnInit {
    
    allowedFormats: BarcodeFormat [];
    qrResultString: string = "init";
    isHidden?: boolean;
    label_productoAdded?: string;
    messageClass: string = "alert  alert-success mt-2";
    scanningAllowed: boolean | undefined;
    isSearchingProduct: boolean = false;
    
    productName: string = '';  // Almacena el texto ingresado
    
    filteredProduct: Producto[] = [];  
    custButtonLabel: string = 'Buscar Producto';

    constructor(
        public ventasService: SaleService,
        private userState: UserStateService,
        private navigationService: NavigationService
    ) 
    {
        this.allowedFormats = barcodeFormats.allowedFormats;
    }

    ngOnInit(): void {
        
        this.isHidden = true;
        const companyId = this.userState.getUserStateLocalStorage().companyId;
        this.ventasService.cacheProductCatalog(companyId);
        this.scanningAllowed = true;
    }    

    searchProduct() {
       this.filteredProduct = [];
       let searchItem = this.productName.trim().toLowerCase();
       if(searchItem != '')
       {
            this.filteredProduct = this.ventasService.productCatalog.filter(p => p.name.toLowerCase().includes(searchItem) );
            if(this.filteredProduct.length == 0)
                this.navigationService.showUIMessage(`No hay productos con el nombre: ${this.productName}`, AlertLevel.Warning);
        }
     }

    selectProduct(product: Producto) {
        
        this.custButtonLabel = 'Buscar Producto';
        this.isSearchingProduct = false;
        this.filteredProduct = [];

        this.codigo.setValue(product.barcode);
        this.addProductToSaleList();
        this.productName = "";
    }

    showSearchProduct(): void {
        this.isSearchingProduct = !this.isSearchingProduct;
        this.custButtonLabel = this.isSearchingProduct ? 'Cerrar' : 'Buscar Producto';
        if(this.isSearchingProduct)
        {
        this.ventasService.productCatalog
        
        }
    }

    formVenta = new FormGroup({
        codigo : new FormControl('', Validators.required)
    });
    
    videoConstraints = {
        facingMode: { ideal: 'environment' }, // Usa la cámara trasera por defecto
        width: { ideal: 1280 },
        height: { ideal: 720 }
      };

      
    onCodeResult(resultString: string) {
        if (this.scanningAllowed) {
            this.scanningAllowed = false; // Deshabilitar escaneo temporalmente
        
            this.codigo.setValue(resultString);
            this.addProductToSaleList();
        
            
            // Establecer el delay antes de permitir otro escaneo
            setTimeout(() => {
                this.scanningAllowed = true;
            }, 3000); 
        }       
    }

    get codigo(){
        return this.formVenta.get('codigo') as FormControl;
    }

    async addProductToSaleList()
    {
        var isAdded = await this.ventasService.addProduct(this.codigo.value, this.userState.getUserStateLocalStorage().companyId);
        this.isHidden = false;

        if(isAdded)
        {
            this.messageClass = "alert  alert-success mt-2";
            this.label_productoAdded = `Producto ${ this.codigo.value} agregado correctamente`;     
            this.ventasService.playBeep(true);   
        }
        else
        {
            this.messageClass = "alert  alert-danger mt-2";
            this.label_productoAdded = `El producto no existe : ${ this.codigo.value}`;        
            this.ventasService.playBeep(false);
        }

        
        setTimeout(() => {
            this.isHidden = true; // Oculta el div después de 5 segundos
        }, 3000);
    
        this.formVenta.get('codigo')?.setValue('');
    }
    
}