export interface RoomType {
  id: number;
  name: string;
  basePrice: number;
  capacity: number;
  description: string;
}

export interface Room {
  id: number;
  roomTypeId: number;
  roomNumber: string;
  status: 'available' | 'occupied' | 'maintenance';
  roomType?: RoomType;
}