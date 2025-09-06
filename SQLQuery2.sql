
IF DB_ID(N'AdditionDb') IS NULL
BEGIN
    CREATE DATABASE AdditionDb;
END
GO

USE AdditionDb;
GO

IF OBJECT_ID(N'dbo.CalculationStorage', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CalculationStorage
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Operand1 FLOAT NOT NULL,
        Operand2 FLOAT NOT NULL,
        Operation NVARCHAR(50) NOT NULL,
        Result   FLOAT NOT NULL
    );
END
GO

SELECT DB_NAME() AS CurrentDb, COUNT(*) AS RowsInTable
FROM dbo.CalculationStorage;
