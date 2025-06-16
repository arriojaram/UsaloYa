import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class QuestionService {
  
  private readonly apiUrl = 'https://localhost:7290/api/Pregunta';

  constructor(private http: HttpClient) {}

  getPreguntas(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/GetPreguntas`);
  }
}
