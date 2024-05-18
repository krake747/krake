USE [KrakeDB]
GO

IF OBJECT_ID('Portfolios.Exchanges', 'U') IS NULL
    BEGIN
        CREATE Table [Portfolios].[Exchanges]
        (
            [ExchangeId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
            [Name] NVARCHAR(100) NULL,
            [Code] NVARCHAR(100) NULL,
            [Mic] NVARCHAR(4) NULL,
            [Country] NVARCHAR(100) NULL,
            [Currency] NVARCHAR(3) NULL,
            [CountryIso2] NVARCHAR(2) NULL,
            [CountryIso3] NVARCHAR(3) NULL
        );

        CREATE UNIQUE INDEX [UQ_Exchanges_Mic]
            ON [Portfolios].[Exchanges] ([Mic]);
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
            [Mic] NVARCHAR(4) NOT NULL,
            [Sector] NVARCHAR(100) NOT NULL,
            [Symbol] NVARCHAR(10) NOT NULL,
            [Isin] NVARCHAR(12) NOT NULL
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

IF OBJECT_ID('Portfolios.InstrumentPrices', 'U') IS NULL
    BEGIN
        CREATE Table [Portfolios].[InstrumentPrices]
        (
            [InstrumentId] UNIQUEIDENTIFIER NOT NULL,
            [Date] DATE NOT NULL,
            [Open] DECIMAL(19, 4) NOT NULL,
            [High] DECIMAL(19, 4) NOT NULL,
            [Low] DECIMAL(19, 4) NOT NULL,
            [Close] DECIMAL(19, 4) NOT NULL,
            [AdjustedClose] DECIMAL(19, 4) NOT NULL,
            [Volume] DECIMAL(19, 4) NOT NULL,
            CONSTRAINT [PK_InstrumentPrices] PRIMARY KEY ([InstrumentId], [Date]),
            CONSTRAINT [FK_Instruments] FOREIGN KEY ([InstrumentId]) REFERENCES [Portfolios].[Instruments]([Id])
        );
    END
GO
