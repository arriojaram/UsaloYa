export interface SaleSummary {
    saleID: number;
    folio: number;
    saleDate: string;
    totalSale: number;
    returned?: boolean;
    fullName: string;
    userName: string;
}
