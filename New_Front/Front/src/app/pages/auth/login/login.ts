import { HttpErrorResponse } from '@angular/common/http';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs';

import { AuthService } from '../../../features/auth/services/auth.service';
import { TranslatePipe } from '../../../shared/i18n/translate.pipe';
import { ResultDialogService } from '../../../shared/services/result-dialog';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, TranslatePipe],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class LoginPage {
  private readonly auth = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly resultDialog = inject(ResultDialogService);
  private readonly destroyRef = inject(DestroyRef);

  readonly isSubmitting = signal(false);
  readonly form = new FormGroup({
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    password: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
  });

  submit(): void {
    if (this.isSubmitting()) return;

    this.form.markAllAsTouched();
    if (this.form.invalid) return;

    this.isSubmitting.set(true);
    this.auth
      .login(this.form.getRawValue())
      .pipe(
        finalize(() => this.isSubmitting.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          void this.router.navigateByUrl(this.getSafeReturnUrl());
        },
        error: (error: unknown) => {
          this.resultDialog.show(
            'auth.login.error_title',
            this.getErrorMessageKey(error),
            'error',
          );
        },
      });
  }

  private getSafeReturnUrl(): string {
    const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
    if (!returnUrl || !returnUrl.startsWith('/') || returnUrl.startsWith('//')) {
      return '/dashboard';
    }

    return returnUrl;
  }

  private getErrorMessageKey(error: unknown): string {
    if (!(error instanceof HttpErrorResponse)) return 'auth.login.request_failed';

    if (error.status === 401) return 'auth.login.invalid_credentials';
    if (error.status === 429) return 'auth.login.too_many_attempts';
    if (error.status === 0) return 'auth.login.server_unavailable';
    return 'auth.login.request_failed';
  }
}
