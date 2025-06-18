import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SaveQuestionDto } from '../dto/saveQuestionDto';

@Injectable({
  providedIn: 'root'
})
export class QuestionService {
  
  private readonly apiUrl = 'https://localhost:7290/api/Questionnaire';

  constructor(private http: HttpClient) {}

  getQuestions(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/GetQuestionnaireToAsk`);
  }

saveQuestions(payload: SaveQuestionDto[]) {
  return this.http.post<boolean>(`${this.apiUrl}/SaveQuestionnaire`, payload);
}



}
