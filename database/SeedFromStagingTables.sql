Use [KrakeDB]
GO

IF NOT EXISTS (SELECT * FROM [Portfolios].[InstrumentPrices])
    BEGIN
        INSERT INTO [Portfolios].[InstrumentPrices]
        SELECT
            CONVERT(UNIQUEIDENTIFIER, TRIM([InstrumentId])),
            CAST(TRIM([Date]) AS DATE),
            CONVERT(DECIMAL(19, 4), TRIM([Open])),
            CONVERT(DECIMAL(19, 4), TRIM([High])),
            CONVERT(DECIMAL(19, 4), TRIM([Low])),
            CONVERT(DECIMAL(19, 4), TRIM([Close])),
            CONVERT(DECIMAL(19, 4), TRIM([AdjustedClose])),
            CONVERT(DECIMAL(19, 4), TRIM(REPLACE(REPLACE([Volume], CHAR(13), ''), CHAR(10), '')))
        FROM [Portfolios].[InstrumentPrices_Staging];
    END
GO

DROP TABLE [Portfolios].[InstrumentPrices_Staging];
GO