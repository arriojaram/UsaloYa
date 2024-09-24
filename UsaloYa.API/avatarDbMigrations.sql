IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240620011923_InitialMigration')
BEGIN
    CREATE TABLE [Company] (
        [CompanyId] int NOT NULL IDENTITY,
        [Name] varchar(50) NOT NULL,
        [Address] varchar(250) NULL,
        CONSTRAINT [PK_Company] PRIMARY KEY ([CompanyId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240620011923_InitialMigration')
BEGIN
    CREATE TABLE [Groups] (
        [GroupId] int NOT NULL IDENTITY,
        [Name] varchar(50) NOT NULL,
        [Description] varchar(250) NULL,
        [Permissions] xml NOT NULL,
        [CompanyId] int NOT NULL,
        CONSTRAINT [PK_Groups] PRIMARY KEY ([GroupId]),
        CONSTRAINT [FK_Groups_Company] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([CompanyId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240620011923_InitialMigration')
BEGIN
    CREATE TABLE [Products] (
        [ProductId] int NOT NULL IDENTITY,
        [Name] varchar(250) NOT NULL,
        [Description] varchar(500) NULL,
        [CategoryId] int NULL,
        [SupplierId] int NULL,
        [UnitPrice] decimal(10,2) NULL,
        [UnitsInStock] int NOT NULL,
        [Discontinued] bit NOT NULL,
        [ImgUrl] varchar(250) NULL,
        [DateAdded] datetime NOT NULL DEFAULT ((getdate())),
        [DateModified] datetime NOT NULL DEFAULT ((getdate())),
        [Weight] decimal(10,2) NULL,
        [SKU] varchar(50) NULL,
        [Barcode] varchar(50) NULL,
        [Brand] varchar(50) NULL,
        [Color] varchar(50) NULL,
        [Size] varchar(50) NULL,
        [CompanyId] int NOT NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY ([ProductId]),
        CONSTRAINT [FK_Products_Company] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([CompanyId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240620011923_InitialMigration')
BEGIN
    CREATE TABLE [Users] (
        [UserId] int NOT NULL IDENTITY,
        [UserName] varchar(50) NOT NULL,
        [Token] varchar(100) NOT NULL,
        [FirstName] varchar(50) NOT NULL,
        [LastName] varchar(50) NOT NULL,
        [CompanyId] int NOT NULL,
        [GroupId] int NOT NULL,
        [LastAccess] datetime NULL,
        [IsEnabled] bit NOT NULL DEFAULT ((CONVERT([bit],(0)))),
        CONSTRAINT [PK_User_Id] PRIMARY KEY ([UserId]),
        CONSTRAINT [FK_Users_Company] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([CompanyId]),
        CONSTRAINT [FK_Users_Groups] FOREIGN KEY ([GroupId]) REFERENCES [Groups] ([GroupId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240620011923_InitialMigration')
BEGIN
    CREATE TABLE [Sales] (
        [SaleId] int NOT NULL IDENTITY,
        [CustomerId] int NULL,
        [SaleDate] datetime NOT NULL DEFAULT ((getdate())),
        [PaymentMethod] varchar(50) NOT NULL DEFAULT (('Efectivo')),
        [Tax] decimal(10,2) NOT NULL,
        [Status] varchar(50) NOT NULL DEFAULT (('Completada')),
        [TotalSale] decimal(10,2) NOT NULL,
        [Notes] varchar(500) NULL,
        [UserId] int NOT NULL,
        CONSTRAINT [PK_Sales] PRIMARY KEY ([SaleId]),
        CONSTRAINT [FK_Sales_Users] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240620011923_InitialMigration')
BEGIN
    CREATE TABLE [SaleDetails] (
        [SaleId] int NOT NULL,
        [ProductId] int NOT NULL,
        [Quantity] int NOT NULL,
        [UnitPrice] decimal(10,2) NOT NULL,
        [TotalPrice] decimal(10,2) NOT NULL,
        CONSTRAINT [PK_SaleDetails] PRIMARY KEY ([SaleId], [ProductId]),
        CONSTRAINT [FK_SaleDetails_Products] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([ProductId]),
        CONSTRAINT [FK_Sale_SaleDetails] FOREIGN KEY ([SaleId]) REFERENCES [Sales] ([SaleId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240620011923_InitialMigration')
BEGIN
    CREATE INDEX [IX_Groups_CompanyId] ON [Groups] ([CompanyId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240620011923_InitialMigration')
BEGIN
    CREATE INDEX [IX_Products] ON [Products] ([Name], [Description]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240620011923_InitialMigration')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Products_Barcode_SKU] ON [Products] ([Barcode], [SKU]) WHERE ([Barcode] IS NOT NULL AND [SKU] IS NOT NULL)');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240620011923_InitialMigration')
BEGIN
    CREATE INDEX [IX_Products_CompanyId] ON [Products] ([CompanyId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240620011923_InitialMigration')
BEGIN
    CREATE INDEX [IX_SaleDetails_ProductId] ON [SaleDetails] ([ProductId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240620011923_InitialMigration')
BEGIN
    CREATE INDEX [IX_Sales_UserId] ON [Sales] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240620011923_InitialMigration')
BEGIN
    CREATE INDEX [IX_Users_CompanyId] ON [Users] ([CompanyId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240620011923_InitialMigration')
BEGIN
    CREATE INDEX [IX_Users_GroupId] ON [Users] ([GroupId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240620011923_InitialMigration')
BEGIN
    CREATE UNIQUE INDEX [IX_Users_UserName] ON [Users] ([UserName]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240620011923_InitialMigration')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240620011923_InitialMigration', N'7.0.20');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240620012839_AddCompanyToSale')
BEGIN
    ALTER TABLE [Sales] ADD [CompanyId] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240620012839_AddCompanyToSale')
BEGIN
    CREATE INDEX [IX_Sales_CompanyId] ON [Sales] ([CompanyId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240620012839_AddCompanyToSale')
BEGIN
    ALTER TABLE [Sales] ADD CONSTRAINT [FK_Sale_Company] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([CompanyId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240620012839_AddCompanyToSale')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240620012839_AddCompanyToSale', N'7.0.20');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240726152319_AddUserStatusIdInt')
BEGIN
    ALTER TABLE [Users] ADD [StatusId] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240726152319_AddUserStatusIdInt')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240726152319_AddUserStatusIdInt', N'7.0.20');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240912180834_AddBuyPrice')
BEGIN
    DROP INDEX [IX_Products_Barcode_SKU] ON [Products];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240912180834_AddBuyPrice')
BEGIN
    ALTER TABLE [Products] ADD [BuyPrice] decimal(18,2) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240912180834_AddBuyPrice')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Products_Barcode_SKU] ON [Products] ([CompanyId], [Barcode], [SKU]) WHERE ([Barcode] IS NOT NULL AND [SKU] IS NOT NULL)');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240912180834_AddBuyPrice')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240912180834_AddBuyPrice', N'7.0.20');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240913161525_AddBuyPriceToSales')
BEGIN
    ALTER TABLE [SaleDetails] ADD [BuyPrice] decimal(10,2) NOT NULL DEFAULT 0.0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240913161525_AddBuyPriceToSales')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Products]') AND [c].[name] = N'BuyPrice');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Products] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Products] ALTER COLUMN [BuyPrice] decimal(10,2) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240913161525_AddBuyPriceToSales')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240913161525_AddBuyPriceToSales', N'7.0.20');
END;
GO

COMMIT;
GO

