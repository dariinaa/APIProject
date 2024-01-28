using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Abstraction.Repositories
{
    public interface IDailyStatisticsRepository
    {
        Task<int> GetCompaniesCreatedTodayCount();
        Task<int> GetIndustriesCreatedTodayCount();
        Task<int> GetCompaniesUpdatedTodayCount();
        Task<int> GetIndustriesUpdatedTodayCount();

    }
}
