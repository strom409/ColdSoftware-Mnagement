using ColdStoreManagement.BLL.Models;
using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ColdStoreManagement.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class VehicleInfoController(
        IVehicleInfoService vehicleService,
        ILogger<VehicleInfoController> logger) : ControllerBase
    {
        private readonly IVehicleInfoService _vehicleService = vehicleService;
        private readonly ILogger<VehicleInfoController> _logger = logger;

        // GET: api/VehicleInfo/GetAllVehGroup
        [HttpGet("GetAllVehGroup")]
        public async Task<IActionResult> GetAllVehGroup()
        {
            try
            {
                var result = await _vehicleService.GetAllVehGroup();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error fetching vehicle groups");
                return Problem(
                    title: "Failed to fetch vehicle groups",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        // GET: api/VehicleInfo/Getvehid/5
        [HttpGet("Getvehid/{vehid:int}")]
        public async Task<IActionResult> Getvehid(int vehid)
        {
            try
            {
                var result = await _vehicleService.Getvehid(vehid);

                if (result == null)
                    return NotFound($"Vehicle with id {vehid} not found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error fetching vehicle id {vehid}");
                return Problem(
                    title: "Failed to fetch vehicle",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        // POST: api/VehicleInfo/Addveh
        [HttpPost("Addveh")]
        public async Task<IActionResult> Addveh([FromBody] VehInfoModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _vehicleService.Addveh(model);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error adding vehicle");
                return Problem(
                    title: "Failed to add vehicle",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        // PUT: api/VehicleInfo/UpdateVeh
        [HttpPut("UpdateVeh")]
        public async Task<IActionResult> UpdateVeh([FromBody] VehInfoModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _vehicleService.UpdateVeh(model);

                if (result == null)
                    return NotFound("Vehicle not found");

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error updating vehicle");
                return Problem(
                    title: "Failed to update vehicle",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        // PATCH: api/VehicleInfo/UpdatevehStatus/5
        [HttpPatch("UpdatevehStatus/{id:int}")]
        public async Task<IActionResult> UpdatevehStatus(int id)
        {
            try
            {
                var success = await _vehicleService.UpdatevehStatus(id);

                if (!success)
                    return NotFound($"Vehicle with id {id} not found");

                return Ok(success);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error updating status for vehicle id {id}");
                return Problem(
                    title: "Failed to update vehicle status",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        // DELETE: api/VehicleInfo/DeleteVeh/5
        [HttpDelete("DeleteVeh/{id:int}")]
        public async Task<IActionResult> DeleteVeh(int id, [FromBody] CompanyModel model)
        {
            try
            {
                var result = await _vehicleService.DeleteVeh(id, model);

                if (result == null)
                    return NotFound($"Vehicle with id {id} not found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error deleting vehicle id {id}");
                return Problem(
                    title: "Failed to delete vehicle",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        // GET: api/VehicleInfo/GetallItemGroup
        [HttpGet("GetallItemGroup")]
        public async Task<IActionResult> GetallItemGroup()
        {
            try
            {
                var result = await _vehicleService.GetallItemGroup();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error fetching item groups");
                return Problem(
                    title: "Failed to fetch item groups",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }
    }
}
