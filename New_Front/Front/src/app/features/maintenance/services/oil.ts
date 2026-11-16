import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Oil, CreateOil, UpdateOil } from '../models/oil';

@Injectable({
  providedIn: 'root'
})
export class OilService {
  private http = inject(HttpClient);

  private apiUrl = 'https://localhost:7170/api/Oils';

  getAll(): Observable<Oil[]> {
    return this.http.get<Oil[]>(this.apiUrl);
  }

  getById(id: number): Observable<Oil> {
    return this.http.get<Oil>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateOil): Observable<Oil> {
    return this.http.post<Oil>(this.apiUrl, dto);
  }

  update(id: number, dto: UpdateOil): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
