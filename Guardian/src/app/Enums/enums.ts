export enum RentTypeId
{ 
    Desconocido = 0,
    Mensualidad = 1,
    Condonacion = 2,
    Extension = 3,
}

export enum PriceLevel
{ 
    UnitPrice = 0,
    UnitPrice1 = 1,
    UnitPrice2 = 2,
    UnitPrice3 = 3,
}


export enum Roles {
    Unknown = 0,
    User = 1,
    Admin = 2,
    Ventas = 3,
    SysAdmin = 13,
    Root = 14
}

export enum CompanyStatus
{
    Inactive = 0,
    PendingPayment = 1,
    Expired = 2,
    Active = 3
}

export enum AlertLevel
{
    Info = 0,
    Sucess =1,
    Warning =2,
    Error = 3
}

export enum UserStatus
{
    Desconectado = 0,
    Conectado =1,
    Deshabilitado =2,
    Desconocido = 3
}

export enum StatusVentaEnum {
    Completada,
    Cancelada 
}

export function getCompanyStatusEnumName(value: number): string {
    const name = CompanyStatus[value];
    if (name === undefined) {
        return CompanyStatus.Inactive.toString();
    }
    return CompanyStatus[value];  
}

export function getUserStatusEnumName(value: number): string {
    const name = UserStatus[value];
    if (name === undefined) {
        return UserStatus.Desconocido.toString();
    }
    return UserStatus[value];  
}