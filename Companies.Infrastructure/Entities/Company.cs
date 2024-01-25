﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Infrastructure.Entities
{
    public class Company
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public int Founded { get; set; }

        //public string Industry { get; set; }
        public int Employees { get; set; }
    }
}
