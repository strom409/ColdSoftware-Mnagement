using ColdStoreManagement.BLL.Data.UserData;
using ColdStoreManagement.BLL.Models;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface IUserService
    {
        Task<CompanyModel?> AddAccountName(CompanyModel model);
        Task<List<CompanyModel>> GetallAccountlist();
       
        Task<List<CompanyModel>> GetUserlist();
        Task<CompanyModel?> AdduserName(CompanyModel model);
        Task<CompanyModel?> UpdateuserName(CompanyModel model);
        Task<CompanyModel?> UpdateUserPasswordAsync(
            CompanyModel model,
            string username,
            string oldPassword,
            string newPassword);
       
    }
}
