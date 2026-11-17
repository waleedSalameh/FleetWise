import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import { API_BASE_URL } from '../../../shared/config/api-config';
import { PermissionService } from './permission.service';

describe('PermissionService', () => {
  let service: PermissionService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
    service = TestBed.inject(PermissionService);
    http = TestBed.inject(HttpTestingController);
  });
  afterEach(() => http.verify());

  it('loads only the effective permission codes returned by the backend', () => {
    let result: ReadonlySet<string> | undefined;
    service.loadCurrentPermissions().subscribe(value => result = value);
    http.expectOne(`${API_BASE_URL}/auth/permissions`).flush({ permissions: ['Vehicles.View'] });
    expect(result?.has('Vehicles.View')).toBe(true);
    expect(result?.has('Vehicles.Delete')).toBe(false);
    expect(service.has('Vehicles.Delete')).toBe(false);
    expect(service.isLoading()).toBe(false);
  });

  it('clears permissions when the session is cleared', () => {
    service.permissions.set(new Set(['Vehicles.View']));
    service.clear();
    expect(service.permissions().size).toBe(0);
  });
});
