export interface RefundProduct {
  productId: number;
  barcode: string;
  productName: string;
  reason: string;
  measure: string;
  quantity: number;
  unitPriceRefund: number;
  refundAmount: number
}
