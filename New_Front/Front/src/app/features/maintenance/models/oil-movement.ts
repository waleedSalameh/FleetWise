export { InventoryMovementType } from './spare-part-movement';
import { InventoryMovementType } from './spare-part-movement';

export interface OilMovement {
  oilStockMovementId: number;
  oilId: number;
  oilName: string;
  movementType: string;
  quantity: number;
  unitCost: number;
  totalCost: number;
  date: string;
  maintenanceRecordId: number | null;
  referenceNumber: string | null;
  notes: string | null;
}

export interface CreateOilMovement {
  oilId: number;
  movementType: InventoryMovementType;
  quantity: number;
  unitCost: number;
  date: string;
  maintenanceRecordId?: number | null;
  referenceNumber?: string | null;
  notes?: string | null;
}
