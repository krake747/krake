CREATE DATABASE KrakeDB;
GO

Use KrakeDB
GO
    
IF OBJECT_ID('Portfolios', 'U') IS NULL 
CREATE Table Portfolios (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [Name] NVARCHAR(MAX) NOT NULL
);
GO

INSERT INTO Portfolios 
    ([Name])
Values
    ('Krake Master'),
    ('Krake ETFs');
GO
