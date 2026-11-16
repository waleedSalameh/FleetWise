import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  Driver,
  CreateDriver,
  UpdateDriver
} from '../models/driver';

@Injectable({
  providedIn: 'root'
})
export class DriverService {

  private http = inject(HttpClient);

  private apiUrl = 'https://localhost:7170/api/Drivers';

  getAll(): Observable<Driver[]> {
    return this.http.get<Driver[]>(this.apiUrl);
  }

  getById(id: number): Observable<Driver> {
    return this.http.get<Driver>(`${this.apiUrl}/${id}`);
  }

  create(driver: CreateDriver): Observable<Driver> {
    return this.http.post<Driver>(this.apiUrl, driver);
  }

  update(id: number, driver: UpdateDriver): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, driver);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
