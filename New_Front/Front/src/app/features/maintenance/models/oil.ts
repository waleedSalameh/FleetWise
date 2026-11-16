export interface Oil {
  oilId: number;
  oilName: string;
  brand: string | null;
  viscosity: string | null;
  unit: string;
  purchasePrice: number;
  minimumQuantity: number;
}

export interface CreateOil {
  oilName: string;
  brand?: string | null;
  viscosity?: string | null;
  unit: string;
  purchasePrice: number;
  minimumQuantity: number;
}

export type UpdateOil = CreateOil;
