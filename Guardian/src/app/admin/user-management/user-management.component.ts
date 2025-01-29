import { Component } from '@angular/core';
import { UserService } from '../../services/user.service';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup,  ReactiveFormsModule, Validators } from '@angular/forms';
import { userDto } from '../../dto/userDto';
import { NavigationService } from '../../services/navigation.service';
import { UserStateService } from '../../services/user-state.service';
import { NgFor, NgIf } from '@angular/common';
import { format } from 'date-fns';
import { first } from 'rxjs';
import { adminGroupDto } from '../../dto/adminGroupDto';
import { AdminCompanyDto } from '../../dto/adminCompanyDto';
import { AlertLevel, getUserStatusEnumName, Roles } from '../../Enums/enums';
import { CompanyService } from '../../services/company.service';

@Component({
    selector: 'app-user-management',
    imports: [ReactiveFormsModule, NgFor, NgIf],
    templateUrl: './user-management.component.html',
    styleUrl: './user-management.component.css'
})
export class UserManagementComponent {
  userForm: FormGroup;
  passwordForm: FormGroup;
  selectedUser: userDto | null = null;
  userList: userDto[] = [];
  userState: userDto;
  passwordErrorMsg: string;
  groups: adminGroupDto [] = [];
  availableRoles: any;
  showRoles: boolean = false;
  companies: AdminCompanyDto [] = [];
  rol = Roles;
  isAutorized: boolean = false;

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
    this.passwordErrorMsg = '';
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


    this.searchUsersInternal('-1');
    this.navigationService.checkScreenSize();

    this.userService.getGroups().pipe(first())
      .subscribe((data) => {
        this.groups = data;
      });

    this.companyService.getAll4List(this.userState.companyId, '-1').pipe(first())
      .subscribe(
      {
        next: (data) => 
          {  
            // Filter the companies to display only the one the user owns to
            if(!(this.userState.roleId >= this.rol.Ventas))
            {
              data = data.filter(c => c.companyId == this.userState.companyId)
            }

            this.companies = data; 
          },
        error: (error) =>
        {
          if (error.error instanceof ErrorEvent) {
            let message = error.error.message || error.statusText;
            this.navigationService.showUIMessage(message);
          }
          else{
            if(error.status == 401)
              this.navigationService.showUIMessage('No autorizado');
           
          }
          
        }
      });

    this.initRoles();
  }

  private initRoles()
  {
    if(this.userState.roleId !== 0)
    {
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

  private initPasswordForm() : FormGroup {
    return this.fb.group({
      password: ['', [Validators.required, Validators.minLength(8)]]
    });
  }

  newUser(): void {
    this.selectedUser = null;
    this.userForm.reset();
    this.userForm.patchValue({userId:0, userName:'', roleId:1, firstName:'', lastName:'', groupId:0, isEnabled:true, password:''});
  }

  selectUser(userId: number): void {
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

  saveUser(): void {
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      
      return;
    }

    if (this.userForm.valid) {
      const user: userDto = this.userForm.value;
      user.token = "a-Fc1C149Afbf4c8--996++"; //Temp password for new users
      user.lastUpdatedBy = this.userState.userId;
      user.createdBy = this.userState.userId;
      
      this.userService.saveUser(user).pipe(first())
        .subscribe({
          next: (result) => {
            this.searchUsersInternal("-1");
            this.selectUser(result.userId);
            this.navigationService.showUIMessage("Usuario guardado (" + result.userName + ")", AlertLevel.Sucess);
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
    if(this.selectedUser == null){
      this.passwordForm.markAllAsTouched(); 
      this.passwordErrorMsg = 'Selecciona un usuario';
      return;
    }

    if (this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched(); 
      this.passwordErrorMsg = 'La contraseña debe tener una longitud mínima de 8 caracteres.';
      return;
    }
    
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
    const keyword = inputElement.value || '-1'; // Default to an empty string if keyword is null or undefined
    this.searchUsersInternal(keyword);
  }
  
  private searchUsersInternal(name: string): void {
    let companyId = this.userState.companyId;
    if(this.userState.roleId === Roles.Root)
      companyId = 0;

    this.userService.getAllUser(companyId, name).pipe(first())
    .subscribe({
      next: (users) => {
        this.userList = users.sort((a,b) => (a.firstName?? '').localeCompare((b.firstName?? '')));
      },
      error: (e) => {
        this.navigationService.showUIMessage(e.error);
      }
    });
  }

}
