using ColdStoreManagement.BLL.Models;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ColdStoreManagement.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [AllowAnonymous]
    public class UnitController(
        IUnitService unitService,
        ILogger<UnitController> logger) : ControllerBase
    {
        private readonly IUnitService _unitService = unitService;
        private readonly ILogger<UnitController> _logger = logger;

        // GET: api/unit
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _unitService.GetAllAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching units");
                throw;
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var result = await _unitService.GetByIdAsync(id);
                if (result == null)
                    return NotFound("Unit not found");

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching units");
                throw;
            }
        }
        [HttpGet("details/{unitName}")]
        public async Task<IActionResult> GetByNameAsync(string unitName)
        {
            try
            {
                var result = await _unitService.GetByNameAsync(unitName);
                if (result == null)
                    return NotFound("Unit not found");

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching units");
                throw;
            }
        }

        // POST: api/unit
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UnitMasterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _unitService.AddAsync(model);
                return Ok(new { message = "Unit created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating unit");
                throw;
            }
        }

        // PUT: api/unit/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UnitMasterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _unitService.UpdateAsync(id, model);
                return Ok(new { message = "Unit updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating unit {Id}", id);
                throw;
            }
        }

        // DELETE: api/unit/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _unitService.DeleteAsync(id);
                return Ok(new { message = "Unit deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting unit {Id}", id);
                throw;
            }
        }
    }
}
