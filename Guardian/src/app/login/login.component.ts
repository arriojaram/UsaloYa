import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NgIf } from '@angular/common';
import { AuthServiceService } from '../services/authorization.service';
import { Router, RouterModule } from '@angular/router';
import { UserService } from '../services/user.service';


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
    private authService: AuthServiceService,
    private userService: UserService
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
      let isLoggedIn = this.authService.login(loginData).subscribe({
        next: (loginResults) => {
          console.log('user:' + isLoggedIn);
        },
        complete: () =>
        {
          try
          {
            this.userService.getUser();
            this.router.navigate(['/main'])
          }
          catch(e)
          {
            console.error(e);
          }
        },
        error: (e) => 
        {
          console.error('login() | ' + e);
        }
      });
    }
    else
    {
      console.log('Form invalido');
    }    
  }
}