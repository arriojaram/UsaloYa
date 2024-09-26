BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240926234312_AddCustomerTable')
BEGIN
    CREATE TABLE [Customers] (
        [CustomerId] int NOT NULL IDENTITY,
        [Address] varchar(300) NOT NULL,
        [Notes] varchar(500) NOT NULL,
        [FirstName] varchar(50) NOT NULL,
        [LastName1] varchar(50) NOT NULL,
        [LastName2] varchar(50) NOT NULL,
        [FullName] AS CONCAT(FirstName, ' ', LastName1,  COALESCE(LastName2 + ' ', '')),
        [CompanyId] int NOT NULL,
        CONSTRAINT [PK_Customer_Id] PRIMARY KEY ([CustomerId]),
        CONSTRAINT [FK_Customer_Company] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([CompanyId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240926234312_AddCustomerTable')
BEGIN
    CREATE INDEX [IX_Sales_CustomerId] ON [Sales] ([CustomerId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240926234312_AddCustomerTable')
BEGIN
    CREATE INDEX [IX_Customer_CompanyId] ON [Customers] ([CompanyId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240926234312_AddCustomerTable')
BEGIN
    CREATE INDEX [IX_Customer_FirstName] ON [Customers] ([FirstName]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240926234312_AddCustomerTable')
BEGIN
    CREATE INDEX [IX_Customer_LastName1] ON [Customers] ([LastName1]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240926234312_AddCustomerTable')
BEGIN
    ALTER TABLE [Sales] ADD CONSTRAINT [FK_Sales_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240926234312_AddCustomerTable')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240926234312_AddCustomerTable', N'7.0.20');
END;
GO

COMMIT;
GO

