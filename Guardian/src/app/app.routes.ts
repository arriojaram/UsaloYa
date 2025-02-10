import { Routes } from '@angular/router';
import { PuntoDeVentaComponent } from './pdv/scanner.component';
import { LoginComponent } from './login/login.component';
import { AuthGuard } from './shared/auth-guard.guard';
import { ProductManagementComponent } from './admin/product-management/product-management.component';
import { UserManagementComponent } from './admin/user-management/user-management.component';
import { SalesReportComponent } from './admin/sales-report/sales-report.component';
import { CustomerManagementComponent } from './admin/customer-management/customer-management.component';
import { ImportProductsComponent } from './admin/import-products/import-products.component';
import { CompanyManagementComponent } from './admin/company-management/company-management.component';
import { PolicyComponent } from './resources/policy.component';
import { AgreementsComponent } from './resources/agreements.component';
import { InventarioReportComponent } from './admin/inventario-report/inventario-report.component';
import { PcategoriesComponent } from './admin/pcategories/pcategories.component';

export const routes: Routes = [
    {path: '', redirectTo: '/login', pathMatch: 'full' },
    {path: 'login', component: LoginComponent },
    { path: 'reporteinventario', component: InventarioReportComponent, canActivate: [AuthGuard] },
    { path: 'reporteventas', component: SalesReportComponent, canActivate: [AuthGuard] },
    { path: 'importar', component: ImportProductsComponent, canActivate: [AuthGuard] },  
    { path: 'companies', component: CompanyManagementComponent, canActivate: [AuthGuard] },  
    { path: 'productos', component: ProductManagementComponent, canActivate: [AuthGuard] },
    { path: 'categorias', component: PcategoriesComponent, canActivate: [AuthGuard] },
    { path: 'usuarios', component: UserManagementComponent, canActivate: [AuthGuard] },
    { path: 'clientes', component: CustomerManagementComponent, canActivate: [AuthGuard] },
    { path: 'main', component: PuntoDeVentaComponent, canActivate: [AuthGuard] },

    { path: 'policy', component: PolicyComponent },
    { path: 'agreements', component: AgreementsComponent }
];



