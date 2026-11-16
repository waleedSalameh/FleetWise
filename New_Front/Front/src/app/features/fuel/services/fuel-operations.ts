import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateFuelMeasurement, CreateFuelTransfer, FuelMeasurement, FuelTransaction, FuelTransfer, MonthlyFuelReport } from '../models/fuel-operations';

const baseUrl = 'https://localhost:7170/api';

@Injectable({ providedIn: 'root' })
export class FuelOperationsService {
  private readonly http = inject(HttpClient);

  getTransfers(): Observable<FuelTransfer[]> { return this.http.get<FuelTransfer[]>(`${baseUrl}/FuelTransfers`); }
  createTransfer(value: CreateFuelTransfer): Observable<FuelTransfer> { return this.http.post<FuelTransfer>(`${baseUrl}/FuelTransfers`, value); }
  reverseTransfer(id: number, notes: string): Observable<FuelTransfer> { return this.http.post<FuelTransfer>(`${baseUrl}/FuelTransfers/${id}/reverse`, JSON.stringify(notes), { headers: { 'Content-Type': 'application/json' } }); }
  deleteTransfer(id: number): Observable<void> { return this.http.delete<void>(`${baseUrl}/FuelTransfers/${id}`); }
  getMeasurements(vehicleId: number): Observable<FuelMeasurement[]> { return this.http.get<FuelMeasurement[]>(`${baseUrl}/vehicles/${vehicleId}/fuel-measurements`); }
  createMeasurement(value: CreateFuelMeasurement): Observable<FuelMeasurement> { return this.http.post<FuelMeasurement>(`${baseUrl}/vehicles/${value.vehicleId}/fuel-measurements`, value); }
  approveMeasurement(vehicleId: number, id: number, approvedBy: string): Observable<FuelMeasurement> { return this.http.post<FuelMeasurement>(`${baseUrl}/vehicles/${vehicleId}/fuel-measurements/${id}/approve`, { approvedBy }); }
  getMonthly(startDate: string, endDate: string, vehicleId?: number): Observable<MonthlyFuelReport[]> {
    let params = new HttpParams().set('startDate', startDate).set('endDate', endDate);
    if (vehicleId) params = params.set('vehicleId', vehicleId);
    return this.http.get<MonthlyFuelReport[]>(`${baseUrl}/FuelReports/monthly`, { params });
  }
  getTransactions(startDate: string, endDate: string, vehicleId?: number): Observable<FuelTransaction[]> {
    let params = new HttpParams().set('startDate', startDate).set('endDate', endDate);
    if (vehicleId) params = params.set('vehicleId', vehicleId);
    return this.http.get<FuelTransaction[]>(`${baseUrl}/FuelReports/transactions`, { params });
  }
}
