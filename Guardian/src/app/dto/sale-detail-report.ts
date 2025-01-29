export interface SaleDetailReport {
    saleID: number;
    saleDate: Date;
    userId: number;
    userName: string;
    fullName: string;
    notes: string;
    status: string;
    totalSale: number;
    payment: string;
    
    customerName: string;
  }
  
  
  export interface ProductSaleDetailReport {
    barcode: string;
    productName: string;
    quantity: number;
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
  }
  