import { ExtraService } from "./extra-service.model";

export interface ReservationAddOn {
  extraServiceId: number;
  quantity: number;
  unitPrice: number;
  subtotal: number;
  extraService?: ExtraService;
}

export interface Reservation {
  id?: number;
  bookingCode?: string;
  userId: number;
  promotionId?: number | null;
  checkInDate: string;
  checkOutDate: string;
  actualCheckoutTime?: string | null;
  status?: string;
  totalNights: number;
  roomSubtotal: number;
  lateCheckoutFee: number;
  addOnsTotal: number;
  promoDiscount: number;
  grandTotal: number;
  roomIds: number[];
  addOns: ReservationAddOn[];
}