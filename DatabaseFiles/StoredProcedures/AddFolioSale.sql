
CREATE OR ALTER PROCEDURE [AddFolioSale]
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

