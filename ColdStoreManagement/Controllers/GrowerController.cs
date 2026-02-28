using ColdStoreManagement.BLL.Models.Grower;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ColdStoreManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GrowerController(IGrowerService growerService) : BaseController
    {
        private readonly IGrowerService _growerService = growerService;

        [HttpGet("GetallGrowers")]
        public async Task<IActionResult> GetallGrowers()
        {
            var result = await _growerService.GetallGrowers();
            return Ok(result);
        }

        [HttpGet("GetallSubGrowers/{id}")]
        public async Task<IActionResult> GetallSubGrowers(int id)
        {
            var result = await _growerService.GetallSubGrowers(id);
            return Ok(result);
        }

        [HttpPost("AddGrowerGroup")]
        public async Task<IActionResult> AddGrowerGroup([FromBody] GrowerModel model)
        {
            var result = await _growerService.AddGrowerGroup(model);
            if (result == null) return BadRequest("Failed to add grower");
            
            // Check for database validation flags as per legacy logic
            if (result.RetFlag?.Trim().Equals("FALSE", StringComparison.OrdinalIgnoreCase) == true)
            {
                return BadRequest(new { Message = result.RetMessage ?? "Failed to add grower" });
            }

            return Ok(result);
        }

        [HttpPost("UpdateGrowerStatus")]
        public async Task<IActionResult> UpdateGrowerStatus([FromBody] GrowerModel model)
        {
            // Note: Service ignores ID param and uses model.Growerid, so we pass 0 or model.Growerid
            var result = await _growerService.UpdateGrowerStatus(model.Growerid, model);
            if (result)
            {
                return Ok(new { Message = "Status updated successfully" });
            }
            return BadRequest(new { Message = "Failed to update status" });
        }

        [HttpPost("DeleteGrowerGroup")]
        public async Task<IActionResult> DeleteGrowerGroup([FromBody] GrowerDeleteModel model)
        {
            var result = await _growerService.DeleteGrowerGroup(model.Growerid, model);
            
             if (result?.RetFlag?.Trim().Equals("FALSE", StringComparison.OrdinalIgnoreCase) == true)
            {
                 return BadRequest(new { Message = result.RetMessage ?? "Failed to delete grower" });
            }

            return Ok(result);
        }

        [HttpGet("GrowerPriv")]
        public async Task<IActionResult> GrowerPriv([FromQuery] string Ugroup)
        {
            var result = await _growerService.GrowerPriv(Ugroup);
            return Ok(result);
        }

        [HttpPost("AddGrowerSub")]
        public async Task<IActionResult> AddGrowerSub([FromBody] SubGrowerModel model)
        {
            var result = await _growerService.AddGrowerSub(model);
            if (result == null) return BadRequest("Failed to add sub-grower");

            if (result.RetFlag?.Trim().Equals("FALSE", StringComparison.OrdinalIgnoreCase) == true)
            {
                return BadRequest(new { Message = result.RetMessage ?? "Failed to add sub-grower" });
            }

            return Ok(result);
        }

        [HttpPost("DeleteSubGrowerGroup")]
        public async Task<IActionResult> DeleteSubGrowerGroup([FromBody] GrowerDeleteModel model)
        {
            var result = await _growerService.DeleteSubGrowerGroup(model.Growerid, model);

            if (result?.RetFlag?.Trim().Equals("FALSE", StringComparison.OrdinalIgnoreCase) == true)
            {
                return BadRequest(new { Message = result.RetMessage ?? "Failed to delete sub-grower" });
            }

            return Ok(result);
        }

        [HttpPost("AddChallanGrower")]
        public async Task<IActionResult> AddChallanGrower([FromBody] ChallanModel model)
        {
            var result = await _growerService.AddChallanGrower(model);
            if (result == null) return BadRequest("Failed to add challan");

            if (result.RetFlag?.Trim().Equals("FALSE", StringComparison.OrdinalIgnoreCase) == true)
            {
                return BadRequest(new { Message = result.RetMessage ?? "Failed to add challan" });
            }

            return Ok(result);
        }

        [HttpGet("GetallChallanlist")]
        public async Task<IActionResult> GetallChallanlist([FromQuery] string growerGroup)
        {
            var result = await _growerService.GetallChallanlist(growerGroup);
            return Ok(result);
        }

        [HttpPost("DeleteChallanGroup")]
        public async Task<IActionResult> DeleteChallanGroup([FromBody] GrowerDeleteModel model)
        {
            var result = await _growerService.DeleteChallanGroup(model.Growerid, model);

            if (result?.RetFlag?.Trim().Equals("FALSE", StringComparison.OrdinalIgnoreCase) == true)
            {
                return BadRequest(new { Message = result.RetMessage ?? "Failed to delete challan" });
            }

            return Ok(result);
        }

        [HttpPost("GenchamberAgg")]
        public async Task<IActionResult> GenchamberAgg([FromQuery] int growerId)
        {
            var result = await _growerService.GenchamberAgg(growerId);
            if (result) return Ok(new { Message = "Chamber aggregate generated successfully" });
            return BadRequest(new { Message = "Failed to generate chamber aggregate" });
        }

        [HttpPost("GenGrowerAgg")]
        public async Task<IActionResult> GenGrowerAgg([FromQuery] int growerId)
        {
            var result = await _growerService.GenGrowerAgg(growerId);
            if (result) return Ok(new { Message = "Grower aggregate generated successfully" });
            return BadRequest(new { Message = "Failed to generate grower aggregate" });
        }

        [HttpPost("UpdateGrowerGroup")]
        public async Task<IActionResult> UpdateGrowerGroup([FromBody] GrowerModel model)
        {
            var result = await _growerService.UpdateGrowerGroup(model);
            if (result == null) return BadRequest("Failed to update grower");

            if (result.RetFlag?.Trim().Equals("FALSE", StringComparison.OrdinalIgnoreCase) == true)
            {
                return BadRequest(new { Message = result.RetMessage ?? "Failed to update grower" });
            }

            return Ok(result);
        }

        [HttpGet("GetGrowerId/{id}")]
        public async Task<IActionResult> GetGrowerIdAsync(int id)
        {
            var result = await _growerService.GetGrowerIdAsync(id);
            if (result == null) return NotFound($"Grower with ID {id} not found");
            return Ok(result);
        }

        [HttpGet("GetFinyear")]
        public async Task<IActionResult> GetFinyear()
        {
            var result = await _growerService.GetFinyear();
            return Ok(result);
        }

        [HttpGet("GetServices")]
        public async Task<IActionResult> GetServices()
        {
            var result = await _growerService.GetServices();
            return Ok(result);
        }

        [HttpGet("GetItemname")]
        public async Task<IActionResult> GetItemname()
        {
            var result = await _growerService.GetItemname();
            return Ok(result);
        }

        [HttpGet("GetAgreementGroupName/{id}")]
        public async Task<IActionResult> GetAgreementGroupName(int id)
        {
            var result = await _growerService.GetAgreementGroupName(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("GetAgreementByGrowerId/{id}")]
        public async Task<IActionResult> GetAgreementByGrowerId(int id)
        {
            var result = await _growerService.GetAgreementByGrowerId(id);
            return Ok(result);
        }

        [HttpGet("GetAgreementById/{id}")]
        public async Task<IActionResult> GetAgreementById(int id)
        {
            var result = await _growerService.GetAgreementId(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("GetInstallments/{id}")]
        public async Task<IActionResult> GetInstallmentsByGrowerAsync(int id)
        {
            var result = await _growerService.GetInstallmentsByGrowerAsync(id);
            return Ok(result);
        }

        [HttpPost("AddAgreement")]
        public async Task<IActionResult> AddAgreement([FromBody] GrowerAgreementModel model)
        {
            var result = await _growerService.AddAgreement(model);
            if (result == null) return BadRequest("Failed to add agreement");
            return Ok(result);
        }

        [HttpPost("UpdateAgreement/{agreementId}")]
        public async Task<IActionResult> UpdateAgreement([FromBody] GrowerAgreementModel model, int agreementId)
        {
            var result = await _growerService.UpdateAgreement(model, agreementId);
            if (result == null) return BadRequest("Failed to update agreement");
            return Ok(result);
        }

        [HttpPost("BulkInsertInstallments")]
        public async Task<IActionResult> BulkInsertInstallments([FromBody] BulkInstallmentRequest request)
        {
            if (request == null || request.Installments == null || request.Agreement == null)
                return BadRequest("Invalid request data");

            await _growerService.BulkInsertInstallmentsAsync(request.Installments, request.Agreement);
            return Ok(new { Message = "Installments inserted successfully" });
        }
    }

    public class BulkInstallmentRequest
    {
        public List<InstallmentModel> Installments { get; set; } = new();
        public GrowerAgreementModel Agreement { get; set; } = new();
    }
}
