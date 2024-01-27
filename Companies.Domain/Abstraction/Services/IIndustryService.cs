using Companies.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Abstraction.Services
{
    public interface IIndustryService
    {
        Task InsertIndustry(string indusryName);
        Task UpdateIndustryName(IndustryInsertion industry);
        Task DeleteIndustryByName(string industryName);
        Task<IEnumerable<Industry>> GetAllIndustries();
    }
}
