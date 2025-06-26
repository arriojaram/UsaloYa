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
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { LoadingService } from '../services/loading.service';
import { FormValidationService } from '../services/form-validation.service';

@Component({
  selector: 'app-questions',
  standalone: true,
  imports: [ReactiveFormsModule, NgFor, MatProgressSpinnerModule],
  templateUrl: './questions.component.html',
  styleUrls: ['./questions.component.css']
})
export class QuestionsComponent implements OnInit, OnDestroy {
  questions: string[] = [];
  form!: FormGroup;
  loading_i$ = this.loadingService.loading$;



  private destroy$ = new Subject<void>();

  constructor(
    private questionService: QuestionService,
    private sharedDataService: SharedDataService,
    private registerDataService: RegisterDataService,
    private userService: UserService,
    private router: Router,
    private fb: FormBuilder,
    private navigationService: NavigationService,
    private translate: TranslateService,
    private loadingService: LoadingService,
    private validationService: FormValidationService
  ) {}

  ngOnInit(): void {
    this.loadQuestions();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadQuestions(): void {
    this.loadingService.show();
    this.questionService.getQuestions()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.questions = data;
          this.createForm(data.length);
          this.loadingService.hide();
        },
        error: () => {
          this.navigationService.showUIMessage(
            this.translate.instant('questions.load_error'),
            AlertLevel.Error
          );
          
        }
      });
  }

  createForm(length: number) {
    const group: any = {};
    for (let i = 0; i < length; i++) {
      group[`respuesta${i}`] = [''];
    }
    this.form = this.fb.group(group);
    // Registrar validez inicial
this.validationService.setFormValid('questions', this.form.valid);

// Actualizar cada vez que el estado cambie
this.form.statusChanges
  .pipe(takeUntil(this.destroy$))
  .subscribe(() => {
    this.validationService.setFormValid('questions', this.form.valid);
  });

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

  this.loadingService.show();

  this.userService.registerNewUser(payload)
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: (_) => {
        this.navigationService.showUIMessage(
          this.translate.instant('questions.sucess'),
          AlertLevel.Sucess
        );
        this.loadingService.hide();

        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.navigationService.showUIMessage(
          this.translate.instant('questions.register_error') + err.message,
          AlertLevel.Error
        );
        this.loadingService.hide();
      }
    });
}
}