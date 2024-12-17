import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NavigationService } from '../../services/navigation.service';
import { UserStateService } from '../../services/user-state.service';
import { format } from 'date-fns';
import { NgFor, NgIf } from '@angular/common';
import { userDto } from '../../dto/userDto';
import { companyDto } from '../../dto/companyDto';
import { CompanyService } from '../../services/company.service';
import { first } from 'rxjs';
import { getCompanyStatusEnumName, Roles } from '../../Enums/enums';

@Component({
  selector: 'app-company-management',
  standalone: true,
  imports: [ReactiveFormsModule,  NgFor, NgIf],
  templateUrl: './company-management.component.html',
  styleUrl: './company-management.component.css'
})
export class CompanyManagementComponent implements OnInit {
  companyForm: FormGroup;
  selectedCompany: companyDto | null = null;
  companyList: companyDto[] = [];
  userState: userDto;
  rol = Roles;
  
  constructor(
    private fb: FormBuilder,
    private companyService: CompanyService,
    private userStateService: UserStateService,
    public navigationService: NavigationService
  ) 
  {
    this.userState = userStateService.getUserStateLocalStorage();
    this.companyForm = this.initCompanyForm();
    
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
      statusId: [0, Validators.required],
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
      this.navigationService.checkScreenSize();
    });
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
}


