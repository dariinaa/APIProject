using Companies.Domain.Abstraction;
using Companies.Domain.Abstraction.Mappers;
using Companies.Domain.Abstraction.Repositories;
using Companies.Infrastructure.Entities;
using Microsoft.Data.Sqlite;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Services.Repositories
{
    public class IndustryRepository:IIndustryRepository
    {
        private readonly IDataBaseContext _dataBaseContext;
        private readonly IMyMapper _myMapper;

        public IndustryRepository(IDataBaseContext dataBaseContext, IMyMapper myMapper)
        {
            _dataBaseContext = dataBaseContext;
            _myMapper = myMapper;
        }

        public async Task InsertIndustry(string industryName)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@Name", industryName }
            };

            try
            {
                await _dataBaseContext.ExequteSqliteCommand("INSERT INTO Industries (Name) VALUES (@Name)",
                    _dataBaseContext.GetConnection(), parameters);

                Log.Information($"Industry '{industryName}' inserted into the database.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An exception occurred while trying to insert the industry '{industryName}'.");
            }
        }

        public async Task UpdateIndustryName(string industryId, string newIndustryName)
        {
            var parameters = new Dictionary<string, object>
        {
            { "@IndustryId", industryId },
            { "@NewName", newIndustryName }
        };

            try
            {
                await _dataBaseContext.ExequteSqliteCommand("UPDATE Industries SET Name = @NewName WHERE Id = @IndustryId",
                    _dataBaseContext.GetConnection(), parameters);

                Log.Information($"Industry with ID '{industryId}' updated with new name: '{newIndustryName}'.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An exception occurred while trying to update the industry with ID '{industryId}'.");
            }
        }

        public async Task DeleteIndustryByName(string industryName)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@Name", industryName }
            };

            try
            {
                await _dataBaseContext.ExequteSqliteCommand("DELETE FROM Industries WHERE Name = @Name",
                    _dataBaseContext.GetConnection(), parameters);

                Log.Information($"Industry '{industryName}' deleted from the database.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An exception occurred while trying to delete the industry '{industryName}'.");
            }
        }

        public async Task<IEnumerable<Industry>> GetAllIndustries()
        {
            try
            {
                List<Industry> industries = new List<Industry>();
                using (var command = new SqliteCommand("SELECT * FROM Industries", _dataBaseContext.GetConnection()))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            industries.Add(_myMapper.DataToIndustry(reader));
                        }
                    }
                }
                Log.Information("successfully retrieved all stocks from database.");
                return industries;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while trying to retrieve all industries.");
                return null;
            }
        }
    }
}
