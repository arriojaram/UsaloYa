import { Component, OnInit } from '@angular/core';
import { QuestionService } from '../services/questions.service';
import { ReactiveFormsModule } from '@angular/forms';
import { NgIf, NgFor } from '@angular/common';

@Component({
  selector: 'app-questions',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf, NgFor],
  templateUrl: './questions.component.html',
    styleUrls: ['./questions.component.css', '../../css/styles.css']
})
export class QuestionsComponent implements OnInit {
  preguntas: string[] = [];
  loading: boolean = false;
  error: string | null = null;

  constructor(private questionService: QuestionService) {}

  ngOnInit(): void {
    this.loadPreguntas();
  }

  loadPreguntas(): void {
    this.loading = true;
    this.error = null;

    this.questionService.getPreguntas().subscribe({
      next: (data) => {
        this.preguntas = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Error al cargar las preguntas';
        console.error(err);
        this.loading = false;
      }
    });
  }
}
