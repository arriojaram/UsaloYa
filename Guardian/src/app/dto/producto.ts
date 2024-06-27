export interface Producto {
  productId: number;
  name: string;
  description: string;
  categoryId?: number;
  supplierId?: number;
  unitPrice?: number;
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
}
