import { Injectable } from '@angular/core';
import { HttpBackend, HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

/**
 * RawHttpService
 * Servicio para realizar peticiones HTTP que evitan los interceptores.
 * Útil para llamadas que no requieren autenticación o encabezados especiales,
 * como validaciones durante el registro (por ejemplo, verificar si un correo ya existe).
 */
@Injectable({ providedIn: 'root' })
export class RawHttpService {
  private http: HttpClient;

  /**
   * httpBackend es el manejador de bajo nivel de Angular
   * que se utiliza para evitar los interceptores.
   */
  constructor(private httpBackend: HttpBackend) {
    this.http = new HttpClient(httpBackend);
  }

  /**
   * Realiza una petición GET sin pasar por los interceptores.
   * Retorna un Observable del tipo T.
   */
  get<T>(url: string): Observable<T> {
    return this.http.get<T>(url);
  }

  /**
   * Realiza una petición POST sin pasar por los interceptores.
   */
  post<T>(url: string, body: any): Observable<T> {
    return this.http.post<T>(url, body);
  }
}
