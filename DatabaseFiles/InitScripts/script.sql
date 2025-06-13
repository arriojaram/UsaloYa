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

CREATE TABLE [PlanRentas] (
    [Id] int NOT NULL IDENTITY,
    [Name] varchar(50) NOT NULL,
    [Notes] nvarchar(500) NULL,
    [Price] decimal(10,0) NOT NULL DEFAULT (((1))),
    [StatusId] int NOT NULL,
    [NumUsers] int NOT NULL,
    CONSTRAINT [PK_PlanRentas] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Company] (
    [CompanyId] int NOT NULL IDENTITY,
    [Name] varchar(50) NOT NULL,
    [Address] varchar(250) NULL,
    [CreatedBy] int NULL,
    [CreationDate] datetime NULL,
    [LastUpdateBy] int NULL,
    [PaymentsJson] xml NULL,
    [StatusId] int NOT NULL,
    [ExpirationDate] datetime NULL,
    [PhoneNumber] varchar(10) NULL,
    [CelphoneNumber] varchar(10) NULL,
    [Email] varchar(30) NULL,
    [OwnerInfo] nvarchar(500) NULL,
    [PlanId] int NULL,
    CONSTRAINT [PK_Company] PRIMARY KEY ([CompanyId]),
    CONSTRAINT [FK_Company_PlanRentas] FOREIGN KEY ([PlanId]) REFERENCES [PlanRentas] ([Id])
);
GO

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
    [FullName] AS (concat([FirstName],' ',[LastName1],coalesce(' '+[LastName2],''))),
    [CompanyId] int NOT NULL,
    CONSTRAINT [PK_Customer_Id] PRIMARY KEY ([CustomerId]),
    CONSTRAINT [FK_Customer_Company] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([CompanyId])
);
GO

CREATE TABLE [Groups] (
    [GroupId] int NOT NULL IDENTITY,
    [Name] varchar(50) NOT NULL,
    [Description] varchar(250) NULL,
    [Permissions] xml NOT NULL,
    [CompanyId] int NOT NULL,
    CONSTRAINT [PK_Groups] PRIMARY KEY ([GroupId]),
    CONSTRAINT [FK_Groups_Company] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([CompanyId])
);
GO

CREATE TABLE [ProductCategory] (
    [CategoryId] int NOT NULL IDENTITY,
    [Name] varchar(50) NOT NULL,
    [Description] varchar(500) NULL,
    [CompanyId] int NOT NULL,
    CONSTRAINT [PK_ProductCategory] PRIMARY KEY ([CategoryId]),
    CONSTRAINT [FK_ProductCategory_Company] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([CompanyId])
);
GO

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
    [StatusId] int NOT NULL,
    [CreatedBy] int NULL,
    [LastUpdateBy] int NULL,
    [CreationDate] datetime NULL,
    [RoleId] int NULL,
    [DeviceId] nvarchar(255) NULL,
    [SessionToken] uniqueidentifier NULL,
    [CodeVerification] nvarchar(10) NULL,
    [IsVerifiedCode] bit NULL DEFAULT ((CONVERT([bit],(0)))),
    [Email] nvarchar(100) NULL,
    CONSTRAINT [PK_User_Id] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_Users_Company] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([CompanyId]),
    CONSTRAINT [FK_Users_Groups] FOREIGN KEY ([GroupId]) REFERENCES [Groups] ([GroupId]),
    CONSTRAINT [FK_Users_Users_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users] ([UserId]),
    CONSTRAINT [FK_Users_Users_LastUpdateBy] FOREIGN KEY ([LastUpdateBy]) REFERENCES [Users] ([UserId])
);
GO

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
    [BuyPrice] decimal(10,2) NULL,
    [UnitPrice1] decimal(10,2) NULL,
    [UnitPrice2] decimal(10,2) NULL,
    [UnitPrice3] decimal(10,2) NULL,
    [InVentario] int NULL,
    [AlertaStockNumProducts] int NULL,
    [IsInVentarioUpdated] bit NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([ProductId]),
    CONSTRAINT [FK_Products_Company] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([CompanyId]),
    CONSTRAINT [FK_Products_ProductCategory] FOREIGN KEY ([CategoryId]) REFERENCES [ProductCategory] ([CategoryId])
);
DECLARE @defaultSchema AS sysname;
SET @defaultSchema = SCHEMA_NAME();
DECLARE @description AS sql_variant;
SET @description = N'Valor utilizado para guardar informacion temporal del inventario del producto';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'Products', 'COLUMN', N'InVentario';
GO

CREATE TABLE [Rentas] (
    [Id] int NOT NULL IDENTITY,
    [CompanyId] int NOT NULL,
    [ReferenceDate] datetime NOT NULL,
    [Amount] decimal(10,2) NOT NULL,
    [AddedByUserId] int NOT NULL,
    [StatusId] int NOT NULL,
    [TipoRentaDesc] varchar(15) NULL,
    [Notas] varchar(500) NULL,
    [ExpirationDate] datetime NULL,
    CONSTRAINT [PK_Rentas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Rentas_Company] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([CompanyId]),
    CONSTRAINT [FK_Rentas_Users] FOREIGN KEY ([AddedByUserId]) REFERENCES [Users] ([UserId])
);
GO

