using Companies.Domain.Abstraction;
using Companies.Domain.Abstraction.Repositories;
using Companies.Domain.Abstraction.Services;
using Companies.Infrastructure.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Services
{
    public class IndustryService:IIndustryService
    {
        private readonly IIndustryRepository _industryRepository;

        public IndustryService(IIndustryRepository industryRepository)
        {
            _industryRepository = industryRepository;
        }

        public async Task InsertIndustry(string industryName)
        {
            await _industryRepository.InsertIndustry(industryName);
        }

        public async Task UpdateIndustryName(IndustryInsertion industry)
        {
            await _industryRepository.UpdateIndustryName(industry);
        }

        public async Task DeleteIndustryByName(string industryName)
        {
            await _industryRepository.DeleteIndustryByName(industryName);
        }

        public async Task<IEnumerable<Industry>> GetAllIndustries()
        {
            return await _industryRepository.GetAllIndustries();

        }
    }
}
