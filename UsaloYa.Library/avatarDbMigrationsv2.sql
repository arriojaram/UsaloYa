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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250729214504_CashCount')
BEGIN
    ALTER TABLE [Questions] DROP CONSTRAINT [FK_Questions_Users_IdUser];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250729214504_CashCount')
BEGIN
    CREATE TABLE [CashCount] (
        [CashCountId] bigint NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [StatusId] int NOT NULL,
        [ReferenceDate] datetime NOT NULL,
        [InitialBalance] decimal(18,2) NOT NULL,
        [Cash] decimal(18,2) NOT NULL,
        [CredictCard] decimal(18,2) NULL,
        [Spei] decimal(18,2) NULL,
        [CashOutput] decimal(18,2) NULL,
        [Notes] varchar(500) NULL,
        [FinalCash] nchar(10) NULL,
        CONSTRAINT [PK_CashCount] PRIMARY KEY ([CashCountId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250729214504_CashCount')
BEGIN
    CREATE TABLE [CashOutput] (
        [OutputId] int NOT NULL IDENTITY,
        [Balance] decimal(18,2) NOT NULL,
        [Reason] varchar(50) NOT NULL,
        [ReferenceDate] datetime NOT NULL DEFAULT ((getdate())),
        [UserId] int NOT NULL,
        [IsCashCount] bit NOT NULL,
        [CashCountId] bigint NOT NULL,
        CONSTRAINT [PK_CashOutput] PRIMARY KEY ([OutputId]),
        CONSTRAINT [FK_CashOutput_CashCount] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250729214504_CashCount')
BEGIN
    CREATE INDEX [IX_CashOutput_UserId] ON [CashOutput] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250729214504_CashCount')
BEGIN
    ALTER TABLE [Questions] ADD CONSTRAINT [FK_Questions_Users_IdUser] FOREIGN KEY ([IdUser]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250729214504_CashCount')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250729214504_CashCount', N'7.0.20');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250731161944_CashCount')
BEGIN
    ALTER TABLE [CashOutput] DROP CONSTRAINT [FK_CashOutput_CashCount];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250731161944_CashCount')
BEGIN
    EXEC sp_rename N'[CashCount].[CashOutput]', N'CashOutputTotal', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250731161944_CashCount')
BEGIN
    CREATE INDEX [IX_CashOutput_CashCountId] ON [CashOutput] ([CashCountId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250731161944_CashCount')
BEGIN
    CREATE INDEX [IX_CashCount_UserId] ON [CashCount] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250731161944_CashCount')
BEGIN
    ALTER TABLE [CashCount] ADD CONSTRAINT [FK_User_CashCount] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250731161944_CashCount')
BEGIN
    ALTER TABLE [CashOutput] ADD CONSTRAINT [FK_CashCount_CashOutput] FOREIGN KEY ([CashCountId]) REFERENCES [CashCount] ([CashCountId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250731161944_CashCount')
BEGIN
    ALTER TABLE [CashOutput] ADD CONSTRAINT [FK_User_CashOutput] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250731161944_CashCount')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250731161944_CashCount', N'7.0.20');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250731172849_RefundTableAddCanRefundedAndMaxDaysToRefund')
BEGIN
    ALTER TABLE [Products] ADD [CanRefunded] bit NULL DEFAULT ((CONVERT([bit],(0))));
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250731172849_RefundTableAddCanRefundedAndMaxDaysToRefund')
BEGIN
    ALTER TABLE [Company] ADD [MaxDaysToRefund] int NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250731172849_RefundTableAddCanRefundedAndMaxDaysToRefund')
BEGIN
    CREATE TABLE [Refunds] (
        [SaleId] int NOT NULL,
        [UserId] int NOT NULL,
        [RefundDate] datetime NOT NULL,
        [RefundMethod] varchar(50) NOT NULL,
        [Barcode] nvarchar(50) NOT NULL,
        [Reason] text NOT NULL,
        [Measure] int NOT NULL,
        [Quantity] decimal(10,2) NOT NULL,
        [UnitPriceRefund] decimal(10,2) NOT NULL,
        [RefundAmount] decimal(10,2) NOT NULL,
        CONSTRAINT [PK_Refunds] PRIMARY KEY ([SaleId]),
        CONSTRAINT [FK_Refunds_Sales] FOREIGN KEY ([SaleId]) REFERENCES [Sales] ([SaleId]),
        CONSTRAINT [FK_Refunds_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250731172849_RefundTableAddCanRefundedAndMaxDaysToRefund')
BEGIN
    CREATE INDEX [IX_Refunds_UserId] ON [Refunds] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250731172849_RefundTableAddCanRefundedAndMaxDaysToRefund')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250731172849_RefundTableAddCanRefundedAndMaxDaysToRefund', N'7.0.20');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250731173328_AddPublicVirtualRefund')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250731173328_AddPublicVirtualRefund', N'7.0.20');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250731183617_AddProductIdColumnInRefunds')
BEGIN
    ALTER TABLE [Refunds] DROP CONSTRAINT [PK_Refunds];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250731183617_AddProductIdColumnInRefunds')
BEGIN
    ALTER TABLE [Refunds] ADD [ProductId] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250731183617_AddProductIdColumnInRefunds')
BEGIN
    ALTER TABLE [Refunds] ADD CONSTRAINT [PK_Refunds] PRIMARY KEY ([SaleId], [ProductId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250731183617_AddProductIdColumnInRefunds')
BEGIN
    CREATE INDEX [IX_Refunds_ProductId] ON [Refunds] ([ProductId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250731183617_AddProductIdColumnInRefunds')
BEGIN
    ALTER TABLE [Refunds] ADD CONSTRAINT [FK_Refunds_Products] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([ProductId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250731183617_AddProductIdColumnInRefunds')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250731183617_AddProductIdColumnInRefunds', N'7.0.20');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250804201434_CanMakeRefundinUsers')
BEGIN
    ALTER TABLE [Users] ADD [CanMakeReturns] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250804201434_CanMakeRefundinUsers')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Products]') AND [c].[name] = N'CanRefunded');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Products] DROP CONSTRAINT [' + @var0 + '];');
    EXEC(N'UPDATE [Products] SET [CanRefunded] = (CONVERT([bit],(0))) WHERE [CanRefunded] IS NULL');
    ALTER TABLE [Products] ALTER COLUMN [CanRefunded] bit NOT NULL;
    ALTER TABLE [Products] ADD DEFAULT ((CONVERT([bit],(0)))) FOR [CanRefunded];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250804201434_CanMakeRefundinUsers')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250804201434_CanMakeRefundinUsers', N'7.0.20');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250805002957_UpdateFinalCashColumnType')
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CashCount]') AND [c].[name] = N'FinalCash');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [CashCount] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [CashCount] ALTER COLUMN [FinalCash] decimal(18,2) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250805002957_UpdateFinalCashColumnType')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250805002957_UpdateFinalCashColumnType', N'7.0.20');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250807185014_RenameCanRefundedToCanBeRefunded')
BEGIN
    EXEC sp_rename N'[Products].[CanRefunded]', N'CanBeRefunded', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250807185014_RenameCanRefundedToCanBeRefunded')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250807185014_RenameCanRefundedToCanBeRefunded', N'7.0.20');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250808192348_RemoveMaxDaysToRefundColumn')
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Company]') AND [c].[name] = N'MaxDaysToRefund');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Company] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [Company] DROP COLUMN [MaxDaysToRefund];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250808192348_RemoveMaxDaysToRefundColumn')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250808192348_RemoveMaxDaysToRefundColumn', N'7.0.20');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250818141237_IncreaseColSizeForPhone')
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProductCategory]') AND [c].[name] = N'Name');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [ProductCategory] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [ProductCategory] ALTER COLUMN [Name] varchar(100) NOT NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250818141237_IncreaseColSizeForPhone')
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Company]') AND [c].[name] = N'PhoneNumber');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Company] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [Company] ALTER COLUMN [PhoneNumber] varchar(15) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250818141237_IncreaseColSizeForPhone')
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Company]') AND [c].[name] = N'Email');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Company] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [Company] ALTER COLUMN [Email] varchar(100) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250818141237_IncreaseColSizeForPhone')
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Company]') AND [c].[name] = N'CelphoneNumber');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Company] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [Company] ALTER COLUMN [CelphoneNumber] varchar(15) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250818141237_IncreaseColSizeForPhone')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250818141237_IncreaseColSizeForPhone', N'7.0.20');
END;
GO

COMMIT;
GO

