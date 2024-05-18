docker compose up -d krake.database.sql krake.redis krake.seq
docker compose up -d krake.database.sql.migrator krake.database.sql.seed.testing
docker compose up -d krake.api