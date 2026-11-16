import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  Vehicle,
  CreateVehicle,
  UpdateVehicle
} from '../models/vehicle';

@Injectable({
  providedIn: 'root'
})
export class VehicleService {

  private http = inject(HttpClient);

  private apiUrl = 'https://localhost:7170/api/Vehicles';

  // Get all vehicles
  getAll(): Observable<Vehicle[]> {
    return this.http.get<Vehicle[]>(this.apiUrl);
  }

  // Get vehicle by ID
  getById(id: number): Observable<Vehicle> {
    return this.http.get<Vehicle>(`${this.apiUrl}/${id}`);
  }

  // Create vehicle
  create(dto: CreateVehicle): Observable<Vehicle> {
    return this.http.post<Vehicle>(this.apiUrl, dto);
  }

  // Update vehicle
  update(id: number, dto: UpdateVehicle): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }

  // Delete vehicle
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
