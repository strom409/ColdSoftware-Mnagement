using ColdStoreManagement.BLL.Models;
using ColdStoreManagement.BLL.Models.Auth;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface IAuthService
    {
        /// <summary>
        /// Check user authentication.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<LoginResultModel?> CheckUser(LoginModel model);
        /// <summary>
        /// Generates a JWT token for the authenticated user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        string GenerateJwtToken(LoginResultModel user);


        Task<CompanyModel?> UpdateUserPassword(UpdateUserPasswordRequest companyModel);

        Task<CompanyModel?> UpdateUserPasswordAsync(
            string username,
            string oldPassword,
            string newPassword);
    }
}
