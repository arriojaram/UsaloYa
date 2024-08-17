import { Component } from '@angular/core';
import { UserService } from '../../services/user.service';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { userDto } from '../../dto/userDto';
import { NavigationService } from '../../services/navigation.service';
import { UserStateService } from '../../services/user-state.service';
import { NgFor, NgIf } from '@angular/common';
import { format } from 'date-fns';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [ReactiveFormsModule, NgFor, NgIf],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.css'
})
export class UserManagementComponent {
  userForm: FormGroup;
  passwordForm: FormGroup;
  selectedUser: userDto | null = null;
  isSearchPanelHidden = false;
  userList: userDto[] = [];
  userState: userDto;
  passwordErrorMsg: string;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private userStateService: UserStateService,
    private route: ActivatedRoute,
    private navigationService: NavigationService
  ) 
  {
    this.userState = userStateService.getUserStateLocalStorage();

    this.userForm = this.initUserForm();
    this.passwordForm = this.initPasswordForm();
    this.passwordErrorMsg = '';
  }

  ngOnInit(): void {
    this.userForm.get('lastAccess')?.disable();
    this.userForm.get('statusId')?.disable();
    this.searchUsersInternal('-1');
  }

  private initUserForm(): FormGroup {
    return this.fb.group({
      userId: [0],
      userName: ['', [Validators.required, Validators.maxLength(50)]],
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      companyId: ['', Validators.required],
      groupId: ['', Validators.required],
      lastAccess4UI: [''],
      isEnabled: [true, Validators.required],
      statusIdStr: ['']
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
    this.userForm.patchValue({userName:'', firstName:'', lastName:'', groupId:0, isEnabled:true, password:''});
  }

  selectUser(userId: number): void {
    this.userService.getUser(userId).subscribe(user => {
      user.lastAccess4UI = undefined;
      if(user.lastAccess != null)
        user.lastAccess4UI = format(user.lastAccess, 'dd-MMM-yyyy hh:mm a');

      switch (user.statusId) {
        case 0:
          user.statusIdStr = "Desconectado"
          break;
        case 1:
          user.statusIdStr = "Conectado"
          break;
        case 2:
            user.statusIdStr = "Deshabilitado"
            break;
        default:
           user.statusIdStr = "Desconocido"
          break;
      }

      this.selectedUser = user;
      this.userForm.patchValue(user);
    });
  }

  saveUser(): void {
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      
      return;
    }

    if (this.userForm.valid) {
      const user: userDto = this.userForm.value;
      user.token = "a-Fc1C149Afbf4c8--996++";
     
      this.userService.saveUser(user).subscribe({
        next: (result) => {
          this.searchUsersInternal("-1");
          this.selectUser(result.userId);
          this.navigationService.showUIMessage("Usuario guardado (" + result.userName + ")");
        },
        error:(err) => {
          this.navigationService.showUIMessage(err.error.message);
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

      this.userService.setPassword(username, this.passwordForm.value.password).subscribe(result => {
        this.navigationService.showUIMessage("Password actualizado.");
      });
    } 
  }
  searchUsers(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    const keyword = inputElement.value || '-1'; // Default to an empty string if keyword is null or undefined
    this.searchUsersInternal(keyword);
  }
  
  private searchUsersInternal(name: string): void {
    this.userService.getAllUser(this.userState.companyId, name).subscribe(users => {
      this.userList = users;
    });
  }

  toggleSearchPanel(): void {
    this.isSearchPanelHidden = !this.isSearchPanelHidden;
  }
}
