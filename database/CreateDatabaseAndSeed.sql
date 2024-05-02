CREATE DATABASE KrakeDB;
GO

Use KrakeDB
GO

CREATE SCHEMA Portfolios;
GO

IF OBJECT_ID('Portfolios.Portfolios', 'U') IS NULL
    BEGIN
        CREATE Table [Portfolios].[Portfolios] (
            [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
            [Name] NVARCHAR(100) NOT NULL
        );
    END
GO

INSERT INTO [Portfolios].[Portfolios]
    ([Id], [Name])
Values
    ('C3EE6C05-514D-4D43-AA26-57E58840F4AC', 'Krake Master'),
    ('B93FC178-7EF6-4754-A341-0349834B22A5', 'Krake ETFs');
GO
