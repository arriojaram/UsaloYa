SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('[AddFolioSale]', 'P') IS NULL
BEGIN
    EXEC('
    CREATE PROCEDURE [AddFolioSale]
        @SaleId INT,
        @CompanyId INT
    AS
    BEGIN
        SET NOCOUNT ON;

        DECLARE @NewFolio INT;

        SELECT @NewFolio = MAX(Folio)
        FROM Sales
        WHERE CompanyId = @CompanyId;

        IF @NewFolio IS NULL
            SET @NewFolio = 1;
        ELSE
            SET @NewFolio = @NewFolio + 1;

        UPDATE Sales
        SET Folio = @NewFolio
        WHERE SaleId = @SaleId;

        SELECT @NewFolio AS NewFolio;
    END
    ')
    PRINT 'Se creó el procedimiento de generación de folio';
    RETURN;
END

PRINT 'Ya existe el procedimiento de generación de folio';
GO
