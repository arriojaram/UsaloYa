import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReportsService } from '../../services/reports.service';
import { CompanyReportDto } from '../../dto/CompanyReportDto';
import { UserStateService } from '../../services/user-state.service';
import { userDto } from '../../dto/userDto';
import { TranslateService } from '@ngx-translate/core';
import { NavigationService } from '../../services/navigation.service';
import { Roles, getCompanyStatusEnumName, MeasureType } from '../../Enums/enums';
import { Subject, first, takeUntil } from 'rxjs';
import { UserService } from '../../services/user.service';
import { CurrencyPipe } from '@angular/common';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { DaysForWeeks, getDaysForWeeksLabel } from '../../Enums/enums';


@Component({
  selector: 'app-returns-report',
  standalone: true,
  imports: [CommonModule, CurrencyPipe, ReactiveFormsModule],
  templateUrl: './company-report.component.html',
  styleUrls: ['./company-report.component.css']
})
export class CompanyReportComponent implements OnInit, OnDestroy {
  showMainReport: boolean = true;
  reportForm!: FormGroup;
  userState!: userDto;
  isAutorized = false;
  companies: CompanyReportDto[] = [];
  keyword = "";
  weeks: { value: number, label: string }[] = [];
  private unsubscribe$ = new Subject<void>();

  measuere = MeasureType;
  rol = Roles;

  constructor(
    private fb: FormBuilder,
    private reportService: ReportsService,
    private userStateService: UserStateService,
    private navigationService: NavigationService,
    private translate: TranslateService,
  ) {
    this.userState = this.userStateService.getUserStateLocalStorage();
  }

  toggleReport(): void {
    this.showMainReport = !this.showMainReport;
  }

  ngOnInit(): void {
    if (this.userState.roleId < Roles.SysAdmin) {
      this.navigationService.showUIMessage(this.translate.instant('Companies_report.permission_denied'));
      return;
    }

    this.isAutorized = true;
    this.reportForm = this.initForm();

    this.weeks = Object.values(DaysForWeeks)
      .filter(value => typeof value === 'number')
      .map(value => ({
        value: value as number,
        label: getDaysForWeeksLabel(value as DaysForWeeks)
      }));
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
      userId: [this.userState.userId],
      inactiveDays: [0, Validators.required],
      status: [1, Validators.required],
    });
  }

  getCompanies(): void {
    if (this.reportForm.invalid) {
      this.reportForm.markAllAsTouched();
      return;
    }

    const inactiveDays = this.reportForm.get('inactiveDays')?.value;
    const status = this.reportForm.get('status')?.value;
    const company = "-1";

    this.reportService.getCompanyReport(this.userState.companyId, inactiveDays, status, company)
      .pipe(first())
      .subscribe({
        next: (data) => {
          this.companies = data.map(c => ({
            ...c,
            statusDesc: getCompanyStatusEnumName(c.status)
          }));
        },
        error: (err) => {
          this.companies = [];
          this.navigationService.showUIMessage(err.message);
        }
      });
  }

  searchCompanies(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    this.keyword = inputElement.value.trim();
    if (this.keyword) {
      this.searchCompaniesInternal(this.keyword);
    } else {
      this.getCompanies();
    }
  }

  private searchCompaniesInternal(name: string): void {
    let companyId = this.userState.companyId;
    if (this.userState.roleId < Roles.SysAdmin) {
      companyId = 0;
    }

    this.reportService.getCompanyReport(companyId, 0, 1, name)
      .pipe(first())
      .subscribe({
        next: (data) => {
          this.companies = data.map(c => ({
            ...c,
            statusDesc: getCompanyStatusEnumName(c.status)
          }));
        },
        error: (e) => {
          this.navigationService.showUIMessage(e.error);
        }
      });
  }
}

