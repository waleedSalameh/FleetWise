import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  FuelRecord,
  CreateExternalFuelRecord,
  CreateInternalFuelRecord,
  UpdateFuelRecord
} from '../models/fuel-record';

@Injectable({
  providedIn: 'root'
})
export class FuelRecordService {

  private http = inject(HttpClient);

  private apiUrl = 'https://localhost:7170/api/FuelRecords';

  getAll(): Observable<FuelRecord[]> {
    return this.http.get<FuelRecord[]>(this.apiUrl);
  }

  getById(id: number): Observable<FuelRecord> {
    return this.http.get<FuelRecord>(`${this.apiUrl}/${id}`);
  }

  createExternal(dto: CreateExternalFuelRecord): Observable<FuelRecord> {
    return this.http.post<FuelRecord>(`${this.apiUrl}/external`, dto);
  }

  createInternal(dto: CreateInternalFuelRecord): Observable<FuelRecord> {
    return this.http.post<FuelRecord>(`${this.apiUrl}/internal`, dto);
  }

  update(id: number, dto: UpdateFuelRecord): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
