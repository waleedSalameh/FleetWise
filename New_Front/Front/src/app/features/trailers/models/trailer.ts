export interface Trailer {
  id: number;
  plateNumber: number;
  axleCount: number;
  maxLoad: number;
  trailerType: number;
  licenseExpiryDate: string;
}

export interface CreateTrailer {
  plateNumber: number;
  axleCount: number;
  maxLoad: number;
  trailerType: number;
  licenseExpiryDate: string;
}

export interface UpdateTrailer {
  plateNumber: number;
  axleCount: number;
  maxLoad: number;
  trailerType: number;
  licenseExpiryDate: string;
}
export enum TrailerType {
  ContainerTransport = 1,
  CrudeOil = 2,
  Refrigerated = 3,
  Sides = 4
}
