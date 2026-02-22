using ColdStoreManagement.BLL.Models.Chamber;
using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.BLL.Models.DTOs;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ColdStoreManagement.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class ChamberController(IChamberService chamberService,
        ILogger<ChamberController> logger) : ControllerBase
    {
        private readonly IChamberService _service = chamberService;
        private readonly ILogger<ChamberController> _logger = logger;

        #region Chamber & Basic Retrieval

        [HttpGet("all")]
        public async Task<IActionResult> GetAllChambers()
        {
            try { return Ok(await _service.GetAllChambersAsync()); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all chambers");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("all-details")]
        public async Task<IActionResult> GetAllChamberDetails()
        {
            try { return Ok(await _service.GetAllChamberDetailsAsync()); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chamber details");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("in-stock")]
        public async Task<IActionResult> GetChambersIn()
        {
            try { return Ok(await _service.GetChambersInAsync()); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock-in chambers");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetChamberDetailById(int id)
        {
            try { return Ok(await _service.GetChamberDetailByIdAsync(id)); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chamber {Id}", id);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("list-by-unit/{unitId}")]
        public async Task<IActionResult> GetAllChambersList(int unitId)
        {
            try { return Ok(await _service.GetAllChambersList(unitId)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("edit-model/{id}")]
        public async Task<IActionResult> GetChamberById(int id)
        {
            try { return Ok(await _service.GetChamberByIdAsync(id)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("allocation-view")]
        public async Task<IActionResult> GetChambers()
        {
            try { return Ok(await _service.GetChambersAsync()); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        #endregion

        #region Allocation & Transactions

        [HttpPost("release-allocation")]
        public async Task<IActionResult> ReleaseAllocation(int growerId, int chamberId)
        {
            try { await _service.ReleaseAllocationAsync(growerId, chamberId); return Ok(); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpPost("save-allocation")]
        public async Task<IActionResult> SaveChamberAllocation([FromBody] SaveChamberAllocationRequest req, string user)
        {
            try { return Ok(await _service.SaveChamberAllocationAsync(req, user)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpPut("update-allocation/{id}")]
        public async Task<IActionResult> UpdateChamberAllocation(int id, decimal qty)
        {
            try { return Ok(await _service.UpdateChamberAllocationAsync(id, qty)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpDelete("delete-allocation/{id}")]
        public async Task<IActionResult> DeleteAllocation(int id, [FromBody] CompanyModel model)
        {
            try { return Ok(await _service.DeleteAllocation(id, model)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        #endregion

        #region Grower Operations

        [HttpGet("growers-all")]
        public async Task<IActionResult> GetallGrowers()
        {
            try { return Ok(await _service.GetallGrowers()); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("grower/{id}")]
        public async Task<IActionResult> GetGrowerId(int id)
        {
            try { return Ok(await _service.GetGrowerIdAsync(id)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpPost("add-grower-sub")]
        public async Task<IActionResult> AddGrowerSubdirect([FromBody] CompanyModel model)
        {
            try { return Ok(await _service.AddGrowerSubdirect(model)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpPut("update-grower-status/{id}")]
        public async Task<IActionResult> UpdateGrowerStatus(int id)
        {
            try { return Ok(await _service.UpdateGrowerStatus(id)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpDelete("delete-grower-group/{id}")]
        public async Task<IActionResult> DeleteGrowerGroup(int id, [FromBody] CompanyModel model)
        {
            try { return Ok(await _service.DeleteGrowerGroup(id, model)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        #endregion

        #region Stock & Analysis Reports

        [HttpGet("stock-chamber/{growerId}")]
        public async Task<IActionResult> GetallStockChamber(int growerId)
        {
            try { return Ok(await _service.GetallStockChamber(growerId)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("stock-grower-lots/{growerId}")]
        public async Task<IActionResult> GetallStockGrowerlots(int growerId)
        {
            try { return Ok(await _service.GetallStockGrowerlots(growerId)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("chamber-details/{id}")]
        public async Task<IActionResult> GetChamberDetails(int id)
        {
            try { return Ok(await _service.GetChamberDetailsAsync(id)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("grower-details/{partyId}/{chamberId}")]
        public async Task<IActionResult> GetChamberGrowerDetails(int partyId, int chamberId)
        {
            try { return Ok(await _service.GetChamberGrowerDetailsAsync(partyId, chamberId)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        #endregion

        #region Summaries & Totals

        [HttpGet("summary/{id}")]
        public async Task<IActionResult> Getchsummary(int id)
        {
            try { return Ok(await _service.Getchsummary(id)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("summary-grower/{id}")]
        public async Task<IActionResult> GetchsummaryGrower(int id)
        {
            try { return Ok(await _service.GetchsummaryGrower(id)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("summary-grower-det/{id}")]
        public async Task<IActionResult> GetchsummaryGrowerdet(int id)
        {
            try { return Ok(await _service.GetchsummaryGrowerdet(id)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("dashboard-totals")]
        public async Task<IActionResult> GetDashboardTotals()
        {
            try { return Ok(await _service.GetDashboardTotalsAsync()); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        #endregion

        #region Lists & Filters

        [HttpGet("chambers-party/{unitId}/{growerName}")]
        public async Task<IActionResult> GetallChambersParty(int unitId, string growerName)
        {
            try { return Ok(await _service.GetallChambersParty(unitId, growerName)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("chambers-sub/{unitId}/{grower}/{sub}")]
        public async Task<IActionResult> GetallChambersSub(int unitId, string grower, string sub)
        {
            try { return Ok(await _service.GetallChambersSub(unitId, grower, sub)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("grower-list-by-chamber/{name}")]
        public async Task<IActionResult> GetallchamberGrowerlist(string name)
        {
            try { return Ok(await _service.GetallchamberGrowerlist(name)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("grower-list")]
        public async Task<IActionResult> GetallGrowerlist()
        {
            try { return Ok(await _service.GetallGrowerlist()); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        #endregion

        #region Actions & Commands

        [HttpPost("lock-unlock/{id}")]
        public async Task<IActionResult> LockUnlockChamber(int id)
        {
            try { return Ok(await _service.LockUnlockChamber(id)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpPost("save-demands")]
        public async Task<IActionResult> SaveDemands([FromBody] List<string> irns, [FromQuery] CompanyModel model, int unit)
        {
            try { return Ok(await _service.SaveDemandsToDatabase(irns, model, unit)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpPost("add-challan")]
        public async Task<IActionResult> AddChallanGrower([FromBody] CompanyModel model)
        {
            try { return Ok(await _service.AddChallanGrower(model)); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpPost("add-new")]
        public async Task<IActionResult> AddNewChamber([FromBody] ChamberDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _service.AddNewChamber(model);

                if (result?.RetFlag?.ToString().ToUpper() == "FALSE")
                    return BadRequest(result.RetMessage);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new chamber");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateChamber(int id, [FromBody] ChamberUpdateDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _service.UpdateChamber(id, model);

                if (result?.RetFlag?.ToString().ToUpper() == "FALSE")
                    return BadRequest(result.RetMessage);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating chamber {Id}", id);
                return StatusCode(500, ex.Message);
            }
        }

        #endregion
    }
}
