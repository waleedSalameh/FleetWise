import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  FuelPurchase,
  CreateFuelPurchase,
  FuelInventorySummary
} from '../models/fuel-purchase';

@Injectable({
  providedIn: 'root'
})
export class FuelPurchaseService {

  private http = inject(HttpClient);

  private apiUrl = 'https://localhost:7170/api/FuelPurchases';

  getAll(): Observable<FuelPurchase[]> {
    return this.http.get<FuelPurchase[]>(this.apiUrl);
  }

  getSummary(): Observable<FuelInventorySummary> {
    return this.http.get<FuelInventorySummary>(`${this.apiUrl}/summary`);
  }

  create(dto: CreateFuelPurchase): Observable<{ message: string; id: number }> {
    return this.http.post<{ message: string; id: number }>(this.apiUrl, dto);
  }
}
