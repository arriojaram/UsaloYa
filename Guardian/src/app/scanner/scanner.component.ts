import { Component, OnInit } from "@angular/core"; 
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { SaleService } from "../services/sale.service";  
import { CommonModule } from "@angular/common"; 
import { ZXingScannerModule } from "@zxing/ngx-scanner";
import { BarcodeFormat } from "@zxing/library";
import { ListaVentaComponent } from "../lista-venta/lista-venta.component";

@Component({
    selector: 'app-scanner',
    standalone: true,
    imports: [ReactiveFormsModule, ListaVentaComponent, CommonModule, ZXingScannerModule],
    templateUrl: './scanner.component.html',
    styleUrls: ['./scanner.component.css']
})

export class ScannerComponent implements OnInit {

    constructor(public ventasService: SaleService) 
    {
        this.allowedFormats = [  BarcodeFormat.CODE_128,
            BarcodeFormat.DATA_MATRIX,
            BarcodeFormat.EAN_13,]    

            this.scannerBtnLabel = "Abrir escaner";
            this.isScannerEnabled = false;
    }

    ngOnInit(): void {
        this.isHidden = true;
    }    

    isScannerEnabled: boolean;
    scannerBtnLabel: string;
    allowedFormats: BarcodeFormat [];
    qrResultString: string = "init";
    isHidden?: boolean;
    label_productoAdded?: string;
    messageClass: string = "alert  alert-success mt-2";

    formVenta = new FormGroup({
        codigo : new FormControl('', Validators.required)
    });
    
    onCodeResult(resultString: string) {
        this.codigo.setValue(resultString);
        this.addProductToSaleList();
    }
   
    setScannerStatus()
    {
        console.log("set status");
        this.isScannerEnabled = !this.isScannerEnabled;
        if(this.isScannerEnabled)
            this.scannerBtnLabel = "Apagar escaner";
        else
            this.scannerBtnLabel = "Prender escaner";
    }

    get codigo(){
        return this.formVenta.get('codigo') as FormControl;
    }

    addProductToSaleList()
    {
        var isAdded = this.ventasService.addProduct(this.codigo.value);
        this.isHidden = false;

        if(isAdded)
        {
            this.messageClass = "alert  alert-success mt-2";
            this.label_productoAdded = `Producto ${ this.codigo.value} agregado correctamente`;        
        }
        else
        {
            this.messageClass = "alert  alert-danger mt-2";
            this.label_productoAdded = `El producto no existe : ${ this.codigo.value}`;        
        }

        
        setTimeout(() => {
            this.isHidden = true; // Oculta el div después de 5 segundos
        }, 3000);
    
        this.formVenta.get('codigo')?.setValue('');
    }
    
}