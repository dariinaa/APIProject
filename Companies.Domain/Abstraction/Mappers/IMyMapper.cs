using Companies.Infrastructure.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Abstraction.Mappers
{
    public interface IMyMapper
    {
        Industry DataToIndustry(SqliteDataReader reader);
        Company DataToCompany(SqliteDataReader reader);
        CompanyInsertion MapToCompanyInsertion(CsvRecord csvRecord);
    }
}
