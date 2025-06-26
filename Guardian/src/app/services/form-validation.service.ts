
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class FormValidationService {
  private formStates = new Map<string, BehaviorSubject<boolean>>();

  setFormValid(key: string, valid: boolean): void {
    if (!this.formStates.has(key)) {
      // Si no existe, crea un nuevo BehaviorSubject con el valor inicial
      this.formStates.set(key, new BehaviorSubject(valid));
    } else {
      // Si ya existe, emite el nuevo valor al observable
      this.formStates.get(key)!.next(valid);
    }
  }

  isFormValidNow(key: string): boolean {
    return this.formStates.get(key)?.getValue() ?? false;
  }
}
