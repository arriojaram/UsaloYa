BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241017163100_AddCreatedByToUser')
BEGIN
    ALTER TABLE [Users] ADD [CreatedBy] int NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241017163100_AddCreatedByToUser')
BEGIN
    ALTER TABLE [Users] ADD [LastUpdateBy] int NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241017163100_AddCreatedByToUser')
BEGIN
    CREATE INDEX [IX_Users_CreatedBy] ON [Users] ([CreatedBy]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241017163100_AddCreatedByToUser')
BEGIN
    CREATE INDEX [IX_Users_LastUpdateBy] ON [Users] ([LastUpdateBy]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241017163100_AddCreatedByToUser')
BEGIN
    ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_Users_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241017163100_AddCreatedByToUser')
BEGIN
    ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_Users_LastUpdateBy] FOREIGN KEY ([LastUpdateBy]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241017163100_AddCreatedByToUser')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241017163100_AddCreatedByToUser', N'7.0.20');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241017180457_AddCreationDate')
BEGIN
    ALTER TABLE [Users] ADD [CreationDate] datetime NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241017180457_AddCreationDate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241017180457_AddCreationDate', N'7.0.20');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241018175218_AddCompanyProps')
BEGIN
    ALTER TABLE [Company] ADD [CreatedBy] int NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241018175218_AddCompanyProps')
BEGIN
    ALTER TABLE [Company] ADD [CreationDate] datetime NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241018175218_AddCompanyProps')
BEGIN
    ALTER TABLE [Company] ADD [LastUpdateBy] int NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241018175218_AddCompanyProps')
BEGIN
    ALTER TABLE [Company] ADD [PaymentsJson] xml NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241018175218_AddCompanyProps')
BEGIN
    CREATE INDEX [IX_Company_CreatedBy] ON [Company] ([CreatedBy]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241018175218_AddCompanyProps')
BEGIN
    CREATE INDEX [IX_Company_LastUpdateBy] ON [Company] ([LastUpdateBy]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241018175218_AddCompanyProps')
BEGIN
    ALTER TABLE [Company] ADD CONSTRAINT [FK_Company_Users_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241018175218_AddCompanyProps')
BEGIN
    ALTER TABLE [Company] ADD CONSTRAINT [FK_Company_Users_LastUpdateBy] FOREIGN KEY ([LastUpdateBy]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241018175218_AddCompanyProps')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241018175218_AddCompanyProps', N'7.0.20');
END;
GO

COMMIT;
GO

