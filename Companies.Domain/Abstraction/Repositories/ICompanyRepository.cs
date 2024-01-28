using Companies.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Abstraction.Repositories
{
    public interface ICompanyRepository
    {
        Task InsertCompany(CompanyInsertion company);
        Task<IEnumerable<Company>> GetAllCompanies();
        Task DeleteCompanyByOrganizationId(string organizationId);
        Task UpdateCompany(CompanyInsertion updatedCompany);
        bool IsUniqueOrganizationIds(string organizationId, string excludeCompanyId = null);
        Task InsertCompanies(IEnumerable<CompanyInsertion> companies);
        Task<Company> GetCompanyByOrganizationId(string organizationId);
    }
}
