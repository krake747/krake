USE [master]
GO

IF DB_ID('KrakeDB') IS NULL
    BEGIN
        EXEC ('CREATE DATABASE [KrakeDB]')
    END