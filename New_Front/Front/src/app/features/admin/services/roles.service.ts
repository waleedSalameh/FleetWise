import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../../shared/config/api-config';
import {
  CreateRoleRequest,
  ReplaceRolePermissionsRequest,
  RolePermissionStatus,
  RoleSummary,
  UpdateRoleRequest,
} from '../models/admin.models';

@Injectable({ providedIn: 'root' })
export class RolesService {
  private readonly http = inject(HttpClient);
  private readonly url = `${API_BASE_URL}/admin/roles`;

  getAll(): Observable<RoleSummary[]> {
    return this.http.get<RoleSummary[]>(this.url);
  }

  create(request: CreateRoleRequest): Observable<RoleSummary> {
    return this.http.post<RoleSummary>(this.url, request);
  }

  update(roleId: string, request: UpdateRoleRequest): Observable<RoleSummary> {
    return this.http.put<RoleSummary>(`${this.url}/${encodeURIComponent(roleId)}`, request);
  }

  delete(roleId: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${encodeURIComponent(roleId)}`);
  }

  getPermissions(roleId: string): Observable<RolePermissionStatus[]> {
    return this.http.get<RolePermissionStatus[]>(
      `${this.url}/${encodeURIComponent(roleId)}/permissions`,
    );
  }

  replacePermissions(
    roleId: string,
    request: ReplaceRolePermissionsRequest,
  ): Observable<void> {
    return this.http.put<void>(
      `${this.url}/${encodeURIComponent(roleId)}/permissions`,
      request,
    );
  }
}
