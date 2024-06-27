import { Component } from '@angular/core';
import { VentaService } from '../services/venta.service';
import { CommonModule } from '@angular/common';
import { DbService } from '../services/db.service';
import { Producto } from '../dto/producto';

@Component({
  selector: 'app-lista-venta',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './lista-venta.component.html',
  styleUrl: './lista-venta.component.css'
})
export class ListaVentaComponent {
  constructor(public ventaService: VentaService, public dbService: DbService)
  {
    this.isHidden = true;
  }

  isHidden?: boolean;
  message?: string;
  messageClass: string = "alert  alert-success mt-2";

  canFinishSale()
  {
    var terminarVenta = false;
    if(this.ventaService.ventaProductos.length > 0)
      terminarVenta = true;
    
    return !terminarVenta;
  }
  
  async finalizarVenta()
  {
    const newProduct = this.ventaService.ventaProductos[0];

    this.isHidden = false;
         
    this.ventaService.saveVenta(newProduct).subscribe(
      {
        next: (response) => 
        {
          this.message = `Venta registrada: ${JSON.stringify(response)}`;
          this.messageClass = "alert  alert-success mt-2";
          this.ventaService.ventaProductos = [];
        },
        error: async (error) => {
          this.messageClass = "alert  alert-warning mt-2";
          
          try {
            const id = await this.dbService.addProduct(newProduct);
            console.log('Added product with id:', id);
            this.message = `Venta registrada: ${JSON.stringify(error)}`;
          } catch (error) {
            console.error('Error adding product:', error);
          }
        }
    });
    this.ventaService.ventaProductos = [];
    setTimeout(() => {
      this.isHidden = true; // Oculta el div después de X segundos
  }, 5000);
  }

}
