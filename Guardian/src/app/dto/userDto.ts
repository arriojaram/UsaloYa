export interface userDto{
    userId: number,
    userName: string,
    firstName?: string,
    lastName?: string,
    companyId: number,
    groupId: number,
    statusId: number,
    statusIdStr?: string,
    lastAccess?: Date,
    lastAccess4UI?: string,
    isEnabled?: boolean,
    token?: string
}