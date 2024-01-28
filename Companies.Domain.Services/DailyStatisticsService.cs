using Companies.Domain.Abstraction.Repositories;
using Companies.Domain.Abstraction.Services;
using Companies.Domain.Settings;
using Companies.Infrastructure.Models.JSON;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Companies.Domain.Services
{
    public class DailyStatisticsService:IDailyStatisticsService
    {
        private readonly IDailyStatisticsRepository _dailyStatisticsRepository;
        private readonly string _jsonFolderPath;

        public DailyStatisticsService(IOptions<JSONSettings> jsonSettings, IDailyStatisticsRepository dailyStatisticsRepository)
        {
            _jsonFolderPath = jsonSettings.Value.JSONFolderPath;
            _dailyStatisticsRepository = dailyStatisticsRepository;
        }

        public async Task<DailyStatistics> GetDailyStatistics()
        {
            try
            {
                var companiesCreatedToday = await _dailyStatisticsRepository.GetCompaniesCreatedTodayCount();
                var industriesCreatedToday = await _dailyStatisticsRepository.GetIndustriesCreatedTodayCount();
                var companiesUpdatedToday = await _dailyStatisticsRepository.GetCompaniesUpdatedTodayCount();
                var industriesUpdatedToday = await _dailyStatisticsRepository.GetIndustriesUpdatedTodayCount();

                var dailyStatistics = new DailyStatistics
                {
                    GenerationDateTime = DateTime.Now,
                    CompaniesCreatedToday = companiesCreatedToday,
                    IndustriesCreatedToday = industriesCreatedToday,
                    CompaniesUpdatedToday = companiesUpdatedToday,
                    IndustriesUpdatedToday = industriesUpdatedToday
                };

                return dailyStatistics;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while trying to retrieve daily statistics.");
                return null;
            }
        }

        public async Task SaveDailyStatisticsToJsonFile()
        {
            try
            {
                var dailyStatistics = await GetDailyStatistics();
                var json = JsonSerializer.Serialize(dailyStatistics);
                var filePath = $"{_jsonFolderPath}{DateTime.Now:yyyyMMddHHmmss}.JSON";

                await File.WriteAllTextAsync(filePath, json);

                Log.Information($"Daily statistics saved to JSON file: {filePath}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while trying to save daily statistics to a JSON file.");
            }
        }
    }
}
