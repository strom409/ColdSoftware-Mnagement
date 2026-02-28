using ColdStoreManagement.BLL.Models.DTOs;
using ColdStoreManagement.DAL.Services.Interface.TransactionsOut;
using Microsoft.AspNetCore.Mvc;

namespace ColdStoreManagement.Controllers.TransactionsOut
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemandOrderController(IDemandOrderService demandOrderService) : ControllerBase
    {
        private readonly IDemandOrderService _demandOrderService = demandOrderService;

        [HttpPost("UpdateLotOrderQuantity")]
        public async Task<IActionResult> UpdateLotOrderQuantity([FromBody] DemandOrderDto EditModel)
        {
            var result = await _demandOrderService.UpdateLotOrderQuantity(EditModel);
            return Ok(result);
        }

        [HttpPost("GenerateTempLotReport")]
        public async Task<IActionResult> GenerateTempLotReport([FromBody] DemandOrderDto EditModel, int unit, int Tempid)
        {
            var result = await _demandOrderService.GenerateTempLotReport(EditModel, unit, Tempid);
            return Ok(result);
        }

        [HttpPost("GenerateTempLotRawReportEdit")]
        public async Task<IActionResult> GenerateTempLotRawReportEdit([FromBody] DemandOrderDto EditModel, int unit, int Tempid)
        {
            var result = await _demandOrderService.GenerateTempLotRawReportEdit(EditModel, unit, Tempid);
            return Ok(result);
        }
        [HttpPost("generateDemandReport")]
        public async Task<IActionResult> generateDemandReport([FromBody] DemandOrderDto EditModel, int unit)
        {
            var result = await _demandOrderService.generateDemandReport(EditModel, unit);
            return Ok(result);
        }

        [HttpGet("GetAllDemands")]
        public async Task<IActionResult> GetAllDemands(int UnitId)
        {
            var result = await _demandOrderService.GetAllDemands(UnitId);
            return Ok(result);
        }

        [HttpGet("GetAllOrderby")]
        public async Task<IActionResult> GetAllOrderby(int UnitId)
        {
            var result = await _demandOrderService.GetAllOrderby(UnitId);
            return Ok(result);
        }

        [HttpGet("GetDemandPriv")]
        public async Task<IActionResult> GetDemandPriv(string Ugroup)
        {
            var result = await _demandOrderService.GetDemandPriv(Ugroup);
            return Ok(result);
        }

        [HttpPost("ValidatedemandStatus")]
        public async Task<IActionResult> ValidatedemandStatus([FromBody] DemandOrderDto companyModel, int outid)
        {
            var result = await _demandOrderService.ValidatedemandStatus(companyModel, outid);
            return Ok(result);
        }

        [HttpPost("GenerateDemandPreview")]
        public async Task<IActionResult> GenerateDemandPreview(int selectedGrowerId, [FromBody] DemandOrderDto companyModel)
        {
            var result = await _demandOrderService.GenerateDemandPreview(selectedGrowerId, companyModel);
            return Ok(result);
        }

        [HttpPost("UpdateOrderStatusraw")]
        public async Task<IActionResult> UpdateOrderStatusraw(int id, [FromBody] DemandOrderDto companyModel)
        {
            var result = await _demandOrderService.UpdateOrderStatusraw(id, companyModel);
            return Ok(result);
        }

        [HttpPost("DeleteDemandOrderRaw")]
        public async Task<IActionResult> DeleteDemandOrderRaw(int selectedGrowerId, [FromBody] DemandOrderDto companyModel)
        {
            var result = await _demandOrderService.DeleteDemandOrderRaw(selectedGrowerId, companyModel);
            return Ok(result);
        }

        [HttpGet("GetDemandGrower")]
        public async Task<IActionResult> GetDemandGrower(string demandirn)
        {
            var result = await _demandOrderService.GetDemandGrower(demandirn);
            return Ok(result);
        }

        [HttpGet("GetDemandbyAsync")]
        public async Task<IActionResult> GetDemandbyAsync(int outid)
        {
            var result = await _demandOrderService.GetDemandbyAsync(outid);
            return Ok(result);
        }

        [HttpGet("GetRawDemandbyAsync")]
        public async Task<IActionResult> GetRawDemandbyAsync(int outid)
        {
            var result = await _demandOrderService.GetRawDemandbyAsync(outid);
            return Ok(result);
        }

        [HttpGet("GetDemand")]
        public async Task<IActionResult> GetDemand(int outid)
        {
            var result = await _demandOrderService.GetDemand(outid);
            return Ok(result);
        }

        [HttpGet("GetDemandwithstore")]
        public async Task<IActionResult> GetDemandwithstore(int outid)
        {
            var result = await _demandOrderService.GetDemandwithstore(outid);
            return Ok(result);
        }

        [HttpGet("GetDraft")]
        public async Task<IActionResult> GetDraft(int draftid)
        {
            var result = await _demandOrderService.GetDraft(draftid);
            return Ok(result);
        }

        [HttpPost("SaveDemandsToDatabase")]
        public async Task<IActionResult> SaveDemandsToDatabase([FromBody] SaveDemandsRequestDto request)
        {
            var result = await _demandOrderService.SaveDemandsToDatabase(request.DemandIRNs, request.EditModel, request.Unit);
            return Ok(result);
        }

        [HttpGet("GetAllunitDemands")]
        public async Task<IActionResult> GetAllunitDemands()
        {
            var result = await _demandOrderService.GetAllunitDemands();
            return Ok(result);
        }

        [HttpGet("GetAllDemandsapprove")]
        public async Task<IActionResult> GetAllDemandsapprove(int UnitId)
        {
            var result = await _demandOrderService.GetAllDemandsapprove(UnitId);
            return Ok(result);
        }

        [HttpGet("GetAllGrowerDemands")]
        public async Task<IActionResult> GetAllGrowerDemands(int unitid, string GrowerName)
        {
            var result = await _demandOrderService.GetAllGrowerDemands(unitid, GrowerName);
            return Ok(result);
        }
        [HttpPost("AddFinalDemand")]
        public async Task<IActionResult> AddFinalDemand([FromBody] DemandOrderDto EditModel)
        {
            var result = await _demandOrderService.AddFinalDemand(EditModel);
            return Ok(result);
        }

        [HttpPost("AddFinalRaw")]
        public async Task<IActionResult> AddFinalRaw([FromBody] DemandOrderDto EditModel)
        {
            var result = await _demandOrderService.AddFinalRaw(EditModel);
            return Ok(result);
        }
    }
}


