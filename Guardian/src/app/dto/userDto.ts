export interface userDto{
    userId: number,
    userName: string,
    firstName?: string,
    lastName?: string,
    companyId: number,
    companyName: string,
    groupId: number,
    statusId: number,
    statusIdStr?: string,
    lastAccess?: Date,
    lastAccess4UI?: string,
    isEnabled?: boolean,
    token?: string,
    createdBy?: number,
    lastUpdatedBy?: number
    createdByUserName?: string,
    lastUpdatedByUserName?: string
    creationDate?: Date,
    creationDateUI?: string,
    roleId: number,
    companyStatusId?: number
}