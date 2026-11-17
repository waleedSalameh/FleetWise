import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../../shared/config/api-config';
import {
  CreateUserRequest,
  ReplaceUserRolesRequest,
  ResetUserPasswordRequest,
  RoleSummary,
  UserSummary,
} from '../models/admin.models';

@Injectable({ providedIn: 'root' })
export class UsersService {
  private readonly http = inject(HttpClient);
  private readonly url = `${API_BASE_URL}/admin/users`;

  getAll(pageNumber = 1, pageSize = 100): Observable<UserSummary[]> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);
    return this.http.get<UserSummary[]>(this.url, { params });
  }

  create(request: CreateUserRequest): Observable<UserSummary> {
    return this.http.post<UserSummary>(this.url, request);
  }

  activate(userId: string): Observable<void> {
    return this.http.post<void>(`${this.url}/${encodeURIComponent(userId)}/activate`, {});
  }

  disable(userId: string): Observable<void> {
    return this.http.post<void>(`${this.url}/${encodeURIComponent(userId)}/disable`, {});
  }

  resetPassword(userId: string, request: ResetUserPasswordRequest): Observable<void> {
    return this.http.put<void>(`${this.url}/${encodeURIComponent(userId)}/password`, request);
  }

  getRoles(userId: string): Observable<RoleSummary[]> {
    return this.http.get<RoleSummary[]>(`${this.url}/${encodeURIComponent(userId)}/roles`);
  }

  replaceRoles(userId: string, request: ReplaceUserRolesRequest): Observable<void> {
    return this.http.put<void>(`${this.url}/${encodeURIComponent(userId)}/roles`, request);
  }
}
