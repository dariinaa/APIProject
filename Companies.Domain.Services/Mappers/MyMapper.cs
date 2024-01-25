using AutoMapper;
using Companies.Domain.Abstraction.Mappers;
using Companies.Infrastructure.Entities;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
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
                Id = reader["Id"].ToString(),
                Name = reader["Name"].ToString(),
            };
        }
    }
}
