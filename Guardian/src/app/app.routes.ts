import { Routes } from '@angular/router';
import { ScannerComponent } from './scanner/scanner.component';
import { LoginComponent } from './login/login.component';
import { AuthGuard } from './shared/auth-guard.guard';

export const routes: Routes = [
    {path: '', redirectTo: '/login', pathMatch: 'full'},
    {path: 'login', component: LoginComponent},
    //{path: 'main', component: ScannerComponent}
    {path: 'main', component: ScannerComponent, canActivate: [AuthGuard]}
];
