using Companies.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Abstraction.Services
{
    public interface ICompanyService
    {
        Task InsertCompany(CompanyInsertion company);
        Task<IEnumerable<Company>> GetAllCompanies();
        Task DeleteCompanyByOrganizationId(string companyName);
        Task UpdateCompany(CompanyInsertion updatedCompany);
        Task<IEnumerable<Company>> GetTop10CompaniesByEmployees();

    }
}
