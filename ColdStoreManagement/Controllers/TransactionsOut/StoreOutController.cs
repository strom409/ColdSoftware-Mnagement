using ColdStoreManagement.BLL.Models.DTOs;
using ColdStoreManagement.DAL.Services.Interface.TransactionsOut;
using Microsoft.AspNetCore.Mvc;

namespace ColdStoreManagement.Controllers.TransactionsOut
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreOutController(IStoreOutService storeOutService) : ControllerBase
    {
        private readonly IStoreOutService _storeOutService = storeOutService;

        [HttpGet("GetStoreOutStatus")]
        public async Task<IActionResult> GetStoreOutStatus(string stat, int UnitId, string demandirn, string avuser)
        {
            var result = await _storeOutService.GetStoreOutStatus(stat, UnitId, demandirn, avuser);
            return Ok(result);
        }

        [HttpPost("UpdateDraftQuantity")]
        public async Task<IActionResult> UpdateDraftQuantity([FromBody] StoreOutDto EditModel)
        {
            var result = await _storeOutService.UpdateDraftQuantity(EditModel);
            return Ok(result);
        }

        [HttpPost("ForceUpload")]
        public async Task<IActionResult> ForceUpload(int id, string Frems)
        {
            var result = await _storeOutService.ForceUpload(id, Frems);
            return Ok(result);
        }

        [HttpPost("ValidateStoreOutTransQty")]
        public async Task<IActionResult> ValidateStoreOutTransQty([FromBody] StoreOutDto companyModel)
        {
            var result = await _storeOutService.ValidateStoreOutTransQty(companyModel);
            return Ok(result);
        }

        [HttpPost("AddStoreOut")]
        public async Task<IActionResult> AddStoreOut([FromBody] StoreOutDto companyModel)
        {
            var result = await _storeOutService.AddStoreOut(companyModel);
            return Ok(result);
        }
    }
}
