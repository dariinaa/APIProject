using Companies.Domain.Abstraction.Services;
using Companies.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;

namespace Companies.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReadCsvFileController : Controller
    {
        private readonly IReadCsvFileService _readCsvFileService;

        public ReadCsvFileController(IReadCsvFileService readCsvFileService)
        {
            _readCsvFileService = readCsvFileService;
        }

        [HttpPost]
        [Route("read-csv-file")]
        public async Task<IActionResult> ReadCsvDataToDatabase([FromQuery] string filepath)
        {
            try
            {
                if (filepath == null || filepath.Length == 0)
                {
                    return BadRequest("File is empty or null.");
                }
                await _readCsvFileService.SaveCsvDataToDatabaseAsync(filepath);
                return Ok("Data inserted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to insert data. Error: {ex.Message}");
            }
        }
    }
}
