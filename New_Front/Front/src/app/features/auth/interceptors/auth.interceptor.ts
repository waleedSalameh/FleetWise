import {
  HttpContextToken,
  HttpErrorResponse,
  HttpInterceptorFn,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';

import { API_BASE_URL } from '../../../shared/config/api-config';
import { AuthService } from '../services/auth.service';

const SKIP_AUTH_REFRESH = new HttpContextToken<boolean>(() => false);

const apiUrl = new URL(API_BASE_URL);
const apiPath = apiUrl.pathname.replace(/\/$/, '');
const authPath = `${apiPath}/auth`;

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const auth = inject(AuthService);
  const requestUrl = resolveRequestUrl(request.url);

  if (!requestUrl || !isFleetWiseApiRequest(requestUrl) || isPublicAuthRequest(requestUrl)) {
    return next(request);
  }

  const accessToken = auth.getAccessToken();
  const authorizedRequest = accessToken
    ? request.clone({ setHeaders: { Authorization: `Bearer ${accessToken}` } })
    : request;

  return next(authorizedRequest).pipe(
    catchError(error => {
      if (
        error instanceof HttpErrorResponse &&
        error.status === 401 &&
        isCurrentPermissionsRequest(requestUrl)
      ) {
        auth.clearSession();
        return throwError(() => error);
      }

      if (
        !(error instanceof HttpErrorResponse) ||
        error.status !== 401 ||
        request.context.get(SKIP_AUTH_REFRESH) ||
        auth.getAccessToken() === null
      ) {
        // A 403 means the user is authenticated but lacks permission; never refresh for it.
        return throwError(() => error);
      }

      return auth.refreshSession().pipe(
        switchMap(response => {
          const retryRequest = request.clone({
            setHeaders: { Authorization: `Bearer ${response.accessToken}` },
            context: request.context.set(SKIP_AUTH_REFRESH, true),
          });

          return next(retryRequest).pipe(
            catchError(retryError => {
              if (retryError instanceof HttpErrorResponse && retryError.status === 401) {
                auth.clearSession();
              }
              return throwError(() => retryError);
            }),
          );
        }),
        catchError(refreshError => {
          auth.clearSession();
          return throwError(() => refreshError);
        }),
      );
    }),
  );
};

function resolveRequestUrl(url: string): URL | null {
  try {
    return new URL(url, apiUrl.origin);
  } catch {
    return null;
  }
}

function isFleetWiseApiRequest(url: URL): boolean {
  return url.origin === apiUrl.origin && url.pathname.startsWith(`${apiPath}/`);
}

function isPublicAuthRequest(url: URL): boolean {
  return (
    url.pathname === `${authPath}/login` ||
    url.pathname === `${authPath}/refresh` ||
    url.pathname === `${authPath}/logout`
  );
}

function isCurrentPermissionsRequest(url: URL): boolean {
  return url.pathname === `${authPath}/permissions`;
}
