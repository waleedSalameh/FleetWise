import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { IncomingAqabaBill, IncomingAqabaBillSave, IncomingAqabaContainer, IncomingAqabaContainerSave } from '../models/incoming-aqaba';
@Injectable({ providedIn: 'root' })
export class IncomingAqabaService {
  private readonly http = inject(HttpClient);
  private readonly bills = 'https://localhost:7170/api/IncomingAqabaBill';
  private readonly containers = 'https://localhost:7170/api/IncomingAqabaContainer';
  getAll() { return this.http.get<IncomingAqabaBill[]>(this.bills); }
  getById(id: number) { return this.http.get<IncomingAqabaBill>(`${this.bills}/${id}`); }
  createBill(dto: IncomingAqabaBillSave) { return this.http.post<IncomingAqabaBill>(this.bills, dto); }
  updateBill(id: number, dto: Omit<IncomingAqabaBillSave, 'containers'>) { return this.http.put<void>(`${this.bills}/${id}`, dto); }
  deleteBill(id: number) { return this.http.delete<void>(`${this.bills}/${id}`); }
  createContainer(billId: number, dto: IncomingAqabaContainerSave) { return this.http.post<IncomingAqabaContainer>(`${this.containers}/${billId}`, dto); }
  updateContainer(id: number, dto: IncomingAqabaContainerSave) { return this.http.put<void>(`${this.containers}/${id}`, dto); }
  deleteContainer(id: number) { return this.http.delete<void>(`${this.containers}/${id}`); }
}
