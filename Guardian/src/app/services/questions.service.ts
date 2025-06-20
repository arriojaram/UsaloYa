import { Injectable } from '@angular/core';
import { HttpClient, HttpBackend } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SaveQuestionDto } from '../dto/SaveQuestionDto';
import { environment } from '../environments/enviroment';

@Injectable({
  providedIn: 'root'
})
export class QuestionService {

  private rawHttp: HttpClient;
  
  private baseUrl = environment.apiUrlBase + '/api/Questionnaire';

  constructor(
    private http: HttpClient,
    private httpBackend: HttpBackend
  ) {
    this.rawHttp = new HttpClient(httpBackend);
  }

  getQuestions(): Observable<string[]> {
    const apiUrl = `${this.baseUrl}/GetQuestionnaireToAsk`;
    return this.rawHttp.get<string[]>(apiUrl);
  }

  saveQuestions(payload: SaveQuestionDto[]): Observable<boolean> {
    const apiUrl = `${this.baseUrl}/SaveQuestionnaire`;
    return this.rawHttp.post<boolean>(apiUrl, payload);
  }
}
