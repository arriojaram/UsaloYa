import { BarcodeFormat } from "@zxing/library";

export class barcodeFormats
{
    static allowedFormats: BarcodeFormat[] = [  
        BarcodeFormat.CODE_39,
        BarcodeFormat.EAN_13,
        BarcodeFormat.CODE_128,
        BarcodeFormat.UPC_A,
        BarcodeFormat.UPC_E,
        BarcodeFormat.UPC_EAN_EXTENSION,
        BarcodeFormat.ITF
    ] ;
}