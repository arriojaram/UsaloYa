-- Agrega un índice temporal si no hay uno en CompanyId
-- y asegura que Folio permite valores NULL o duplicados inicialmente

WITH FoliosNumerados AS (
    SELECT 
        SaleId,
        CompanyId,
        ROW_NUMBER() OVER (PARTITION BY CompanyId ORDER BY SaleDate, SaleId) AS NuevoFolio
    FROM Ventas
)
UPDATE V
SET Folio = F.NuevoFolio
FROM Sales V
JOIN FoliosNumerados F ON V.SaleId = F.SaleId;