import { RefundProduct } from './refundProductDto';

export interface RequestRefundDto {
  saleId: number;
  userId: number;
  saleDate: string | null;
  refundMethod: string;
  productRefundList: RefundProduct[];
}
