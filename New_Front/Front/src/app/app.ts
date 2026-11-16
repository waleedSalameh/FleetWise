import { TranslatePipe } from './shared/i18n/translate.pipe';
import { Component, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { DialogModule } from 'primeng/dialog';

import { ConfirmDialogService } from './shared/services/confirm-dialog';
import { ResultDialogService } from './shared/services/result-dialog';
import { LanguageService } from './shared/i18n/language.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [TranslatePipe, 
    RouterOutlet,
    DialogModule
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  readonly language = inject(LanguageService);

  private confirmService = inject(ConfirmDialogService);
  private resultService = inject(ResultDialogService);

  // Delete Dialog
  displayConfirmDialog = false;
  confirmTitle = '';
  confirmMessage = '';
  confirmActionLabel = 'app.yes_delete';
  confirmVariant: 'danger' | 'primary' = 'danger';
  private confirmAction: (() => void) | null = null;

  // Result Dialog
  displayResultDialog = false;
  resultTitle = '';
  resultMessage = '';
  resultType: 'success' | 'error' = 'success';

  ngOnInit(): void {

    // Listen to Delete Confirmation
    this.confirmService.confirm$.subscribe(data => {
      this.confirmTitle = data.title;
      this.confirmMessage = data.message;
      this.confirmActionLabel = data.actionLabel ?? 'app.yes_delete';
      this.confirmVariant = data.variant ?? 'danger';
      this.confirmAction = data.onConfirm;

      this.displayConfirmDialog = true;
    });

    // Listen to Success / Error Messages
    this.resultService.result$.subscribe(data => {
      this.resultTitle = data.title;
      this.resultMessage = data.message;
      this.resultType = data.type;

      this.displayResultDialog = true;
    });
  }

  // Confirm Delete
  confirm(): void {
    this.displayConfirmDialog = false;

    this.confirmAction?.();
    this.confirmAction = null;
  }

  // Cancel Delete
  cancel(): void {
    this.displayConfirmDialog = false;
    this.confirmAction = null;
  }

  // Close Result Dialog
  closeResultDialog(): void {
    this.displayResultDialog = false;
  }
}