CREATE TABLE [Sales] (
    [SaleId] int NOT NULL IDENTITY,
    [CustomerId] int NULL,
    [SaleDate] datetime NOT NULL DEFAULT ((getdate())),
    [PaymentMethod] varchar(50) NOT NULL,
    [Tax] decimal(10,2) NOT NULL,
    [Status] varchar(50) NOT NULL DEFAULT (('Completada')),
    [TotalSale] decimal(10,2) NOT NULL,
    [Notes] varchar(500) NULL,
    [UserId] int NOT NULL,
    [CompanyId] int NOT NULL,
    CONSTRAINT [PK_Sales] PRIMARY KEY ([SaleId]),
    CONSTRAINT [FK_Sale_Company] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([CompanyId]),
    CONSTRAINT [FK_Sales_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]) ON DELETE SET NULL,
    CONSTRAINT [FK_Sales_Users] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId])
);
GO

CREATE TABLE [SaleDetails] (
    [SaleId] int NOT NULL,
    [ProductId] int NOT NULL,
    [Quantity] int NOT NULL,
    [UnitPrice] decimal(10,2) NOT NULL,
    [TotalPrice] decimal(10,2) NOT NULL,
    [BuyPrice] decimal(10,2) NOT NULL,
    [PriceLevel] int NULL,
    CONSTRAINT [PK_SaleDetails] PRIMARY KEY ([SaleId], [ProductId]),
    CONSTRAINT [FK_SaleDetails_Products] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([ProductId]),
    CONSTRAINT [FK_Sale_SaleDetails] FOREIGN KEY ([SaleId]) REFERENCES [Sales] ([SaleId])
);
GO

CREATE INDEX [IX_Company_CreatedBy] ON [Company] ([CreatedBy]);
GO

CREATE INDEX [IX_Company_LastUpdateBy] ON [Company] ([LastUpdateBy]);
GO

CREATE INDEX [IX_Company_PlanId] ON [Company] ([PlanId]);
GO

CREATE INDEX [IX_Customer_CompanyId] ON [Customers] ([CompanyId]);
GO

CREATE INDEX [IX_Customer_Email] ON [Customers] ([Email]);
GO

CREATE INDEX [IX_Customer_Name] ON [Customers] ([FirstName], [LastName1], [LastName2]);
GO

CREATE INDEX [IX_Customer_Phone] ON [Customers] ([CellPhoneNumber], [WorkPhoneNumber]);
GO

CREATE INDEX [IX_Groups_CompanyId] ON [Groups] ([CompanyId]);
GO

CREATE INDEX [IX_ProductCategory_CompanyId] ON [ProductCategory] ([CompanyId]);
GO

CREATE INDEX [IX_Products] ON [Products] ([Name], [Description]);
GO

CREATE UNIQUE INDEX [IX_Products_Barcode_SKU] ON [Products] ([CompanyId], [Barcode], [SKU]) WHERE ([Barcode] IS NOT NULL AND [SKU] IS NOT NULL);
GO

CREATE INDEX [IX_Products_CategoryId] ON [Products] ([CategoryId]);
GO

CREATE INDEX [IX_Products_CompanyId] ON [Products] ([CompanyId]);
GO

CREATE INDEX [IX_Rentas_AddedByUserId] ON [Rentas] ([AddedByUserId]);
GO

CREATE INDEX [IX_Rentas_CompanyId] ON [Rentas] ([CompanyId]);
GO

CREATE INDEX [IX_SaleDetails_ProductId] ON [SaleDetails] ([ProductId]);
GO

CREATE INDEX [IX_Sales_CompanyId] ON [Sales] ([CompanyId]);
GO

CREATE INDEX [IX_Sales_CustomerId] ON [Sales] ([CustomerId]);
GO

CREATE INDEX [IX_Sales_UserId] ON [Sales] ([UserId]);
GO

CREATE INDEX [IX_Users_CompanyId] ON [Users] ([CompanyId]);
GO

CREATE INDEX [IX_Users_CreatedBy] ON [Users] ([CreatedBy]);
GO

CREATE INDEX [IX_Users_GroupId] ON [Users] ([GroupId]);
GO

CREATE INDEX [IX_Users_LastUpdateBy] ON [Users] ([LastUpdateBy]);
GO

CREATE UNIQUE INDEX [IX_Users_UserName] ON [Users] ([UserName]);
GO

ALTER TABLE [Company] ADD CONSTRAINT [FK_Company_Users_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users] ([UserId]);
GO

ALTER TABLE [Company] ADD CONSTRAINT [FK_Company_Users_LastUpdateBy] FOREIGN KEY ([LastUpdateBy]) REFERENCES [Users] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250604173402_InitialCreate', N'7.0.20');
GO

COMMIT;
GO

