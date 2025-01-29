export interface companyDto
{
    companyId: number,
    name: string,
    address?: string,
    createdBy?: number,
    createdByUserName?: string,
    createdByFullName?: string,

    creationDate?: Date,
    creationDateUI?: string,
    
    lastUpdateBy?: number,
    lastUpdateByUserName?: string,
    paymentsJson?: string,
    
    statusId: number,
    statusDesc?: string,
    
    expirationDate?: Date,
    expirationDateUI?: string,
    phoneNumber?: string,
    cellphoneNumber?: string,
    email?: string,
    ownerInfo?: string,

    planId?: number,
    planIdUI?: string,
    planPrice?: number
}
