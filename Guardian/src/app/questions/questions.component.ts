import { Component, OnInit, OnDestroy } from '@angular/core';
import { QuestionService } from '../services/questions.service';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { NgIf, NgFor } from '@angular/common';
import { Subject, takeUntil } from 'rxjs';
import { SaveQuestionDto } from '../dto/SaveQuestionDto';
import { SharedDataService } from '../services/shared-data.service';
import { NavigationService } from '../services/navigation.service';
import { AlertLevel } from '../Enums/enums';
import { Router } from '@angular/router';
import { RegisterDataService } from '../services/register-data.service';
import { RegisterUserQuestionnaireAndCompanyDto } from '../dto/RegisterUserQuestionnaireAndCompanyDto';
import { UserService } from '../services/user.service';
import { TranslateService } from '@ngx-translate/core';

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
    private registerDataService: RegisterDataService,
    private userService: UserService,
    private router: Router,
    private fb: FormBuilder,
    private navigationService: NavigationService,
    private translate: TranslateService
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
            this.translate.instant('questions.load_error'),
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
    const userData = this.registerDataService.getUserData();
    const companyData = this.registerDataService.getCompanyData();

    if (!userData || !companyData) {
      this.navigationService.showUIMessage(
        this.translate.instant('questions.missing_data'),
        AlertLevel.Error
      );
      return;
    }

    const answers: SaveQuestionDto[] = this.questions.map((question, index) => {
      const respuesta = this.form.get(`respuesta${index}`)?.value;
      return {
        questionName: question,
        reply: respuesta === 'si',
        idUser: 0
      };
    });

    const payload: RegisterUserQuestionnaireAndCompanyDto = {
      requestRegisterNewUserDto: userData,
      companyDto: companyData,
      requestSaveQuestionnaireDto: answers
    };

    this.loading = true;

    this.userService.registerNewUser(payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (_) => {
          this.navigationService.showUIMessage(
            this.translate.instant('questions.sucess'),
            AlertLevel.Sucess
          );
          this.loading = false;
          this.router.navigate(['/login']);
        },
        error: (err) => {
          this.navigationService.showUIMessage(
            this.translate.instant('questions.register_error') + err.message,
            AlertLevel.Error
          );
          this.loading = false;
        }
      });
  }
}
