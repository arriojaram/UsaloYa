import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReportsService } from '../../services/reports.service';
import { RefundReportDto } from '../../dto/refundReportDto';
import { RefundProduct } from '../../dto/refundProductDto';
import { UserStateService } from '../../services/user-state.service';
import { userDto } from '../../dto/userDto';
import { TranslateService } from '@ngx-translate/core';
import { NavigationService } from '../../services/navigation.service';
import { Roles, CompanyStatus, MeasureType } from '../../Enums/enums';
import { Subject, first, takeUntil } from 'rxjs';
import { UserService } from '../../services/user.service';
import { CurrencyPipe } from '@angular/common';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';


@Component({
  selector: 'app-returns-report',
  imports: [CommonModule, CurrencyPipe, ReactiveFormsModule],
  templateUrl: './returns-report.component.html',
  styleUrls: ['./returns-report.component.css']
})
export class ReturnsReportComponent implements OnInit, OnDestroy {
  reportForm!: FormGroup;
  userState!: userDto;
  isAutorized = false;
  showMainReport = true;
  measuere = MeasureType;
  rol = Roles;
  totalReturnedProducts = 0;


  refunds: RefundReportDto[] = [];
  filteredRefunds: RefundReportDto[] = [];
  filterText = '';

  selectedRefund: RefundReportDto | null = null;
  selectedProducts: RefundProduct[] = [];
  selectedFolio: number | null = null;

  totalAmount = 0;
  showColumns = false;
  companyUsers: userDto[] = [];

  private unsubscribe$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private reportService: ReportsService,
    private userStateService: UserStateService,
    private navigationService: NavigationService,
    private translate: TranslateService,
    private userService: UserService
  ) {
    this.userState = this.userStateService.getUserStateLocalStorage();
  }

  ngOnInit(): void {
    if (this.userState.roleId < Roles.User) {
      this.navigationService.showUIMessage(this.translate.instant('Returns_report.permission_denied'));
      return;
    }

    this.isAutorized = true;
    this.reportForm = this.initForm();
    this.loadCompanyUsers();

    this.reportForm.get('dateFrom')?.valueChanges.pipe(takeUntil(this.unsubscribe$)).subscribe(date => {
      this.reportForm.get('dateTo')?.setValue(date);
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  initForm(): FormGroup {
    const today = new Date().toISOString().substring(0, 10);
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    const tomorrowStr = tomorrow.toISOString().substring(0, 10);

    return this.fb.group({
      dateFrom: [today, Validators.required],
      dateTo: [tomorrowStr, Validators.required],
      userId: [this.userState.userId]
    });
  }

  loadCompanyUsers(): void {
    this.userService.getAllUser(this.userState.companyId, '-1').pipe(first()).subscribe({
      next: users => {
        this.companyUsers = users.sort((a, b) => (a.firstName ?? '').localeCompare(b.firstName ?? ''));
      },
      error: err => this.navigationService.showUIMessage(err.error)
    });
  }

  getRefunds(): void {
    if (this.reportForm.invalid) {
      this.reportForm.markAllAsTouched();
      return;
    }

    this.filteredRefunds = [];
    this.selectedRefund = null;
    this.selectedProducts = [];
    this.totalAmount = 0;

    const fromDate = this.reportForm.get('dateFrom')?.value;
    const toDate = this.reportForm.get('dateTo')?.value;
    let userId = this.reportForm.get('userId')?.value;

    if (this.userState.roleId === Roles.User) {
      userId = this.userState.userId;
    }

this.reportService.getRefundsReport(fromDate, toDate, this.userState.companyId).pipe(first()).subscribe({
  next: data => {
    this.refunds = data;
    this.filteredRefunds = data;
    this.totalAmount = data.reduce((acc, item) => acc + item.refundAmountTotal, 0);
    this.totalReturnedProducts = data.reduce((acc, r) => {
      const productQty = r.products?.reduce((sum, p) => sum + p.quantity, 0) || 0;
      return acc + productQty;
    }, 0);
  },
  error: err => {
    this.refunds = [];
    this.navigationService.showUIMessage(err.message);
  }
});
  }
  filterRefunds(event: Event): void {
    const value = (event.target as HTMLInputElement).value.toLowerCase();
    this.filteredRefunds = this.refunds.filter(refund =>
      refund.folio?.toString().includes(value) ||
      refund.name.toLowerCase().includes(value) ||
      refund.refundMethod.toLowerCase().includes(value)
    );
  }

  getRefundDetails(refund: RefundReportDto): void {
    this.reportService.getRefundDetails(refund.saleId, this.userState.companyId).pipe(first()).subscribe({
      next: detail => {
        this.selectedRefund = detail;
        this.selectedProducts = detail.products ?? [];
        this.selectedFolio = detail.folio ?? null;
        this.showMainReport = false;
      },
      error: err => this.navigationService.showUIMessage(err.message)
    });
  }

  goBack(): void {
    this.showMainReport = true;
    this.selectedRefund = null;
    this.selectedProducts = [];
  }

  toggleColumns(): void {
    this.showColumns = !this.showColumns;
  }

  getMeasureName(measure: number): string {
    return MeasureType[measure];
  }
}
