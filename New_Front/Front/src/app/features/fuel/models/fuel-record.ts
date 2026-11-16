export enum FuelSourceType {
  Internal = 0,
  External = 1
}

export interface FuelBatchConsumption {
  fuelPurchaseId: number;
  quantity: number;
}

export interface FuelBatchConsumptionResult {
  fuelPurchaseId: number;
  quantity: number;
  pricePerLiter: number;
  totalCost: number;
}

export interface FuelRecord {
  fuelRecordId: number;
  vehicleId: number;
  date: string;
  quantity: number;
  pricePerLiter: number;
  totalCost: number;
  odometer: number;
  plateNumber: number;
  fuelSourceType: FuelSourceType;
  stationName: string | null;
  isDeleted: boolean;
  batches: FuelBatchConsumptionResult[];
}

export interface CreateExternalFuelRecord {
  vehicleId: number;
  date: string;
  quantity: number;
  pricePerLiter: number;
  odometer: number;
  stationName: string;
}

export interface CreateInternalFuelRecord {
  vehicleId: number;
  date: string;
  quantity: number;
  odometer: number;
  batches: FuelBatchConsumption[];
}

export interface UpdateFuelRecord {
  vehicleId: number;
  date: string;
  quantity: number;
  pricePerLiter: number;
  odometer: number;
  stationName: string;
}
