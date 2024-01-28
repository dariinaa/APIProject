using AutoMapper;
using Companies.Domain.Abstraction.Mappers;
using Companies.Infrastructure.Models;
using Companies.Infrastructure.Models.Auth;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Services.Mappers
{
    public class MyMapper:IMyMapper
    {
        private readonly IMapper _mapper;
        public MyMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public Industry DataToIndustry(SqliteDataReader reader)
        {
            return new Industry
            {
                Name = reader["Name"].ToString(),
            };
        }

        public Company DataToCompany(SqliteDataReader reader)
        {
            return new Company
            {
                Id = reader["Id"].ToString(),
                OrganizationId = reader["OrganizationId"].ToString(),
                Name = reader["Name"].ToString(),
                Website = reader["Website"].ToString(),
                Country = reader["Country"].ToString(),
                Description = reader["Description"].ToString(),
                Founded = reader["Founded"].ToString(),
                Employees = reader["Employees"].ToString(),
            };
        }

        public CompanyInsertion MapToCompanyInsertion(CsvRecord csvRecord)
        {
            return new CompanyInsertion
            {
                Id = csvRecord.Index,
                OrganizationId = csvRecord.OrganizationId,
                Name = csvRecord.Name,
                Website = csvRecord.Website,
                Country = csvRecord.Country,
                Description = csvRecord.Description,
                Founded = int.Parse(csvRecord.Founded),
                Employees = int.Parse(csvRecord.Employees),
                Industries = ParseIndustries(csvRecord.Industry),
            };
        }

        private List<IndustryInsertion> ParseIndustries(string industryString)
        {
            if (string.IsNullOrEmpty(industryString))
            {
                return new List<IndustryInsertion>();
            }

            return industryString.Split('/')
                .Select(industryName => new IndustryInsertion
                { Name = industryName.Trim() }).ToList();
        }

        public User MapUserFromDataReader(SqliteDataReader reader)
        {
            return new User
            {
                Id = Convert.ToInt32(reader["Id"]),
                Username = reader["Username"].ToString(),
                EmailAddress = reader["EmailAddress"].ToString(),
                Password = reader["Password"].ToString(),
                GivenName = reader["GivenName"].ToString(),
                Surname = reader["Surname"].ToString(),
                Role = reader["Role"].ToString(),
            };
        }
    }
}
