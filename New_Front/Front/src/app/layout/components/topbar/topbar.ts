import { TranslatePipe } from '../../../shared/i18n/translate.pipe';
import { Component, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../../features/auth/services/auth.service';
import { Language, LanguageService } from '../../../shared/i18n/language.service';
import { ResultDialogService } from '../../../shared/services/result-dialog';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [TranslatePipe],
  templateUrl: './topbar.html',
  styleUrl: './topbar.scss',
})
export class Topbar {
  readonly language = inject(LanguageService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly resultDialog = inject(ResultDialogService);

  readonly currentUser = this.auth.currentUser;
  readonly roleLabel = computed(() => this.currentUser()?.roles.join(', ') ?? '');
  readonly isLoggingOut = signal(false);

  change(value: string): void {
    if (value === 'en' || value === 'ar') this.language.use(value as Language);
  }

  logout(): void {
    if (this.isLoggingOut()) return;

    this.isLoggingOut.set(true);
    this.auth
      .logout()
      .pipe(finalize(() => this.isLoggingOut.set(false)))
      .subscribe({
        next: () => void this.router.navigateByUrl('/login'),
        error: () => {
          this.resultDialog.show(
            'layout.components.topbar.logout_error_title',
            'layout.components.topbar.logout_error',
            'error',
          );
          void this.router.navigateByUrl('/login');
        },
      });
  }
}
