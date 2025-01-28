export interface rentRequestDto {
    id: number;
    companyId: number;
    referenceDate: Date;
    expirationDate?: Date;
    amount: number;
    addedByUserId: number;
    statusId: number;
    tipoRentaDesc?: string; 

    byUserName?: string;
    statusIdUI?: string;
    notas?: string;
}
