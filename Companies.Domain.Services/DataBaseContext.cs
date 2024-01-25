using Companies.Domain.Abstraction;
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

        //execute sqlite command
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

        public async Task InitializeDatabase()
        {
            try
            {
                await ExequteSqliteCommand("CREATE TABLE IF NOT EXISTS Companies " +
                    "(Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "OrganizationId TEXT, Name TEXT, Website TEXT, Country TEXT, " +
                    "Description TEXT, Founded INTEGER, Industry TEXT, Employees INTEGER)",
                    _connection, new Dictionary<string, object>());

                await ExequteSqliteCommand("CREATE TABLE IF NOT EXISTS Industries " +
                    "(Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT)",
                    _connection, new Dictionary<string, object>());

                await ExequteSqliteCommand("CREATE TABLE IF NOT EXISTS CompanyIndustry " +
                    "(CompanyId INTEGER, IndustryId INTEGER, " +
                    "FOREIGN KEY (CompanyId) REFERENCES Companies (Id), " +
                    "FOREIGN KEY (IndustryId) REFERENCES Industries (Id), " +
                    "PRIMARY KEY (CompanyId, IndustryId))",
                    _connection, new Dictionary<string, object>());

                Log.Information("The database was initialized.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while trying to initialize the tables.");
            }
        }


        //close and dispose of connection
        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
