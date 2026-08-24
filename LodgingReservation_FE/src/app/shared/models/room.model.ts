export interface RoomType {
  id: number;
  name: string;
  basePrice: number;
  capacity: number;
  description: string;
  imageUrl?: string;
  availableCount?: number;
  rooms?: Room[];
}

export interface Room {
  id: number;
  roomNumber: string;
  status: string | number;
  roomTypeId?: number;
  roomType?: RoomType;
}
