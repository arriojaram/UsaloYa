import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { RouterModule } from '@angular/router';
import { NgFor, NgClass } from '@angular/common';
import { filter, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-fromNavigator',
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

  constructor(private router: Router) {}

  ngOnInit() {
    if (this.router.url === '/forms-navigator') {
      this.router.navigate(['forms-navigator', this.steps[0].route]);
    }

    this.router.events
      .pipe(
        filter(event => event instanceof NavigationEnd),
        takeUntil(this.destroy$)
      )
      .subscribe((event: NavigationEnd) => {
        const url = event.urlAfterRedirects || event.url;
        const foundIndex = this.steps.findIndex(step => url.includes(step.route));

        if (foundIndex !== -1) {
          if (foundIndex > this.currentStep) {
            this.steps[this.currentStep].completed = true;
          }
          this.currentStep = foundIndex;
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  goNext() {
    if (this.currentStep < this.steps.length - 1) {
      this.steps[this.currentStep].completed = true;
      this.currentStep++;
      this.router.navigate(['forms-navigator', this.steps[this.currentStep].route]);
    }
  }

  goBack() {
    if (this.currentStep > 0) {
      this.currentStep--;
      this.router.navigate(['forms-navigator', this.steps[this.currentStep].route]);
    }
  }

  navigateToStep(index: number): void {
    const step = this.steps[index];

    if (!step || !step.route) {
      console.error(`Paso inválido en el índice ${index}`, step);
      return;
    }

    this.router.navigate(['forms-navigator', step.route]);
  }

  markStepAsCompleted(index: number) {
    if (this.steps[index]) {
      this.steps[index].completed = true;
    }
  }
}
