using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Infrastructure.Entities
{
    public class CompanyIndustry
    {
        public int CompanyId { get; set; }
        public int IndustryId { get; set; }

        public Company Company { get; set; }
        public Industry Industry { get; set; }
    }
}
