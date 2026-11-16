export enum FuelPurchaseStatus {
  InUse = 0,
  Completed = 1
}

export enum FuelInventoryBatchOrigin { Purchase = 1, VehicleReturn = 2 }

export interface FuelInventorySummary { totalQuantity: number; totalValueJod: number; purchasedQuantity: number; returnedQuantity: number; availableBatchCount: number; }

export interface FuelPurchase {
  fuelPurchaseId: number;
  purchaseDate: string;
  quantity: number;
  remainingQuantity: number;
  pricePerLiter: number;
  supplierName: string | null;
  invoiceNumber: string | null;
  status: FuelPurchaseStatus;
  batchOrigin: FuelInventoryBatchOrigin;
  sourceVehicleId: number | null;
}

export interface CreateFuelPurchase {
  purchaseDate: string;
  quantity: number;
  pricePerLiter: number;
  supplierName: string | null;
  invoiceNumber: string | null;
}
