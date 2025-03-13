import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { NavigationService } from '../../services/navigation.service';
import { UserStateService } from '../../services/user-state.service';
import { format } from 'date-fns';
import { CommonModule, NgFor, NgIf } from '@angular/common';
import { userDto } from '../../dto/userDto';
import { companyDto } from '../../dto/companyDto';
import { CompanyService } from '../../services/company.service';
import { first } from 'rxjs';
import { AlertLevel, getCompanyStatusEnumName, RentTypeId, Roles } from '../../Enums/enums';
import { rentRequestDto } from '../../dto/rentRequestDto';
import { SettingsComponent } from "./settings.component";

@Component({
    selector: 'app-company-management',
    imports: [CommonModule, FormsModule, ReactiveFormsModule, NgFor, NgIf, SettingsComponent],
    templateUrl: './company-management.component.html',
    styleUrl: './company-management.component.css'
})
export class CompanyManagementComponent implements OnInit {
  @ViewChild('settingsComponent') settingsCompont: SettingsComponent | undefined;

  companyForm: FormGroup;
  selectedCompany: companyDto | null = null;
  companyList: companyDto[] = [];
  userState: userDto;
  rol = Roles;
  activeTab: string = 'tab1';
  isSearchPanelHidden: boolean;
  isAutorized: boolean = false;
  
  tipoPagoList = Object.keys(RentTypeId).filter(key => !isNaN(Number(key)))
                  .map(key => ({
                      name: RentTypeId[key as any],
                      value: key
                  }));

  showAddPaymentSection: boolean = false;
  mostrarConfirmacion: boolean = false;
  paymentHistory: rentRequestDto[] = [];
  rentAmmount: number = 0;
  paymentTypeId: any;
  notas: string = "";

  constructor(
    private fb: FormBuilder,
    private companyService: CompanyService,
    private userStateService: UserStateService,
    public navigationService: NavigationService
  ) 
  {
    this.userState = userStateService.getUserStateLocalStorage();
    this.companyForm = this.initCompanyForm();
    this.isSearchPanelHidden = true;
    
  }

  ngOnInit(): void {
    this.userState = this.userStateService.getUserStateLocalStorage();
    if(this.userState.roleId < Roles.Admin)
      {
        this.navigationService.showUIMessage("Petición incorrecta.");
        return;
      }
      else
      {
        this.isAutorized = true;
      }

      if(this.userState.roleId <= Roles.Free)
      {
        this.selectCompany(this.userState.companyId, true);
        this.navigationService.toggleSearchPanel();
      }
      else
      {
        this.searchCompaniesInternal('-1');      
      }
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

  ValidateEnteredAmount() {
    if(this.rentAmmount > 0)
    {
      if(this.paymentTypeId != RentTypeId.Desconocido)
        this.mostrarConfirmacion = true;
      else
        this.navigationService.showUIMessage('Elige un tipo de pago válido.', AlertLevel.Warning);
    } 
    else
      this.navigationService.showUIMessage('Ingresa un monto mayor a cero.', AlertLevel.Warning);
  }

  newCompany(): void {
    this.selectedCompany = null;
    this.companyForm.reset();
    this.companyForm.patchValue({companyId:0, name:'', address:'', statusId:0});
  }

  selectCompany(companyId: number, selectDetails: boolean): void {
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
        
        this.companyService.selectedCompanyId = c.companyId;
        this.selectedCompany = c;
        this.companyForm.patchValue(c);
        this.navigationService.checkScreenSize();
        
        if(selectDetails)
          this.activeTab = "tabDetalles";
     

    });
  }

  setActiveTab(tab: string): void {
    switch (tab) {
      case 'tabDetalles':
      
        break;
      case 'tabPagos':
        if(this.selectedCompany == null || this.selectedCompany?.companyId == 0)
          return;
        
        this.getPaymentHistory();
        this.cancelAddPayment();
      
        break;
      case 'tabSettings':
        this.activeTab = "tabSettings";
        this.settingsCompont?.loadSettings(this.selectedCompany?.companyId?? 0);
        break;
    }
    this.activeTab = tab;
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
            if(c.companyId == 0)
              this.companyList.unshift(result);
            
            this.selectCompany(result.companyId, true);
            this.navigationService.showUIMessage("Información guardada (" + result.name + ")", AlertLevel.Sucess);
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
    let userId = this.userState.userId;

    this.companyService.getAll4List(userId, name).pipe(first())
    .subscribe({
      next: (c) => {
        this.companyList = c.sort((a,b) => (a.name?? '').localeCompare((b.name?? '')));
        if(c.length > 0)
        {
          this.selectCompany(c[0].companyId, true);
        }
      },
      error: (e) =>{
        this.navigationService.showUIMessage(e.error);
      }
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
    if(this.rentAmmount <= 0 || this.selectedCompany == null)
    {
      this.navigationService.showUIMessage('El monto debe ser mayor a cero.', AlertLevel.Warning);
      return;
    }

    const renta: rentRequestDto = {
      id:0,
      addedByUserId:this.userState.userId, 
      amount: this.rentAmmount, 
      companyId:this.selectedCompany.companyId, 
      referenceDate:new Date(), 
      statusId:this.paymentTypeId,
      notas: this.notas
    };

    this.companyService.addCompanyRent(renta).pipe(first())
    .subscribe({
      next: (result) => {
        this.navigationService.showUIMessage('Pago agregado con éxito', AlertLevel.Sucess);
        this.mostrarConfirmacion = false;
        this.cancelAddPayment();
        this.getPaymentHistory(); // Actualiza el historial de pagos
        this.selectCompany(renta.companyId, false); // Recarga la información de la compañia
      },
      error:(err) => {
        this.navigationService.showUIMessage('Error al agregar el pago');
        const m1 = err.error.message;
        if(m1)
          this.navigationService.showUIMessage(m1);
        else
          this.navigationService.showUIMessage(err.error);
      }
    });
  }

  confirmarPago(monto: number, metodo: string): void {
    this.mostrarConfirmacion = true; // Muestra el panel de confirmación
  }

  cancelarPago(): void {
    this.mostrarConfirmacion = false; // Oculta el panel de confirmación
  }
 
  cancelAddPayment() {
    this.showAddPaymentSection = false; 
    this.rentAmmount=0; 
    this.paymentTypeId=0;
  }

  /*** End TAB - Pagos *****/

}


