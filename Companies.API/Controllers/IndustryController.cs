using Companies.Domain.Abstraction;
using Companies.Domain.Abstraction.Services;
using Companies.Domain.Services;
using Companies.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;

namespace Companies.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndustryController : Controller
    {
        private readonly IIndustryService _industryService;

        public IndustryController(IIndustryService industryService)
        {
            _industryService = industryService;
        }

        [HttpPost]
        [Route("insert-industry")]
        public async Task<IActionResult> InsertIndustry([FromQuery] string industryName)
        {
            try
            {
                await _industryService.InsertIndustry(industryName);
                return Ok("Industry inserted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to insert industry. Error: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("update-industry")]
        public async Task<IActionResult> UpdateIndustry([FromQuery] IndustryInsertion industry)
        {
            try
            {
                await _industryService.UpdateIndustryName(industry);
                return Ok($"Industry with ID '{industry.Id}' updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update industry. Error: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("delete-industry-by-name")]
        public async Task<IActionResult> DeleteIndustryByName(string industryName)
        {
            try
            {
                await _industryService.DeleteIndustryByName(industryName);
                return Ok($"Industry '{industryName}' deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to delete industry. Error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("get-all-industries")]
        public async Task<IActionResult> GetAllIndustries()
        {
            try
            {
                var industries = await _industryService.GetAllIndustries();
                return Ok(industries);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to retrieve industries. Error: {ex.Message}");
            }
        }
    }
}
