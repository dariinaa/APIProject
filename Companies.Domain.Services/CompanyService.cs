using Companies.Domain.Abstraction;
using Companies.Domain.Abstraction.Repositories;
using Companies.Domain.Abstraction.Services;
using Companies.Domain.Services.Repositories;
using Companies.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Services
{
    public class CompanyService:ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task InsertCompany(CompanyInsertion company)
        {
            await _companyRepository.InsertCompany(company);
        }

        public async Task UpdateCompany(CompanyInsertion updatedCompany)
        {
            await _companyRepository.UpdateCompany(updatedCompany);
        }

        public async Task<IEnumerable<Company>> GetAllCompanies()
        {
            return await _companyRepository.GetAllCompanies();
        }

        public async Task DeleteCompanyByOrganizationId(string organizationId)
        {
            await _companyRepository.DeleteCompanyByOrganizationId(organizationId);
        }
    }
}
