DECLARE @PortfolioId UNIQUEIDENTIFIER = NULL --'C3EE6C05-514D-4D43-AA26-57E58840F4AC'

SELECT
    p.[Id] AS [Id],
    p.[Name] AS [Name],
    p.[Currency] AS [Currency],
    i.[Id] AS [InstrumentId],
    i.[Name] AS [InstrumentName],
    i.[Currency] AS [InstrumentCurrency],
    i.[Country] AS [InstrumentCountry],
    i.[Mic] AS [InstrumentMic],
    i.[Sector] AS [InstrumentSector],
    i.[Symbol] AS [InstrumentSymbol],
    i.[Isin] AS [InstrumentIsin],
    pi.[PurchaseDate] AS [PurchaseDate],
    pi.[PurchasePrice] AS [PurchasePrice],
    pi.[Quantity] AS [Quantity]
FROM [Portfolios].[Portfolios] p
LEFT JOIN [Portfolios].[PortfolioInvestments] pi
    ON p.[Id] = pi.[PortfolioId]
LEFT JOIN [Portfolios].[Instruments] i
    ON pi.[InstrumentId] = i.[Id]
WHERE (p.[Id] = @PortfolioId OR @PortfolioId IS NULL)
ORDER BY p.[Name], i.[Name] ASC
