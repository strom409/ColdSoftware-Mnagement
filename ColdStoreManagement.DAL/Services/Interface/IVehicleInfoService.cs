using ColdStoreManagement.BLL.Models;
using ColdStoreManagement.BLL.Models.Company;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface IVehicleInfoService
    {
        Task<List<VehInfoModel>> GetAllVehGroup();
        Task<VehInfoModel?> Getvehid(int vehid);
        Task<CompanyModel?> Addveh(VehInfoModel model);
        Task<CompanyModel?> UpdateVeh(VehInfoModel model);
        Task<bool> UpdatevehStatus(int id);
        Task<CompanyModel?> DeleteVeh(int id, CompanyModel model);
        Task<List<CompanyModel>> GetallItemGroup();
    }
}
