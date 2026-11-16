import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { DeliveryLocation, SaveDeliveryLocation } from '../models/delivery-location';

@Injectable({ providedIn: 'root' })
export class DeliveryLocationService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'https://localhost:7170/api/DeliveryLocation';

  getAll(): Observable<DeliveryLocation[]> { return this.http.get<DeliveryLocation[]>(this.apiUrl); }
  create(dto: SaveDeliveryLocation): Observable<DeliveryLocation> { return this.http.post<DeliveryLocation>(this.apiUrl, dto); }
  update(id: number, dto: SaveDeliveryLocation): Observable<void> { return this.http.put<void>(`${this.apiUrl}/${id}`, dto); }
  delete(id: number): Observable<void> { return this.http.delete<void>(`${this.apiUrl}/${id}`); }
}
