export interface SaleProduct {
  barcode: string;
  productId: number;
  name: string;
  quantity: number;
  unitPrice: number;
  measure: number;
  reason: string;
  selected: boolean;
  returnable: boolean;
  returnQuantity: number;
  editing: boolean;
  customReason: string | null;
}
