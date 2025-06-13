import { Component, OnInit } from '@angular/core';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import {  NgFor, NgClass } from '@angular/common';
import { filter } from 'rxjs/operators';


@Component({
  selector: 'app-stepper',
  standalone: true,
  imports: [RouterModule, NgFor, NgClass],
  templateUrl: './stepper.component.html',
  styleUrls: ['./stepper.component.css']
})
export class StepperComponent implements OnInit {
  currentStep = 0;

  steps = [
    { label: 'Usuario', route: 'register', completed: false },
    { label: 'Compañía', route: 'rcompany', completed: false },
    { label: 'Preguntas', route: 'questions', completed: false }
  ];

  constructor(private router: Router) {}

  ngOnInit() {
    // Navega al primer paso si no hay ruta activa
    if (this.router.url === '/stepper') {
      this.router.navigate(['stepper', this.steps[0].route]);
    }

    // Detectar cambio de ruta y actualizar el stepper
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        const url = event.urlAfterRedirects || event.url;
        const foundIndex = this.steps.findIndex(step => url.includes(step.route));

        if (foundIndex !== -1) {
          // Si se avanzó, marca el paso anterior como completado
          if (foundIndex > this.currentStep) {
            this.steps[this.currentStep].completed = true;
          }

          this.currentStep = foundIndex;
        }
      });
  }

  goNext() {
    if (this.currentStep < this.steps.length - 1) {
      this.steps[this.currentStep].completed = true;
      this.currentStep++;
      this.router.navigate(['stepper', this.steps[this.currentStep].route]);
    }
  }

  goBack() {
    if (this.currentStep > 0) {
      this.currentStep--;
      this.router.navigate(['stepper', this.steps[this.currentStep].route]);
    }
  }
navigateToStep(index: number): void {
  const step = this.steps[index];

  if (!step || !step.route) {
    console.error(`Paso inválido en el índice ${index}`, step);
    return;
  }

  this.router.navigate([step.route]);
}


  markStepAsCompleted(index: number) {
    if (this.steps[index]) {
      this.steps[index].completed = true;
    }
  }
}
