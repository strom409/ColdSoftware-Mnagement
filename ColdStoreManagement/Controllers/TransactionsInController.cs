using ColdStoreManagement.BLL.Models.TransactionsIn;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ColdStoreManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsInController : ControllerBase
    {
        private readonly ITransactionsInService _service;

        public TransactionsInController(ITransactionsInService service)
        {
            _service = service;
        }

        // --- Preinward Endpoints ---

        [HttpPost("AddPreinward")]
        public async Task<IActionResult> AddPreinward([FromBody] TransactionsInModel model)
        {
            var result = await _service.AddPreinwardAsync(model);
            return Ok(result);
        }

        [HttpGet("GetPreinwardProcBydate")]
        public async Task<IActionResult> GetPreinwardProcBydate([FromQuery] DateTime DateFrom, [FromQuery] DateTime Dateto, [FromQuery] string Prestat)
        {
            var result = await _service.GetPreinwardProcBydateAsync(DateFrom, Dateto, Prestat);
            return Ok(result);
        }

        [HttpPost("GeneratePreinward")]
        public async Task<IActionResult> GeneratePreinward([FromBody] TransactionsInModel model)
        {
            var result = await _service.GeneratePreinwardAsync(model);
            return Ok(result);
        }

        [HttpPost("GeneratePreinwardReport")]
        public async Task<IActionResult> GeneratePreinwardReport([FromBody] TransactionsInModel model)
        {
            var result = await _service.GeneratePreinwardReportAsync(model);
            return Ok(result);
        }

        [HttpGet("GetPreinwardId/{id}")]
        public async Task<IActionResult> GetPreinwardId(int id)
        {
            var result = await _service.GetPreinwardIdAsync(id);
             if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("GetPreinwardIdlist/{id}")]
        public async Task<IActionResult> GetPreinwardIdlist(int id)
        {
            var result = await _service.GetPreinwardIdlistAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }


        // --- Chamber Allocation Endpoints ---

        [HttpPost("GenchamberAgg/{id}")]
        public async Task<IActionResult> GenchamberAgg(int id)
        {
            var result = await _service.GenchamberAggAsync(id);
            return Ok(result);
        }

        [HttpGet("CheckChamberAllocation")]
        public async Task<IActionResult> CheckChamberAllocation([FromQuery] string selectedPurchase)
        {
            var result = await _service.CheckChamberAllocationAsync(selectedPurchase);
            return Ok(result);
        }

        [HttpGet("GetallStockChamber/{GrowerId}")]
        public async Task<IActionResult> GetallStockChamber(int GrowerId)
        {
            var result = await _service.GetallStockChamberAsync(GrowerId);
            return Ok(result);
        }

        [HttpPost("SaveChamberAllocation")]
        public async Task<IActionResult> SaveChamberAllocation([FromBody] TransactionsInModel model)
        {
            var result = await _service.SaveChamberAllocationAsync(model);
            return Ok(result);
        }

        [HttpPost("UpdateChamberAllocation")]
        public async Task<IActionResult> UpdateChamberAllocation([FromBody] TransactionsInModel model)
        {
            var result = await _service.UpdateChamberAllocationAsync(model);
            return Ok(result);
        }

        [HttpGet("CheckChamberStatus")]
        public async Task<IActionResult> CheckChamberStatus([FromQuery] string selectedPurchase)
        {
            var result = await _service.CheckChamberStatusAsync(selectedPurchase);
            return Ok(result);
        }

        [HttpGet("CheckChamberQty")]
        public async Task<IActionResult> CheckChamberQty([FromQuery] string selectedPurchase, [FromQuery] int chamberid)
        {
            var result = await _service.CheckChamberQtyAsync(selectedPurchase, chamberid);
            return Ok(result);
        }

        [HttpGet("CheckChamber")]
        public async Task<IActionResult> CheckChamber([FromQuery] int selectedNewchamber)
        {
            var result = await _service.CheckChamberAsync(selectedNewchamber);
            return Ok(result);
        }

        // --- Quality Endpoints ---

        [HttpGet("GetPendingQuality/{unitId}/{status}")]
        public async Task<IActionResult> GetPendingQuality(int unitId, string status)
        {
            var result = await _service.GetPendingQualityAsync(unitId, status);
            return Ok(result);
        }

        [HttpGet("GetQcPriv/{userGroup}")]
        public async Task<IActionResult> GetQcPriv(string userGroup)
        {
            var result = await _service.GetQcPrivAsync(userGroup);
            return Ok(result);
        }

        // --- Dock Endpoints ---

        [HttpGet("GetPendingDock/{unitId}/{dockPosting}")]
        public async Task<IActionResult> GetPendingDock(int unitId, int dockPosting)
        {
            var result = await _service.GetPendingDockAsync(unitId, dockPosting);
            return Ok(result);
        }

        [HttpGet("GetDockPriv/{userGroup}")]
        public async Task<IActionResult> GetDockPriv(string userGroup)
        {
            var result = await _service.GetDockPrivAsync(userGroup);
            return Ok(result);
        }

        // --- Location Endpoints ---

        [HttpGet("GetPendingLocation/{unitId}/{status}")]
        public async Task<IActionResult> GetPendingLocation(int unitId, string status)
        {
            var result = await _service.GetPendingLocationAsync(unitId, status);
            return Ok(result);
        }

        [HttpGet("GetLocationPriv/{userGroup}")]
        public async Task<IActionResult> GetLocationPriv(string userGroup)
        {
            var result = await _service.GetLocationPrivAsync(userGroup);
            return Ok(result);
        }

        [HttpPost("UpdateItemStatus/{itemId}")]
        public async Task<IActionResult> UpdateItemStatus(int itemId)
        {
            var result = await _service.UpdateItemStatusAsync(itemId);
            return Ok(result);
        }
    }
}
