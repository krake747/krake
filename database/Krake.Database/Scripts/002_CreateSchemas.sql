USE [KrakeDB]
GO

IF (SCHEMA_ID('Portfolios') IS NULL)
BEGIN
    EXEC ('CREATE SCHEMA [Portfolios];')
END