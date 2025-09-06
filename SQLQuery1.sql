CREATE DATABASE AdditionDb;
GO

USE AdditionDb;
GO


CREATE TABLE CalculationStorage (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Operand1 FLOAT NOT NULL,
    Operand2 FLOAT NOT NULL,
    Operation NVARCHAR(10) NOT NULL,
    Result FLOAT NOT NULL
);
GO
