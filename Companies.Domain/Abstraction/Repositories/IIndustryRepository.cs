using Companies.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Abstraction.Repositories
{
    public interface IIndustryRepository
    {
        Task InsertIndustry(string industryName);
        Task DeleteIndustryByName(string industryName);
        Task<IEnumerable<Industry>> GetAllIndustries();
        Task UpdateIndustryName(IndustryInsertion industry);
        bool IsUniqueIndustryName(string industryName, string excludeIndustryId = null);
        Task InsertIndustries(IEnumerable<IndustryInsertion> industries);
    }
}
