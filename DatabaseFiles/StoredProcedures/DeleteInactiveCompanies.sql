CREATE PROCEDURE DeleteInactiveCompanies
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DiasInactividad INT = 60;

    -- Crear tabla temporal con compañías y días desde último acceso
    SELECT 
        c.CompanyId,
        MAX(u.LastAccess) AS UltimoAcceso,
        c.CreationDate,
        ISNULL(DATEDIFF(DAY, MAX(u.LastAccess), GETDATE()), DATEDIFF(DAY, c.CreationDate, GETDATE())) AS DiasInactiva
    INTO #CompaniasParaEliminar
    FROM Company c
    LEFT JOIN Users u ON u.CompanyId = c.CompanyId
    GROUP BY c.CompanyId, c.CreationDate;

   
    DELETE FROM SaleDetails
    WHERE SaleId IN (
        SELECT SaleId
        FROM Sales
        WHERE CompanyId IN (
            SELECT CompanyId
            FROM #CompaniasParaEliminar
            WHERE DiasInactiva > @DiasInactividad
        )
    );

    DELETE FROM Sales
    WHERE CompanyId IN (
        SELECT CompanyId
        FROM #CompaniasParaEliminar
        WHERE DiasInactiva > @DiasInactividad
    );

    DELETE FROM Customers
    WHERE CompanyId IN (
        SELECT CompanyId
        FROM #CompaniasParaEliminar
        WHERE DiasInactiva > @DiasInactividad
    );

    DELETE FROM Products
    WHERE CompanyId IN (
        SELECT CompanyId
        FROM #CompaniasParaEliminar
        WHERE DiasInactiva > @DiasInactividad
    );

    DELETE FROM ProductCategory
    WHERE CompanyId IN (
        SELECT CompanyId
        FROM #CompaniasParaEliminar
        WHERE DiasInactiva > @DiasInactividad
    );

    DELETE FROM Users
    WHERE CompanyId IN (
        SELECT CompanyId
        FROM #CompaniasParaEliminar
        WHERE DiasInactiva > @DiasInactividad
    );

    DELETE FROM Company
    WHERE CompanyId IN (
        SELECT CompanyId
        FROM #CompaniasParaEliminar
        WHERE DiasInactiva > @DiasInactividad
    );

    DROP TABLE #CompaniasParaEliminar;
END;
