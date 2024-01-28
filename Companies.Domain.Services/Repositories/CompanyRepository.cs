using Companies.Domain.Abstraction.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Companies.Infrastructure.Models;
using Companies.Domain.Abstraction.Repositories;
using Microsoft.Data.Sqlite;
using System.Data.Common;
using System.Data;
using Companies.Domain.Abstraction.Services;
using Companies.Domain.Services;

namespace Companies.Domain.Services.Repositories
{
    public class CompanyRepository:ICompanyRepository
    {
        private readonly IDataBaseContext _dataBaseContext;
        private readonly IMyMapper _myMapper;
        private readonly HashSet<string> _uniqueOrganizationIds;
        private readonly IIndustryRepository _industryRepository;
        private readonly ISQLiteBulkInsert _companyBulkInsert;
        private readonly ICompanyIndustryAssociation _companyIndustryAssociation;

        public CompanyRepository(ICompanyIndustryAssociation companyIndustryAssociation, IDataBaseContext dataBaseContext, IMyMapper myMapper, IIndustryRepository industryRepository)
        {
            _companyIndustryAssociation = companyIndustryAssociation;
            _dataBaseContext = dataBaseContext;
            _myMapper = myMapper;
            _industryRepository = industryRepository;
            _uniqueOrganizationIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            LoadUniqueOrganizationIds();

            _companyBulkInsert = new SQLiteBulkInsert( "Companies", _dataBaseContext);
            InitializeCompanyBulkInsertParameters();
        }

        private void InitializeCompanyBulkInsertParameters()
        {
            _companyBulkInsert.AddParameter("Id", DbType.Int32);
            _companyBulkInsert.AddParameter("Name", DbType.String);
            _companyBulkInsert.AddParameter("OrganizationId", DbType.String);
            _companyBulkInsert.AddParameter("Website", DbType.String);
            _companyBulkInsert.AddParameter("Country", DbType.String);
            _companyBulkInsert.AddParameter("Description", DbType.String);
            _companyBulkInsert.AddParameter("Founded", DbType.Int32);
            _companyBulkInsert.AddParameter("Employees", DbType.Int32);
        }

        private void LoadUniqueOrganizationIds()
        {
            var existingOrganizationIds = GetAllOrganizationIds();
            foreach (var organizationId in existingOrganizationIds)
            {
                _uniqueOrganizationIds.Add(organizationId);
            }
        }

        public bool IsUniqueOrganizationIds(string organizationId, string excludeCompanyId = null)
        {
            return !_uniqueOrganizationIds.Contains(organizationId) || string.Equals(organizationId, excludeCompanyId, StringComparison.OrdinalIgnoreCase);
        }

        public async Task InsertCompany(CompanyInsertion company)
        {
            if (!IsUniqueOrganizationIds(company.OrganizationId)) return;
            var parameters = new Dictionary<string, object>
            {
                { "@Id", company.Id },
                { "@OrganizationId", company.OrganizationId },
                { "@Name", company.Name },
                { "@Website", company.Website },
                { "@Country", company.Country },
                { "@Description", company.Description },
                { "@Founded", company.Founded },
                { "@Employees", company.Employees }
            };

            try
            {
                await _dataBaseContext.ExequteSqliteCommand(
                    "INSERT INTO Companies (Id, OrganizationId, Name, Website, Country, Description, Founded, Employees) " +
                    "VALUES (@Id, @OrganizationId, @Name, @Website, @Country, @Description, @Founded, @Employees)",
                    _dataBaseContext.GetConnection(), parameters);

                Log.Information($"Company '{company.Name}' inserted into the database.");

                await _industryRepository.InsertIndustries(company.Industries);

                List<CompanyInsertion> companyList = new List<CompanyInsertion>();
                companyList.Add(company);

                await _companyIndustryAssociation.AssociateCompaniesWithIndustries(companyList);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An exception occurred while trying to insert the company '{company.Name}'.");
            }
        }

        public async Task UpdateCompany(CompanyInsertion updatedCompany)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@Id", updatedCompany.Id },
                { "@OrganizationId", updatedCompany.OrganizationId },
                { "@Name", updatedCompany.Name },
                { "@Website", updatedCompany.Website },
                { "@Country", updatedCompany.Country },
                { "@Description", updatedCompany.Description },
                { "@Founded", updatedCompany.Founded },
                { "@Employees", updatedCompany.Employees }
            };

