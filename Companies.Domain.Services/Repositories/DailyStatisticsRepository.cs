using Companies.Domain.Abstraction.Repositories;
using Companies.Domain.Abstraction.Services;
using Microsoft.Data.Sqlite;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Services.Repositories
{
    public class DailyStatisticsRepository: IDailyStatisticsRepository
    {
        private readonly IDataBaseContext _dataBaseContext;

        public DailyStatisticsRepository(IDataBaseContext dataBaseContext)
        {
            _dataBaseContext = dataBaseContext;
        }

        public async Task<int> GetCompaniesCreatedTodayCount()
        {
            try
            {
                var today = DateTime.Today;
                using (var command = new SqliteCommand("SELECT COUNT(*) FROM Companies WHERE DATE(CreatedAt) = @Today", _dataBaseContext.GetConnection()))
                {
                    command.Parameters.AddWithValue("@Today", today.ToString("yyyy-MM-dd"));

                    var count = Convert.ToInt32(await command.ExecuteScalarAsync());

                    Log.Information($"Successfully retrieved the count of companies created today: {count}");
                    return count;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while trying to retrieve the count of companies created today.");
                return 0;
            }
        }

        public async Task<int> GetIndustriesCreatedTodayCount()
        {
            try
            {
                var today = DateTime.Today;
                using (var command = new SqliteCommand("SELECT COUNT(*) FROM Industries WHERE DATE(CreatedAt) = @Today", _dataBaseContext.GetConnection()))
                {
                    command.Parameters.AddWithValue("@Today", today.ToString("yyyy-MM-dd"));

                    var count = Convert.ToInt32(await command.ExecuteScalarAsync());

                    Log.Information($"Successfully retrieved the count of industries created today: {count}");
                    return count;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while trying to retrieve the count of companies created today.");
                return 0;
            }
        }

        public async Task<int> GetCompaniesUpdatedTodayCount()
        {
            try
            {
                var today = DateTime.Today;
                using (var command = new SqliteCommand("SELECT COUNT(*) FROM Companies WHERE DATE(UpdatedAt) = @Today", _dataBaseContext.GetConnection()))
                {
                    command.Parameters.AddWithValue("@Today", today.ToString("yyyy-MM-dd"));

                    var count = Convert.ToInt32(await command.ExecuteScalarAsync());

                    Log.Information($"Successfully retrieved the count of companies created today: {count}");
                    return count;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while trying to retrieve the count of companies created today.");
                return 0;
            }
        }

        public async Task<int> GetIndustriesUpdatedTodayCount()
        {
            try
            {
                var today = DateTime.Today;
                using (var command = new SqliteCommand("SELECT COUNT(*) FROM Industries WHERE DATE(UpdatedAt) = @Today", _dataBaseContext.GetConnection()))
                {
                    command.Parameters.AddWithValue("@Today", today.ToString("yyyy-MM-dd"));

                    var count = Convert.ToInt32(await command.ExecuteScalarAsync());

                    Log.Information($"Successfully retrieved the count of industries created today: {count}");
                    return count;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while trying to retrieve the count of companies created today.");
                return 0;
            }
        }

    }
}
