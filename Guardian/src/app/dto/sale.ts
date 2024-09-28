export interface SaleDetail {
    ProductId: number;
    Quantity: number;
    UnitPrice: number;
    TotalPrice: number;
    SaleId:number;
}
  
export interface Sale {
    id: number | undefined, /*Used for internal offline sync management */
    saleId: number,
    customerId: number | undefined;
    paymentMethod: string;
    tax: number;
    notes: string;
    userId: number;
    companyId: number;
    saleDetailsList: SaleDetail[];
}