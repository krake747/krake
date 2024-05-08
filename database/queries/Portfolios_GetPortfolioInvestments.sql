DECLARE @PortfolioId UNIQUEIDENTIFIER = NULL --'C3EE6C05-514D-4D43-AA26-57E58840F4AC'

SELECT
    p.[Id] AS [Id],
    p.[Name] AS [Name],
    p.[Currency] AS [Currency],
    s.[Id] AS [InstrumentId],
    s.[Name] AS [InstrumentName],
    s.[Currency] AS [InstrumentCurrency],
    pi.[PurchaseDate] AS [PurchaseDate],
    pi.[PurchasePrice] AS [PurchasePrice],
    pi.[Quantity] AS [Quantity]
FROM [Portfolios].[Portfolios] p
LEFT JOIN [Portfolios].[PortfolioInvestments] pi
    ON p.[Id] = pi.[PortfolioId]
LEFT JOIN [Portfolios].[Instruments] s
    ON pi.[InstrumentId] = s.[Id]
WHERE (p.[Id] = @PortfolioId OR @PortfolioId IS NULL)
ORDER BY p.[Id], s.[Name] ASC
