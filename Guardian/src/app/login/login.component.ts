import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NgIf } from '@angular/common';
import { AuthorizationService } from '../services/authorization.service';
import { Router, RouterModule } from '@angular/router';
import { UserStateService } from '../services/user-state.service';
import { NavigationService } from '../services/navigation.service';
import { userDto } from '../dto/userDto';
import { catchError, first, of, Subject, switchMap, takeUntil } from 'rxjs';
import { environment } from '../environments/enviroment';
import { TokenDto } from '../dto/authenticateDto';
import { loginResponseDto } from '../dto/loginReponseDto';
import { AlertLevel } from '../Enums/enums';


@Component({
    selector: 'app-login',
    imports: [RouterModule, ReactiveFormsModule, HttpClientModule, NgIf],
    templateUrl: './login.component.html',
    styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit, OnDestroy {
  loginForm: FormGroup;
  private unsubscribe$: Subject<void> = new Subject();
  
  constructor(private fb: FormBuilder, 
    private router: Router,
    private authService: AuthorizationService,
    private userStateService: UserStateService,
    private navigation: NavigationService,
    
  ) {
    
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      token: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    if(!this.navigation.getItemWithExpiry('deviceId'))
    {
      let deviceId = this.authService.generateDeviceId();
      this.navigation.setItemWithExpiry('deviceId', deviceId);
    }
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      const loginData: TokenDto = this.loginForm.value;
     
      this.authService.login(loginData).pipe(first(),
        switchMap((loginResults: loginResponseDto) => {
          
          if(loginResults.msg)
            this.navigation.showUIMessage(loginResults.msg, AlertLevel.Info);

          return this.userStateService.getLoggedUser(loginResults.id);
        }),
        catchError((e) => {
          if (e.status === 0) {
            // Error de conexión o servidor no disponible
            this.navigation.showUIMessage('Error de conexión: El servidor no está disponible.');
          } else if (e.status >= 500) {
            // Error del servidor (5xx)
            this.navigation.showUIMessage('Error del servidor: ' + e.error.message);
          }
          else
          {
            let errMessage = e.error;
            if(e.error == '$_Expired_License')
              errMessage = environment.paymentExpiredMsg;
            this.navigation.showUIMessage(errMessage);
          }
          console.error(e);
          return of(null); // Retornamos un observable nulo para continuar el flujo
        })
      ).subscribe({
        next: (userResults: userDto | null) => {
          if (userResults) {
            this.userStateService.setUserStateLocalStorage(userResults);
            this.navigation.setUserState(userResults);
            this.router.navigate(['/main']); 
          }
        },
        error: (e) => {
          this.navigation.showUIMessage(e.error);
          console.error(e);
        }
      });
    } else {
      this.navigation.showUIMessage('Formulario inválido');
    }
  }
  
}