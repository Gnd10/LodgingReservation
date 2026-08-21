export interface ExtraService {
  id: number;
  name: string;
  price: number;
  unitType: 'per_night' | 'per_person' | 'per_trip' | 'per_item';
}