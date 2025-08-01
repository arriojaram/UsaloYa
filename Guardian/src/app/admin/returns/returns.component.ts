import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DatePipe, CurrencyPipe, NgIf, NgFor, NgClass } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ReportsService } from '../../services/reports.service';
import { MeasureType, AlertLevel } from '../../Enums/enums';
import { userDto } from '../../dto/userDto';
import { UserStateService } from '../../services/user-state.service';
import { TranslateService } from '@ngx-translate/core';
import { NavigationService } from '../../services/navigation.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-returns',
  templateUrl: './returns.component.html',
  imports: [DatePipe, CurrencyPipe, NgFor, NgIf, NgClass, FormsModule, ReactiveFormsModule],
  styleUrls: ['./returns.component.css']
})
export class ReturnsComponent implements OnInit, OnDestroy {
  form: FormGroup;
  saleProducts: any[] = [];
  currentDate = new Date();
  showMainView = true;
  sales: any[] = [];
  filteredSales: any[] = [];
  selectedFolio: number | null = null;
  selectedSaleTotal: number = 0;
  filterText: string = '';
  userState!: userDto;
  measure = MeasureType;
  selectAllChecked: boolean = false;
  globalReason: string = '';
  globalCustomReason: string = '';

  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private reportService: ReportsService,
    private userStateService: UserStateService,
    private navigationService: NavigationService,
    private translate: TranslateService
  ) {
    this.form = this.fb.group({
      ticketNumber: ['', Validators.required],
      refundMethod: ['cash', Validators.required],
    });
  }

  ngOnInit(): void {
    this.userState = this.userStateService.getUserStateLocalStorage();

    const fromDateIso = new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString();
    const toDateIso = new Date().toISOString();

    this.reportService.getSales(fromDateIso, toDateIso, this.userState.companyId, 0)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (sales) => {
          this.sales = [...sales];
          this.filteredSales = [...this.sales];
        },
        error: (err) => console.error('Error al cargar ventas:', err)
      });
  }

  showReturnForm(saleId: number, folio: number, total: number): void {
    this.selectedFolio = folio;
    this.selectedSaleTotal = total;
    this.showMainView = false;
    this.form.patchValue({ ticketNumber: saleId });

    this.reportService.getProductSalesDetails(saleId, this.userState.companyId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (details) => {
          this.saleProducts = details.map(p => ({
            barcode: p.barcode,
            productId: p.productId,
            name: p.productName,
            quantity: p.quantity,
            unitPrice: p.soldPrice,
            measure: p.measure,
            totalPrice: p.totalPrice,
            reason: '',
            select: false,
            returnable: p.canRefunded ?? true,
            returnQuantity: p.quantity,
            editing: false,
            cuestomReason: '',
          }));
          this.selectAllChecked = false;
          this.globalReason = '';
          this.globalCustomReason = '';
        },
        error: (err) => {
          this.navigationService.showUIMessage(this.translate.instant('returns.no_product_sale'), AlertLevel.Error);
          this.saleProducts = [];
        }
      });
  }

  getMeasureName(measure: number): string {
    return MeasureType[measure];
  }
  //Regresa a la vista principal y limpia datos relacionados con la devolución.
  goBack(): void {
    this.showMainView = true;
    this.saleProducts = [];
    this.selectedFolio = null;
  }
  // Calcula el total a devolver sumando las cantidades seleccionadas y su precio.
  calculateTotalReturn(): number {
    return this.saleProducts
      .filter(p => p.selected && p.returnable && p.returnQuantity > 0)
      .reduce((sum, p) => sum + (p.returnQuantity * p.unitPrice), 0);
  }

  //Valida el formulario y, si es válido, prepara y envía los datos de la devolución. (No funciona aun )
  confirmReturn(): void {
    if (!this.isReturnFormValid()) {
      this.navigationService.showUIMessage(this.translate.instant('returns.error_return'), AlertLevel.Warning);
      return;
    }

    const returnData = {
      saleId: this.form.value.ticketNumber,
      refundMethod: this.form.value.refundMethod,
      user: this.userState.userName,
      date: this.currentDate,
      products: this.saleProducts
        .filter(p => p.selected && p.returnable && p.returnQuantity > 0)
        .map(p => ({
          code: p.barcode,
          productId: p.productId,
          quantity: p.returnQuantity,
          reason: p.reason,
          unitPrice: p.unitPrice,
        }))
    };

    console.log('Datos preparados para envío:', returnData);

    /*
    this.reportService.registerReturn(returnData)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
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
  //Cancela la devolución actual, pregunta confirmación y limpia el formulario.
  cancel(): void {
    if (confirm('¿Seguro que quieres cancelar la devolución?')) {
      this.resetForm();
      this.goBack();
    }
  }
  // Resetea el formulario y variables relacionadas con la devolución.
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
  // Validación del formulario de devolución
  isReturnFormValid(): boolean {
    const selected = this.saleProducts.filter(p => p.selected && p.returnable);
    if (selected.length === 0) return false;

    for (let item of selected) {
      if (!item.returnQuantity || item.returnQuantity <= 0 || item.returnQuantity > item.quantity) {
        return false;
      }
      if (!item.reason || item.reason.trim() === '') {
        return false;
      }
    }
    return this.form.valid;
  }

  //Al seleccionar o deseleccionar un producto, ajusta cantidad y motivo.
  onSelectItem(item: any): void {
    if (item.selected) {
      item.returnQuantity = item.quantity;
    } else {
      item.returnQuantity = item.quantity;
      item.reason = '';
      item.editing = false;
    }
  }
  //Validar cantidad de devolución
  isValidQuantity(item: any): boolean {
    const value = item.returnQuantity ?? item.quantity;
    return value > 0 && value <= item.quantity;
  }
  //Ajusta cantidades inválidas y redondea si es unidad.
  onBlurCantidad(item: any): void {
    let qty = parseFloat(item.returnQuantity);

    if (isNaN(qty) || qty < 1) {
      qty = 1;
    } else if (qty > item.quantity) {
      const messageKey = item.measure === this.measure.Ud
        ? 'returns.invalid_quantity_unit'
        : 'returns.invalid_quantity_weight';
      this.navigationService.showUIMessage(this.translate.instant(messageKey), AlertLevel.Warning);
      qty = item.quantity;
    }
    if (item.measure === this.measure.Ud) {
      qty = Math.floor(qty);
    }
    item.returnQuantity = qty;
    item.editing = false;
  }

  toggleSelectAll(): void {
    this.saleProducts.forEach(item => {
      if (item.returnable) {
        item.selected = this.selectAllChecked;
        if (this.selectAllChecked && !item.returnQuantity) {
          item.returnQuantity = item.quantity;
        }
      }
    });
    this.navigationService.showUIMessage(this.translate.instant('returns.select_all'), AlertLevel.Warning);
  }

  applyReasonToSelected(): void {
    this.saleProducts.forEach(p => {
      if (p.selected) {
        p.reason = this.globalReason;
        p.customReason = this.globalCustomReason;
      }
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
