using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Infrastructure.Models
{
    public class CsvRecord
    {
        [Name("Index")]
        public int Index { get; set; }

        [Name("Organization Id")]
        public string OrganizationId { get; set; }

        [Name("Name")]
        public string Name { get; set; }

        [Name("Website")]
        public string Website { get; set; }

        [Name("Country")]
        public string Country { get; set; }

        [Name("Description")]
        public string Description { get; set; }

        [Name("Founded")]
        public string Founded { get; set; }

        [Name("Industry")]
        public string Industry { get; set; }

        [Name("Number of employees")]
        public string Employees { get; set; }
    }
}
