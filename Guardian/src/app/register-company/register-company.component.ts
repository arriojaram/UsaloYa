import { Component, OnInit, OnDestroy } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NgIf } from '@angular/common';
import { NavigationService } from '../services/navigation.service';
import { AlertLevel } from '../Enums/enums';
import { companyDto } from '../dto/companyDto';
import { RequestRegisterNewUserDto } from '../dto/RequestRegisterNewUserDto ';
import { RegisterDataService } from '../services/register-data.service';
import { map, catchError, of, takeUntil, Subject } from 'rxjs';
import { CompanyService } from '../services/company.service';
import { Router } from '@angular/router';
import { SharedDataService } from '../services/shared-data.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-register-company',
  standalone: true,
  imports: [ReactiveFormsModule, HttpClientModule, NgIf],
  templateUrl: './register-company.component.html',
  styleUrl: './register-company.component.css'
})
export class RegisterCompanyComponent implements OnInit, OnDestroy {

  companyForm: FormGroup;
  userData: RequestRegisterNewUserDto | null = null;
  loading = false;
  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private navigationService: NavigationService,
    private registerDataService: RegisterDataService,
    private router: Router,
    private companyService: CompanyService,
    private sharedDataService: SharedDataService,
    private translate: TranslateService
  ) {
    this.companyForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)], [this.nameValidator()]],
      address: [''],
      phoneNumber: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(15)]],
      cellphoneNumber: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(15)]],
      email: [{ value: '', disabled: true }, [Validators.email]],
      ownerInfo: [''],
    });
  }

  ngOnInit(): void {
    this.userData = this.registerDataService.getUserData();

    if (!this.userData) {
      this.navigationService.showUIMessage(
        this.translate.instant('register_company.missing_user_data'),
        AlertLevel.Warning
      );
      return;
    }

    const email = this.userData.email || this.sharedDataService.getEmail();
    if (email) {
      this.companyForm.patchValue({ email: email });
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onSubmit(): void {
    if (this.companyForm.invalid) {
      this.companyForm.markAllAsTouched();
      this.navigationService.showUIMessage(
        this.translate.instant('register_company.invalid_form'),
        AlertLevel.Warning
      );
      return;
    }

    if (!this.userData) {
      this.navigationService.showUIMessage(
        this.translate.instant('register_company.incomplete_user_data'),
        AlertLevel.Error
      );
      return;
    }

    const company: companyDto = {
      companyId: 0,
      name: this.companyForm.value.name,
      address: this.companyForm.value.address,
      phoneNumber: this.companyForm.value.phoneNumber,
      cellphoneNumber: this.companyForm.value.cellphoneNumber,
      email: this.companyForm.getRawValue().email,
      ownerInfo: this.companyForm.value.ownerInfo,
      planId: 1,
      statusId: 1,
    };

    this.registerDataService.setCompanyData(company);

    this.navigationService.showUIMessage(
      this.translate.instant('register_company.company_saved'),
      AlertLevel.Sucess
    );

    this.router.navigate(['/forms-navigator/questions']);
  }

  private nameValidator(): AsyncValidatorFn {
    return (control: AbstractControl) => {
      if (!control.value) return of(null);
      return this.companyService.checkCompanyUnique(control.value).pipe(
        takeUntil(this.destroy$),
        map(isUnique => (isUnique ? null : { nameTaken: true })),
        catchError(() => of(null))
      );
    };
  }
}
