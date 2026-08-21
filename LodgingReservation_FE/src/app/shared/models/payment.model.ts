export interface Payment {
  id?: number;
  reservationId: number;
  invoiceNumber: string;
  paymentMethod: 'BankTransfer' | 'CreditCard' | 'Cash' | 'QRIS';
  amountPaid: number;
  paymentStatus: 'Pending' | 'Paid' | 'Refunded' | 'Failed';
  paidAt?: string;
}