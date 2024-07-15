export interface SaleDetail {
    ProductId: number;
    Quantity: number;
    UnitPrice: number;
    TotalPrice: number;
    SaleId:number;
}
  
export interface Sale {
    SaleId: number,
    CustomerId: number;
    PaymentMethod: string;
    Tax: number;
    Notes: string;
    UserId: number;
    CompanyId: number;
    SaleDetailsList: SaleDetail[];
}