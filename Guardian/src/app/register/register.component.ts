import { Component, OnInit, OnDestroy } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NgIf } from '@angular/common';
import { Router } from '@angular/router';
import { NavigationService } from '../services/navigation.service';
import { AlertLevel } from '../Enums/enums';
import { RegisterDataService } from '../services/register-data.service';
import { UserService } from '../services/user.service';
import { SharedDataService } from '../services/shared-data.service';
import { catchError, map, takeUntil } from 'rxjs/operators';
import { of, Subject } from 'rxjs';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, HttpClientModule, NgIf],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit, OnDestroy {
  registerForm!: FormGroup;
  loading = false;

  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private navigationService: NavigationService,
    private router: Router,
    private registerDataService: RegisterDataService,
    private userService: UserService,
    private sharedDataService: SharedDataService
  ) { }

  ngOnInit(): void {
    const savedData = this.registerDataService.getUserData();
    this.registerForm = this.fb.group({
      name: [savedData?.firstName || '', [Validators.required, Validators.minLength(3)]],
      email: [savedData?.email || '', [Validators.required, Validators.email], [this.emailValidator()]],
      username: [savedData?.userName || '', [Validators.required, Validators.pattern(/^[a-zA-Z0-9._-]{4,20}$/)], [this.usernameValidator()]],
      token: ['', [Validators.required, Validators.minLength(4)]]
    });
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      this.navigationService.showUIMessage('Por favor corrige los errores del formulario', AlertLevel.Warning);
      return;
    }

    this.loading = true;

    const formValue = this.registerForm.value;

    const payload = {
      userName: formValue.username,
      token: formValue.token,
      firstName: formValue.name,
      lastName: '',
      email: formValue.email,
      phone: '',
    };

    this.registerDataService.setUserData(payload);
    this.sharedDataService.setEmail(formValue.email);

    this.navigationService.showUIMessage(
      'Datos de usuario capturados correctamente. Continúa con los datos de la compañía.',
      AlertLevel.Sucess
    );

    this.loading = false;

    this.router.navigate(['/forms-navigator/register-company']);
  }

  usernameValidator(): AsyncValidatorFn {
    return (control: AbstractControl) => {
      if (!control.value) return of(null);

      return this.userService.checkUsernameUnique(control.value).pipe(
        takeUntil(this.destroy$),
        map((isUnique: boolean) => (isUnique ? null : { usernameTaken: true })),
        catchError(() => of(null))
      );
    };
  }

  private emailValidator(): AsyncValidatorFn {
    return (control: AbstractControl) => {
      if (!control.value) return of(null);

      return this.userService.checkEmailUnique(control.value).pipe(
        takeUntil(this.destroy$),
        map(isUnique => (isUnique ? null : { emailTaken: true })),
        catchError(() => of(null))
      );
    };
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
