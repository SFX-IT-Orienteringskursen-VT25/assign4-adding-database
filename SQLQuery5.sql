USE AdditionDb;

SELECT OBJECT_ID('dbo.CalculationStorage') AS TableId;

SELECT COUNT(*) AS RowCount FROM dbo.CalculationStorage;

SELECT TOP 50 *
FROM dbo.CalculationStorage
ORDER BY Id DESC;
