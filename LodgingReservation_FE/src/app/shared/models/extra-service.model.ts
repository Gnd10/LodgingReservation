export type ExtraUnitType = 'NIGHT' | 'PERSON' | 'TRIP' | 'ITEM' | string | number;

export interface ExtraService {
  id: number;
  name: string;
  price: number;
  unitType?: ExtraUnitType;
  type?: ExtraUnitType;
}

export interface AddOnSelection {
  extraServiceId: number;
  quantity: number;
}
