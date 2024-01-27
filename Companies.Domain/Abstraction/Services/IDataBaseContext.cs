using Companies.Infrastructure.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Abstraction.Services
{
    public interface IDataBaseContext
    {
        SqliteConnection GetConnection();
        Task ResetDatabase();
        Task InitializeDatabase();
        Task ExequteSqliteCommand(string commandText, SqliteConnection connection, Dictionary<string, object> parameters);
    }
}
