using Companies.Domain.Abstraction.Mappers;
using Companies.Domain.Abstraction.Repositories;
using Companies.Domain.Abstraction.Services;
using Companies.Infrastructure.Models;
using Companies.Infrastructure.Models.Auth;
using Microsoft.Data.Sqlite;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Services.Repositories.Auth
{
    public class UserRepository:IUserRepository
    {
        private readonly IDataBaseContext _dataBaseContext;
        private readonly IMyMapper _myMapper;

        public UserRepository(IMyMapper myMapper, IDataBaseContext dataBaseContext)
        {
            _myMapper = myMapper;
            _dataBaseContext = dataBaseContext;
        }

        public async Task<User> GetUserByUsername(string username)
        {
            try
            {
                using (var command = new SqliteCommand("SELECT * FROM Users WHERE Username = @Username", _dataBaseContext.GetConnection()))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Log.Information("Successfully found user.");
                            return _myMapper.MapUserFromDataReader(reader);
                        }
                    }
                }
                Log.Information("User not found.");
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while trying to get user.");
                return null;
            }
        }

        public async Task<bool> AddUser(Register newUser)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@Username", newUser.Username },
                    { "@EmailAddress", newUser.EmailAddress },
                    { "@Password", newUser.Password },
                    { "@GivenName", newUser.GivenName },
                    { "@Surname", newUser.Surname },
                    { "@Role", "Standart" }
                };

                await _dataBaseContext.ExequteSqliteCommand("INSERT INTO Users (Username, EmailAddress, Password, GivenName, Surname, Role) VALUES (@Username, @EmailAddress, @Password, @GivenName, @Surname, @Role)",
                    _dataBaseContext.GetConnection(), parameters);

                Log.Information("User added successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while trying to add a new user.");
                return false;
            }
        }
    }
}
