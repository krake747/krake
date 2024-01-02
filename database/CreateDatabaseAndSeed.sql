CREATE DATABASE KrakeDB;
GO

Use KrakeDB
GO
    
IF OBJECT_ID('Portfolios', 'U') IS NULL 
CREATE Table Portfolios (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(MAX) NOT NULL
);
GO

INSERT INTO Portfolios 
    ([Id], [Name])
Values
    (NEWID(), 'Krake Master'),
    ('B93FC178-7EF6-4754-A341-0349834B22A5', 'Krake ETFs');
GO
