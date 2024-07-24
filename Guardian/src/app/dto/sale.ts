export interface SaleDetail {
    ProductId: number;
    Quantity: number;
    UnitPrice: number;
    TotalPrice: number;
    SaleId:number;
}
  
export interface Sale {
    id: number, /*Used for internal offline management */
    saleId: number,
    customerId: number;
    paymentMethod: string;
    tax: number;
    notes: string;
    userId: number;
    companyId: number;
    saleDetailsList: SaleDetail[];
}