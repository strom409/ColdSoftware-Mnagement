using ColdStoreManagement.BLL.Models;
using ColdStoreManagement.BLL.Models.Chamber;
using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.BLL.Models.DTOs;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface IChamberService
    {
        // Chamber Retrieval Methods 
        Task<List<ChamberModel>> GetAllChambersAsync();
        Task<ChamberModel?> GetChamberDetailByIdAsync(int id);
        Task<List<ChamberStockModel>> GetChambersInAsync();
        Task<List<ChamberModel>> GetAllChambersList(int unitId);
        Task<ChamberEditModel?> GetChamberByIdAsync(int chamberId);
        Task<List<ChamberModelVM>> GetAllChamberDetailsAsync(); 
        Task<List<ChamberAllocationViewModel>> GetChambersAsync();

        // Allocation Management
        Task ReleaseAllocationAsync(int growerId, int chamberId);
        Task<CompanyModel?> SaveChamberAllocationAsync(SaveChamberAllocationRequest request, string currentUser);
        Task<CompanyModel?> UpdateChamberAllocationAsync(int allocationId, decimal chamberAllocation);
        Task<CompanyModel?> DeleteAllocation(int selectedGrowerId, CompanyModel companyModel);
        Task<List<ChamberAllocationModel>> GetChallocation(int chamberid);
        Task<ChamberAllocationModel?> Getallocationdet(int selectedno);

        // Dashboard and Permissions
        Task<DashboardTotalsModel?> GetDashboardTotalsAsync();
        Task<UserPrivModel?> GetAllocationpriv(string userGroup);
        Task<UserPrivModel?> GrowerPriv(string Ugroup);

        // Chamber & Grower CRUD/Status
        Task<ChamberDto?> AddNewChamber(ChamberDto model);
        Task<ChamberUpdateDto?> UpdateChamber(int chamberid, ChamberUpdateDto model);
        Task<bool> LockUnlockChamber(int chamberId);
        Task<bool> GenchamberAgg(int id);
        Task<CompanyModel?> DeleteGrowerGroup(int id, CompanyModel companyModel);
        Task<bool> UpdateGrowerStatus(int id);
        Task<CompanyModel?> AddGrowerSubdirect(CompanyModel EditModel);
        Task<bool> GenGrowerAgg(int id);

        // Stock and Detailed Reports
        Task<List<ChamberPartyStockModel>> GetChamberDetailsAsync(int chamberid);
        Task<List<ChamberGrowerStockModel>> GetChamberGrowerDetailsAsync(int partyId, int chamberId);
        Task<List<CompanyModel>> GetallStockChamber(int GrowerId);
        Task<List<CompanyModel>> GetallStockGrowerlots(int GrowerId);

        // Summary Methods
        Task<CompanyModel?> Getchsummary(int chamberid);
        Task<List<ChamberAllocationModel>> GetchsummaryGrowerdet(int chamberid);
        Task<CompanyModel?> GetchsummaryGrower(int chamberid);

        // Grower and Party Lookups
        Task<List<GrowerModel>> GetallGrowers();
        Task<CompanyModel?> GetGrowerIdAsync(int Growerid);
        Task<List<ChamberModel>> GetallChambersParty(int unitid, string GrowerName);
        Task<List<ChamberModel>> GetallChambersIn();
        Task<List<ChamberModel>> GetallChambersPartydemand(int unitid, string GrowerName);
        Task<List<ChamberModel>> GetallChambersSub(int unitid, string GrowerName, string subgrower);
        Task<List<GrowerModel>> GetallchamberGrowerlist(string GrowerName);
        Task<List<GrowerModel>> GetallGrowerlist();
        Task<List<GrowerModel>> GetSalesGrower();
        Task<List<GrowerModel>> GetallGrowerlistwithin();
        Task<GrowerModel?> GetDemandGrower(string demandirn);
        Task<List<GrowerModel>> GetallGrowerlistnew();

        // Demand Processing
        Task<bool> SaveDemandsToDatabase(List<string> demandIRNs, CompanyModel EditModel, int Unit);

        // Challan Management
        Task<CompanyModel?> AddChallanGrower(CompanyModel EditModel);
        Task<List<CompanyModel>> GetallChallanlist(string ChallanGroup);

    }
}
