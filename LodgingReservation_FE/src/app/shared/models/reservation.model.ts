export interface ReservationAddOn {
  extraServiceId: number;
  quantity: number;
  unitPrice?: number;
  subtotal?: number;
}

export interface ReservationRequest {
  promotionId: number | null;
  roomIds: number[];
  checkInDate: string;
  checkOutDate: string;
  lateCheckoutFee: number;
  addOns: ReservationAddOn[];
}

export interface ReservationResponse {
  id: number;
  userId: number;
  bookingCode: string;
  status: string;
  checkInDate: string;
  checkOutDate: string;
  totalNights: number;
  roomSubtotal: number;
  addOnsTotal: number;
  promoDiscount: number;
  grandTotal: number;
  userName: string;
  roomNumber: string;
  roomTypeName: string;
}
