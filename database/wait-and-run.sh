#!/bin/bash

# Wait for SQL Server to be ready
echo "Waiting for SQL Server to be ready..."
for i in {1..50};
do
    /opt/mssql-tools/bin/sqlcmd -S krake.database.sql -U sa -P Admin#123 -Q "SELECT 1" > /dev/null 2>&1
    if [ $? -eq 0 ]
    then
        echo "SQL Server is ready."
        break
    else
        echo "Not ready yet..."
        sleep 1
    fi
done

# Run SQL scripts
echo "Creating KrakeDB and tables..."
/opt/mssql-tools/bin/sqlcmd -S krake.database.sql -U sa -P Admin#123 -d master -i /CreateDatabaseAndTables.sql

echo "Seeding definition tables..."
/opt/mssql-tools/bin/bcp Portfolios.Exchanges in "/portfolios/portfolios_exchanges.csv" -c -t ',' -S krake.database.sql -U sa -P Admin#123 -d KrakeDB

echo "Seeding tables..."
/opt/mssql-tools/bin/sqlcmd -S krake.database.sql -U sa -P Admin#123 -d master -i /SeedTables.sql