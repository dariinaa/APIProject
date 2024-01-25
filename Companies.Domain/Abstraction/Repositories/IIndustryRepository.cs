using Companies.Infrastructure.Entities;
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
        Task UpdateIndustryName(string industryId, string newIndustryName);
    }
}
