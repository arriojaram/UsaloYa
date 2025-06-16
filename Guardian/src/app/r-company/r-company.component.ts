import { Component, OnInit } from '@angular/core';
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
import { map, catchError, of, take } from 'rxjs';
import { CompanyService } from '../services/company.service';
import { Router } from '@angular/router';
import { SharedDataService } from '../services/shared-data.service';

@Component({
  selector: 'app-r-company',
  standalone: true,
  imports: [ReactiveFormsModule, HttpClientModule, NgIf],
  templateUrl: './r-company.component.html',
  styleUrls: ['./r-company.component.css', '../../css/styles.css'],
})
export class Rcompany implements OnInit {
  companyForm: FormGroup;
  userData: RequestRegisterNewUserDto | null = null;
  loading = false;

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
      email: [{ value: '', disabled: true }, [Validators.email]],  // Campo deshabilitado
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
      .pipe(take(1)) 
      .subscribe({
        next: () => {
          this.navigationService.showUIMessage(
            'Usuario y compañía registrados exitosamente. Se envió un email con el link de verificación',
            AlertLevel.Sucess
          );
          this.companyForm.reset({ planId: 1 });
          this.registerDataService.setUserData(null);
          this.sharedDataService.clear();
          this.loading = false;
          this.router.navigate(['/forms-navigator/register']);
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
        map(isUnique => (isUnique ? null : { nameTaken: true })),
        catchError(() => of(null))
      );
    };
  }
}
