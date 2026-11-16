export interface SparePart {
  sparePartId: number;
  partNumber: string;
  partName: string;
  brand: string | null;
  description: string | null;
  unit: string;
  purchasePrice: number;
  minimumQuantity: number;
}

export interface CreateSparePart {
  partNumber: string;
  partName: string;
  brand?: string | null;
  description?: string | null;
  unit: string;
  purchasePrice: number;
  minimumQuantity: number;
}

export type UpdateSparePart = CreateSparePart;
