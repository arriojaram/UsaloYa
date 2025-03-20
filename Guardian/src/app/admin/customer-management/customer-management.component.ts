import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { CustomerService } from '../../services/customer.service';
import { userDto } from '../../dto/userDto';
import { UserStateService } from '../../services/user-state.service';
import { customerDto } from '../../dto/customerDto';
import { first } from 'rxjs';
import { NavigationService } from '../../services/navigation.service';
import { NgClass, NgFor, NgIf } from '@angular/common';
import { AlertLevel, Roles } from '../../Enums/enums';
import { environment } from '../../environments/enviroment';

@Component({
    selector: 'app-customer-management',
    imports: [ReactiveFormsModule, FormsModule, NgFor, NgIf, NgClass],
    templateUrl: './customer-management.component.html',
    styleUrl: './customer-management.component.css'
})
export class CustomerManagementComponent implements OnInit{
  customerForm: FormGroup;
  customerId!: number;
  userState: userDto | undefined;
  selectedCustomer: customerDto | null = null;
  customerList: customerDto[] = [];

  constructor(
    private fb: FormBuilder,
    private customerService: CustomerService,
    private route: ActivatedRoute,
    private userService: UserStateService,
    public navigationService: NavigationService
  ) 
  {
    this.customerForm = this.initializeForm();
  }

  ngOnInit(): void {
    this.userState = this.userService.getUserStateLocalStorage();
    this.customerId = this.route.snapshot.queryParams['customerId'] || 0;
    this.customerForm = this.initializeForm();
    this.searchCustomersInternal('-1');
    this.navigationService.checkScreenSize();
    if (this.customerId > 0) {
      this.loadCustomerData(this.customerId);
    }

    this.navigationService.showFreeLicenseMsg(this.userState.companyStatusId?? 0);
  }

  // Inicializa el formulario
  initializeForm(): FormGroup {
    return this.fb.group({
      customerId: [0],
      address: [''],
      notes: [''],
      firstName: ['', Validators.required],
      lastName1: ['', Validators.required],
      lastName2: [''],
      workPhoneNumber: [''],
      cellPhoneNumber: ['', Validators.required],
      email: ['', [Validators.email]],
      companyId: [this.userState?.companyId, Validators.required]
    });
  }

  loadCustomerData(customerId: number): void {
    this.customerService.getCustomer(customerId).pipe(first())
    .subscribe((customer: customerDto) => {
      this.customerForm?.patchValue(customer);
    });
  }

  newCustomer(): void {
    this.selectedCustomer = null;
    this.customerForm?.reset();
    this.customerForm?.patchValue({companyId:this.userState?.companyId, customerId:0, lastName1:'', lastName2:'', firstName:'', cellPhoneNumber:'', workPhoneNumber:'', email:''});
    window.scrollTo(0, 0);
  }

  saveCustomer(): void {
    if (this.customerForm?.invalid) {
      this.customerForm.markAllAsTouched();
      return;
    }
    if (this.customerForm?.valid) {
      let customerDto : customerDto = this.customerForm.value;
      this.customerService.saveCustomer(customerDto).subscribe({
        next: (savedCustomer) => {
          if(customerDto.customerId == 0)
            this.customerList.unshift(savedCustomer);
          
          this.selectUser(savedCustomer.customerId);
          this.navigationService.showUIMessage("Cliente guardado (" + savedCustomer.customerId + ")", AlertLevel.Sucess);
        },
        error: (e) => 
        {
          this.navigationService.showUIMessage(e.error.message);
        }
      });
    } else {
      console.log('Form is invalid');
    }
  }
  
  selectUser(customerId: number): void {
    this.customerService.getCustomer(customerId).pipe(first())
    .subscribe(customer => {
      this.selectedCustomer = customer;
      this.customerForm?.patchValue(customer);
      this.customerForm.markAllAsTouched();
      this.navigationService.checkScreenSize();
      
    });
  }

  searchCustomers(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    const keyword = inputElement.value || '-1'; // Default to an empty string if keyword is null or undefined
    
    this.searchCustomersInternal(keyword);
  }
  
  private searchCustomersInternal(nameOrPhoneOrEmail: string): void {
    if(this.userState != null)
    {
     
      this.customerService.getAllCustomer(this.userState.companyId, nameOrPhoneOrEmail).pipe(first())
      .subscribe({
        next:(users) => {
          this.customerList = users.sort((a,b) => (a.firstName?? '').localeCompare((b.firstName?? '')));
          if(users.length > 0)
          {
            this.selectUser(users[0].customerId);
          }
        },
        error:(err) => {
          this.navigationService.showUIMessage("Ocurrio un error al cargar la información de los clientes", AlertLevel.Error);
        },
      });
    }
    else
    {
      console.error("Estado de usuario invalido.");
    }
  }

}
