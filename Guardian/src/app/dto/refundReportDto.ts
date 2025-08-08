

import { RefundProduct } from './refundProductDto';

export interface RefundReportDto {
  saleId: number;
  folio: number | null;
  userId: number;
  name: string;
  refundDate: string; 
  refundMethod: string;
  refundAmountTotal: number;
  products?: RefundProduct[]; 
}
