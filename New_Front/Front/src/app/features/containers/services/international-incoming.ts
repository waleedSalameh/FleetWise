import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { InternationalIncoming, InternationalIncomingSave } from '../models/international-incoming';
@Injectable({ providedIn: 'root' })
export class InternationalIncomingService {
  private readonly http = inject(HttpClient);
  private readonly url = 'https://localhost:7170/api/InternationalIncoming';
  getAll() { return this.http.get<InternationalIncoming[]>(this.url); }
  getById(id: number) { return this.http.get<InternationalIncoming>(`${this.url}/${id}`); }
  create(dto: InternationalIncomingSave) { return this.http.post<InternationalIncoming>(this.url, dto); }
  update(id: number, dto: InternationalIncomingSave) { return this.http.put<void>(`${this.url}/${id}`, dto); }
  delete(id: number) { return this.http.delete<void>(`${this.url}/${id}`); }
}
