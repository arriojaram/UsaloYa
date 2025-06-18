import { Component, OnInit, OnDestroy } from '@angular/core';
import { QuestionService } from '../services/questions.service';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { NgIf, NgFor } from '@angular/common';
import { Subject, takeUntil } from 'rxjs';
import { SaveQuestionDto } from '../dto/saveQuestionDto';
import { SharedDataService } from '../services/shared-data.service';
import { NavigationService } from '../services/navigation.service';
import { AlertLevel } from '../Enums/enums';
import { Router } from '@angular/router';

@Component({
  selector: 'app-questions',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf, NgFor],
  templateUrl: './questions.component.html',
  styleUrls: ['./questions.component.css']
})
export class QuestionsComponent implements OnInit, OnDestroy {
  questions: string[] = [];
  loading: boolean = false;
  form!: FormGroup;

  private destroy$ = new Subject<void>();

  constructor(
    private questionService: QuestionService,
    private sharedDataService: SharedDataService,
    private router: Router,
    private fb: FormBuilder,
    private navigationService: NavigationService
  ) {}

  ngOnInit(): void {
    this.loadQuestions();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadQuestions(): void {
    this.loading = true;

    this.questionService.getQuestions()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.questions = data;
          this.createForm(data.length);
          this.loading = false;
        },
        error: () => {
          this.navigationService.showUIMessage(
            'Error al cargar las preguntas. Intenta más tarde.',
            AlertLevel.Error
          );
          this.loading = false;
        }
      });
  }

  createForm(length: number) {
    const group: any = {};
    for (let i = 0; i < length; i++) {
      group[`respuesta${i}`] = [''];
    }
    this.form = this.fb.group(group);
  }

  submitAnswers(): void {
    const userId = this.sharedDataService.getUserId();

    if (userId === null || userId === undefined) {
      this.navigationService.showUIMessage(
        'No se recibió el ID del usuario. Registro incompleto.',
        AlertLevel.Error
      );
      return;
    }

    const payload: SaveQuestionDto[] = this.questions.map((question, index) => {
      const respuesta = this.form.get(`respuesta${index}`)?.value;
      return {
        QuestionName: question,
        Reply: respuesta === 'si',
        IdUser: userId
      };
    });

    this.questionService.saveQuestions(payload).subscribe({
      next: () => {
        this.navigationService.showUIMessage(
          'Tus respuestas se guardaron correctamente.',
          AlertLevel.Sucess
        );

         this.router.navigate(['/login']);
      },
      error: () => {
        this.navigationService.showUIMessage(
          'Ocurrió un error al guardar las respuestas. Intenta nuevamente.',
          AlertLevel.Error
        );
      }
    });
  }
}
