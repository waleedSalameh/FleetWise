
export enum MaintenanceLocation {
  Internal = 0,
  External = 1
}

export enum MaintenanceStatus {
  Scheduled = 0,
  InProgress = 1,
  Completed = 2,
  Cancelled = 3
}

export interface MaintenanceRecord {
  maintenanceRecordId: number;
  vehicleId: number;
  plateNumber: string;
  maintenanceType: string;
  maintenanceLocation: MaintenanceLocation;
  maintenanceDate: string;
  odometer: number;
  description?: string;
  workshopName?: string;
  technicianName?: string;
  partsCost: number;
  laborCost: number;
  totalCost: number;
  status: MaintenanceStatus;
  oils: OilConsumption[];
  spareParts: SparePartConsumption[];
}

export interface OilConsumption {
  oilId: number;
  quantity: number;
}

export interface SparePartConsumption {
  sparePartId: number;
  quantity: number;
}

export interface CreateMaintenanceRecord {
  vehicleId: number;
  maintenanceType: string;
  maintenanceLocation: MaintenanceLocation;
  maintenanceDate: string;
  odometer: number;
  description?: string;
  workshopName?: string;
  technicianName?: string;
  laborCost: number;
  oils: OilConsumption[];
  spareParts: SparePartConsumption[];
}

export interface UpdateMaintenanceRecord extends CreateMaintenanceRecord {
  status: MaintenanceStatus;
}
