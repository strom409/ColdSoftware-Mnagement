using ColdStoreManagement.BLL.Models.Bank;
using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.BLL.Models.DTOs;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ColdStoreManagement.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class CompanyController(
        ICompanyService companyService,
        ILogger<CompanyController> logger) : BaseController
    {
        private readonly ICompanyService _companyService = companyService;
        private readonly ILogger<CompanyController> _logger = logger;

        // ===================== COMPANY =====================

        [HttpGet("{companyId:int}")]
        public async Task<IActionResult> GetCompanyById(int companyId)
        {
            try
            {
                var result = await _companyService.GetCompanyByIdAsync(companyId);

                if (result == null)
                    return NotFound("Company not found");

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                throw; // handled by ExceptionMiddleware
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> EditCompany(int id, [FromBody] CompanyDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var success = await _companyService.EditCompany(id, model);

                if (!success)
                    return BadRequest("Company update failed");

                return Ok("Company updated successfully");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // ===================== Buiding =====================
        // GET: api/building
        [HttpGet("Buiding")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var buildings = await _companyService.GetAllBuildingsAsync();
                return Ok(buildings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching buildings");
                throw; // handled by ExceptionMiddleware
            }
        }
        [HttpGet("Buiding/{id:int}")]
        public async Task<IActionResult> GetBuidingById(int id)
        {
            try
            {
                var building = await _companyService.GetBuildingById(id);
                if (building == null)
                    return NotFound($"buildings with id {id} not found");

                return Ok(building);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting building {Id}", id);
                throw;
            }
        }
        // POST: api/building
        [HttpPost("Buiding")]
        public async Task<IActionResult> Create([FromBody] BuildingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _companyService.AddBuildingAsync(model);
                if (!result)
                    return BadRequest("Unable to create Building");

                return Ok(new { message = "Building created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating building");
                throw;
            }
        }

        // PUT: api/building/{id}
        [HttpPut("Buiding/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BuildingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _companyService.UpdateBuildingAsync(id, model);
                if (!result)
                    return BadRequest("Unable to create buidling");

                return Ok(new { message = "Building updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating building {Id}", id);
                throw;
            }
        }

        // DELETE: api/building/{id}
        [HttpDelete("Buiding/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _companyService.DeleteBuildingAsync(id);
                return Ok(new { message = "Building deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting building {Id}", id);
                throw;
            }
        }


    }
}
