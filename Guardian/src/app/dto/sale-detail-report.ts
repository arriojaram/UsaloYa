export interface SaleDetailReport {
  saleID: number;
  folio: number;
  saleDate: Date;
  userId: number;
  userName: string;
  fullName: string;
  notes: string;
  status: string;
  totalSale: number;
  payment: string;
  refundAmountTotal?: number; // total $ de devoluciones

  customerName: string;
}


export interface ProductSaleDetailReport {
  barcode: string;
  productId: number;
  productName: string;
  quantity: number;
  measure: number;
  canRefunded: boolean;
  totalPrice: number;
  saleID: number;
  saleDate: Date;
  buyPrice: number;
  soldPrice: number;
  productPrice1: number;
  productPrice2: number;
  productPrice3: number;
  userId: number;
  userName: string;
  fullName: string;
  priceLevel: number;
  refundProducts?: number;
  refundoAmount?: number;
}
