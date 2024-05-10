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

echo "Seeding portfolios definition tables..."
/opt/mssql-tools/bin/bcp Portfolios.Exchanges in "/portfolios/exchanges.csv" -c -t ',' -F 2 -S krake.database.sql -U sa -P Admin#123 -d KrakeDB

echo "Seeding portfolios main tables..."
/opt/mssql-tools/bin/sqlcmd -S krake.database.sql -U sa -P Admin#123 -d master -i /SeedTables.sql

echo "Seeding staging tables..."
/opt/mssql-tools/bin/bcp Portfolios.InstrumentsPriceData_Staging in "/portfolios/instruments_price_data.csv" -c -t ',' -F 2 -S krake.database.sql -U sa -P Admin#123 -d KrakeDB

echo "Seed from staging to data tables"
/opt/mssql-tools/bin/sqlcmd -S krake.database.sql -U sa -P Admin#123 -d master -i /SeedFromStagingTables.sql

echo "Backup KrakeDB database"
#RUN chown mssql: /var/opt/mssql/backup -R
#RUN chmod 775 /var/opt/mssql/backup -R
#RUN chmod +s /var/opt/mssql/backup
/opt/mssql-tools/bin/sqlcmd -S krake.database.sql -U sa -P Admin#123 -Q "BACKUP DATABASE [KrakeDB] TO DISK='/var/opt/mssql/backup/backup_KrakeDB.bak' WITH INIT"

echo "Restore KrakeDB database as KrakeDB.Testing"
/opt/mssql-tools/bin/sqlcmd -S krake.database.sql -U sa -P Admin#123 -Q "RESTORE DATABASE [KrakeDB.Testing] FROM DISK='/var/opt/mssql/backup/backup_KrakeDB.bak' WITH MOVE 'KrakeDB' TO '/var/opt/mssql/data/KrakeDB_Testing.mdf', MOVE 'KrakeDB_log' TO '/var/opt/mssql/data/KrakeDB_Testing_log.ldf', REPLACE, RECOVERY"
