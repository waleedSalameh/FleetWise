import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { CreateSparePart, SparePart, UpdateSparePart } from '../models/spare-part';

@Injectable({
  providedIn: 'root'
})
export class SparePartService {
  private http = inject(HttpClient);

  private apiUrl = 'https://localhost:7170/api/SpareParts';

  getAll(): Observable<SparePart[]> {
    return this.http.get<SparePart[]>(this.apiUrl);
  }

  create(dto: CreateSparePart): Observable<SparePart> {
    return this.http.post<SparePart>(this.apiUrl, dto);
  }

  update(id: number, dto: UpdateSparePart): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
