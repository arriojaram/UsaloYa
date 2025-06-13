export interface InventoryProduct {
  productId: number;
  name: string;
  barcode: string;
  sku: string;
  companyId: number;
  unitsInStock: number;
  totalCashStock: number;
  unitsInVentario: number;
  inVentarioAlertLevel: number; //3:Normal, 2:Warning, 1:Critico
  alertaStockNumProducts: number;
  categoryName: string,
  isInVentarioUpdated: boolean
}


export interface Inventory {
  totalProducts: number;
  totalProductUnits: number;
  totalCash: number;
  products: InventoryProduct [];
}
