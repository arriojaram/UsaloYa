import { Component, OnDestroy, OnInit } from '@angular/core';
import { ProductSaleDetailReport, SaleDetailReport } from '../../dto/sale-detail-report';
import { ReportsService } from '../../services/reports.service';
import { NavigationService } from '../../services/navigation.service';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserStateService } from '../../services/user-state.service';
import { userDto } from '../../dto/userDto';
import { first, Subject, takeUntil } from 'rxjs';
import { SaleService } from '../../services/sale.service';
import { AlertLevel, CompanyStatus, PriceLevel, Roles, StatusVentaEnum } from '../../Enums/enums';
import { UserService } from '../../services/user.service';
import { environment } from '../../environments/enviroment';

@Component({
    selector: 'app-sales-report',
    imports: [CommonModule, ReactiveFormsModule, FormsModule],
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
  totalFinal: number | undefined;
  totalCompletadas: number = 0;
  totalCanceladas: number = 0;

  isAutorized: boolean = false;
  selectedSaleTotal: number = 0;
  selectedSaleId: number = 0;
  showColumns = false;
  companyUsers: userDto[] | undefined;
  selectedUserName: string | undefined;
  rol = Roles;
  cStatus = CompanyStatus;
  
  private unsubscribe$: Subject<void> = new Subject();
  
  constructor(private fb: FormBuilder,
    private salesService: SaleService,
    private reportService: ReportsService,
    private navigationService: NavigationService,
    private userStateService: UserStateService,
    private userService: UserService
  ) 
  {
    this.userState = userStateService.getUserStateLocalStorage();
    this.showMainReport = true;
    this.reportForm = this.initForm();
  }

  ngOnInit(): void {
    if(this.userState.roleId < Roles.User)
      this.navigationService.showUIMessage("Petición incorrecta.");
    else
      this.isAutorized = true;
    
    this.navigationService.showFreeLicenseMsg(this.userState.companyStatusId?? 0);

    this.reportForm.get('dateFrom')?.valueChanges.pipe(takeUntil(this.unsubscribe$)
    ).subscribe(newDate => {
      // Establecer 'dateTo' igual a 'dateFrom'
      this.reportForm.get('dateTo')?.setValue(newDate);
    });

    this.loadCompanyUsers();
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  // Función para alternar la visibilidad de las columnas
  toggleColumns() {
    this.showColumns = !this.showColumns;
  }

  private initForm(): FormGroup {
    const today = new Date().toISOString().substring(0, 10); // 'yyyy-MM-dd'
    const refDate = new Date();
    refDate.setDate(refDate.getDate() + 1);
    const tomorrow = refDate.toISOString().substring(0, 10); // 'yyyy-MM-dd'

    return this.fb.group({
      dateFrom: [today, [Validators.required]],
      dateTo: [tomorrow, [Validators.required]],
      filterText:[''],
      userId:[this.userState.userId]
    });
  }

  loadCompanyUsers()
  {
    this.userService.getAllUser(this.userState.companyId, '-1').pipe(first())
    .subscribe({
      next: (users) => {
        this.companyUsers = users.sort((a,b) => (a.firstName?? '').localeCompare((b.firstName?? '')));
      },
      error: (e) => {
        this.navigationService.showUIMessage(e.error);
      }
    });
  }
  
  redirectToDetails(saleId: number) {
    this.getSale(saleId);
    this.showMainReport = false;
    this.selectedSaleId = 0;
    this.selectedSaleTotal = 0;
  }

  goBack(): void {
    this.showMainReport = true;
  }

  private resolveSaleStatus(status: string): string
  {
    if(status === StatusVentaEnum[StatusVentaEnum.Completada])
    {
      return StatusVentaEnum[StatusVentaEnum.Cancelada];
    }
    return StatusVentaEnum[StatusVentaEnum.Completada];
  }

  updateSaleStatus(saleId: number, status: string)
  {
    const newStatus = this.resolveSaleStatus(status);
    if( newStatus == StatusVentaEnum[StatusVentaEnum.Cancelada])
    {
      if (!confirm('La venta será marcada como cancelada ¿Estás seguro de que quieres continuar con esta acción?')) 
        return;
    }

    this.salesService.updateSaleStatus(saleId, newStatus, this.userState.companyId).pipe(
      first()
    ).subscribe({
      complete:() => {
        this.getSales();
      },
      error: (err) => {
        console.error(err);
        this.navigationService.showUIMessage("Error de disponibilidad de red, intenta mas tarde.");
      },
    });
  }

  getSale(saleId: number): void {
    const userId = 0;
    this.saleProducts = [];
    this.filteredProducts = [];
    this.selectedSaleTotal = 0;
    this.selectedSaleId = saleId;
    this.reportService.getProductSalesDetails(saleId, this.userState.companyId).pipe(first())
    .subscribe({
      next:(data) => {
        if(data.length > 0)
        {
          data.forEach(item => {
            switch (item.priceLevel) {
              case PriceLevel.UnitPrice1:
                item.soldPrice = item.productPrice1;
                break;
              case PriceLevel.UnitPrice2:
                item.soldPrice = item.productPrice2;
                break;
              case PriceLevel.UnitPrice3:
                item.soldPrice = item.productPrice3;
                break;
          
              default:
                break;
            }
          });

          this.saleProducts = data;
          this.filteredProducts = data;
          this.selectedSaleTotal = data.reduce((a, i) => a + i.totalPrice, 0 );
          this.selectedSaleId = saleId;
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

   
    this.sales = [];
    this.filteredSales = [];
    this.setTotalsToZero();
    this.filteredProducts = [];
    

    let fromDate = this.reportForm.get('dateFrom')?.value ?? new Date();
    const fromDateInput = this.reportForm.get('dateFrom')?.value;
    const fromDate2 = fromDateInput ? new Date(fromDateInput) : new Date();
    const toDate = this.reportForm.get('dateTo')?.value ?? new Date();
    let userId = this.reportForm.get('userId')?.value ?? 0;
    if(this.userState.roleId == this.rol.User)
      userId = this.userState.userId;

    if(this.userState.companyStatusId == CompanyStatus.Free)
    {
      this.navigationService.showFreeLicenseMsg(this.userState.companyStatusId?? 0);
      const currentDate = new Date();
            
      const diffTime = currentDate.getTime() - fromDate2.getTime();
      const diffDays = Math.floor(diffTime / (1000 * 60 * 60 * 24));
      
      if (diffDays >= 7) {
        const adjustedDate = new Date(currentDate);
        adjustedDate.setDate(currentDate.getDate() - 7);
        fromDate = adjustedDate.toISOString().substring(0, 10); // 'yyyy-MM-dd'
        this.reportForm.get('dateFrom')?.setValue(fromDate);
      }
    }

    this.reportService.getSales(fromDate, toDate, this.userState.companyId, userId).pipe(first())
    .subscribe({
      next:(data: SaleDetailReport[]) => {
        if(data.length > 0)
        {
          this.sales = data;
          this.filteredSales = data;
          this.totalCompletadas = data.reduce((acumulado, newItem) => acumulado + newItem.totalSale, 0);

          for (let index = 0; index < data.length; index++) {
            const saleItem = data[index];
            
            if(saleItem.status == StatusVentaEnum[StatusVentaEnum.Cancelada])
            {
              this.totalCanceladas += saleItem.totalSale;
            }
          }
          this.totalFinal = this.totalCompletadas - this.totalCanceladas;
        }
        else
        {
          this.setTotalsToZero();
          this.navigationService.showUIMessage('No hay registro de ventas entre las fechas ' + fromDate.toString() + ' - ' + toDate.toString() );  
        }
      },
      error:(err) => {
        this.setTotalsToZero();
        this.navigationService.showUIMessage(err.message);
      },
    });
  }
  
  setTotalsToZero()
  {
    this.totalCanceladas = 0;
    this.totalCompletadas = 0;
    this.totalFinal = 0;
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
