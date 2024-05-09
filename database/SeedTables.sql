Use [KrakeDB]
GO

IF NOT EXISTS (SELECT * FROM [Portfolios].[Exchanges])
    BEGIN
        BULK INSERT [Portfolios].[Exchanges]
        FROM 'C:/Users/kraem/krake/database/portfolios/portfolios_exchanges.csv'
        WITH
        (
            FORMAT = 'CSV',
            FIELDTERMINATOR = ',',
            FIRSTROW = 2
        );
    END
GO

IF NOT EXISTS (SELECT * FROM [Portfolios].[Instruments])
    BEGIN
        INSERT INTO [Portfolios].[Instruments] ([Id], [Name], [Currency], [Country], [Mic], [Sector], [Symbol], [Isin])
        VALUES
            ('15B3C4A2-4053-4C11-AE8F-DE97909CB507', 'Adidas AG', 'EUR', 'DE', 'XETR', 'Consumer Cyclical', 'ADS', 'DE000A1EWWW0'),
            ('9BA41F49-73D9-46B5-95C7-2EAB09A13806', 'Siemens AG', 'EUR', 'DE', 'XETR', 'Industrials', 'SIE', 'DE0007236101'),
            ('84815B16-EEB8-4784-A7CA-2211FC712675', 'BASF SE', 'EUR', 'DE', 'XETR', 'Basic Materials', 'BAS', 'DE000BASF111'),
            ('7B2ED9D3-5735-42F7-8814-F4CCB1B585BA', 'Microsoft Corp', 'USD', 'US', 'XNAS', 'Technology', 'MSFT', 'US5949181045'),
            ('863DC0E1-2F64-4866-A1D8-9F62357E67DC', 'Visa Inc.', 'USD', 'US', 'XNAS', 'Financial Services', 'V', 'US92826C8394'),
            ('BAB9E42C-BB30-4022-AEBA-C78344F2ADA6', 'Coca-Cola Company', 'USD', 'US', 'NYSE', 'Technology', 'KO', 'US1912161007');
    END
GO

IF NOT EXISTS (SELECT * FROM [Portfolios].[Portfolios])
    BEGIN
        INSERT INTO [Portfolios].[Portfolios] ([Id], [Name], [Currency])
        VALUES
            ('C3EE6C05-514D-4D43-AA26-57E58840F4AC', 'Krake Master', 'EUR'),
            ('B93FC178-7EF6-4754-A341-0349834B22A5', 'Krake US Equities', 'USD'),
            ('0F746DBF-3381-4235-ABC6-03420030F38B', 'Krake ETFs', 'EUR');
    END
GO

IF NOT EXISTS (SELECT * FROM [Portfolios].[PortfolioInvestments])
    BEGIN
        INSERT INTO [Portfolios].[PortfolioInvestments] ([PortfolioId], [InstrumentId], [PurchaseDate], [PurchasePrice], [Quantity])
        VALUES
            ('C3EE6C05-514D-4D43-AA26-57E58840F4AC', '15B3C4A2-4053-4C11-AE8F-DE97909CB507', '20240425', 226.40, 65),
            ('C3EE6C05-514D-4D43-AA26-57E58840F4AC', '15B3C4A2-4053-4C11-AE8F-DE97909CB507', '20240503', 225.00, 35),
            ('C3EE6C05-514D-4D43-AA26-57E58840F4AC', '9BA41F49-73D9-46B5-95C7-2EAB09A13806', '20240415', 175.90, 80),
            ('C3EE6C05-514D-4D43-AA26-57E58840F4AC', '9BA41F49-73D9-46B5-95C7-2EAB09A13806', '20240503', 177.34, 20),
            ('C3EE6C05-514D-4D43-AA26-57E58840F4AC', '84815B16-EEB8-4784-A7CA-2211FC712675', '20240503', 49.09, 100),
            ('B93FC178-7EF6-4754-A341-0349834B22A5', '7B2ED9D3-5735-42F7-8814-F4CCB1B585BA', '20240425', 406.66, 10),
            ('B93FC178-7EF6-4754-A341-0349834B22A5', '863DC0E1-2F64-4866-A1D8-9F62357E67DC', '20240424', 275.02, 25),
            ('B93FC178-7EF6-4754-A341-0349834B22A5', '863DC0E1-2F64-4866-A1D8-9F62357E67DC', '20240415', 268.49, 15),
            ('B93FC178-7EF6-4754-A341-0349834B22A5', 'BAB9E42C-BB30-4022-AEBA-C78344F2ADA6', '20240411', 59.05, 10),
            ('B93FC178-7EF6-4754-A341-0349834B22A5', 'BAB9E42C-BB30-4022-AEBA-C78344F2ADA6', '20240503', 62.17, 35);
    END
GO