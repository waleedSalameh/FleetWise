import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  CreateSparePartMovement,
  SparePartMovement
} from '../models/spare-part-movement';

@Injectable({
  providedIn: 'root'
})
export class SparePartMovementService {
  private http = inject(HttpClient);

  private apiUrl = 'https://localhost:7170/api/SparePartMovements';

  getAll(): Observable<SparePartMovement[]> {
    return this.http.get<SparePartMovement[]>(this.apiUrl);
  }

  create(dto: CreateSparePartMovement): Observable<SparePartMovement> {
    return this.http.post<SparePartMovement>(this.apiUrl, dto);
  }

  getCurrentStock(sparePartId: number): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/stock/${sparePartId}`);
  }
}
