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
import { RefundService } from '../../services/refund.service';
import { RequestRefundDto } from '../../dto/requestRefundDto';
import { RefundProduct } from '../../dto/refundProductDto';
import { CompanyService } from '../../services/company.service';
import { ReturnReason, getReturnReasonLabel } from '../../Enums/enums';
import { ReturnableProduct } from '../../dto/ReturnableProduct';
import { SaleSummary } from '../../dto/saleSummaryDto';
import { StatusVentaEnum } from '../../Enums/enums';
import { environment } from '../../environments/enviroment';
@Component({
  selector: 'app-returns',
  templateUrl: './returns.component.html',
  imports: [DatePipe, CurrencyPipe, NgFor, NgIf, NgClass, FormsModule, ReactiveFormsModule],
  styleUrls: ['./returns.component.css']
})
export class ReturnsComponent implements OnInit, OnDestroy {
  form: FormGroup;
  saleProducts: ReturnableProduct[] = [];
  currentDate = new Date();
  showMainView = true;
  sales: SaleSummary[] = [];
  filteredSales: SaleSummary[] = [];
  selectedFolio: number | null = null;
  selectedSaleTotal: number = 0;
  filterText: string = '';
  userState!: userDto;
  measure = MeasureType;
  selectAllChecked: boolean = false;
  globalReason: string = '';
  globalCustomReason: string = '';
  canReturn: boolean = false;
  maxDaysToRefund: number = 0;
  StatusVentaEnum = StatusVentaEnum;

  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private reportService: ReportsService,
    private userStateService: UserStateService,
    private navigationService: NavigationService,
    private translate: TranslateService,
    private refundService: RefundService,
    private companyService: CompanyService
  ) {
    this.form = this.fb.group({
      ticketNumber: ['', Validators.required],
      refundMethod: ['cash', Validators.required],
      maxDaysToRefund: [],
    });

  }
  returnReasons: { value: number, label: string }[] = [];
  ngOnInit(): void {
    this.userState = this.userStateService.getUserStateLocalStorage();


    // Obtener configuración días máximos para devolución antes de cargar ventas
    this.companyService.getCompanySettings(this.userState.companyId).subscribe({
      next: (settings) => {
        if (settings && settings.length > 0) {
          for (let index = 0; index < settings.length; index++) {
            const s = settings[index];
            if (s.key == environment.PAIRSETT_DIAS_DE_DEVOLUCION) {
              const days = Number(s.value) || 0; this.form.get('maxDaysToRefund')?.setValue(s.value);
              this.maxDaysToRefund = days;
            }

          }
        }
        const fromDateIso = new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString();
        const toDateIso = new Date().toISOString();

        this.reportService.getSales(fromDateIso, toDateIso, this.userState.companyId, 0)
          .pipe(takeUntil(this.destroy$))
          .subscribe({
            next: (sales) => {
               const filteredByDate = sales.filter(sale => this.canReturnSale(sale));
              this.sales = filteredByDate.map(sale => ({
                saleID: sale.saleID,
                folio: sale.folio,
                saleDate: new Date(sale.saleDate).toISOString(),
                totalSale: sale.totalSale,
                fullName: sale.fullName ?? '',
                userName: sale.userName ?? '',
                status: sale.status === 'Completada' ? StatusVentaEnum.Completada :
                  sale.status === 'Cancelada' ? StatusVentaEnum.Cancelada :
                    sale.status === 'Reembolsado' ? StatusVentaEnum.Reembolsado :
                      StatusVentaEnum.Completada
              }))
               .sort((a, b) => new Date(b.saleDate).getTime() - new Date(a.saleDate).getTime());
              this.filteredSales = [...this.sales];
            },
            error: (err) => console.error('Error al cargar ventas:', err)
          });

      },
      error: (err) => {
        this.navigationService.showUIMessage(this.translate.instant('returns.error_days'), AlertLevel.Error);
        this.maxDaysToRefund = 0; // Default en caso de error
      }
    });
    this.canReturn = this.userState.canMakeReturns === true;
    this.returnReasons = Object.values(ReturnReason)
      .filter(value => typeof value === 'number')
      .map(value => ({
        value: value as number,
        label: getReturnReasonLabel(value as ReturnReason)
      }));
  }

  // Método para saber si la venta aún puede devolverse según días máximos
  canReturnSale(sale: any): boolean {
    if (!sale || !sale.saleDate) return false;
    const saleDate = new Date(sale.saleDate);
    const today = new Date();
    const limitDate = new Date();
    limitDate.setDate(today.getDate() - this.maxDaysToRefund);
    return saleDate >= limitDate;
  }

  showReturnForm(saleId: number, folio: number, total: number): void {
    if (!this.canReturnSale({ saleDate: this.sales.find(s => s.saleID === saleId)?.saleDate })) {
      this.navigationService.showUIMessage(this.translate.instant('returns.days_exceeded'), AlertLevel.Warning);
      return;
    }

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
            selected: false,
            returnable: p.canBeRefunded ?? true,
            returnQuantity: p.quantity,
            editing: false,
            cuestomReason: '',
            StatusVentaEnum: StatusVentaEnum
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

  confirmReturn(): void {
    const faltanMotivos = this.saleProducts.some(p =>
      p.selected && p.reason === 'Otro' && (!p.customReason || !p.customReason.trim())
    );

    if (faltanMotivos) {
      this.navigationService.showUIMessage(this.translate.instant('returns.invalid_reason'), AlertLevel.Warning);
      return;
    }

    if (!this.isReturnFormValid()) {
      this.navigationService.showUIMessage(this.translate.instant('returns.error_return'), AlertLevel.Warning);
      return;
    }

    const confirmed = confirm('¿Estás seguro de realizar la devolución?');
    if (!confirmed) return;

    const productsToRefund: RefundProduct[] = this.saleProducts
      .filter(p => p.selected && p.returnable && p.returnQuantity > 0)
      .map(p => ({
        productId: p.productId,
        barcode: p.barcode,
        productName: p.name,
        reason: p.reason === 'Otro' ? (p.customReason ?? '') : (p.reason ?? ''),
        measure: p.measure.toString(),
        quantity: p.returnQuantity,
        unitPriceRefund: p.unitPrice,
        refundAmount: p.returnQuantity * p.unitPrice,
      }));

    const refundDto: RequestRefundDto = {
      saleId: this.form.value.ticketNumber,
      userId: this.userState.userId,
      saleDate: this.currentDate.toISOString(),
      refundMethod: this.form.value.refundMethod,
      productRefundList: productsToRefund
    };

    this.refundService.manageRefund(refundDto, this.userState.companyId, this.userState.userId).subscribe({
      next: success => {
        if (success) {
          const saleIndex = this.sales.findIndex(s => s.saleID === refundDto.saleId);
          if (saleIndex !== -1) {
            this.sales[saleIndex].status = StatusVentaEnum.Reembolsado;
          }
          this.navigationService.showUIMessage(this.translate.instant('returns.success_return'), AlertLevel.Sucess);
          this.resetForm();
          this.goBack();
        } else {
          this.navigationService.showUIMessage(this.translate.instant('returns.not_allowed'), AlertLevel.Warning);
        }
      },
      error: err => {
        this.navigationService.showUIMessage(this.translate.instant('returns.server_error'), AlertLevel.Error);
        console.error('Error en devolución:', err);
      }
    });
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
  // Validación del formulario de devolución
  isReturnFormValid(): boolean {
    const invalidItems = this.saleProducts.filter(p =>
      p.selected && p.returnable && (
        !p.returnQuantity || p.returnQuantity <= 0 || p.returnQuantity > p.quantity ||
        !p.reason || p.reason.trim() === ''
      )
    );

    if (invalidItems.length > 0) return false;

    return this.form.valid && this.saleProducts.some(p => p.selected && p.returnable);
  }


  //Al seleccionar o deseleccionar un producto, ajusta cantidad y motivo.
  onSelectItem(item: ReturnableProduct): void {
    if (item.selected) {
      item.returnQuantity = item.quantity;
    } else {
      item.returnQuantity = item.quantity;
      item.reason = '';
      item.editing = false;
    }
  }
  //Validar cantidad de devolución
  isValidQuantity(item: ReturnableProduct): boolean {
    const value = item.returnQuantity ?? item.quantity;
    return value > 0 && value <= item.quantity;
  }
  //Ajusta cantidades inválidas y redondea si es unidad.
  onBlurCantidad(item: ReturnableProduct): void {
    let qty = item.returnQuantity;

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
    const finalReason = this.globalReason === 'Otro' ? this.globalCustomReason : this.globalReason;

    this.saleProducts.forEach(p => {
      if (p.selected) {
        p.reason = finalReason;
        p.customReason = this.globalReason === 'Otro' ? this.globalCustomReason : null;
      }
    });
  }
  onReturnClick(sale: SaleSummary): void {
    if (!this.canReturnSale(sale)) {
      this.navigationService.showUIMessage(this.translate.instant('returns.time_expired'), AlertLevel.Warning
      );
      return;
    }
    if (sale.status === StatusVentaEnum.Reembolsado) {
      this.navigationService.showUIMessage(this.translate.instant('returns.ticket_already_refunded'), AlertLevel.Warning
      );
      return;
    }
    this.showReturnForm(sale.saleID, sale.folio, sale.totalSale);
  }


  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
