import { Routes } from '@angular/router';
import { ScannerComponent } from './scanner/scanner.component';
import { LoginComponent } from './login/login.component';
import { AuthGuard } from './shared/auth-guard.guard';
import { ProductManagementComponent } from './admin/product-management/product-management.component';
import { UserManagementComponent } from './admin/user-management/user-management.component';
import { SalesReportComponent } from './admin/sales-report/sales-report.component';

export const routes: Routes = [
    {path: '', redirectTo: '/login', pathMatch: 'full' },
    {path: 'login', component: LoginComponent },
    { path: 'reporteventas', component: SalesReportComponent, canActivate: [AuthGuard] },
    
    { path: 'productos', component: ProductManagementComponent, canActivate: [AuthGuard] },
    { path: 'usuarios', component: UserManagementComponent, canActivate: [AuthGuard] },
    { path: 'main', component: ScannerComponent, canActivate: [AuthGuard] }
];



