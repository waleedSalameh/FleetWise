import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Trip, TripSave, UpdateTrip } from '../models/trip';

@Injectable({ providedIn: 'root' })
export class TripService {
  private readonly http = inject(HttpClient);
  private readonly url = 'https://localhost:7170/api/Trips';
  getAll() {
    return this.http.get<Trip[]>(this.url);
  }
  getById(id: number) {
    return this.http.get<Trip>(`${this.url}/${id}`);
  }
  create(dto: TripSave) {
    return this.http.post<{ tripId: number; message: string }>(this.url, dto);
  }
  update(id: number, dto: UpdateTrip) {
    return this.http.put<{ message: string }>(`${this.url}/${id}`, dto);
  }
  delete(id: number) {
    return this.http.delete<{ message: string }>(`${this.url}/${id}`);
  }
}
