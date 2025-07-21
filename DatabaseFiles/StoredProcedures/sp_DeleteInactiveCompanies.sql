IF NOT EXISTS (
    SELECT 1 
    FROM sys.objects 
    WHERE object_id = OBJECT_ID(N'sp_DeleteInactiveCompanies') 
      AND type IN (N'P')
)
BEGIN
    EXEC('
    CREATE PROCEDURE sp_DeleteInactiveCompanies
        @DiasInactividad INT = 60
    AS
    BEGIN
        SET NOCOUNT ON;

        DECLARE @CompanyId INT;
        DECLARE @DaysSinceLastAccess INT;

        DECLARE company_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT CompanyId FROM Company;

        OPEN company_cursor;

        FETCH NEXT FROM company_cursor INTO @CompanyId;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            BEGIN TRY
                SELECT @DaysSinceLastAccess = DATEDIFF(DAY, MAX(LastAccess), GETDATE())
                FROM Users
                WHERE CompanyId = @CompanyId;

                IF (@DaysSinceLastAccess IS NOT NULL AND @DaysSinceLastAccess > @DiasInactividad)
                   OR (
                        @DaysSinceLastAccess IS NULL AND 
                        EXISTS (
                            SELECT 1 FROM Company 
                            WHERE CompanyId = @CompanyId 
                              AND DATEDIFF(DAY, CreationDate, GETDATE()) > @DiasInactividad
                            )
                       )
                BEGIN
                    BEGIN TRANSACTION;

                    DELETE FROM SaleDetails
                    WHERE SaleId IN (
                        SELECT SaleId FROM Sales
                        WHERE UserId IN (SELECT UserId FROM Users WHERE CompanyId = @CompanyId)
                    );

                    DELETE FROM Sales
                    WHERE UserId IN (SELECT UserId FROM Users WHERE CompanyId = @CompanyId);

                    DELETE FROM Customers
                    WHERE CompanyId = @CompanyId;

                    DELETE FROM Products
                    WHERE CompanyId = @CompanyId;

                    DELETE FROM ProductCategory
                    WHERE CompanyId = @CompanyId;

                    DELETE FROM Users
                    WHERE CompanyId = @CompanyId;

                    DELETE FROM Company
                    WHERE CompanyId = @CompanyId;

                    COMMIT TRANSACTION;

                    PRINT ''Empresa eliminada: '' + CAST(@CompanyId AS VARCHAR);
                END
            END TRY
            BEGIN CATCH
                IF @@TRANCOUNT > 0
                    ROLLBACK TRANSACTION;

                PRINT ''Error al eliminar la empresa: '' + CAST(@CompanyId AS VARCHAR);
                PRINT ERROR_MESSAGE();
            END CATCH;

            FETCH NEXT FROM company_cursor INTO @CompanyId;
        END

        CLOSE company_cursor;
        DEALLOCATE company_cursor;
    END
    ')
END