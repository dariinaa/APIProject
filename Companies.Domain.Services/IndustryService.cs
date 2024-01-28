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
            if (string.IsNullOrEmpty(industryName))
            {
                throw new ArgumentException("IndustryName cannot be null or empty.", nameof(industryName));
            }
            await _industryRepository.InsertIndustry(industryName);
        }

        public async Task UpdateIndustryName(string currentName, string newName)
        {
            if (string.IsNullOrEmpty(currentName))
            {
                throw new ArgumentException("CurrentName cannot be null or empty.", nameof(currentName));
            }

            if (string.IsNullOrEmpty(newName))
            {
                throw new ArgumentException("NewName cannot be null or empty.", nameof(newName));
            }
            await _industryRepository.UpdateIndustryName(currentName, newName);
        }

        public async Task DeleteIndustryByName(string industryName)
        {
            if (string.IsNullOrEmpty(industryName))
            {
                throw new ArgumentException("IndustryName cannot be null or empty.", nameof(industryName));
            }
            await _industryRepository.DeleteIndustryByName(industryName);
        }

        public async Task<IEnumerable<Industry>> GetAllIndustries()
        {
            return await _industryRepository.GetAllIndustries();
        }
    }
}
