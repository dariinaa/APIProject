using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Infrastructure.Models.JSON
{
    public class DailyStatistics
    {
        public DateTime GenerationDateTime { get; set; }
        public int CompaniesCreatedToday { get; set; }
        public int IndustriesCreatedToday { get; set; }
        public int CompaniesUpdatedToday { get; set; }
        public int IndustriesUpdatedToday { get; set; }
    }
}
