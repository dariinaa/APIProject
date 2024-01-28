using Companies.Domain.Abstraction.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentScheduler;

namespace Companies.Domain.Services.Scheduler
{
    public class MyRegistry:Registry
    {
        private IDailyStatisticsService _dailyStatisticsService { get; set; }
        public MyRegistry(IDailyStatisticsService dailyStatisticsService)
        {
            _dailyStatisticsService = dailyStatisticsService;
            Schedule(() => new DailyJob(_dailyStatisticsService)).ToRunNow().AndEvery(1).Days().At(0, 0);
        }
    }
}
