import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateOilMovement, OilMovement } from '../models/oil-movement';

@Injectable({
  providedIn: 'root'
})
export class OilMovementService {
  private http = inject(HttpClient);

  private apiUrl = 'https://localhost:7170/api/OilMovements';

  getAll(): Observable<OilMovement[]> {
    return this.http.get<OilMovement[]>(this.apiUrl);
  }

  create(dto: CreateOilMovement): Observable<OilMovement> {
    return this.http.post<OilMovement>(this.apiUrl, dto);
  }

  getCurrentStock(oilId: number): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/stock/${oilId}`);
  }
}
