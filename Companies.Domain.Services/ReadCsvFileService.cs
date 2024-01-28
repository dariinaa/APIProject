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
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("FilePath cannot be null or empty.", nameof(filePath));
            }

            string originalString = filePath;
            string escapedString = originalString.Replace("\\", "\\\\");

            await _readCsvFileRepository.SaveCsvDataToDatabaseAsync(escapedString);
        }

        public async Task SaveCsvDataToDatabaseFromFolderAsync(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                throw new ArgumentException("FolderPath cannot be null or empty.", nameof(folderPath));
            }

            var csvFiles = Directory.GetFiles(folderPath, "*.csv");

            foreach (var filePath in csvFiles)
            {
                string escapedString = filePath.Replace("\\", "\\\\");

                await _readCsvFileRepository.SaveCsvDataToDatabaseAsync(escapedString);
            }
        }
    }
}
