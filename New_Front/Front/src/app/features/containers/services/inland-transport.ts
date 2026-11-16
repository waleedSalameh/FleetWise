import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { InlandTransport, InlandTransportSave } from '../models/inland-transport';
@Injectable({ providedIn: 'root' })
export class InlandTransportService {
  private readonly http = inject(HttpClient);
  private readonly url = 'https://localhost:7170/api/InlandTransport';
  getAll() { return this.http.get<InlandTransport[]>(this.url); }
  getById(id: number) { return this.http.get<InlandTransport>(`${this.url}/${id}`); }
  create(dto: InlandTransportSave) { return this.http.post<InlandTransport>(this.url, dto); }
  update(id: number, dto: InlandTransportSave) { return this.http.put<void>(`${this.url}/${id}`, dto); }
  delete(id: number) { return this.http.delete<void>(`${this.url}/${id}`); }
}
