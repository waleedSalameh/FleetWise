
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  MaintenanceRecord,
  CreateMaintenanceRecord,
  UpdateMaintenanceRecord
} from '../models/maintenance-record';

@Injectable({
  providedIn: 'root'
})
export class MaintenanceRecordService {

  private http = inject(HttpClient);

  private apiUrl = 'https://localhost:7170/api/MaintenanceRecords';

  getAll(): Observable<MaintenanceRecord[]> {
    return this.http.get<MaintenanceRecord[]>(this.apiUrl);
  }

  getById(id: number): Observable<MaintenanceRecord> {
    return this.http.get<MaintenanceRecord>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateMaintenanceRecord): Observable<MaintenanceRecord> {
    return this.http.post<MaintenanceRecord>(this.apiUrl, dto);
  }

  update(id: number, dto: UpdateMaintenanceRecord): Observable<MaintenanceRecord> {
    return this.http.put<MaintenanceRecord>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
