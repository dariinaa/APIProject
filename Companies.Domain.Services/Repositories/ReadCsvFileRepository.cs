using Companies.Domain.Abstraction.Mappers;
using Companies.Domain.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Companies.Infrastructure.Models;
using Companies.Domain.Abstraction.Repositories;
using Serilog;
using Companies.Domain.Abstraction.Services;

namespace Companies.Domain.Services.Repositories
{
    public class ReadCsvFileRepository:IReadCsvFileRepository
    {
        private readonly IDataBaseContext _dataBaseContext;
        private readonly IMyMapper _myMapper;
        private readonly ICompanyRepository _companyRepository;

        public ReadCsvFileRepository(IDataBaseContext dataBaseContext, IMyMapper myMapper, ICompanyRepository companyRepository)
        {
            _dataBaseContext = dataBaseContext;
            _myMapper = myMapper;
            _companyRepository = companyRepository;
        }

        public async Task SaveCsvDataToDatabaseAsync(string filePath)
        {
            try
            {
                var companies = new List<CompanyInsertion>();
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    while (csv.Read())
                    {
                        var record = csv.GetRecord<CsvRecord>();

                        if (!_companyRepository.IsUniqueOrganizationIds(record.OrganizationId))
                        {
                            continue;
                        }

                        var company = _myMapper.MapToCompanyInsertion(record);
                        companies.Add(company); 
                    }
                }
                await _companyRepository.InsertCompanies(companies);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An exception occurred while trying to insert the data.");
            }
        }
    }
}
