import { MeasureType } from '../Enums/enums';

export interface ReturnableProduct {
  barcode: string;
  productId: number;
  name: string;
  quantity: number;
  unitPrice: number;
  measure: MeasureType;
  totalPrice: number;
  reason: string;
  customReason?: string | null;
  selected: boolean;
  returnable: boolean;
  returnQuantity: number;
  editing: boolean;
}
