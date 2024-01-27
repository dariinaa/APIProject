using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Infrastructure.Models
{
    public class CompanyInsertion
    {
        public int Id { get; set; }
        public string OrganizationId { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public int Founded { get; set; }
        public List<IndustryInsertion> Industries { get; set; }
        public int Employees { get; set; }
    }
}
