import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { CustomerService } from '../../services/customer.service';
import { userDto } from '../../dto/userDto';
import { UserStateService } from '../../services/user-state.service';
import { customerDto } from '../../dto/customerDto';
import { first } from 'rxjs';
import { NavigationService } from '../../services/navigation.service';
import { NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-customer-management',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, NgFor, NgIf],
  templateUrl: './customer-management.component.html',
  styleUrl: './customer-management.component.css'
})
export class CustomerManagementComponent {
  customerForm: FormGroup;
  customerId!: number;
  userState: userDto | undefined;
  selectedCustomer: customerDto | null = null;
  customerList: customerDto[] = [];
  isSearchPanelHidden = false;
  searchWord: any;

  constructor(
    private fb: FormBuilder,
    private customerService: CustomerService,
    private route: ActivatedRoute,
    private userService: UserStateService,
    private navigationService: NavigationService
  ) 
  {
    this.customerForm = this.initializeForm();
  }

  ngOnInit(): void {
    this.userState = this.userService.getUserStateLocalStorage();
    this.customerId = this.route.snapshot.queryParams['customerId'] || 0;
    this.customerForm = this.initializeForm();
    this.searchCustomersInternal('-1');
    this.checkScreenSize();
    if (this.customerId > 0) {
      this.loadCustomerData(this.customerId);
    }
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
    this.customerForm?.patchValue({customerId:0, lastName1:'', lastName2:'', firstName:'', cellPhoneNumber:'', workPhoneNumber:'', email:''});
    window.scrollTo(0, 0);
  }

  saveCustomer(): void {
    if (this.customerForm?.invalid) {
      this.customerForm.markAllAsTouched();
      return;
    }
    if (this.customerForm?.valid) {
      console.log("Saving... " + this.customerForm.value);
      this.customerService.saveCustomer(this.customerForm.value).subscribe({
        next: (savedCustomer) => {
          this.searchCustomersInternal('-1');
          this.selectUser(savedCustomer.customerId);
          this.navigationService.showUIMessage("Cliente guardado (" + savedCustomer.customerId + ")");
          window.scrollTo(0, 0);
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
  
  checkScreenSize() {
    if (window.innerWidth < 768) {
      this.isSearchPanelHidden = true;  // Ocultar búsqueda en pantallas pequeñas por defecto
      this.searchWord = '';
    }
  }

  selectUser(customerId: number): void {
    this.customerService.getCustomer(customerId).pipe(first())
    .subscribe(customer => {
      this.selectedCustomer = customer;
      this.customerForm?.patchValue(customer);

      this.checkScreenSize();
      
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
     console.log(JSON.stringify(this.userState));
      this.customerService.getAllCustomer(this.userState.companyId, nameOrPhoneOrEmail).pipe(first())
      .subscribe(users => {
        this.customerList = users.sort((a,b) => (a.firstName?? '').localeCompare((b.firstName?? '')));
      });
    }
    else
    {
      console.error("Estado de usuario invalido.");
    }
  }

  toggleSearchPanel(): void {
    this.isSearchPanelHidden = !this.isSearchPanelHidden;
  }
}
