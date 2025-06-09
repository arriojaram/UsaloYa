import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NgIf } from '@angular/common';
import { Router } from '@angular/router';
import { NavigationService } from '../services/navigation.service';
import { AlertLevel } from '../Enums/enums';
import { RegisterDataService } from '../services/register-data.service';

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
    private registerDataService: RegisterDataService
  ) {}

  ngOnInit(): void {
    this.registerForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      phone: ['', [Validators.required, Validators.pattern(/^\d{10,15}$/)]],
      email: ['', [Validators.required, Validators.email]],
      address: [''],
      username: ['', [Validators.required, Validators.pattern(/^[a-zA-Z0-9._-]{4,20}$/)]],
      token: ['', Validators.required, Validators.minLength(4)],
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

    this.router.navigate(['/rcompany']); // REDIRIGE al formulario compañía
  }
}
