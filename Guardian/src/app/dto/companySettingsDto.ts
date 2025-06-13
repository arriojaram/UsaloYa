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
    activarImpresionWeb: boolean;
    impresoraWeb: string;
}