import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { DatePipe, NgIf } from '@angular/common';
import { NgFor } from '@angular/common';
import { ReportsService } from '../../services/reports.service';
import { MeasureType } from '../../Enums/enums';
import { CurrencyPipe } from '@angular/common';
import { userDto } from '../../dto/userDto';
import { UserStateService } from '../../services/user-state.service';

@Component({
  selector: 'app-returns',
  templateUrl: './returns.component.html',
  imports: [DatePipe, ReactiveFormsModule, FormsModule, NgFor, NgIf, CurrencyPipe],
  styleUrls: ['./returns.component.css']
})
export class ReturnsComponent implements OnInit {
  form: FormGroup;
  saleProducts: any[] = []; // Recibimos productos directamente de getSaleDetails
  currentDate = new Date();
  showMainView = true;
  sales: any[] = [];
  filteredSales: any[] = [];
  selectedFolio: number | null = null;
  selectedSaleTotal: number = 0;
  filterText: string = '';
  userState!: userDto;

  constructor(
    private fb: FormBuilder,
    private reportService: ReportsService,
    private userStateService: UserStateService
  ) {
    this.form = this.fb.group({
      ticketNumber: ['', Validators.required],
      refundMethod: ['cash', Validators.required]
    });
  }
  ngOnInit(): void {
    this.userState = this.userStateService.getUserStateLocalStorage();

    const today = new Date();

    // Fecha desde hace 30 días
    const fromDate = new Date();
    fromDate.setDate(today.getDate() - 30);

    // Formatear a ISO strings con tiempos adecuados
    const fromDateIso = fromDate.toISOString(); // 30 días atrás, hora actual
    const toDateIso = today.toISOString(); // Hoy, hora actual

    this.reportService.getSales(fromDateIso, toDateIso, this.userState.companyId, 0).subscribe({
      next: (sales) => {
        this.sales = sales.map(sale => ({
          ...sale,
        }));
        this.filteredSales = [...this.sales];
      },
      error: (err) => {
        console.error('Error al cargar ventas:', err);
      }
    });
  }



  showReturnForm(saleId: number, folio: number, total: number): void {
    this.selectedFolio = folio;
    this.selectedSaleTotal = total;
    this.showMainView = false;
    this.form.patchValue({ ticketNumber: folio });

    this.reportService.getProductSalesDetails(saleId, this.userState.companyId).subscribe({
      next: (details) => {
        this.saleProducts = details.map(p => ({

          barcode: p.barcode,
          name: p.productName,
          quantity: p.quantity,
          unitPrice: p.soldPrice,
          measure: p.measure,
          totalPrice: p.totalPrice,
          reason: ''
        }));
      },
      error: (err) => {
        alert('No se encontraron productos para la venta especificada.');
        this.saleProducts = [];
      }
    });
  }



  getMeasureName(measure: number): string {
    return MeasureType[measure];
  }

  goBack(): void {
    this.showMainView = true;
    this.saleProducts = [];
    this.selectedFolio = null;
  }

  calculateTotalReturn(): number {
    return this.saleProducts
      .filter(p => p.reason && p.reason.trim() !== '')
      .reduce((sum, p) => sum + p.totalPrice, 0);
  }

  confirmReturn(): void {
    if (this.form.invalid) {
      alert('Por favor, complete el formulario correctamente.');
      return;
    }

    const invalid = this.saleProducts.some(p => !p.reason || p.reason.trim() === '');
    if (invalid) {
      alert('Por favor, seleccione un motivo para todos los productos.');
      return;
    }

    const returnData = {
      folio: this.form.value.ticketNumber,
      refundMethod: this.form.value.refundMethod,
      user: this.userState.userName,
      date: this.currentDate,
      products: this.saleProducts.map(p => ({
        code: p.barcode,
        quantity: p.quantity,
        reason: p.reason
      }))
    };

    // Aquí puedes activar el servicio real:
    /*
    this.reportService.registerReturn(returnData).subscribe({
      next: () => {
        alert('Devolución registrada correctamente.');
        this.resetForm();
        this.goBack();
      },
      error: (err) => {
        alert('Error al registrar la devolución.');
      }
    });
    */
  }

  cancel(): void {
    if (confirm('¿Seguro que quieres cancelar la devolución?')) {
      this.resetForm();
      this.goBack();
    }
  }

  private resetForm(): void {
    this.form.reset({ refundMethod: 'cash' });
    this.saleProducts = [];
    this.selectedFolio = null;
  }
  filterSales(): void {
    const text = this.filterText.toLowerCase();
    this.filteredSales = this.sales.filter(s =>
      s.folio.toString().includes(text) ||
      s.saleID.toString().includes(text)
    );
  }

}
