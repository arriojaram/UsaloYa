import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NgIf } from '@angular/common';
import { Router } from '@angular/router';
import { NavigationService } from '../services/navigation.service';
import { AlertLevel } from '../Enums/enums';
import { RegisterDataService } from '../services/register-data.service';
import { UserService } from '../services/user.service';
import { catchError, map } from 'rxjs/operators';
import { of } from 'rxjs';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, HttpClientModule, NgIf],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private navigationService: NavigationService,
    private router: Router,
    private registerDataService: RegisterDataService,
    private userService: UserService
  ) { }

  ngOnInit(): void {
    this.registerForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      phone: ['', [Validators.required, Validators.pattern(/^\d{10,15}$/)]],
      email: ['', [Validators.required, Validators.email], [this.emailValidator()]],
      address: [''],
      username: ['', [Validators.required, Validators.pattern(/^[a-zA-Z0-9._-]{4,20}$/)], [this.usernameValidator()]],
      token: ['', [Validators.required, Validators.minLength(4)]],
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
      phone: formValue.phone,
      email: formValue.email,
      address: formValue.address || '',
    };

    this.registerDataService.setUserData(payload); // GUARDA datos en servicio

    this.navigationService.showUIMessage(
      'Datos de usuario capturados correctamente. Continúa con los datos de la compañía.',
      AlertLevel.Sucess
    );

    this.loading = false;
    this.registerForm.reset();

    this.router.navigate(['/rcompany']);// REDIRIGE al formulario compañía


}

usernameValidator(): AsyncValidatorFn {
  return (control: AbstractControl) => {
    if (!control.value) return of(null);

    return this.userService.checkUsernameUnique(control.value).pipe(
      map((isUnique: boolean) => {
        return isUnique ? null : { usernameTaken: true };
      }),
      catchError(error => {
        console.error('Error al validar username único:', error);
        return of(null); // no bloquees el formulario por un error del servidor
      })
    );
  };
}


  private emailValidator(): AsyncValidatorFn {
    return (control: AbstractControl) => {
      if (!control.value) return of(null);
      return this.userService.checkEmailUnique(control.value).pipe(
        map(isUnique => (isUnique ? null : { emailTaken: true })),
        catchError(() => of(null))
      );
    };
  }
}

