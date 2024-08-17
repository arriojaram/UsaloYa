export interface SaleDetailReport {
    saleID: number;
    saleDate: Date;
    userId: number;
    userName: string;
    fullName: string;
    notes: string;
    status: string;
    totalSale: number;
  }
  
  
  export interface ProductSaleDetailReport {
    barcode: string;
    productName: string;
    quantity: number;
    totalPrice: number;
    saleID: number;
    saleDate: Date;
    totalSale: number;
    userId: number;
    userName: string;
    fullName: string;
  }
  