export interface companySettingsDto
{
    companyId: number;
    settings: pairSettingsDto[]
    
}

export interface pairSettingsDto
{
    key: string;
    value: string;
}

export interface settingsDto
{
    activarImpresionMobile: boolean;
    activarImpresionWeb: boolean;
    impresoraWeb: string;
    maxDaysToRefund: number;
}