import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  Trailer,
  CreateTrailer,
  UpdateTrailer
} from '../models/trailer';

@Injectable({
  providedIn: 'root'
})
export class TrailerService {

  private http = inject(HttpClient);

  private apiUrl = 'https://localhost:7170/api/Trailer';

  // Get All Trailers
  getAll(): Observable<Trailer[]> {
    return this.http.get<Trailer[]>(this.apiUrl);
  }

  // Get Trailer By Id
  getById(id: number): Observable<Trailer> {
    return this.http.get<Trailer>(`${this.apiUrl}/${id}`);
  }

  // Create Trailer
  create(trailer: CreateTrailer): Observable<Trailer> {
    return this.http.post<Trailer>(this.apiUrl, trailer);
  }

  // Update Trailer
  update(id: number, trailer: UpdateTrailer): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, trailer);
  }

  // Delete Trailer
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
