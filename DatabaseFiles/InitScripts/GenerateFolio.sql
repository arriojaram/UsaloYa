BEGIN TRY
    PRINT 'Iniciando proceso de actualización de folios...';

    -- Validación: Verificar si la tabla Ventas existe
    IF OBJECT_ID('Sales', 'U') IS NULL
    BEGIN
        RAISERROR('La tabla Sales no existe en la base de datos.', 16, 1);
        RETURN;
    END

    -- Validación: Verificar que las columnas necesarias existen
    IF NOT EXISTS (
        SELECT 1 
        FROM sys.columns 
        WHERE object_id = OBJECT_ID('Sales') 
          AND name IN ('SaleId', 'CompanyId', 'Folio', 'SaleDate')
    )
    BEGIN
        RAISERROR('Faltan una o más columnas requeridas: SaleId, CompanyId, Folio, SaleDate.', 16, 1);
        RETURN;
    END

    -- Validación: Verificar que existan registros
    IF NOT EXISTS (SELECT 1 FROM Sales)
    BEGIN
        PRINT 'La tabla Ventas está vacía. No se realizarán cambios.';
        RETURN;
    END

    PRINT 'Generando folios numerados...';

    ;WITH FoliosNumerados AS (
        SELECT 
            SaleId,
            CompanyId,
            ROW_NUMBER() OVER (PARTITION BY CompanyId ORDER BY SaleDate, SaleId) AS NuevoFolio
        FROM Sales
    )
    UPDATE V
    SET V.Folio = F.NuevoFolio
    FROM Sales V
    INNER JOIN FoliosNumerados F ON V.SaleId = F.SaleId;

    PRINT 'Actualización de folios finalizada correctamente.';

END TRY
BEGIN CATCH
    PRINT 'Ocurrió un error durante la actualización.';
    PRINT ERROR_MESSAGE();
END CATCH

