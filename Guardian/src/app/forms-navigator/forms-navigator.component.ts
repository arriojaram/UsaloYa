import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { RouterModule } from '@angular/router';
import { NgFor, NgClass } from '@angular/common';
import { filter, Subject, takeUntil } from 'rxjs';
import { FormValidationService } from '../services/form-validation.service';
import { AlertLevel } from '../Enums/enums';
import { TranslateService } from '@ngx-translate/core';
import { NavigationService } from '../services/navigation.service';

@Component({
  selector: 'app-form-navigator',
  standalone: true,
  imports: [NgFor, NgClass, RouterModule],
  templateUrl: './forms-navigator.component.html',
  styleUrls: ['./forms-navigator.component.css']
})
export class FormNavigatorComponent implements OnInit, OnDestroy {
  currentStep = 0;
  private destroy$ = new Subject<void>();

  steps = [
    { label: 'Usuario', route: 'register', completed: false },
    { label: 'Compañía', route: 'register-company', completed: false },
    { label: 'Preguntas', route: 'questions', completed: false }
  ];

  constructor(
    private router: Router,
    private validationService: FormValidationService,
    private navigationService: NavigationService,
    private translate: TranslateService
  ) {}

  ngOnInit() {
    const basePath = '/forms-navigator';

    if (this.router.url === basePath) {
      this.router.navigate([basePath, this.steps[0].route]);
    } else {
      this.setCurrentStepFromUrl(this.router.url);
    }

    this.updateCompletedSteps(); // Actualiza completados al iniciar

    this.router.events
      .pipe(
        filter(event => event instanceof NavigationEnd),
        takeUntil(this.destroy$)
      )
      .subscribe((event: NavigationEnd) => {
        this.setCurrentStepFromUrl(event.urlAfterRedirects);
        this.updateCompletedSteps(); // Actualiza completados tras cada navegación
      });
  }

  private setCurrentStepFromUrl(url: string) {
    // Normaliza la URL para evitar query params o fragmentos
    const path = url.split('?')[0].split('#')[0];
    // Busca el índice basado en el final de la ruta
    const index = this.steps.findIndex(step => path.endsWith(step.route));
    if (index !== -1) {
      this.currentStep = index;
    }
  }
    updateCompletedSteps() {
    this.steps.forEach(step => {
      step.completed = this.validationService.isFormValidNow(step.route);
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  //Navegacion de botones
  goNext() {
    const currentKey = this.steps[this.currentStep].route;

    const isValid = this.validationService.isFormValidNow(currentKey);
    if (!isValid) {
      this.navigationService.showUIMessage(
              this.translate.instant('form-navigator.missing_data'),
              AlertLevel.Warning
            );
            return;
    }

    if (this.currentStep < this.steps.length - 1) {
      this.steps[this.currentStep].completed = true;

      this.router.navigate(['/forms-navigator', this.steps[this.currentStep + 1].route]).then(() => {
        window.scrollTo({ top: 0, behavior: 'smooth' });
      });
    }
  }

  goBack() {
    if (this.currentStep > 0) {
      this.router.navigate(['/forms-navigator', this.steps[this.currentStep - 1].route]);
    }
  }

  //Navegacion de circulos
navigateToStep(index: number): void {
  if (index < 0 || index >= this.steps.length) return;

  if (index === this.currentStep) return;

  if (index < this.currentStep) {
    this.router.navigate(['/forms-navigator', this.steps[index].route]);
    return;
  }

  // Validar el formulario actual antes de avanzar
  const currentKey = this.steps[this.currentStep].route;
  const isValid = this.validationService.isFormValidNow(currentKey);

  if (!isValid) {
     this.navigationService.showUIMessage(
              this.translate.instant('form-navigator.missing_data'),
              AlertLevel.Warning
            );
            return;
  }

  // Marcar pasos intermedios como completados si todo está válido
  for (let i = this.currentStep; i < index; i++) {
    this.steps[i].completed = true;
  }

  this.router.navigate(['/forms-navigator', this.steps[index].route]).then(() => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  });
}


  markStepAsCompleted(index: number) {
    if (this.steps[index]) {
      this.steps[index].completed = true;
    }
  }
}
