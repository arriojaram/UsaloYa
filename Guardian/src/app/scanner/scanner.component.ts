import { Component, OnInit } from "@angular/core"; 
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { SaleService } from "../services/sale.service";  
import { CommonModule } from "@angular/common"; 
import { ZXingScannerModule } from "@zxing/ngx-scanner";
import { BarcodeFormat } from "@zxing/library";
import { ListaVentaComponent } from "../lista-venta/lista-venta.component";
import { UserStateService } from "../services/user-state.service";
import { barcodeFormats } from "../shared/barcode-s-formats";

@Component({
    selector: 'app-scanner',
    standalone: true,
    imports: [ReactiveFormsModule, ListaVentaComponent, CommonModule, ZXingScannerModule],
    templateUrl: './scanner.component.html',
    styleUrls: ['./scanner.component.css']
})

export class ScannerComponent implements OnInit {
    isScannerEnabled: boolean = false;
    scannerBtnLabel: string | undefined;
    allowedFormats: BarcodeFormat [];
    qrResultString: string = "init";
    isHidden?: boolean;
    label_productoAdded?: string;
    messageClass: string = "alert  alert-success mt-2";
    scanningAllowed: boolean | undefined;

    constructor(
        public ventasService: SaleService,
        private userState: UserStateService
    ) 
    {
        this.allowedFormats = barcodeFormats.allowedFormats;
    }

    ngOnInit(): void {
        this.scannerBtnLabel = "Abrir escaner";
        this.isScannerEnabled = false;  
        this.isHidden = true;
        const companyId = this.userState.getUserStateLocalStorage().companyId;
        this.ventasService.cacheProductCatalog(companyId);
        this.scanningAllowed = true;
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
        console.log('Scan: ' + resultString);
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
   
    setScannerStatus()
    {
        this.isScannerEnabled = !this.isScannerEnabled;
        if(this.isScannerEnabled)
            this.scannerBtnLabel = "Apagar escaner";
        else
            this.scannerBtnLabel = "Prender escaner";
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