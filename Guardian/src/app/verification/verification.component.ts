import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { NavigationService } from '../services/navigation.service';
import { AlertLevel } from '../Enums/enums';
import { VerificationResponseDto } from '../dto/VerificationResponseDto';
import { first, switchMap, catchError, of, Subject } from 'rxjs';
import { CommonModule } from '@angular/common';
import { AuthorizationService } from '../services/authorization.service';


interface RequestVerificationCodeDto {
  Code: string;
  Email: string;
}

@Component({
  standalone: true,
  selector: 'app-verificationcode',
  templateUrl: './verification.component.html',
  imports: [ReactiveFormsModule, CommonModule]
})
export class VerifyCodeComponent implements OnInit, OnDestroy {
  verificationForm!: FormGroup;
  private unsubscribe$ = new Subject<void>();
  loading = false;
  

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private authService: AuthorizationService,

    private router: Router,
    private navigationService: NavigationService
  ) { }

  ngOnInit(): void {
    this.verificationForm = this.fb.group({
      Code: ['', Validators.required],
      Email: ['', [Validators.required, Validators.email]]
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  onSubmit(): void {
    this.verificationForm.markAllAsTouched();

    if (this.verificationForm.invalid) {
      this.navigationService.showUIMessage('Por favor, complete todos los campos correctamente.', AlertLevel.Warning);
      return;
    }

    this.loading = true;

    const requestParams: RequestVerificationCodeDto = {
      Code: this.verificationForm.value.Code,
      Email: this.verificationForm.value.Email
    };

    this.userService.requestVerificationCodeEmail(requestParams,
      this.authService.generateDeviceId()
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
        this.navigationService.showUIMessage('Error al verificar el código. Valida que la información que has introducido es correcta.', AlertLevel.Error);
        console.error('Verification error:', error);
      }
    });
  }


}

