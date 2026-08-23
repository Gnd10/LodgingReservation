export interface Promotion {
  id: number;
  promoCode: string;
  discountPercentage: number;
  validUntil: string;
  maxDiscountCap: number;
}

export interface ValidatePromoResponse {
  isValid: boolean;
  message: string;
  promoCode?: string;
  discountPercentage: number;
  discountAmount: number;
  finalAmount: number;
}
