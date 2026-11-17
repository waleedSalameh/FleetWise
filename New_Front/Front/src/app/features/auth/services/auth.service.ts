import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import {
  Observable,
  catchError,
  finalize,
  firstValueFrom,
  map,
  shareReplay,
  switchMap,
  tap,
  throwError,
} from 'rxjs';

import { API_BASE_URL } from '../../../shared/config/api-config';
import { AuthenticatedUser, LoginRequest, LoginResponse } from '../models/auth.models';
import { PermissionService } from './permission.service';

const AUTH_SESSION_HINT_KEY = 'fleetwise.refresh-session-present';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly permissionService = inject(PermissionService);
  private refreshRequest: Observable<LoginResponse> | null = null;

  readonly accessToken = signal<string | null>(null);
  readonly currentUser = signal<AuthenticatedUser | null>(null);
  readonly isAuthenticated = computed(() => this.accessToken() !== null);

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${API_BASE_URL}/auth/login`, request, { withCredentials: true })
      .pipe(
        switchMap(response => {
          this.setSession(response);
          return this.permissionService.loadCurrentPermissions().pipe(
            map(() => response),
            catchError(error => {
              this.clearSession();
              return throwError(() => error);
            }),
          );
        }),
      );
  }

  refreshSession(): Observable<LoginResponse> {
    if (this.refreshRequest) return this.refreshRequest;

    this.refreshRequest = this.http
      .post<LoginResponse>(`${API_BASE_URL}/auth/refresh`, null, { withCredentials: true })
      .pipe(
        tap(response => this.setSession(response)),
        switchMap(response =>
          this.permissionService.loadCurrentPermissions().pipe(map(() => response)),
        ),
        catchError(error => {
          this.clearSession();
          return throwError(() => error);
        }),
        finalize(() => (this.refreshRequest = null)),
        shareReplay({ bufferSize: 1, refCount: false }),
      );

    return this.refreshRequest;
  }

  async restoreSession(): Promise<void> {
    if (!this.hasRefreshSessionHint()) return;

    try {
      await firstValueFrom(this.refreshSession());
    } catch {
      // A missing or expired refresh cookie means the app should start as a guest.
      this.clearSession();
    }
  }

  logout(): Observable<void> {
    return this.http
      .post<void>(`${API_BASE_URL}/auth/logout`, null, { withCredentials: true })
      .pipe(
        tap(() => this.clearSession()),
        catchError(error => {
          this.clearSession();
          return throwError(() => error);
        }),
      );
  }

  getAccessToken(): string | null {
    return this.accessToken();
  }

  clearSession(): void {
    this.accessToken.set(null);
    this.currentUser.set(null);
    this.permissionService.clear();
    this.removeRefreshSessionHint();
  }

  private setSession(response: LoginResponse): void {
    this.accessToken.set(response.accessToken);
    this.currentUser.set(this.readDisplayIdentity(response.accessToken));
    this.setRefreshSessionHint();
  }

  private readDisplayIdentity(token: string): AuthenticatedUser {
    const emptyIdentity: AuthenticatedUser = { id: null, email: null, roles: [] };

    try {
      const payloadSegment = token.split('.')[1];
      if (!payloadSegment) return emptyIdentity;

      const base64 = payloadSegment.replace(/-/g, '+').replace(/_/g, '/');
      const paddedBase64 = base64.padEnd(Math.ceil(base64.length / 4) * 4, '=');
      const payload: unknown = JSON.parse(atob(paddedBase64));
      if (!payload || typeof payload !== 'object') return emptyIdentity;

      const claims = payload as Record<string, unknown>;
      const id = this.firstStringClaim(claims, [
        'sub',
        'nameid',
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier',
      ]);
      const email = this.firstStringClaim(claims, [
        'email',
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress',
      ]);
      const roleClaim =
        claims['role'] ??
        claims['roles'] ??
        claims['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
      const roles = Array.isArray(roleClaim)
        ? roleClaim.filter((role): role is string => typeof role === 'string')
        : typeof roleClaim === 'string'
          ? [roleClaim]
          : [];

      return { id, email, roles };
    } catch {
      // Token identity is used only to display basic account information in the UI.
      return emptyIdentity;
    }
  }

  private firstStringClaim(claims: Record<string, unknown>, names: string[]): string | null {
    for (const name of names) {
      const value = claims[name];
      if (typeof value === 'string') return value;
    }
    return null;
  }

  private hasRefreshSessionHint(): boolean {
    try {
      return localStorage.getItem(AUTH_SESSION_HINT_KEY) === '1';
    } catch {
      return false;
    }
  }

  private setRefreshSessionHint(): void {
    try {
      localStorage.setItem(AUTH_SESSION_HINT_KEY, '1');
    } catch {
      // The hint contains no credential; an unavailable storage API must not block login.
    }
  }

  private removeRefreshSessionHint(): void {
    try {
      localStorage.removeItem(AUTH_SESSION_HINT_KEY);
    } catch {
      // The in-memory session is still cleared when browser storage is unavailable.
    }
  }
}
