using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DockerTutorial.Infrastructure.Config;
using Npgsql;
using Serilog;

namespace DockerTutorial.Infrastructure.Database
{
    public class DatabaseCreator
    {
        public static string TableName = "vehicle";

        private static readonly ILogger Logger = Log.ForContext<DatabaseCreator>();

        public static async Task CreateDatabaseAndTableIfNotExists(DatabaseConfig config)
        {
            await TryCreateDatabase(config);
            await TryCreateTable(config);
        }

        private static async Task TryCreateDatabase(DatabaseConfig config)
        {
            var builder = new NpgsqlConnectionStringBuilder(config.ConnectionString);

            var databaseName = builder.Database;

            using (var connection = new NpgsqlConnection(GetPostgressDatabaseConnectionString(builder)))
            {
                await connection.OpenAsync();

                var exists = await new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{databaseName}'", connection)
                    .ExecuteScalarAsync();

                if (exists == null)
                {
                    Logger.Information("Database {databaseName} does not exist. Creating.", databaseName);
                    await new NpgsqlCommand($"CREATE DATABASE {databaseName}", connection).ExecuteNonQueryAsync();
                }
            }
        }

        private static string GetPostgressDatabaseConnectionString(NpgsqlConnectionStringBuilder builder)
        {
            builder.Database = "postgres";
            return builder.ConnectionString;
        }

        private static async Task TryCreateTable(DatabaseConfig config)
        {
            using (var connection = new NpgsqlConnection(config.ConnectionString))
            {
                await connection.OpenAsync();

                var exists = await new NpgsqlCommand($"SELECT * FROM information_schema.tables WHERE table_name = '{TableName}'", connection)
                    .ExecuteScalarAsync();

                if (exists == null)
                {
                    Logger.Information("Table {tableName} does not exist. Creating.", TableName);
                    
                    await new NpgsqlCommand($"CREATE TABLE {TableName} (Brand VARCHAR NOT NULL, Model VARCHAR NOT NULL); ", connection).ExecuteNonQueryAsync();
                }
            }
        }
    }
}
