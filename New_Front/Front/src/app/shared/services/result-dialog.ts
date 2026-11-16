import { Injectable, inject } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ResultDialogService {

  private resultSubject = new Subject<{
    title: string;
    message: string;
    type: 'success' | 'error';
  }>();

  result$ = this.resultSubject.asObservable();

  show(
    title: string,
    message: string,
    type: 'success' | 'error'
  ): void {
    this.resultSubject.next({
      title,
      message,
      type
    });
  }
}
