import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { InternationalExport, InternationalExportSave } from '../models/international-export';
@Injectable({ providedIn: 'root' })
export class InternationalExportService {
  private readonly http = inject(HttpClient);
  private readonly url = 'https://localhost:7170/api/InternationalExport';
  getAll() { return this.http.get<InternationalExport[]>(this.url); }
  getById(id: number) { return this.http.get<InternationalExport>(`${this.url}/${id}`); }
  create(dto: InternationalExportSave) { return this.http.post<InternationalExport>(this.url, dto); }
  update(id: number, dto: InternationalExportSave) { return this.http.put<void>(`${this.url}/${id}`, dto); }
  delete(id: number) { return this.http.delete<void>(`${this.url}/${id}`); }
}
