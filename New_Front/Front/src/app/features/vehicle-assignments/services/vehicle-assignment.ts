import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  VehicleAssignment,
  CreateVehicleAssignment,
  UpdateVehicleAssignment
} from '../models/vehicle-assignment';

@Injectable({
  providedIn: 'root'
})
export class VehicleAssignmentService {

  private http = inject(HttpClient);

  private apiUrl = 'https://localhost:7170/api/VehicleAssignments';

  getAll(): Observable<VehicleAssignment[]> {
    return this.http.get<VehicleAssignment[]>(this.apiUrl);
  }

  getById(id: number): Observable<VehicleAssignment> {
    return this.http.get<VehicleAssignment>(`${this.apiUrl}/${id}`);
  }

  create(assignment: CreateVehicleAssignment): Observable<VehicleAssignment> {
    return this.http.post<VehicleAssignment>(this.apiUrl, assignment);
  }

  update(id: number, assignment: UpdateVehicleAssignment): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, assignment);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
