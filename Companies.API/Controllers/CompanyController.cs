using Companies.Domain.Abstraction.Services;
using Companies.Domain.Services;
using Companies.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Companies.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpPost]
        [Route("insert-company")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Standart, Administrator")]
        public async Task<IActionResult> InsertCompany([FromBody] CompanyInsertion company)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _companyService.InsertCompany(company);
                return Ok("Company inserted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to insert company. Error: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("update-company")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Standart, Administrator")]
        public async Task<IActionResult> UpdateCompany([FromBody] CompanyInsertion request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _companyService.UpdateCompany(request);
                return Ok($"Company with OrganizationId '{request.Id}' updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update company. Error: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("delete-company-by-OrganizationId")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<IActionResult> DeleteCompanyByOrganizationId(string organizationId)
        {
            if (string.IsNullOrEmpty(organizationId))
            {
                return BadRequest("OrganizationId cannot be null or empty.");
            }

            try
            {
                await _companyService.DeleteCompanyByOrganizationId(organizationId);
                return Ok($"Company '{organizationId}' deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to delete company. Error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("get-all-companies")]
        public async Task<IActionResult> GetAllCompanies()
        {
            try
            {
                var companies = await _companyService.GetAllCompanies();
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to retrieve companies. Error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("get-top10-companies-by-Employees")]
        public async Task<IActionResult> GetTop10CompaniesByEmployees()
        {
            try
            {
                var companies = await _companyService.GetTop10CompaniesByEmployees();
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to retrieve companies. Error: {ex.Message}");
            }
        }
    }
}
