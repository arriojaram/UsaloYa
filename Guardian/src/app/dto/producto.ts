export interface Producto {
  productId: number;
  name: string;
  description: string;
  categoryId: number;
  supplierId: number;
  buyPrice: number;
  unitPrice: number;
  unitPrice1: number,
  unitPrice2: number,
  unitPrice3: number,
  unitsInStock: number;
  discontinued: boolean;
  imgUrl: string;
  dateAdded: Date;
  dateModified: Date;
  weight?: number;
  sku: string;
  barcode: string;
  brand: string;
  color: string;
  size: string;
  companyId: number;

  count: number;
  total: number;
  editing?: boolean;
  priceLevel?: number;
}
