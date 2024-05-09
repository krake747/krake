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

echo "Backup KrakeDB database"
#CMD chown mssql: /var/opt/mssql/backup -R
#CMD chmod 775 /var/opt/mssql/backup -R
#CMD chmod +s /var/opt/mssql/backup
/opt/mssql-tools/bin/sqlcmd -S krake.database.sql -U sa -P Admin#123 -Q "BACKUP DATABASE [KrakeDB] TO DISK='/var/opt/mssql/backup/backup_KrakeDB.bak' WITH INIT"

echo "Restore KrakeDB database as KrakeDB.Testing"
/opt/mssql-tools/bin/sqlcmd -S krake.database.sql -U sa -P Admin#123 -Q "RESTORE DATABASE [KrakeDB.Testing] FROM DISK='/var/opt/mssql/backup/backup_KrakeDB.bak' WITH MOVE 'KrakeDB' TO '/var/opt/mssql/data/KrakeDB_Testing.mdf', MOVE 'KrakeDB_log' TO '/var/opt/mssql/data/KrakeDB_Testing_log.ldf', REPLACE, RECOVERY"
