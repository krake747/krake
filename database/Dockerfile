FROM mcr.microsoft.com/mssql/server:2022-latest

COPY ./portfolios/portfolios_exchanges.csv /portfolios/portfolios_exchanges.csv

COPY ./CreateDatabaseAndTables.sql /CreateDatabaseAndTables.sql
COPY ./SeedTables.sql /SeedTables.sql

COPY ./wait-and-run.sh /wait-and-run.sh

CMD /wait-and-run.sh