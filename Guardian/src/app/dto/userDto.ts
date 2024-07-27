export interface userDto{
    userId: number,
    userName: string,
    firstName?: string,
    lastName?: string,
    companyId: number,
    groupId: number,
    statusId: number,

    lastAccess?: Date,
    isEnabled?: boolean,
    token?: string
}