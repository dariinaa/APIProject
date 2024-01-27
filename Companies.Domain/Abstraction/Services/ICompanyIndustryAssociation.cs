using Companies.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Abstraction.Services
{
    public interface ICompanyIndustryAssociation
    {
        Task AssociateCompanyWithIndustry(string organizationId, string industryName);
        Task AssociateCompaniesWithIndustries(IEnumerable<CompanyInsertion> companies);
        Task DeleteCompanyByOrganizationIdAssociations(string organizationId);
        Task DeleteIndustryByNameAssociations(string industryName);
    }
}
