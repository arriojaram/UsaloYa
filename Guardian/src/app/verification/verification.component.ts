import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { NavigationService } from '../services/navigation.service';
import { AlertLevel } from '../Enums/enums';
import { VerificationResponseDto } from '../dto/VerificationResponseDto';
import { first, switchMap, catchError, of, Subject } from 'rxjs';
import { CommonModule } from '@angular/common';


interface RequestVerificationCodeDto {
  Code: string;
  Email: string;
  DeviceId: string;
}

@Component({
  standalone: true,
  selector: 'app-verificationcode',
  templateUrl: './verification.component.html',
  imports: [ReactiveFormsModule, CommonModule]
})
export class VerifyCodeComponent implements OnInit, OnDestroy {
  verificationForm!: FormGroup;
  deviceId: string = '';
  private unsubscribe$ = new Subject<void>();
  loading = false;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,


    private router: Router,
    private navigationService: NavigationService
  ) {}

  ngOnInit(): void {
    this.loadDeviceId();

    this.verificationForm = this.fb.group({
      Code: ['', Validators.required],
      Email: ['', [Validators.required, Validators.email]]
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  private loadDeviceId(): void {
    const deviceData = localStorage.getItem('deviceId');
    try {
      const parsed = deviceData ? JSON.parse(deviceData) : {};
      this.deviceId = parsed.value || '';
    } catch (e) {
      console.error('Error al parsear deviceId del localStorage', e);
      this.deviceId = '';
    }
  }

 onSubmit(): void {
  this.verificationForm.markAllAsTouched();

  if (this.verificationForm.invalid) {
    this.navigationService.showUIMessage('Por favor, complete todos los campos correctamente.', AlertLevel.Warning);
    return;
  }

  this.loading = true;

  const request: RequestVerificationCodeDto = {
    Code: this.verificationForm.value.Code,
    Email: this.verificationForm.value.Email,
    DeviceId: this.deviceId
  };

  this.userService.requestVerificationCodeEmail(
    { Code: request.Code, Email: request.Email },
    this.deviceId
  ).pipe(
    first()
  ).subscribe({
    next: (res: VerificationResponseDto) => {
      this.loading = false;

      if (!res.isValid || res.userId <= 0) {
        this.navigationService.showUIMessage(res.message || 'Código incorrecto.', AlertLevel.Error);
        return;
      }

      
      this.navigationService.showUIMessage('Verificación exitosa. Inicie sesión.', AlertLevel.Sucess);
      this.router.navigate(['/login']);
    },
    error: (error) => {
      this.loading = false;
      this.navigationService.showUIMessage('Error al verificar el código.', AlertLevel.Error);
      console.error('Verification error:', error);
    }
  });
}


  }

