using Companies.Domain.Abstraction;
using Companies.Domain.Abstraction.Repositories;
using Companies.Domain.Abstraction.Services;
using Companies.Domain.Services.Repositories;
using Companies.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Services
{
    public class ReadCsvFileService:IReadCsvFileService
    {
        private readonly IReadCsvFileRepository _readCsvFileRepository;

        public ReadCsvFileService(IReadCsvFileRepository readCsvFileRepository)
        {
            _readCsvFileRepository = readCsvFileRepository;
        }

        public async Task SaveCsvDataToDatabaseAsync(string filePath)
        {
            string originalString = filePath;
            string escapedString = originalString.Replace("\\", "\\\\");

            await _readCsvFileRepository.SaveCsvDataToDatabaseAsync(escapedString);
        }
    }
}
