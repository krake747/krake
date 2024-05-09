USE [master]
GO

CREATE DATABASE [KrakeDB];
GO

Use [KrakeDB]
GO

CREATE SCHEMA [Portfolios];
GO

IF OBJECT_ID('Portfolios.Exchanges', 'U') IS NULL
    BEGIN
        CREATE Table [Portfolios].[Exchanges]
        (
            [ExchangeId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
            [Name] NVARCHAR(100) NULL,
            [Code] NVARCHAR(100) NULL,
            [MIC] NVARCHAR(4) NULL,
            [Country] NVARCHAR(100) NULL,
            [Currency] NVARCHAR(3) NULL,
            [CountryIso2] NVARCHAR(2) NULL,
            [CountryIso3] NVARCHAR(3) NULL
        );

        CREATE UNIQUE INDEX [UQ_Exchanges_MIC]
        ON [Portfolios].[Exchanges] ([MIC]);
    END
GO

IF OBJECT_ID('Portfolios.Instruments', 'U') IS NULL
    BEGIN
        CREATE Table [Portfolios].[Instruments]
        (
            [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
            [Name] NVARCHAR(100) NOT NULL,
            [Currency] NVARCHAR(3) NOT NULL,
            [Country] NVARCHAR(2) NOT NULL,
            [MIC] NVARCHAR(4) NOT NULL,
            [Sector] NVARCHAR(100) NOT NULL,
            [Symbol] NVARCHAR(10) NOT NULL,
            [ISIN] NVARCHAR(12) NOT NULL
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

IF OBJECT_ID('Portfolios.PortfolioInvestments', 'U') IS NULL
    BEGIN
        CREATE Table [Portfolios].[PortfolioInvestments]
        (
            [PortfolioId] UNIQUEIDENTIFIER NOT NULL,
            [InstrumentId] UNIQUEIDENTIFIER NOT NULL,
            [PurchaseDate] DATE NOT NULL,
            [PurchasePrice] DECIMAL(19, 4) NOT NULL,
            [Quantity] DECIMAL(19, 4) NOT NULL
        );
    END
GO
