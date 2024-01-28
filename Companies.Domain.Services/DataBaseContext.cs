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

        public async Task ExequteSqliteCommandNoParameters(string commandText, SqliteConnection connection)
        {
            try
            {
                using (var command = new SqliteCommand(commandText, connection))
                {
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
                await ExequteSqliteCommandNoParameters("DROP TABLE IF EXISTS Companies", _connection);
                await ExequteSqliteCommandNoParameters("DROP TABLE IF EXISTS Industries", _connection);
                await ExequteSqliteCommandNoParameters("DROP TABLE IF EXISTS CompanyIndustry", _connection);
                await ExequteSqliteCommandNoParameters("DROP TABLE IF EXISTS Users", _connection);

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
                await ExequteSqliteCommandNoParameters("CREATE TABLE IF NOT EXISTS Companies " +
                    "(Id INTEGER, " +
                    "OrganizationId TEXT PRIMARY KEY, Name TEXT, Website TEXT, Country TEXT, " +
                    "Description TEXT, Founded INTEGER, Industry TEXT, Employees INTEGER, " +
                    "CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP, " +
                    "UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP)",
                    _connection);

                await ExequteSqliteCommandNoParameters("CREATE TABLE IF NOT EXISTS Industries " +
                    "(Name TEXT PRIMARY KEY, " +
                    "CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP, " +
                    "UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP)",
                    _connection);

                await ExequteSqliteCommandNoParameters("CREATE TABLE IF NOT EXISTS CompanyIndustry " +
                    "(OrganizationId TEXT, Name TEXT, " +
                    "CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP, " +
                    "UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP, " +
                    "FOREIGN KEY (OrganizationId) REFERENCES Companies (OrganizationId), " +
                    "FOREIGN KEY (Name) REFERENCES Industries (Name), " +
                    "PRIMARY KEY (OrganizationId, Name))",
                    _connection);

                await ExequteSqliteCommandNoParameters("CREATE TABLE IF NOT EXISTS Users " +
                    "(Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "Username TEXT, EmailAddress TEXT, Password TEXT, " +
                    "GivenName TEXT, Surname TEXT, Role TEXT, " +
                    "CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP, " +
                    "UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP)",
                    _connection);

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
