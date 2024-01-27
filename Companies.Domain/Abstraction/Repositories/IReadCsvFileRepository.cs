using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Abstraction.Repositories
{
    public interface IReadCsvFileRepository
    {
        Task SaveCsvDataToDatabaseAsync(string filePath);
    }
}
