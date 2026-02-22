using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.BLL.Models.DTOs;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface ICompanyService
    {
        // Company
        Task<CompanyDto?> GetCompanyByIdAsync(int companyId = 1);
        Task<bool> EditCompany(int id, CompanyDto companyDto);

        // Building
        Task<List<BuildingModel>> GetAllBuildingsAsync();
        Task<BuildingModel?> GetBuildingById(int id);
        Task<BuildingModel?> GetBuildingByName(string buildingName);
        Task<bool> AddBuildingAsync(BuildingModel model);
        Task<bool> UpdateBuildingAsync(int id, BuildingModel model);
        Task<bool> DeleteBuildingAsync(int id);
    }
}
