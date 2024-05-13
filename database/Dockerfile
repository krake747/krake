FROM mcr.microsoft.com/mssql/server:2022-latest

COPY ./wait-and-run.sh /wait-and-run.sh

COPY ./portfolios/exchanges.csv /portfolios/exchanges.csv
COPY ./portfolios/instrument_prices.csv /portfolios/instrument_prices.csv

COPY ./CreateDatabaseAndTables.sql /CreateDatabaseAndTables.sql
COPY ./SeedTables.sql /SeedTables.sql
COPY ./SeedFromStagingTables.sql /SeedFromStagingTables.sql


CMD /wait-and-run.sh