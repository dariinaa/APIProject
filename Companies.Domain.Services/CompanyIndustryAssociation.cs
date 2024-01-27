using Companies.Domain.Abstraction.Services;
using Companies.Infrastructure.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Services
{
    public class CompanyIndustryAssociation:ICompanyIndustryAssociation
    {
        private readonly IDataBaseContext _dataBaseContext;
        private readonly ISQLiteBulkInsert _pivotBulkInsert;

        public CompanyIndustryAssociation(IDataBaseContext dataBaseContext)
        {
            _dataBaseContext = dataBaseContext;
            _pivotBulkInsert = new SQLiteBulkInsert("CompanyIndustry", _dataBaseContext);
            InitializeCompanyIndustryBulkInsertParameters();
        }

        private void InitializeCompanyIndustryBulkInsertParameters()
        {
            _pivotBulkInsert.AddParameter("OrganizationId", DbType.String);
            _pivotBulkInsert.AddParameter("Name", DbType.Int32);

        }

        public async Task AssociateCompanyWithIndustry(string organizationId, string industryName)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@OrganizationId", organizationId },
                { "@Name", industryName }
            };

            try
            {
                string sqlQuery = @"INSERT INTO CompanyIndustry 
                            (OrganizationId, Name)
                            VALUES
                            (@OrganizationId, @Name)";

                await _dataBaseContext.ExequteSqliteCommand(sqlQuery, _dataBaseContext.GetConnection(), parameters);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An exception occurred while trying to associate company '{organizationId}' with industry '{industryName}'.");
            }
        }


        public async Task AssociateCompaniesWithIndustries(IEnumerable<CompanyInsertion> companies)
        {
            try
            {
                foreach (var company in companies)
                {
                    foreach (var industry in company.Industries)
                    {
                        await _pivotBulkInsert.Insert(new object[]
                        {
                            company.OrganizationId,
                            industry.Name
                        });
                    }
                }
                await _pivotBulkInsert.Flush();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An exception occurred while trying to associate companies with industries.");
            }
        }

        public async Task DeleteCompanyByOrganizationIdAssociations(string organizationId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@OrganizationId", organizationId }
            };

            try
            {
                await _dataBaseContext.ExequteSqliteCommand(
                    "DELETE FROM CompanyIndustry WHERE OrganizationId = @OrganizationId",
                    _dataBaseContext.GetConnection(), parameters);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An exception occurred while trying to delete associations of the company with OrganizationId '{organizationId}'.");
            }
        }

        public async Task DeleteIndustryByNameAssociations(string industryName)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@Name", industryName }
            };

            try
            {
                await _dataBaseContext.ExequteSqliteCommand(
                    "DELETE FROM CompanyIndustry WHERE Name = @Name",
                    _dataBaseContext.GetConnection(), parameters);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An exception occurred while trying to delete the industry '{industryName}'.");
            }
        }
    }
}
