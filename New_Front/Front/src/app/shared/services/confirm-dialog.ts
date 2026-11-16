import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ConfirmDialogService {

  private confirmSubject = new Subject<{
    title: string;
    message: string;
    onConfirm: () => void;
    actionLabel?: string;
    variant?: 'danger' | 'primary';
  }>();

  confirm$ = this.confirmSubject.asObservable();

  show(
    onConfirm: () => void,
    title: string = 'Confirm Delete',
    message: string = 'Are you sure you want to delete this item?',
    options?: { actionLabel?: string; variant?: 'danger' | 'primary' }
  ): void {
    this.confirmSubject.next({
      title,
      message,
      onConfirm,
      ...options
    });
  }
}
