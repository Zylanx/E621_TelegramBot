using System;
using System.Data;
using System.Data.SQLite;
using System.Transactions;
using E621Shared.Configs;
using Microsoft.Extensions.Logging;

namespace E621Shared
{
    public class ConnectionProvider : IDisposable
    {
        private readonly string _databaseName;
        private readonly SQLiteConnection _connection;
        private readonly ILogger<ConnectionProvider> _logger;

        public ConnectionProvider(DatabaseConfig config, ILogger<ConnectionProvider> logger)
        {
            this._logger = logger;
            if (string.IsNullOrWhiteSpace(config.DatabaseName))
            {
                throw new ArgumentNullException(nameof(config.DatabaseName));
            }

            _databaseName = config.DatabaseName;
            _connection = new SQLiteConnection($"Data Source={_databaseName}.db;Version=3;Pooling=True;Max Pool Size=100;");
            _logger.LogDebug("Constructed new database connection");
            
        }

        public void Dispose()
        {
            //IoC will call this for us I think
            _connection.Dispose();
        }

        public IDbConnection Get()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            return _connection;
        }
    }
}