import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SaveQuestionDto } from '../dto/SaveQuestionDto';
import { environment } from '../environments/enviroment';

@Injectable({
  providedIn: 'root'
})
export class QuestionService {
  
  private baseUrl  = environment.apiUrlBase + 'api/Questionnaire';

  constructor(private http: HttpClient) {}

  getQuestions(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/GetQuestionnaireToAsk`);
  }

saveQuestions(payload: SaveQuestionDto[]) {
  return this.http.post<boolean>(`${this.baseUrl}/SaveQuestionnaire`, payload);
}



}
