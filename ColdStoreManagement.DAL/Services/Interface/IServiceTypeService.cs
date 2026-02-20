using ColdStoreManagement.BLL.Models;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface IServiceTypeService
    {
        Task<bool> AddService(CompanyModel model);
        Task<bool> UpdateService(int id, CompanyModel model);
        Task<bool> DeleteService(int id);
        Task<bool> DoesServiceExistAsync(string serviceName);

        Task<List<CompanyModel>> GetServices();
        Task<List<CompanyModel>> GetServicesFromAgreement(string selectedPurchase);
        Task<List<CompanyModel>> GetAllServices();
    }
}
