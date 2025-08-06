import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { companySettingsDto, pairSettingsDto, settingsDto } from '../../dto/companySettingsDto';
import { CompanyService } from '../../services/company.service';
import { NavigationService } from '../../services/navigation.service';
import { UserStateService } from '../../services/user-state.service';
import { userDto } from '../../dto/userDto';
import { first } from 'rxjs';
import { AlertLevel } from '../../Enums/enums';
import { QzprintService } from '../../services/qzprint.service';
import { NgFor, NgIf } from '@angular/common';
import { environment } from '../../environments/enviroment';

@Component({
  selector: 'app-settings',
  imports: [ReactiveFormsModule, NgFor, NgIf],
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent implements OnInit {
  isSearchingPrinters: boolean = false;
  settingsForm: FormGroup;
  userState: userDto;
  printerList: string[] | undefined;

  constructor(
    private fb: FormBuilder,
    private companyService: CompanyService,
    private userStateService: UserStateService,
    public navigationService: NavigationService,
    private qzService: QzprintService
  ) {
    this.userState = userStateService.getUserStateLocalStorage();
    this.settingsForm = this.initSettingsForm();
  }

  ngOnInit(): void {
    this.settingsForm = this.initSettingsForm();
    this.loadSettings(this.companyService.selectedCompanyId);
    this.loadMaxDaysToRefund(this.companyService.selectedCompanyId);
  }

  private initSettingsForm(): FormGroup {
    return this.fb.group({
      activarImpresionWeb: [false],
      impresoraWeb: [''],
      selectSelectedPrinter: [''],
      maxDaysToRefund: [0]
    });
  }

  setPrinter(event: Event) {
    const selectElement = event.target as HTMLSelectElement;
    const selectedPrinter = selectElement.value;

    this.settingsForm.get('impresoraWeb')?.setValue(selectedPrinter);
  }

  async buscarImpresoras(): Promise<void> {
    this.isSearchingPrinters = true;
    this.printerList = await this.qzService.findPrinters();
  }

  loadSettings(companyId: number): void {
    this.isSearchingPrinters = false;
    this.companyService.getCompanySettings(companyId).pipe(first())
      .subscribe({
        next: (settings) => {
          if (settings && settings.length > 0) {
            for (let s of settings) {
              if (s.key === environment.PAIRSETT_ACTIVAR_IMPRESORA) {
                this.settingsForm.get(environment.PAIRSETT_ACTIVAR_IMPRESORA)?.setValue(s.value === 'true');
              }
              if (s.key === environment.PAIRSETT_NOMBRE_IMPRESORA) {
                this.settingsForm.get(environment.PAIRSETT_NOMBRE_IMPRESORA)?.setValue(s.value);
              }
            }
          } else {
            this.settingsForm.reset();
          }
        },
        error: (err: any) => {
          const m1 = err.error?.message;
          if (m1)
            this.navigationService.showUIMessage(m1);
          else
            this.navigationService.showUIMessage(err.error);
        },
      });
  }

  loadMaxDaysToRefund(companyId: number): void {
    this.companyService.getMaxDaysToRefund(companyId)
      .pipe(first())
      .subscribe({
        next: (days: number) => {
          this.settingsForm.get('maxDaysToRefund')?.setValue(days);
        },
        error: (err: any) => {
          console.error('Error al obtener maxDaysToRefund', err);
          this.navigationService.showUIMessage("No se pudo cargar la configuración de días de devolución.");
        }
      });
  }

  onSave(): void {
    if (this.settingsForm.valid) {
      this.isSearchingPrinters = false;
      let settings: settingsDto = this.settingsForm.value;
      let pairSettings: pairSettingsDto[] = [];
      pairSettings.push({ key: environment.PAIRSETT_ACTIVAR_IMPRESORA, value: settings.activarImpresionWeb.toString() });
      pairSettings.push({ key: environment.PAIRSETT_NOMBRE_IMPRESORA, value: settings.impresoraWeb });

      let companySettings: companySettingsDto = { companyId: this.companyService.selectedCompanyId, settings: pairSettings };

      this.companyService.setCompanySettings(companySettings)
        .pipe(first())
        .subscribe({
          next: () => {
            this.navigationService.showUIMessage("Configuración guardada", AlertLevel.Sucess);
          },
          error: (err: any) => {
            const m1 = err.error?.message;
            if (m1)
              this.navigationService.showUIMessage(m1);
            else
              this.navigationService.showUIMessage(err.error);
          }
        });
    }
  }

}
