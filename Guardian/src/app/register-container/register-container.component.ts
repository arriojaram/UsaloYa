import { Component } from '@angular/core';
import { RequestRegisterNewUserDto } from '../dto/RequestRegisterNewUserDto ';
import { companyDto } from '../dto/companyDto';
import { UserService } from '../services/user.service';
import { RegisterUserAndCompanyDto } from '../dto/RegisterUserAndCompanyDto ';
import { NavigationService } from '../services/navigation.service';
import { AlertLevel } from '../Enums/enums';

@Component({
  selector: 'app-register-container',
  templateUrl: './register-container.component.html',
  styleUrls: ['./register-container.component.css']
})
export class RegisterContainerComponent {
  userData: RequestRegisterNewUserDto | null = null;
  companyData: companyDto | null = null;

  constructor(
    private userService: UserService,
    private navigationService: NavigationService
  ) {}

  onUserData(data: RequestRegisterNewUserDto) {
    this.userData = data;
  }

  onCompanyData(data: companyDto) {
    this.companyData = data;
  }

  submit() {
    if (!this.userData || !this.companyData) {
      this.navigationService.showUIMessage(
        'Completa ambos formularios antes de enviar',
        AlertLevel.Warning
      );
      return;
    }

    const payload: RegisterUserAndCompanyDto = {
    requestRegisterNewUserDto: this.userData,
    companyDto: this.companyData,
    groupDto: {
      groupId: 0,
      name: '',
      description: '',
      permissions: '',
      companyId: 0
    }
  };

    this.userService.registerNewUser(payload).subscribe({
      next: () => {
        this.navigationService.showUIMessage('¡Registro exitoso!', AlertLevel.Sucess);
        this.userData = null;
        this.companyData = null;
      },
      error: (err) => {
        this.navigationService.showUIMessage('Error en el registro: ' + err.message, AlertLevel.Error);
      }
    });
  }
}
