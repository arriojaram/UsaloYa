BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240927172538_IncludeCustomerInformation')
BEGIN
    CREATE TABLE [Customers] (
        [CustomerId] int NOT NULL IDENTITY,
        [Address] varchar(300) NULL,
        [Notes] varchar(500) NULL,
        [FirstName] varchar(50) NOT NULL,
        [LastName1] varchar(50) NOT NULL,
        [LastName2] varchar(50) NULL,
        [WorkPhoneNumber] varchar(15) NULL,
        [CellPhoneNumber] varchar(15) NULL,
        [Email] varchar(35) NULL,
        [FullName] AS CONCAT(FirstName, ' ', LastName1,  COALESCE(' ' + LastName2, '')),
        [CompanyId] int NOT NULL,
        CONSTRAINT [PK_Customer_Id] PRIMARY KEY ([CustomerId]),
        CONSTRAINT [FK_Customer_Company] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([CompanyId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240927172538_IncludeCustomerInformation')
BEGIN
    CREATE INDEX [IX_Sales_CustomerId] ON [Sales] ([CustomerId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240927172538_IncludeCustomerInformation')
BEGIN
    CREATE INDEX [IX_Customer_CompanyId] ON [Customers] ([CompanyId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240927172538_IncludeCustomerInformation')
BEGIN
    CREATE INDEX [IX_Customer_Email] ON [Customers] ([Email]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240927172538_IncludeCustomerInformation')
BEGIN
    CREATE INDEX [IX_Customer_Name] ON [Customers] ([FirstName], [LastName1], [LastName2]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240927172538_IncludeCustomerInformation')
BEGIN
    CREATE INDEX [IX_Customer_Phone] ON [Customers] ([CellPhoneNumber], [WorkPhoneNumber]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240927172538_IncludeCustomerInformation')
BEGIN
    ALTER TABLE [Sales] ADD CONSTRAINT [FK_Sales_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240927172538_IncludeCustomerInformation')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240927172538_IncludeCustomerInformation', N'7.0.20');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241008024320_Price123')
BEGIN
    ALTER TABLE [Products] ADD [UnitPrice1] decimal(10,2) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241008024320_Price123')
BEGIN
    ALTER TABLE [Products] ADD [UnitPrice2] decimal(10,2) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241008024320_Price123')
BEGIN
    ALTER TABLE [Products] ADD [UnitPrice3] decimal(10,2) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241008024320_Price123')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241008024320_Price123', N'7.0.20');
END;
GO

COMMIT;
GO

