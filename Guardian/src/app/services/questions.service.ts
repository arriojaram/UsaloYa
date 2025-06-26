import { Injectable } from '@angular/core';
import { HttpClient, HttpBackend } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SaveQuestionDto } from '../dto/SaveQuestionDto';
import { environment } from '../environments/enviroment';
import { LoadingService } from './loading.service';

@Injectable({
  providedIn: 'root'
})
export class QuestionService {
  private rawHttp: HttpClient;
  private baseUrl = environment.apiUrlBase + '/api/Questionnaire';

  constructor(
    private http: HttpClient,
    private httpBackend: HttpBackend,
    public loadingService: LoadingService
  ) {
    this.rawHttp = new HttpClient(httpBackend);
  }

  getQuestions(): Observable<string[]> {
    const apiUrl = `${this.baseUrl}/GetQuestionnaireToAsk`;
    this.loadingService.show(); 
    return this.rawHttp.get<string[]>(apiUrl).pipe(
      finalize(() => this.loadingService.hide())
    );
  }

  saveQuestions(payload: SaveQuestionDto[]): Observable<boolean> {
    const apiUrl = `${this.baseUrl}/SaveQuestionnaire`;
    this.loadingService.show();
    return this.rawHttp.post<boolean>(apiUrl, payload).pipe(
      finalize(() => this.loadingService.hide())
    );
  }
}