            try
            {
                await _companyIndustryAssociation.DeleteCompanyByOrganizationIdAssociations(updatedCompany.OrganizationId);

                await _dataBaseContext.ExequteSqliteCommand(
                    "UPDATE Companies SET Id = @Id, Name = @Name, Website = @Website, Country = @Country, " +
                    "Description = @Description, Founded = @Founded, Employees = @Employees " +
                    "WHERE OrganizationId = @OrganizationId",
                    _dataBaseContext.GetConnection(), parameters);

                await _industryRepository.InsertIndustries(updatedCompany.Industries);

                List<CompanyInsertion> companyList = new List<CompanyInsertion>();
                companyList.Add(updatedCompany);
                await _companyIndustryAssociation.AssociateCompaniesWithIndustries(companyList);

                Log.Information($"Company with OrganizationId '{updatedCompany.OrganizationId}' updated in the database.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An exception occurred while trying to update the company with OrganizationId '{updatedCompany.OrganizationId}'.");
            }
        }

        public async Task DeleteCompanyByOrganizationId(string organizationId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@OrganizationId", organizationId }
            };

            try
            {
                await _companyIndustryAssociation.DeleteCompanyByOrganizationIdAssociations(organizationId);

                await _dataBaseContext.ExequteSqliteCommand(
                    "DELETE FROM Companies WHERE OrganizationId = @OrganizationId",
                    _dataBaseContext.GetConnection(), parameters);

                Log.Information($"Company with OrganizationId '{organizationId}' deleted from the database.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An exception occurred while trying to delete the company with OrganizationId '{organizationId}'.");
            }
        }

        public async Task<IEnumerable<Company>> GetAllCompanies()
        {
            try
            {
                List<Company> companies = new List<Company>();
                using (var command = new SqliteCommand("SELECT * FROM Companies LIMIT 150", _dataBaseContext.GetConnection()))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var company = _myMapper.DataToCompany(reader);

                            company.Industries = await GetIndustriesByOrganizationId(company.OrganizationId);

                            companies.Add(company);
                        }
                    }
                }
                Log.Information("Successfully retrieved all companies from the database.");
                return companies;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while trying to retrieve all companies.");
                return null;
            }
        }
        
        public async Task<List<Industry>> GetIndustriesByOrganizationId(string organizationId)
        {
            try
            {
                var industries = new List<Industry>();
                var commandText = "SELECT i.* FROM Industries i " +
                    "JOIN CompanyIndustry ci ON i.Name = ci.Name " +
                    "JOIN Companies c ON ci.OrganizationId = c.OrganizationId " +
                    "WHERE c.OrganizationId = @OrganizationId";

                var parameters = new Dictionary<string, object>
                {
                    { "@OrganizationId", organizationId }
                };

                using (var command = new SqliteCommand(commandText, _dataBaseContext.GetConnection()))
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            industries.Add(new Industry
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            });
                        }
                    }
                }

                return industries;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An exception occurred while trying to retrieve industries for organization ID '{organizationId}'.");
                return null;
            }
        }

        //load all organization ids from database
        public IEnumerable<string> GetAllOrganizationIds()
        {
            var organizationIds = new List<string>();

            try
            {
                using (var command = new SqliteCommand("SELECT OrganizationId FROM Companies", _dataBaseContext.GetConnection()))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            organizationIds.Add(reader["OrganizationId"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while trying to retrieve industry names from the database.");
            }

            return organizationIds;
        }

        public async Task InsertCompanies(IEnumerable<CompanyInsertion> companies)
        {
            try
            {
                foreach (var company in companies)
                {
                    if (!IsUniqueOrganizationIds(company.OrganizationId))
                    {
                        Log.Information($"Duplicate company found: '{company.OrganizationId}'. Skipping.");
                        continue;
                    }

                    await _companyBulkInsert.Insert(new object[]
                    {
                        company.Id,
                        company.Name,
                        company.OrganizationId,
                        company.Website,
                        company.Country,
                        company.Description,
                        company.Founded,
                        company.Employees
                    });

                    _uniqueOrganizationIds.Add(company.OrganizationId);
                    Log.Information($"Company '{company.Name}' added for bulk insertion.");
                }
                await _companyBulkInsert.Flush();
                Log.Information("Bulk insert executed successfully.");

                var allIndustries = companies.SelectMany(company => company.Industries)
                    .Distinct().ToList();

                await _industryRepository.InsertIndustries(allIndustries);

                await _companyIndustryAssociation.AssociateCompaniesWithIndustries(companies);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred during bulk insert of companies.");
            }
        }

        public async Task<Company> GetCompanyByOrganizationId(string organizationId)
        {
            try
            {
                using (var command = new SqliteCommand("SELECT * FROM Companies WHERE OrganizationId = @OrganizationId", _dataBaseContext.GetConnection()))
                {
                    command.Parameters.AddWithValue("@OrganizationId", organizationId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var company = new Company
                            {
                                OrganizationId = reader.GetString(reader.GetOrdinal("OrganizationId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Website = reader.GetString(reader.GetOrdinal("Website")),
                                Country = reader.GetString(reader.GetOrdinal("Country")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                Founded = reader.GetString(reader.GetOrdinal("Founded")),
                                Employees = reader.GetString(reader.GetOrdinal("Employees")),
                                Industries = await GetIndustriesByOrganizationId(organizationId)
                            };

                            return company;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while trying to retrieve a company from the database.");
            }

            return null;
        }

        public async Task<IEnumerable<Company>> GetTop10CompaniesByEmployees()
        {
            try
            {
                List<Company> companies = new List<Company>();
                using (var command = new SqliteCommand("SELECT * FROM Companies ORDER BY Employees DESC LIMIT 10", _dataBaseContext.GetConnection()))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var company = _myMapper.DataToCompany(reader);

                            company.Industries = await GetIndustriesByOrganizationId(company.OrganizationId);

                            companies.Add(company);
                        }
                    }
                }
                Log.Information("Successfully retrieved the top 10 companies by employees from the database.");
                return companies;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while trying to retrieve the top 10 companies by employees.");
                return null;
            }
        }
    }
}
