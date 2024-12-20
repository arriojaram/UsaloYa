import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { NavigationService } from '../../services/navigation.service';
import { UserStateService } from '../../services/user-state.service';
import { format } from 'date-fns';
import { CommonModule, NgFor, NgIf } from '@angular/common';
import { userDto } from '../../dto/userDto';
import { companyDto } from '../../dto/companyDto';
import { CompanyService } from '../../services/company.service';
import { first } from 'rxjs';
import { getCompanyStatusEnumName, RentaStatusId, Roles } from '../../Enums/enums';
import { rentRequestDto } from '../../dto/rentRequestDto';

@Component({
  selector: 'app-company-management',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule,  NgFor, NgIf],
  templateUrl: './company-management.component.html',
  styleUrl: './company-management.component.css'
})
export class CompanyManagementComponent implements OnInit {
  companyForm: FormGroup;
  selectedCompany: companyDto | null = null;
  companyList: companyDto[] = [];
  userState: userDto;
  rol = Roles;
  activeTab: string = 'tab1';
  isSearchPanelHidden: boolean;
  tipoPagoList = Object.keys(RentaStatusId).filter(key => !isNaN(Number(key)))
                  .map(key => ({
                      name: RentaStatusId[key as any],
                      value: key
                  }));

  mostrarSeccion2: boolean = false;
  mostrarConfirmacion: boolean = false;
  estadoProceso: string = '';
  paymentHistory: rentRequestDto[] = [];
  rentAmmount: number = 0;
  paymentTypeId: any;

  constructor(
    private fb: FormBuilder,
    private companyService: CompanyService,
    private userStateService: UserStateService,
    public navigationService: NavigationService
  ) 
  {
    this.userState = userStateService.getUserStateLocalStorage();
    this.companyForm = this.initCompanyForm();
    this.isSearchPanelHidden = false;
    
  }

  ngOnInit(): void {
    this.userState = this.userStateService.getUserStateLocalStorage();
    
    this.searchCompaniesInternal('-1');
    this.navigationService.checkScreenSize();
    
  }

  private initCompanyForm(): FormGroup {
    return this.fb.group({
      companyId: [0],
      name: ['', [Validators.required, Validators.maxLength(50)]],
      address: ['', [Validators.required, Validators.maxLength(50)]],
      createdBy: [0],
      createdByUserName: ['', Validators.maxLength(50)],
      creationDate: [''],
      creationDateUI: [''],
      lastUpdateBy: [0],
      lastUpdateByUserName: ['', Validators.maxLength(50)],
      paymentsJson: [''],
      statusId: [0],
      statusDesc: [''],
      expirationDate: [''],
      expirationDateUI: [''],
      telNumber: ['', Validators.maxLength(10)],
      celNumber: ['', Validators.maxLength(10)],
      email: ['', Validators.maxLength(50)],
      ownerInfo: [''],
    });
  }

  newCompany(): void {
    this.selectedCompany = null;
    this.companyForm.reset();
    this.companyForm.patchValue({companyId:0, name:'', address:'', statusId:0});
  }

  selectCompany(companyId: number): void {
    this.companyService.getCompany(companyId).pipe(first())
    .subscribe(c => {
        c.expirationDateUI = undefined;
        c.creationDateUI = undefined;
        
        if(c.expirationDate != null)
        {
          c.expirationDateUI = format(c.expirationDate, 'dd-MMM-yyyy hh:mm a');
        }
        if(c.creationDate != null)
        {
            c.creationDateUI = format(c.creationDate, 'dd-MMM-yyyy hh:mm a');
        }
        c.statusDesc = getCompanyStatusEnumName(c.statusId);
        
      this.selectedCompany = c;
      this.companyForm.patchValue(c);
      this.activeTab = "tabDetalles";
      this.navigationService.checkScreenSize();
    });
  }

  setActiveTab(tab: string): void {
    this.activeTab = tab;
    switch (tab) {
      case 'tabDetalles':
        
        break;
      case 'tabPagos':
        this.getPaymentHistory();
        this.paymentTypeId = 0;
        break;
      case 'tab3':
        
        break;
    }
  }
  

  saveCompany(): void {
    if (this.companyForm.invalid) {
      this.companyForm.markAllAsTouched();
      
      return;
    }

    if (this.companyForm.valid) {
      const c: companyDto = this.companyForm.value;
      c.lastUpdateBy = this.userState.userId;
      c.createdBy = this.userState.userId;
      c.createdByUserName = this.userState.userName;
      c.lastUpdateByUserName = this.userState.userName;
      c.creationDate = new Date();
      c.expirationDate = new Date();
      
      this.companyService.saveCompany(c).pipe(first())
        .subscribe({
          next: (result) => {
            this.searchCompaniesInternal("-1");
            this.selectCompany(result.companyId);
            this.navigationService.showUIMessage("Información guardada (" + result.name + ")");
          },
          error:(err) => {
            const m1 = err.error.message;
            if(m1)
              this.navigationService.showUIMessage(m1);
            else
              this.navigationService.showUIMessage(err.error);
          },
      });
    }
  }

  searchCompany(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    const keyword = inputElement.value || '-1'; // Default to an empty string if keyword is null or undefined
    this.searchCompaniesInternal(keyword);
  }
  
  private searchCompaniesInternal(name: string): void {
    let companyId = this.userState.companyId;

    this.companyService.getAll4List().pipe(first())
    .subscribe(c => {
      this.companyList = c.sort((a,b) => (a.name?? '').localeCompare((b.name?? '')));
    });
  }

  /***** Pagos TAB - Start *******/
  getPaymentHistory() {
    let companyId = 0;
    if(this.selectedCompany != null)
      companyId = this.selectedCompany.companyId;


    this.companyService.getPaymentHistory(companyId).pipe(first())
    .subscribe({
      next: (result) => {
        this.paymentHistory = result;
      },
      error:(err) => {
        const m1 = err.error.message;
        if(m1)
          this.navigationService.showUIMessage(m1);
        else
          this.navigationService.showUIMessage(err.error);
      },
  });
  }

  addConfirmedPayment(): void {
    if(this.rentAmmount <= 0)
    {
      this.navigationService.showUIMessage('El monto debe ser mayor a cero.');
      return;
    }

    const renta: rentRequestDto = {
      id:0,
      addedByUserId:this.userState.userId, 
      amount: this.rentAmmount, 
      companyId:this.userState.companyId, 
      referenceDate:new Date(), 
      statusId:this.paymentTypeId
    };

    this.companyService.addCompanyRent(renta).pipe(first())
    .subscribe({
      next: (result) => {
        this.estadoProceso = 'Pago agregado con éxito';
        this.mostrarConfirmacion = false; // Oculta el panel de confirmación
        this.getPaymentHistory(); // Actualiza el historial de pagos
      },
      error:(err) => {
        this.estadoProceso = 'Error al agregar el pago';
        const m1 = err.error.message;
        if(m1)
          this.navigationService.showUIMessage(m1);
        else
          this.navigationService.showUIMessage(err.error);
      }
    });
  }

  confirmarPago(monto: number, metodo: string): void {
    this.estadoProceso = ''; // Resetea el estado del proceso
    this.mostrarConfirmacion = true; // Muestra el panel de confirmación
  }

  cancelarPago(): void {
    this.mostrarConfirmacion = false; // Oculta el panel de confirmación
  }
 
  /*** End TAB - Pagos *****/

}


