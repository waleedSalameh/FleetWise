import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Observable, catchError, finalize, map, tap, throwError } from 'rxjs';

import { API_BASE_URL } from '../../../shared/config/api-config';

interface CurrentUserPermissionsResponse {
  permissions: string[];
}

@Injectable({ providedIn: 'root' })
export class PermissionService {
  private readonly http = inject(HttpClient);

  readonly permissions = signal<ReadonlySet<string>>(new Set<string>());
  readonly isLoading = signal(false);

  loadCurrentPermissions(): Observable<ReadonlySet<string>> {
    this.isLoading.set(true);

    return this.http
      .get<CurrentUserPermissionsResponse>(`${API_BASE_URL}/auth/permissions`)
      .pipe(
        map(response => new Set(response.permissions)),
        tap(permissions => this.permissions.set(permissions)),
        catchError(error => {
          this.clear();
          return throwError(() => error);
        }),
        finalize(() => this.isLoading.set(false)),
      );
  }

  has(permissionCode: string): boolean {
    return this.permissions().has(permissionCode);
  }

  hasAny(permissionCodes: readonly string[]): boolean {
    return permissionCodes.some(permissionCode => this.has(permissionCode));
  }

  hasAll(permissionCodes: readonly string[]): boolean {
    return permissionCodes.every(permissionCode => this.has(permissionCode));
  }

  clear(): void {
    this.permissions.set(new Set<string>());
    this.isLoading.set(false);
  }
}
