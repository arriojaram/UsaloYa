export enum Roles {
    Desconocido = 0,
    Usuario = 1,
    Administrador = 2,
    SysAdmin = 3,
    Root = 4
}

export enum CompanyStatus
{
    Inactive = 0,
    PendingPayment = 1,
    Expired = 2,
    Active = 3
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