using ColdStoreManagement.BLL.Models.Crate;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace ColdStoreManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrateController : BaseController
    {
        private readonly ICrateService _crateService;

        public CrateController(ICrateService crateService)
        {
            _crateService = crateService;
        }

        [HttpPost("GenerateCrateReport")]
        public async Task<IActionResult> GenerateCrateReport([FromBody] CrateModel model)
        {
            var result = await _crateService.GenerateCrateReportModelAsync(model);
            return Ok(result);
        }

        [HttpGet("GetCrateSummaryMain")]
        public async Task<IActionResult> GetCrateSummaryMain()
        {
            var result = await _crateService.GetCrateSummaryMainModelAsync();
            return Ok(result);
        }

        [HttpGet("GetDailyCratesProcEmpty")]
        public async Task<IActionResult> GetDailyCratesProcEmpty()
        {
            var result = await _crateService.GetDailyCratesProcEmptyAsync();
            return Ok(result);
        }

        [HttpGet("GetDailyCratesProcOut")]
        public async Task<IActionResult> GetDailyCratesProcOut()
        {
            var result = await _crateService.GetDailyCratesProcOutAsync();
            return Ok(result);
        }

        [HttpGet("CheckCratesPartyOut")]
        public async Task<IActionResult> CheckCratesPartyOut([FromQuery] string growerGroupName, [FromQuery] string flag)
        {
            var result = await _crateService.CheckCratesPartyOutModelAsync(growerGroupName, flag);
            return Ok(result);
        }

        [HttpGet("CheckCratesPartySubOut")]
        public async Task<IActionResult> CheckCratesPartySubOut([FromQuery] string growerName, [FromQuery] string growerGroupName, [FromQuery] string flag)
        {
            var result = await _crateService.CheckCratesPartySubOutModelAsync(growerName, growerGroupName, flag);
            return Ok(result);
        }

        [HttpPost("AddCrateOut")]
        public async Task<IActionResult> AddCrateOut([FromBody] CrateModel model)
        {
            var result = await _crateService.AddCrateOutAsync(model);
            
            if (result?.RetFlag?.Trim().Equals("FALSE", StringComparison.OrdinalIgnoreCase) == true)
            {
                return BadRequest(new { Message = result.RetMessage ?? "Failed to add crate issue" });
            }

            return Ok(result);
        }

        [HttpPost("UpdateCrateIssueOut")]
        public async Task<IActionResult> UpdateCrateIssueOut([FromBody] CrateModel model)
        {
            var result = await _crateService.UpdateCrateIssueOutAsync(model);
             if (result?.RetFlag?.Trim().Equals("FALSE", StringComparison.OrdinalIgnoreCase) == true)
            {
                return BadRequest(new { Message = result.RetMessage ?? "Failed to update crate issue" });
            }
            return Ok(result);
        }

        [HttpPost("DeleteCrateIssue")]
        public async Task<IActionResult> DeleteCrateIssue([FromBody] CrateModel model)
        {
            // Assuming model.CrissueId is populated
            var result = await _crateService.DeleteCrateIssueAsync(model.CrissueId, model);
             if (result?.RetFlag?.Trim().Equals("FALSE", StringComparison.OrdinalIgnoreCase) == true)
            {
                return BadRequest(new { Message = result.RetMessage ?? "Failed to delete crate issue" });
            }
            return Ok(result);
        }

        [HttpPost("GenerateCratePreview")]
        public async Task<IActionResult> GenerateCratePreview([FromBody] CrateModel model)
        {
            // Assuming the preview generation needs the ID and the model (for flags etc)
            // The service takes (int id, CrateModel model). We use model.CrissueId
            var result = await _crateService.GenerateCratePreviewAsync(model.CrissueId, model);
            return Ok(result);
        }

        [HttpGet("GetCrateOrderNo")]
        public async Task<IActionResult> GetCrateOrderNo()
        {
            var result = await _crateService.GetCrateOrderNoAsync();
            return Ok(result);
        }

        [HttpGet("GetallCrateMarks")]
        public async Task<IActionResult> GetallCrateMarks()
        {
            var result = await _crateService.GetallCrateMarksAsync();
            return Ok(result);
        }

        [HttpGet("GetCrateIssueDet/{id}")]
        public async Task<IActionResult> GetCrateIssueDet(int id)
        {
            var result = await _crateService.GetCrateIssueDetAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("GetCratePrivs2/{userGroup}")]
        public async Task<IActionResult> GetCratePrivs2(string userGroup)
        {
            var result = await _crateService.GetCratePrivs2Async(userGroup);
             if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("GetallCrateflagall")]
        public async Task<IActionResult> GetallCrateflagall()
        {
            var result = await _crateService.GetallCrateflagallAsync();
            return Ok(result);
        }

        [HttpGet("GetCrateordersEmpty")]
        public async Task<IActionResult> GetCrateordersEmpty()
        {
            var result = await _crateService.GetCrateordersEmptyAsync();
            return Ok(result);
        }

        [HttpPost("GenerateCrateReportPdf")]
        public async Task<IActionResult> GenerateCrateReportPdf([FromBody] CrateModel model)
        {
            var result = await _crateService.GenerateCrateReportPdfAsync(model);
             if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("GenerateCrateReportDel")]
        public async Task<IActionResult> GenerateCrateReportDel([FromBody] CrateModel model)
        {
            var result = await _crateService.GenerateCrateReportDelAsync(model);
            return Ok(result);
        }

        [HttpPost("GenerateRawCrateReport/{unit}")]
        public async Task<IActionResult> GenerateRawCrateReport([FromBody] CrateModel model, int unit)
        {
            var result = await _crateService.GenerateRawCrateReportAsync(model, unit);
            return Ok(result);
        }
    }
}
