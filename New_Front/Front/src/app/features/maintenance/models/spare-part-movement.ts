export enum InventoryMovementType {
  Receipt = 0,
  Issue = 1,
  Return = 2,
  Adjustment = 3
}

export interface SparePartMovement {
  sparePartMovementId: number;
  sparePartId: number;
  partNumber: string;
  partName: string;
  movementType: string;
  quantity: number;
  unitCost: number;
  totalCost: number;
  date: string;
  maintenanceRecordId: number | null;
  referenceNumber: string | null;
  notes: string | null;
}

export interface CreateSparePartMovement {
  sparePartId: number;
  movementType: InventoryMovementType;
  quantity: number;
  unitCost: number;
  date: string;
  maintenanceRecordId?: number | null;
  referenceNumber?: string | null;
  notes?: string | null;
}
