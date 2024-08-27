import { Component, OnDestroy, OnInit } from '@angular/core';
import { ProductSaleDetailReport, SaleDetailReport } from '../../dto/sale-detail-report';
import { HttpClient } from '@angular/common/http';
import { ReportsService } from '../../services/reports.service';
import { NavigationService } from '../../services/navigation.service';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserStateService } from '../../services/user-state.service';
import { userDto } from '../../dto/userDto';
import { Router } from '@angular/router';
import { first, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-sales-report',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './sales-report.component.html',
  styleUrl: './sales-report.component.css'
})
export class SalesReportComponent implements OnInit, OnDestroy {
  sales: SaleDetailReport[] = [];
  filteredSales: SaleDetailReport[] = [];
  reportForm: FormGroup;
  userState: userDto;
  filterText = '';
  saleProducts: ProductSaleDetailReport[] = [];
  filteredProducts: ProductSaleDetailReport[] = [];
  showMainReport: boolean;
  private unsubscribe$: Subject<void> = new Subject();
  
  constructor(private fb: FormBuilder,
    private http: HttpClient,
    private reportService: ReportsService,
    private navigationService: NavigationService,
    private userService: UserStateService,
    private router: Router
  ) 
  {
    this.reportForm = this.initForm();
    this.userState = userService.getUserStateLocalStorage();
    this.showMainReport = true;
  }

  ngOnInit(): void {
    this.reportForm.get('dateFrom')?.valueChanges.pipe(takeUntil(this.unsubscribe$)
    ).subscribe(newDate => {
      // Establecer 'dateTo' igual a 'dateFrom'
      this.reportForm.get('dateTo')?.setValue(newDate);
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  private initForm(): FormGroup {
    const today = new Date().toISOString().substring(0, 10); // 'yyyy-MM-dd'
    const refDate = new Date();
    refDate.setDate(refDate.getDate() + 1);
    const tomorrow = refDate.toISOString().substring(0, 10); // 'yyyy-MM-dd'

    return this.fb.group({
      dateFrom: [today, [Validators.required]],
      dateTo: [tomorrow, [Validators.required]]
      
      ,filterText:['']
    });
  }
  
  redirectToDetails(saleId: number) {
    this.getSale(saleId);
    this.showMainReport = false;
  }

  goBack(): void {
    this.showMainReport = true;
  }

  getSale(saleId: number): void {
    const userId = 0;

    this.reportService.getProductSalesDetails(saleId, this.userState.companyId).pipe(first())
    .subscribe({
      next:(data) => {
        if(data.length > 0)
        {
          this.saleProducts = data;
          this.filteredProducts = data;
        }
        else
        {
          this.navigationService.showUIMessage('No hay registro de productos para la venta seleccionada' );  
        }
      },
      error:(err) => {
        this.navigationService.showUIMessage(err.message);
      },
    });
  }
  
  filterProducts(event: Event): void {
    const inputElement = event.target as HTMLInputElement; 
    const value = inputElement.value; 
    
    if(value != null)
    {
      this.filterText = value;
      
      this.filteredProducts = this.saleProducts.filter(sale => 
        sale.barcode.includes(this.filterText) ||
        sale.productName.includes(this.filterText)
      );
    }
  }

  getSales(): void {
    if (this.reportForm.invalid) {
      this.reportForm.markAllAsTouched();
      
      return;
    }
    const fromDate = this.reportForm.get('dateFrom')?.value ?? new Date();
    const toDate = this.reportForm.get('dateTo')?.value ?? new Date();
    const userId = 0;

    this.reportService.getSales(fromDate, toDate, this.userState.companyId, userId).pipe(first())
    .subscribe({
      next:(data) => {
        if(data.length > 0)
        {
          this.sales = data;
          this.filteredSales = data;
        }
        else
        {
          this.navigationService.showUIMessage('No hay registro de ventas entre las fechas ' + fromDate.toString() + ' - ' + toDate.toString() );  
        }
      },
      error:(err) => {
        this.navigationService.showUIMessage(err.message);
      },
    });
  }
  
  filterSales(event: Event): void {
    const inputElement = event.target as HTMLInputElement; 
    const value = inputElement.value; 
    
    if(value != null)
    {
      this.filterText = value;
      
      this.filteredSales = this.sales.filter(sale => 
        sale.saleID.toString().includes(this.filterText) ||
        sale.userName.includes(this.filterText) ||
        sale.fullName.includes(this.filterText) ||
        sale.status.includes(this.filterText) ||
        sale.totalSale.toString().includes(this.filterText) 
      );
    }
  }

  
}
