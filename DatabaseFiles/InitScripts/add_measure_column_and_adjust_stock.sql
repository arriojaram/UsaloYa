-- 1. Agregar la columna Measure como INT con valor por defecto
ALTER TABLE Products
ADD Measure INT NOT NULL DEFAULT 1;

-- 2. Cambiar columnas Quantity y UnitsInStock a decimal
ALTER TABLE SaleDetails
ALTER COLUMN Quantity DECIMAL(10, 2) NOT NULL;

ALTER TABLE Products
ALTER COLUMN UnitsInStock DECIMAL(10, 2) NOT NULL;