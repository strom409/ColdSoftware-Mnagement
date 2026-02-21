using ColdStoreManagement.BLL.Errors;
using ColdStoreManagement.BLL.Models;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ColdStoreManagement.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class PackageController(
        IPackageService packageService,
        ILogger<PackageController> logger) : ControllerBase
    {
        private readonly IPackageService _packageService = packageService;
        private readonly ILogger<PackageController> _logger = logger;

        /// <summary>
        /// This endpoint used to GET ALL Packages
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllPackages()
        {
            try
            {
                var result = await _packageService.GetAllPackagesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching packages");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Failed to fetch packages");
            }
        }
        /// <summary>
        /// Get Package BY id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPackageById(int id)
        {
            try
            {
                var result = await _packageService.GetByIdAsync(id);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Package not found: {Id}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting package");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Failed to delete package");
            }
        }
        /// <summary>
        /// Add new Package
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddPackage([FromBody] PackageModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _packageService.AddPackageAsync(model);
                return Ok(new { message = "Package added successfully" });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Invalid package payload");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding package");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Failed to add package");
            }
        }
        /// <summary>
        /// UPDATE exiting Package
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePackage(int id, [FromBody] PackageModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _packageService.UpdatePackageAsync(id, model);
                return Ok(new { message = "Package updated successfully" });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Package not found: {Id}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating package");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Failed to update package");
            }
        }

        /// <summary>
        /// DELETE Package
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePackage(int id)
        {
            try
            {
                await _packageService.DeletePackageAsync(id);
                return Ok(new { message = "Package deleted successfully" });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Package not found: {Id}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting package");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Failed to delete package");
            }
        }
    }

}
