using System.Collections.Generic;
using System.Threading.Tasks;
using ColdStoreManagement.BLL.Models.Crate;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface ICrateService
    {
        Task<List<CrateModel>> GenerateCrateReportAsync(CrateModel model);
        Task<List<CrateModel>> GetCrateSummaryMainAsync();
        Task<List<CrateModel>> GetDailyCratesProcEmptyAsync();
        Task<List<CrateModel>> GetDailyCratesProcOutAsync();
        Task<List<CrateModel>> CheckCratesPartyOutAsync(string growerGroupName, string flag);
        Task<List<CrateModel>> CheckCratesPartySubOutAsync(string growerName, string growerGroupName, string flag);
        Task<CrateModel> AddCrateOutAsync(CrateModel model);
        Task<CrateModel> UpdateCrateIssueOutAsync(CrateModel model);
        Task<CrateModel> DeleteCrateIssueAsync(int id, CrateModel model);
        Task<List<CrateModel>> GenerateCratePreviewAsync(int id, CrateModel model);
        // Added based on CrateSummary usage
        Task<List<CrateModel>> GetCrateOrderNoAsync(); 
        Task<List<CrateModel>> GetallCrateMarksAsync();

        // New methods for Phase 2 Refactoring
        Task<CrateModel?> GetCrateIssueDetAsync(int selectedGrowerId);
        Task<CrateModel?> GetCratePrivs2Async(string Ugroup);
        Task<List<CrateModel>> GetallCrateflagallAsync();
        Task<List<CrateModel>> GetCrateordersEmptyAsync();
        Task<CrateModel?> GenerateCrateReportPdfAsync(CrateModel model);
        
        Task<List<CrateModel>> GenerateCrateReportDelAsync(CrateModel model);
        Task<List<CrateModel>> GenerateRawCrateReportAsync(CrateModel model, int unit);
    }
}
