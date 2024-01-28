using Companies.Domain.Abstraction.Services;
using Companies.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        [Route("save-records-from-csv-file")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Standart, Administrator")]
        public async Task<IActionResult> ReadCsvDataToDatabase([FromQuery] string filepath)
        {
            try
            {
                if (string.IsNullOrEmpty(filepath))
                {
                    return BadRequest("File path is empty or null.");
                }
                await _readCsvFileService.SaveCsvDataToDatabaseAsync(filepath);
                return Ok("Data inserted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to insert data. Error: {ex.Message}");
            }
        }

        /*[HttpPost]
        [Route("save-records-from-folder")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Standart, Administrator")]
        public async Task<IActionResult> SaveCsvDataToDatabaseFromFolderAsync([FromQuery] string folderPath)
        {
            try
            {
                if (string.IsNullOrEmpty(folderPath))
                {
                    return BadRequest("Folder path is empty or null.");
                }

                await _readCsvFileService.SaveCsvDataToDatabaseAsync(folderPath);
                return Ok("Data inserted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to insert data. Error: {ex.Message}");
            }
        }*/

    }
}
