using Companies.Domain.Abstraction.Services;
using Companies.Domain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Companies.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DailyStatisticsController : Controller
    {
        private readonly IDailyStatisticsService _dailyStatisticsService;

        public DailyStatisticsController(IDailyStatisticsService dailyStatisticsService)
        {
            _dailyStatisticsService = dailyStatisticsService;
        }

        [HttpGet]
        [Route("get-daily-statistics")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Standart, Administrator")]
        public async Task<IActionResult> GetDailyStatistics()
        {
            try
            {
                var result = await _dailyStatisticsService.GetDailyStatistics();
                if(result == null) { BadRequest("Failed to get daily statistics.");}
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while processing the request.");
                return BadRequest("Failed to get daily statistics.");
            }
        }

        [HttpGet]
        [Route("save-daily-statistics-to-JSON")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Standart, Administrator")]
        public async Task<IActionResult> SaveDailyStatistics()
        {
            try
            {
                await _dailyStatisticsService.SaveDailyStatisticsToJsonFile();
                return Created("JSONStatistics/", "JSON file generated successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while processing the request.");
                return BadRequest("Failed to save daily statistics.");
            }
        }
    }
}
