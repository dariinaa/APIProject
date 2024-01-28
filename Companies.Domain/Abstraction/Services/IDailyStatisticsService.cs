using Companies.Infrastructure.Models.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Abstraction.Services
{
    public interface IDailyStatisticsService
    {
        Task<DailyStatistics> GetDailyStatistics();
        Task SaveDailyStatisticsToJsonFile();
    }
}
