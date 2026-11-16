import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Customer, SaveCustomer } from '../models/customer';

@Injectable({ providedIn: 'root' })
export class CustomerService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'https://localhost:7170/api/Customers';

  getAll(): Observable<Customer[]> { return this.http.get<Customer[]>(this.apiUrl); }
  create(dto: SaveCustomer): Observable<Customer> { return this.http.post<Customer>(this.apiUrl, dto); }
  update(id: number, dto: SaveCustomer): Observable<void> { return this.http.put<void>(`${this.apiUrl}/${id}`, dto); }
  delete(id: number): Observable<void> { return this.http.delete<void>(`${this.apiUrl}/${id}`); }
}
