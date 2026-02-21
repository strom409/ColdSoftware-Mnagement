using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.BLL.Models.Crate;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface ICrateService
    {
        // ========= CrateModel Based (New) =========
        Task<List<CrateModel>> GenerateCrateReportModelAsync(CrateModel model);
        Task<List<CrateModel>> GetCrateSummaryMainModelAsync();
        Task<List<CrateModel>> GetDailyCratesProcEmptyAsync();
        Task<List<CrateModel>> GetDailyCratesProcOutAsync();
        Task<List<CrateModel>> CheckCratesPartyOutModelAsync(string growerGroupName, string flag);
        Task<List<CrateModel>> CheckCratesPartySubOutModelAsync(string growerName, string growerGroupName, string flag);
        Task<CrateModel> AddCrateOutAsync(CrateModel model);
        Task<CrateModel> UpdateCrateIssueOutAsync(CrateModel model);
        Task<CrateModel> DeleteCrateIssueAsync(int id, CrateModel model);
        Task<List<CrateModel>> GenerateCratePreviewAsync(int id, CrateModel model);
        Task<List<CrateModel>> GetCrateOrderNoAsync(); 
        Task<List<CrateModel>> GetallCrateMarksAsync();
        Task<CrateModel?> GetCrateIssueDetAsync(int selectedGrowerId);
        Task<CrateModel?> GetCratePrivs2Async(string Ugroup);
        Task<List<CrateModel>> GetallCrateflagallAsync();
        Task<List<CrateModel>> GetCrateordersEmptyAsync();
        Task<CrateModel?> GenerateCrateReportPdfAsync(CrateModel model);
        Task<List<CrateModel>> GenerateCrateReportDelAsync(CrateModel model);
        Task<List<CrateModel>> GenerateRawCrateReportAsync(CrateModel model, int unit);

        // ========= CompanyModel Based (Original) =========

        // ========= Crate Type =========
        Task<bool> DoesCrateTypeExistAsync(string name);
        Task<bool> AddCrateTypeAsync(CompanyModel model);
        Task<bool> UpdateCrateTypeAsync(CompanyModel model);
        Task<bool> DeleteCrateTypeAsync(int id);

        // ========= Crate Flag =========
        Task<bool> DoesCrateFlagExistAsync(string name);
        Task<bool> AddCrateFlagAsync(CrateFlags model);
        Task<bool> UpdateCrateFlagAsync(CrateFlags model);
        Task<bool> DeleteCrateFlagAsync(int id);

        // ========= Issue / Transaction =========
        Task<CompanyModel?> AddCrateIssueAsync(CompanyModel model);

        // ========= Lookups =========
        Task<int> GetMaxCrateIssueIdAsync();
        Task<List<CompanyModel>> GetAllCrateMarksAsync();

        // ========= Daily =========
        Task<List<CompanyModel>> GetDailyCratesAsync();
        Task<List<CompanyModel>> GetDailyCratesAdjustmentAsync();
        Task<List<CompanyModel>> GetDailyCratesOutAsync();
        Task<List<CompanyModel>> GetDailyCratesEmptyAsync();

        Task<List<CompanyModel>> GetDailyCratesByDateAsync(DateTime from, DateTime to);
        Task<List<CompanyModel>> GetDailyCratesAdjByDateAsync(DateTime from, DateTime to);
        Task<List<CompanyModel>> GetDailyCratesOutByDateAsync(DateTime from, DateTime to);
        Task<List<CompanyModel>> GetDailyCratesEmptyByDateAsync(DateTime from, DateTime to);

        // ========= Summary =========
        Task<List<CompanyModel>> GetCrateSummaryMainAsync();
        Task<List<CompanyModel>> GetCrateSummarySubGrowerAsync(string partyName);

        // ========= Reports =========
        Task<List<CompanyModel>> GenerateCrateReportAsync(CompanyModel filter);

        // ========= Checks =========
        Task<List<CompanyModel>> CheckCratesPartyAsync(string party);
        Task<List<CompanyModel>> CheckCratesAgreementAsync(string party);
        Task<List<CompanyModel>> CheckCratesAgreementOnVQtyAsync(string party);

        Task<List<CompanyModel>> CheckCratesPartyOutAsync(string party, string flag);
        Task<List<CompanyModel>> CheckCratesPartyEmptyAsync(string party);

        Task<List<CompanyModel>> CheckCratesPartySubAsync(string party, string grower);
        Task<List<CompanyModel>> CheckCratesPartySubEmptyAsync(string party, string grower);
        Task<List<CompanyModel>> CheckCratesPartySubOutAsync(string party, string grower, string flag);
    }
}
