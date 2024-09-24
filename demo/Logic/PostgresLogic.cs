using demo.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Polly;
using System.Data.Common;
using System.Net;
//this3
namespace demo.Logic
{
    static class Extensions
    {
        public static void OpenWithRetry(this DbConnection connection) =>
            Policy
                .Handle<NpgsqlException>()
                .WaitAndRetry(new[]
                {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(5)
                })
                .Execute(() => connection.Open());
    }

    public class PostgresLogic : IPostgresLogic
    {
        public string? dbConnectionString { get; set; }
        private readonly ILogger<PostgresLogic> _logger;

        public PostgresLogic(ILogger<PostgresLogic> logger)
        {
            _logger = logger;
            CreateConnectionString();
        }

        public void CreateConnectionString() 
        {
            try
            {
                var db_host = Environment.GetEnvironmentVariable("DB_HOST");
                var db_user = Environment.GetEnvironmentVariable("DB_USER");
                var db_pass = Environment.GetEnvironmentVariable("DB_PASS");
                var db_name = Environment.GetEnvironmentVariable("DB_NAME");
                var db_port = Environment.GetEnvironmentVariable("DB_PORT");
                var pool_size = Environment.GetEnvironmentVariable("POOL_SIZE");
                var pool_timeout = Environment.GetEnvironmentVariable("POOL_TIMEOUT");
                var pool_recycle = Environment.GetEnvironmentVariable("POOL_RECYCLE");

                var connectionString = new NpgsqlConnectionStringBuilder()
                {
                    Host = db_host,
                    Username = db_user,
                    Password = db_pass,
                    Database = db_name,
                    SslMode = SslMode.Prefer,
                    Port = int.Parse(db_port),
                    ConnectionIdleLifetime = string.IsNullOrEmpty(pool_recycle) ? 300 : int.Parse(pool_recycle),
                };
                connectionString.MinPoolSize = 0;
                connectionString.Pooling = true;
                connectionString.Timeout = string.IsNullOrEmpty(pool_timeout) ? 30 : int.Parse(pool_timeout);
                connectionString.MaxPoolSize = string.IsNullOrEmpty(pool_size) ? 5 : int.Parse(pool_size);

                dbConnectionString = connectionString.ConnectionString;
                _logger.Log(LogLevel.Information, "Connection String Created");
            }
            catch(Exception exception)
            {
                _logger.Log(LogLevel.Error, "Failed to create a database connection.");
                _logger.Log(LogLevel.Error, "Message ---\n{0}", exception.Message);
                _logger.Log(LogLevel.Error, "HelpLink ---\n{0}", exception.HelpLink);
                _logger.Log(LogLevel.Error, "Source ---\n{0}", exception.Source);
                _logger.Log(LogLevel.Error, "StackTrace ---\n{0}", exception.StackTrace);
                _logger.Log(LogLevel.Error, "TargetSite ---\n{0}", exception.TargetSite);
            }
        }

        public async Task<IActionResult> HealthCheck()
        {
            ObjectResult result;
            try
            {
                using (DbConnection connection = new NpgsqlConnection(dbConnectionString))
                {
                    connection.OpenWithRetry();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "select 1 from pg_catalog.pg_tables;";
                        int rowsAffected = command.ExecuteNonQuery();
                    }
                }
                _logger.Log(LogLevel.Information, "Postgres Connection Successful");
                result = new ObjectResult("Postgres connection successful.");
                result.StatusCode = 200;
            }
            catch (Exception exception) {
                _logger.Log(LogLevel.Error, $"Message ---\n{exception.Message}");
                _logger.Log(LogLevel.Error, $"StackTrace ---\n{exception.StackTrace}");
                result = new ObjectResult(exception.Message);
                result.StatusCode = 500;
            }

            return result;
        }

    }
}
