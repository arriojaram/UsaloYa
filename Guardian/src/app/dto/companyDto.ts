export interface companyDto
{
    companyId: number,
    name: string,
    address?: string,
    createdBy?: number,
    createdByUserName?: string,
    
    creationDate?: Date,
    creationDateUI?: string,
    
    lastUpdateBy?: number,
    lastUpdateByUserName?: string,
    paymentsJson?: string,
    
    statusId: number,
    statusDesc?: string,
    
    expirationDate?: Date,
    expirationDateUI?: string

}
