import { TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { describe, expect, it } from 'vitest';
import { PermissionService } from '../services/permission.service';
import { permissionGuard } from './permission.guard';

describe('permissionGuard', () => {
  beforeEach(() => TestBed.configureTestingModule({ providers: [provideRouter([]), provideHttpClient()] }));

  it('allows navigation when all required permissions are present', () => {
    const permissions = TestBed.inject(PermissionService);
    permissions.permissions.set(new Set(['Vehicles.View', 'Vehicles.Update']));
    const result = TestBed.runInInjectionContext(() => permissionGuard(
      { data: { permissions: ['Vehicles.View', 'Vehicles.Update'], permissionMatch: 'all' } } as any,
      {} as any,
    ));
    expect(result).toBe(true);
  });

  it('redirects a user missing a required permission to the forbidden page', () => {
    const permissions = TestBed.inject(PermissionService);
    permissions.permissions.set(new Set(['Vehicles.View']));
    const result = TestBed.runInInjectionContext(() => permissionGuard(
      { data: { permissions: 'Vehicles.Delete' } } as any,
      {} as any,
    ));
    expect(result).toEqual(TestBed.inject(Router).parseUrl('/forbidden'));
  });
});
