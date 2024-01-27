using Companies.Domain.Abstraction;
using Companies.Domain.Abstraction.Mappers;
using Companies.Domain.Abstraction.Repositories;
using Companies.Domain.Abstraction.Services;
using Companies.Domain.Services.BulkClasses;
using Companies.Infrastructure.Models;
using Microsoft.Data.Sqlite;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Services.Repositories
{
    public class IndustryRepository:IIndustryRepository
    {
        private readonly IDataBaseContext _dataBaseContext;
        private readonly IMyMapper _myMapper;
        private readonly HashSet<string> _uniqueIndustryNames;
        private readonly SQLiteBulkInsert _industryBulkInsert;

        public IndustryRepository(IDataBaseContext dataBaseContext, IMyMapper myMapper)
        {
            _dataBaseContext = dataBaseContext;
            _myMapper = myMapper;
            _uniqueIndustryNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            LoadUniqueIndustryNames();

            _industryBulkInsert = new SQLiteBulkInsert("Industries", _dataBaseContext);
            _industryBulkInsert.AddParameter("Name", DbType.String);  
        }

        private void LoadUniqueIndustryNames()
        {
            var existingIndustryNames = GetAllIndustryNames();
            foreach (var name in existingIndustryNames)
            {
                _uniqueIndustryNames.Add(name);
            }
        }

        public bool IsUniqueIndustryName(string industryName, string excludeIndustryId = null)
        {
            return !_uniqueIndustryNames.Contains(industryName) || string.Equals(industryName, excludeIndustryId, StringComparison.OrdinalIgnoreCase);
        }

        public async Task InsertIndustry(string industryName)
        {
            if (!IsUniqueIndustryName(industryName)) return;

                var parameters = new Dictionary<string, object>
                {
                    { "@Name", industryName }
                };

            try
            {
                await _dataBaseContext.ExequteSqliteCommand("INSERT INTO Industries (Name) VALUES @Name)",
                    _dataBaseContext.GetConnection(), parameters);

                Log.Information($"Industry '{industryName}' inserted into the database.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An exception occurred while trying to insert the industry '{industryName}'.");
            }
        }

        public async Task UpdateIndustryName(IndustryInsertion industry)
        {
            if (!IsUniqueIndustryName(industry.Name)) return;

            var parameters = new Dictionary<string, object>
            {
                { "@IndustryId", industry.Id },
                { "@NewName", industry.Name }
            };

            try
            {
                await _dataBaseContext.ExequteSqliteCommand("UPDATE Industries SET Name = @NewName WHERE Id = @IndustryId",
                    _dataBaseContext.GetConnection(), parameters);

                Log.Information($"Industry with ID '{industry.Id}' updated with new name: '{industry.Name}'.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An exception occurred while trying to update the industry with ID '{industry.Id}'.");
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

        //load all industry names from the database
        public IEnumerable<string> GetAllIndustryNames()
        {
            var industryNames = new List<string>();

            try
            {
                using (var command = new SqliteCommand("SELECT Name FROM Industries", _dataBaseContext.GetConnection()))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            industryNames.Add(reader["Name"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while trying to retrieve industry names from the database.");
            }

            return industryNames;
        }

        public async Task InsertIndustries(IEnumerable<IndustryInsertion> industries)
        {
            try
            {
                foreach (var industry in industries)
                {
                    if (!IsUniqueIndustryName(industry.Name))
                    {
                        Log.Information($"Duplicate industry found: '{industry.Name}'. Skipping.");
                        continue;
                    }

                    await _industryBulkInsert.Insert(new object[] 
                    { 
                        industry.Name 
                    });
                    _uniqueIndustryNames.Add(industry.Name);
                    Log.Information($"Industry '{industry.Name}' added for bulk insertion.");
                }

                await _industryBulkInsert.Flush();
                Log.Information("Bulk insert executed successfully.");

            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred during bulk insert of industries.");
            }
        }
    }
}
