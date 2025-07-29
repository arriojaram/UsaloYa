import { Component } from '@angular/core';
import { UserService } from '../../services/user.service';
import { ActivatedRoute } from '@angular/router';
import { AbstractControl, FormBuilder, FormGroup,  ReactiveFormsModule, Validators } from '@angular/forms';
import { userDto } from '../../dto/userDto';
import { NavigationService } from '../../services/navigation.service';
import { UserStateService } from '../../services/user-state.service';
import { NgClass, NgFor, NgIf, NgStyle } from '@angular/common';
import { format } from 'date-fns';
import { first } from 'rxjs';
import { adminGroupDto } from '../../dto/adminGroupDto';
import { AdminCompanyDto } from '../../dto/adminCompanyDto';
import { AlertLevel, CompanyStatus, getUserStatusEnumName, Roles } from '../../Enums/enums';
import { CompanyService } from '../../services/company.service';
import { environment } from '../../environments/enviroment';


@Component({
    selector: 'app-user-management',
    imports: [ReactiveFormsModule, NgFor, NgIf, NgClass, NgStyle ],
    templateUrl: './user-management.component.html',
    styleUrl: './user-management.component.css',
   
})
export class UserManagementComponent {
  userForm: FormGroup;
  passwordForm: FormGroup;
  selectedUser: userDto | null = null;
  selectedCompany: AdminCompanyDto | null = null;
  expandedCompanyIds: number[] = [];
  userList: userDto[] = [];
  userListsByCompany: { [companyId: number]: userDto[] } = {};
  userState: userDto;
  
  groups: adminGroupDto [] = [];
  availableRoles: any;
  showRoles: boolean = false;
  companies: AdminCompanyDto [] = [];
  rol = Roles;
  cStatus = CompanyStatus;
  isAutorized: boolean = false;

