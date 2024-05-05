CREATE DATABASE KrakeDB;
GO

Use KrakeDB
GO

CREATE SCHEMA Portfolios;
GO

IF OBJECT_ID('Portfolios.Instruments', 'U') IS NULL
    BEGIN
        CREATE Table [Portfolios].[Instruments]
        (
            [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
            [Name] NVARCHAR(100) NOT NULL,
            [Currency] NVARCHAR(3) NOT NULL
        );
    END
GO

IF OBJECT_ID('Portfolios.Portfolios', 'U') IS NULL
    BEGIN
        CREATE Table [Portfolios].[Portfolios]
        (
            [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
            [Name] NVARCHAR(100) NOT NULL,
            [Currency] NVARCHAR(3) NOT NULL
        );
    END
GO

IF OBJECT_ID('Portfolios.Investments', 'U') IS NULL
    BEGIN
        CREATE Table [Portfolios].[Investments]
        (
            [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
            [PortfolioId] UNIQUEIDENTIFIER NOT NULL,
            [InstrumentsId] UNIQUEIDENTIFIER NOT NULL,
            [PurchaseDate] DATE NOT NULL,
            [PurchasePrice] DECIMAL(19, 4) NOT NULL,
            [Quantity] DECIMAL(19, 4) NOT NULL
        );
    END
GO

INSERT INTO [Portfolios].[Instruments] ([Id], [Name], [Currency])
VALUES
    ('15B3C4A2-4053-4C11-AE8F-DE97909CB507', 'Adidas AG', 'EUR'),
    ('9BA41F49-73D9-46B5-95C7-2EAB09A13806', 'Siemens AG', 'EUR'),
    ('84815B16-EEB8-4784-A7CA-2211FC712675', 'BASF SE', 'EUR');
GO

INSERT INTO [Portfolios].[Portfolios] ([Id], [Name], [Currency])
VALUES
    ('C3EE6C05-514D-4D43-AA26-57E58840F4AC', 'Krake Master', 'EUR'),
    ('B93FC178-7EF6-4754-A341-0349834B22A5', 'Krake ETFs', 'EUR');
GO

INSERT INTO [Portfolios].[Investments] ([Id], [PortfolioId], [InstrumentsId], [PurchaseDate], [PurchasePrice], [Quantity])
VALUES
    ('2E522CF4-A0D7-411F-B0F1-501F619C0793', 'C3EE6C05-514D-4D43-AA26-57E58840F4AC', '15B3C4A2-4053-4C11-AE8F-DE97909CB507', '20240503', 177.34, 100),
    ('AFDA0743-5070-4DE0-AC3E-209069E4A6B8', 'C3EE6C05-514D-4D43-AA26-57E58840F4AC', '9BA41F49-73D9-46B5-95C7-2EAB09A13806', '20240503', 225.00, 100),
    ('9CD885B3-B150-46F3-832B-CF29F0056315', 'C3EE6C05-514D-4D43-AA26-57E58840F4AC', '84815B16-EEB8-4784-A7CA-2211FC712675', '20240503', 49.09, 100);
GO