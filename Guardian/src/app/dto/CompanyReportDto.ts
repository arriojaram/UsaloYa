export interface CompanyReportDto
{
    numberOfUsers: number,
    companyName: string,
    lastAccess: Date,
    phone? : string,
    status: number,
    statusDesc? : string,
    numberOfProducts: number,
    numberOfSales: number
}