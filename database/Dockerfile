FROM mcr.microsoft.com/mssql/server:2022-latest

COPY ./portfolios/exchanges.csv /portfolios/exchanges.csv
COPY ./portfolios/instruments_price_data.csv /portfolios/instruments_price_data.csv

COPY ./CreateDatabaseAndTables.sql /CreateDatabaseAndTables.sql
COPY ./SeedTables.sql /SeedTables.sql
COPY ./SeedFromStagingTables.sql /SeedFromStagingTables.sql

COPY ./wait-and-run.sh /wait-and-run.sh

CMD /wait-and-run.sh