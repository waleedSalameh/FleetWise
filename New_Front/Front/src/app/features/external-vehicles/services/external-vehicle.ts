import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  ExternalVehicle,
  CreateExternalVehicle
} from '../models/external-vehicle';

@Injectable({
  providedIn: 'root'
})
export class ExternalVehicleService {

  private http = inject(HttpClient);

  private apiUrl = 'https://localhost:7170/api/ExternalVehicles';

  getAll(): Observable<ExternalVehicle[]> {
    return this.http.get<ExternalVehicle[]>(this.apiUrl);
  }

  getById(id: number): Observable<ExternalVehicle> {
    return this.http.get<ExternalVehicle>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateExternalVehicle): Observable<ExternalVehicle> {
    return this.http.post<ExternalVehicle>(this.apiUrl, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
