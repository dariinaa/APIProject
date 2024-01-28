using Companies.Domain.Abstraction.Repositories;
using Companies.Domain.Abstraction.Services;
using FluentScheduler;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Services.Scheduler
{
    public class DailyJob:IJob
    {
        private readonly IDailyStatisticsService _dailyStatisticsService;
        public DailyJob(IDailyStatisticsService dailyStatisticsService)
        {
            _dailyStatisticsService = dailyStatisticsService;
        }
        public void Execute()
        {
            try
            {
                _dailyStatisticsService.SaveDailyStatisticsToJsonFile().GetAwaiter().GetResult();
                Log.Information("saved daily statistic file.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "saving daily statistic file was unsuccessful.");
            }
        }
    }
}
