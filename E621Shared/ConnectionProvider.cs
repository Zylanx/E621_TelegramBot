using System;
using System.Data;
using System.Data.SQLite;
using E621Shared.Configs;

namespace E621Shared
{
    public class ConnectionProvider
    {
        private readonly string _databaseName;

        public ConnectionProvider(DatabaseConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.DatabaseName))
            {
                throw new ArgumentNullException(nameof(config.DatabaseName));
            }

            _databaseName = config.DatabaseName;
        }

        public IDbConnection Get()
        {
            //Dapper opens the connection for you if there is need for that.
            return
                new SQLiteConnection($"Data Source={_databaseName}.db;Version=3;");
        }
    }
}