import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../../shared/config/api-config';
import {
  SetUserPermissionOverrideRequest,
  UserPermissionStatus,
} from '../models/admin.models';

@Injectable({ providedIn: 'root' })
export class AdminPermissionsService {
  private readonly http = inject(HttpClient);
  private readonly url = `${API_BASE_URL}/admin/users`;

  getUserPermissions(userId: string): Observable<UserPermissionStatus[]> {
    return this.http.get<UserPermissionStatus[]>(
      `${this.url}/${encodeURIComponent(userId)}/permissions`,
    );
  }

  setOverride(userId: string, request: SetUserPermissionOverrideRequest): Observable<void> {
    return this.http.put<void>(
      `${this.url}/${encodeURIComponent(userId)}/permissions`,
      request,
    );
  }

  removeOverride(userId: string, permissionCode: string): Observable<void> {
    return this.http.delete<void>(
      `${this.url}/${encodeURIComponent(userId)}/permissions/${encodeURIComponent(permissionCode)}`,
    );
  }
}
