using Companies.Domain.Abstraction;
using Companies.Domain.Abstraction.Services;
using Companies.Infrastructure.Models;
using Microsoft.Data.Sqlite;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Services
{
    public class DataBaseContext : IDataBaseContext
    {
        private readonly SqliteConnection _connection;

        public DataBaseContext(string connectionString)
        {
            _connection = new SqliteConnection(connectionString);
            _connection.Open();
        }

        public SqliteConnection GetConnection()
        {
            return _connection;
        }

        public async Task ExequteSqliteCommand(string commandText, SqliteConnection connection,
            Dictionary<string, object> parameters)
        {
            try
            {
                using (var command = new SqliteCommand(commandText, connection))
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
                    }
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "an exception occured while trying to execute the command.");
            }
        }

        public async Task ResetDatabase()
        {
            try
            {
                await ExequteSqliteCommand("DROP TABLE IF EXISTS Companies", _connection, new Dictionary<string, object>());
                await ExequteSqliteCommand("DROP TABLE IF EXISTS Industries", _connection, new Dictionary<string, object>());
                await ExequteSqliteCommand("DROP TABLE IF EXISTS CompanyIndustry", _connection, new Dictionary<string, object>());

                Log.Information("The database tables were dropped.");

                await InitializeDatabase();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while trying to reset the database.");
            }
        }

        public async Task InitializeDatabase()
        {
            try
            {
                await ExequteSqliteCommand("CREATE TABLE IF NOT EXISTS Companies " +
                    "(Id INTEGER, " +
                    "OrganizationId TEXT PRIMARY KEY, Name TEXT, Website TEXT, Country TEXT, " +
                    "Description TEXT, Founded INTEGER, Industry TEXT, Employees INTEGER)",
                    _connection, new Dictionary<string, object>());

                await ExequteSqliteCommand("CREATE TABLE IF NOT EXISTS Industries " +
                    "(Name TEXT PRIMARY KEY)",
                    _connection, new Dictionary<string, object>());

                await ExequteSqliteCommand("CREATE TABLE IF NOT EXISTS CompanyIndustry " +
                    "(OrganizationId TEXT, Name TEXT, " +
                    "FOREIGN KEY (OrganizationId) REFERENCES Companies (OrganizationId), " +
                    "FOREIGN KEY (Name) REFERENCES Industries (Name), " +
                    "PRIMARY KEY (OrganizationId, Name))",
                    _connection, new Dictionary<string, object>());

                Log.Information("The database was initialized.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while trying to initialize the tables.");
            }
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
