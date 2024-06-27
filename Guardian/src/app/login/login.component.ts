import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NgIf } from '@angular/common';
import { AuthorizationService } from '../services/authorization.service';
import { Router, RouterModule } from '@angular/router';
import { UserStateService } from '../services/userState.service';
import { NavigationService } from '../services/navigation.service';
import { userStateDto } from '../dto/userDto';
import { catchError, of, switchMap } from 'rxjs';


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterModule, ReactiveFormsModule, HttpClientModule, NgIf],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;

  constructor(private fb: FormBuilder, 
    private router: Router,
    private authService: AuthorizationService,
    private userService: UserStateService,
    private navigation: NavigationService
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      token: ['', Validators.required]
    });
  }

  ngOnInit(): void {}

  onSubmit(): void {
    if (this.loginForm.valid) {
      const loginData = this.loginForm.value;

      this.authService.login(loginData).pipe(
        switchMap((loginResults) => {
          return this.userService.loadUser(loginResults);
        }),
        catchError((e) => {
          this.navigation.showUIMessage(e.message);
          console.error('login() | ' + e);
          return of(null); // Retornamos un observable nulo para continuar el flujo
        })
      ).subscribe({
        next: (userResults: userStateDto | null) => {
          if (userResults) {
            this.userService.setUserState(userResults);
          }
        },
        complete: () => 
        {
          this.router.navigate(['/main']); 
        },
        error: (e) => {
          // Este error es para cualquier error en la cadena de observables
          this.navigation.showUIMessage(e);
          console.error(e);
        }
      });
    } else {
      this.navigation.showUIMessage('Formulario inválido');
    }
  }
  
}