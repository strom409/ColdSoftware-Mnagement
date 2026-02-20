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
        public async Task<IActionResult> DeleteGrowerGroup([FromBody] GrowerModel model)
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
    }
}
