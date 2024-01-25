using Companies.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Abstraction
{
    public interface IIndustryService
    {
        Task InsertIndustry(string industryName);
        Task UpdateIndustryName(string industryId, string newIndustryName);
        Task DeleteIndustryByName(string industryName);
        Task<IEnumerable<Industry>> GetAllIndustries();
    }
}
