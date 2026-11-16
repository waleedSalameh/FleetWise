import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { ExportAqabaBooking, ExportAqabaBookingSave, ExportAqabaContainer, ExportAqabaContainerSave } from '../models/export-aqaba';
@Injectable({ providedIn: 'root' })
export class ExportAqabaService {
  private readonly http = inject(HttpClient);
  private readonly bookings = 'https://localhost:7170/api/ExportAqabaBooking';
  private readonly containers = 'https://localhost:7170/api/ExportAqabaContainer';
  getAll() { return this.http.get<ExportAqabaBooking[]>(this.bookings); }
  getById(id: number) { return this.http.get<ExportAqabaBooking>(`${this.bookings}/${id}`); }
  createBooking(dto: ExportAqabaBookingSave) { return this.http.post<ExportAqabaBooking>(this.bookings, dto); }
  updateBooking(id: number, dto: Omit<ExportAqabaBookingSave, 'containers'>) { return this.http.put<void>(`${this.bookings}/${id}`, dto); }
  deleteBooking(id: number) { return this.http.delete<void>(`${this.bookings}/${id}`); }
  createContainer(bookingId: number, dto: ExportAqabaContainerSave) { return this.http.post<ExportAqabaContainer>(`${this.containers}/${bookingId}`, dto); }
  updateContainer(id: number, dto: ExportAqabaContainerSave) { return this.http.put<void>(`${this.containers}/${id}`, dto); }
  deleteContainer(id: number) { return this.http.delete<void>(`${this.containers}/${id}`); }
}
