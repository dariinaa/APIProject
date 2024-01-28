using Companies.Domain.Abstraction.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Globalization;

namespace Companies.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfSummaryController : Controller
    {
        private readonly IPdfSummaryService _pdfSummaryService;

        public PdfSummaryController(IPdfSummaryService pdfSummaryService)
        {
            _pdfSummaryService = pdfSummaryService;
        }

        [HttpGet]
        [Route("generate-pdf-company-summary-by-OrganizationId")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Standart, Administrator")]
        public async Task<IActionResult> GeneratePdf([FromQuery] string organizationId)
        {
            if (string.IsNullOrEmpty(organizationId))
            {
                return BadRequest("Insert valid OrganizationId.");
            }
            try
            {

                await _pdfSummaryService.GeneratePdf(organizationId);
                return Created("PdfSummaries/", "pdf file generated successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while processing the request.");
                return BadRequest("Failed to generate pdf summary file.");
            }
        }
    }
}
