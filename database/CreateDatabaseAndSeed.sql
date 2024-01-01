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
    ([Id], Name)
Values
    (NEWID(), 'Krake Master'),
    (NEWID(), 'Krake ETFs');
GO
