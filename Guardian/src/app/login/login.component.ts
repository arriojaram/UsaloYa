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


@Component({
  selector: 'app-login',
  standalone: true,
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

  ngOnInit(): void {}

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      const loginData = this.loginForm.value;

      this.authService.login(loginData).pipe(first(),
        switchMap((loginResults) => {
          return this.userStateService.getLoggedUser(loginResults);
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
            this.navigation.showUIMessage(e.error);
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
          // Este error es para cualquier error en la cadena de observables
          this.navigation.showUIMessage(e.error);
          console.error(e);
        }
      });
    } else {
      this.navigation.showUIMessage('Formulario inválido');
    }
  }
  
}