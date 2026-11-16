import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { SaveShippingLine, ShippingLine } from '../models/shipping-line';

@Injectable({ providedIn: 'root' })
export class ShippingLineService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'https://localhost:7170/api/ShippingLine';

  getAll(): Observable<ShippingLine[]> { return this.http.get<ShippingLine[]>(this.apiUrl); }
  create(dto: SaveShippingLine): Observable<ShippingLine> { return this.http.post<ShippingLine>(this.apiUrl, dto); }
  update(id: number, dto: SaveShippingLine): Observable<void> { return this.http.put<void>(`${this.apiUrl}/${id}`, dto); }
  delete(id: number): Observable<void> { return this.http.delete<void>(`${this.apiUrl}/${id}`); }
}
