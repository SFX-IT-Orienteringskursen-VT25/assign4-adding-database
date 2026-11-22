IF DB_ID('AdditionDb') IS NULL
BEGIN
    CREATE DATABASE AdditionDb;
END
GO

USE AdditionDb;
GO

IF OBJECT_ID('dbo.Numbers', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Numbers
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Value INT NOT NULL
    );
END
GO