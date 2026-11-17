import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

import { PermissionService } from '../services/permission.service';

export const permissionGuard: CanActivateFn = route => {
  const router = inject(Router);
  const permissions = inject(PermissionService);
  const required = route.data['permissions'] as string | string[] | undefined;
  const mode = route.data['permissionMatch'] === 'any' ? 'any' : 'all';

  if (!required) return router.createUrlTree(['/forbidden']);

  const requiredPermissions = Array.isArray(required) ? required : [required];
  const allowed = mode === 'any'
    ? permissions.hasAny(requiredPermissions)
    : permissions.hasAll(requiredPermissions);

  return allowed ? true : router.createUrlTree(['/forbidden']);
};
