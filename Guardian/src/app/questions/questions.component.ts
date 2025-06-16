import { Component, OnInit, OnDestroy } from '@angular/core';
import { QuestionService } from '../services/questions.service';
import { ReactiveFormsModule } from '@angular/forms';
import { NgIf, NgFor } from '@angular/common';
import { Subject, takeUntil } from 'rxjs';

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
  error: string | null = null;

  private destroy$ = new Subject<void>();

  constructor(private questionService: QuestionService) { }

  ngOnInit(): void {
    this.loadQuestions();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadQuestions(): void {
    this.loading = true;
    this.error = null;

    this.questionService.getQuestions()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.questions = data;
          this.loading = false;
        },
        error: (err) => {
          // Este mensaje se muestra en la vista mediante *ngIf="error"
          this.error = 'Error al cargar las preguntas';

          // Mostramos el error técnico en la consola del navegador para depuración.
          console.error(err);
          this.loading = false;
        }
      });
  }
}
