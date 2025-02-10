export interface Producto {
  productId: number;
  name: string;
  description: string;
  categoryId: number;
  
  buyPrice: number;
  unitPrice: number;
  unitPrice1: number,
  unitPrice2: number,
  unitPrice3: number,
  unitsInStock: number;
  discontinued: boolean;
  sku: string;
  barcode: string;
  companyId: number;
  lowInventoryStart?: number;
  isInVentarioUpdated?: boolean;

  count: number;
  total: number;
  editing?: boolean;
  priceLevel?: number;
}
