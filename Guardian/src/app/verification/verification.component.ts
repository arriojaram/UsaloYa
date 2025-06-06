import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../services/user.service';
import { Router } from '@angular/router';
import { NavigationService } from '../services/navigation.service';
import { AlertLevel } from '../Enums/enums';

interface RequestVerificationCodeDto {
  Code: string;
  Email: string;
  DeviceId: string;
}

@Component({
  standalone: true,
  selector: 'app-verify-code',
  templateUrl: './verification.component.html',
  imports: [ReactiveFormsModule]
})
export class VerifyCodeComponent implements OnInit {
  verificationForm!: FormGroup;
  deviceId: string = '';

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private router: Router,
    private navigationService: NavigationService
  ) {}

  ngOnInit() {
    this.loadDeviceId();

    this.verificationForm = this.fb.group({
      Code: ['', Validators.required],
      Email: ['', [Validators.required, Validators.email]]
    });
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

  onSubmit() {
    this.verificationForm.markAllAsTouched();

    if (this.verificationForm.invalid) {
      this.navigationService.showUIMessage('Por favor, complete todos los campos correctamente.', AlertLevel.Warning);
      return;
    }

    const request: RequestVerificationCodeDto = {
      Code: this.verificationForm.value.Code,
      Email: this.verificationForm.value.Email,
      DeviceId: this.deviceId
    };

    this.userService.requestVerificationCodeEmail(request).subscribe({
      next: (res) => {
        if (res.isValid) {
          this.navigationService.showUIMessage('Código verificado correctamente.', AlertLevel.Sucess);
          this.router.navigate(['/main']);
        } else {
          this.navigationService.showUIMessage(res.message || 'Código incorrecto.', AlertLevel.Error);
        }
      },
      error: (err) => {
        this.navigationService.showUIMessage('Error al verificar el código.', AlertLevel.Error);
        console.error(err);
      }
    });
  }
}
