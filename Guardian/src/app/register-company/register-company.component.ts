import { Component, OnInit, OnDestroy } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NgIf } from '@angular/common';
import { UserService } from '../services/user.service';
import { NavigationService } from '../services/navigation.service';
import { AlertLevel } from '../Enums/enums';
import { companyDto } from '../dto/companyDto';
import { RequestRegisterNewUserDto } from '../dto/RequestRegisterNewUserDto ';
import { RegisterUserAndCompanyDto } from '../dto/RegisterUserAndCompanyDto ';
import { RegisterDataService } from '../services/register-data.service';
import { map, catchError, of, takeUntil, Subject } from 'rxjs';
import { CompanyService } from '../services/company.service';
import { Router } from '@angular/router';
import { SharedDataService } from '../services/shared-data.service';
@Component({
  selector: 'app-register-company',
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
    private userService: UserService,
    private navigationService: NavigationService,
    private registerDataService: RegisterDataService,
    private router: Router,
    private companyService: CompanyService,
    private sharedDataService: SharedDataService
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
        'Por favor registra primero los datos del usuario.',
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
      this.navigationService.showUIMessage('Formulario inválido', AlertLevel.Warning);
      return;
    }

    if (!this.userData) {
      this.navigationService.showUIMessage('Faltan los datos del usuario. Por favor regístrate primero.', AlertLevel.Error);
      return;
    }

    this.loading = true;

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

    const payload: RegisterUserAndCompanyDto = {
      requestRegisterNewUserDto: this.userData,
      companyDto: company,
    };

    this.userService.registerNewUser(payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          const userId = response?.userId;

          if (userId) {
            this.sharedDataService.setUserId(userId);
            this.navigationService.showUIMessage(
              'Usuario y compañía registrados exitosamente. Se envió un email con el link de verificación',
              AlertLevel.Sucess
            );
            this.companyForm.reset({ planId: 1 });
            this.registerDataService.setUserData(null);
            this.sharedDataService.clearEmail();
            this.loading = false;
            this.router.navigate(['/forms-navigator/questions']);
          } else {
            this.navigationService.showUIMessage(
              'No se recibió el ID del usuario. Registro incompleto.',
              AlertLevel.Error
            );
            this.loading = false;
          }
        },
        error: (err) => {
          this.navigationService.showUIMessage(
            'Error al registrar usuario y compañía: ' + err.message,
            AlertLevel.Error
          );
          this.loading = false;
        },
      });
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