  passwordVisible: boolean = false;
  keyword: string = "";

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private companyService: CompanyService,
    private userStateService: UserStateService,
    private route: ActivatedRoute,
    public navigationService: NavigationService
  ) 
  {
    this.userState = userStateService.getUserStateLocalStorage();

    this.userForm = this.initUserForm();
    if(this.userState.roleId < this.rol.Admin)
      this.userForm.get('roleId')?.disable();
    
    this.passwordForm = this.initPasswordForm();
    
  }

  ngOnInit(): void {
    this.userState = this.userStateService.getUserStateLocalStorage();
    this.userForm.get('lastAccess')?.disable();
    this.userForm.get('statusId')?.disable();

    if(this.userState.roleId < Roles.Admin)
    {
      this.navigationService.showUIMessage("Petición incorrecta.");
      return;
    }
    else
      this.isAutorized = true;


    this.searchCompaniesInternal();
    this.navigationService.checkScreenSize();

    this.userService.getGroups().pipe(first())
      .subscribe((data) => {
        this.groups = data;
      });

    this.initRoles();
    this.navigationService.showFreeLicenseMsg(this.userState.companyStatusId?? 0);
  }

  private initRoles()
  {
    if(this.userState.roleId !== 0)
    {
      if(this.userState.companyStatusId == CompanyStatus.Free)
      {
        this.availableRoles = [];
        return;
      }

      this.showRoles = true;
      this.availableRoles = Object.values(Roles)
      .filter(value => typeof value === 'number')
      .map(roleId => ({
          id: roleId as number,
          name: Roles[roleId as number] as string
      }));
      // Delete this role by security purposes and on purpose, this rol must be assigned directly on the DB
      this.availableRoles = this.availableRoles.filter((role: { id: any; }) => role.id !== Roles.Root);

      if(this.userState.roleId <= Roles.Admin)
      {
        
        this.availableRoles = this.availableRoles.filter((role: { id: any; }) => role.id !== Roles.Ventas);
        this.availableRoles = this.availableRoles.filter((role: { id: any; }) => role.id !== Roles.SysAdmin);  
      }
      else if(this.userState.roleId <= Roles.Ventas)
      {
        this.availableRoles = this.availableRoles.filter((role: { id: any; }) => role.id !== Roles.SysAdmin);    
      }
      
      const groupIdControl = this.userForm.get('groupId');
      const roleIdControl = this.userForm.get('roleId');
      this.userState.roleId > 1 ? groupIdControl?.enable() : groupIdControl?.disable();   
      this.userState.roleId > 1 ? roleIdControl?.enable() : roleIdControl?.disable();   
    }
  }

  private initUserForm(): FormGroup {
    return this.fb.group({
      userId: [0],
      userName: ['', [Validators.required, Validators.maxLength(50)]],
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      companyId: ['', Validators.required],
      groupId: [0, Validators.required],
      lastAccess4UI: [''],
      isEnabled: [true, Validators.required],
      statusIdStr: [''],
      createdByUserName: [''],
      lastUpdatedByUserName: [''],
      creationDateUI: [''],
      roleId: [0]
    });
  }

  togglePasswordVisibility(): void {
    this.passwordVisible = !this.passwordVisible;
  }

  private initPasswordForm() : FormGroup {
    let passForm = this.fb.group({
      password: ['', [Validators.required, Validators.minLength(8)]],
      passwordConfirmation:['', [Validators.required, Validators.minLength(8)]],
    },);

    passForm.addValidators(this.passwordMatchValidator);

    return passForm;
  }

  passwordMatchValidator(formGroup: AbstractControl): { [key: string]: boolean } | null {
    const password = formGroup.get('password')?.value;
    const confirmPassword = formGroup.get('passwordConfirmation')?.value;
    return password === confirmPassword ? null : { mismatch: true };
  }

  private resetPasswordForm() {
    this.passwordForm.reset();
  }

  newUser(): void {
    this.selectedUser = null;
    this.userForm.reset();
    this.userForm.patchValue({userId:0, userName:'', roleId:1, firstName:'', lastName:'', groupId:0, isEnabled:true, password:''});
  }

  selectUser(userId: number): void {
    this.resetPasswordForm();
    this.userService.getUser(userId).pipe(first())
    .subscribe(user => {
        user.lastAccess4UI = undefined;
        const roleIdControl = this.userForm.get('roleId');
        roleIdControl?.enable();
      

        if(user.lastAccess != null)
        {
          user.lastAccess4UI = format(user.lastAccess, 'dd-MMM-yyyy hh:mm a');
        }
        if(user.creationDate != null)
        {
            user.creationDateUI = format(user.creationDate, 'dd-MMM-yyyy hh:mm a');
        }
        user.statusIdStr = getUserStatusEnumName(user.statusId);
       
        /*if(user.roleId === 0)
        {
          user.roleId = Roles.User;

        }*/

      this.selectedUser = user;
      this.userForm.patchValue(user);
      this.navigationService.checkScreenSize();

      
      if(user.roleId === Roles.Root)
        {          
          roleIdControl?.disable({ onlySelf: true });  
        }
    });
  }

  selectCompany(companyId: number): void {
    const index = this.expandedCompanyIds.indexOf(companyId);
    if (index !== -1) {
      this.expandedCompanyIds.splice(index, 1);
    } else { 
      this.expandedCompanyIds.push(companyId);
    }
    this.resetPasswordForm();
    
    if (!this.userListsByCompany[companyId]) {
      this.userService.GetUsersByCompany(companyId).pipe(first())
      .subscribe({
        next: (users) => {
          this.userListsByCompany[companyId] = users.sort((a,b) => (a.firstName ?? '').localeCompare((b.firstName ?? '')));
          
        },
        error: (e) => {
          this.navigationService.showUIMessage(e.error);
        }
      });
    }
  }

  companiesFiltered(): AdminCompanyDto[] {
  if (this.keyword) { 
    return this.companies.filter(c => this.userListsByCompany[c.companyId]?.length > 0);
  } else {
    return this.companies;
  }
}

  

  saveUser(): void {
  if (this.userForm.invalid) {
    this.userForm.markAllAsTouched();
    return;
  }

  if (this.userForm.valid) {
    const user: userDto = this.userForm.value;
    user.token = "a-Fc1C149Afbf4c8--996++"; // Temp password for new users
    user.lastUpdatedBy = this.userState.userId;
    user.createdBy = this.userState.userId;

    this.userService.saveUser(user).pipe(first())
      .subscribe({
        next: (result) => {
          const companyId = result.companyId;

          this.userService.GetUsersByCompany(companyId).pipe(first())
            .subscribe({
              next: (users) => {
                this.userListsByCompany[companyId] = users.sort((a,b) => (a.firstName?? '').localeCompare((b.firstName?? '')));              
                this.selectUser(result.userId);
                this.navigationService.showUIMessage("Usuario guardado (" + result.userName + ")", AlertLevel.Sucess);
              },
              error: (e) => {
                this.navigationService.showUIMessage(e.error);
              }
            });
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


  setPassword(): void {
     
    if(this.selectedUser != null){
      const username = this.selectedUser.userName;

      this.userService.setPassword(username, this.passwordForm.value.password).pipe(first())
      .subscribe(result => {
        this.navigationService.showUIMessage("Password actualizado.", AlertLevel.Sucess);
      });
    } 
  }

  searchUsers(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    this.keyword = inputElement.value.trim();
    if (this.keyword) {
      this.searchUsersInternal(this.keyword);
    } else {
      this.userListsByCompany = {};
      this.expandedCompanyIds = [];
      this.searchCompaniesInternal();
    }
  }
 
  private searchUsersInternal(name: string): void {
  let companyId = this.userState.companyId;
  if(this.userState.roleId === Roles.Root)
    companyId = 0;

  this.userService.getAllUser(companyId, name).pipe(first())
    .subscribe({
      next: (users) => {
        // Limpiar anteriores resultados
        this.userListsByCompany = {};
        this.expandedCompanyIds = [];

        users.forEach(user => {
          const cid = user.companyId;
          // Si no existe la lista para esa compañía, crearla
          if (!this.userListsByCompany[cid]) {
            this.userListsByCompany[cid] = [];
            // expandir esa compañía
            this.expandedCompanyIds.push(cid);
          }
          this.userListsByCompany[cid].push(user);
        });

        // Ordenar cada lista
        Object.keys(this.userListsByCompany).forEach(cid => {
          this.userListsByCompany[+cid] = this.userListsByCompany[+cid]
            .sort((a,b) => (a.firstName ?? '').localeCompare((b.firstName ?? '')));
        });

        // Si hay al menos un usuario, seleccionamos el primero (puedes ajustar esto)
        if (users.length > 0) {
          this.selectUser(users[0].userId);
        }
      },
      error: (e) => {
        this.navigationService.showUIMessage(e.error);
      }
    });
}

  
  private searchCompaniesInternal(): void {
    this.companyService.getAll4List(this.userState.companyId, '-1').pipe(first())
    .subscribe(
      {
        next: (companies) => 
          {
            if(!(this.userState.roleId >= this.rol.Ventas))
            {
              companies = companies.filter(c => c.companyId == this.userState.companyId)
            }
            this.companies = companies.sort((a,b) => (a.name?? '').localeCompare((b.name?? '')));
       
          },
        error: (error) =>
          {
            if (error.error instanceof ErrorEvent) {
              let message = error.error.message || error.statusText;
              this.navigationService.showUIMessage(message);
            }
            else
            {
              if(error.status == 401)
                this.navigationService.showUIMessage('No autorizado');
            }          
          }
      });
    
  }

  getCompanyColor(companyId: number): string {
    const colors = ['#FF6B6B', '#6BCB77', '#4D96FF', '#FFD93D', '#FF6EC7', '#9B59B6', '#E67E22', '#1ABC9C'];
    return colors[companyId % colors.length];
  }

}
