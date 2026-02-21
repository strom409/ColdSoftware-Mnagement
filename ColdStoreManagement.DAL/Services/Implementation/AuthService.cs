using ColdStoreManagement.BLL.Models.Auth;
using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.DAL.Helper;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public class AuthService(SQLHelperCore sql,
        IConfiguration configuration) : BaseService(sql), IAuthService
    {
        private readonly IConfiguration _config = configuration;

        public async Task<LoginResultModel?> CheckUser(LoginModel model)
        {
            if (model == null) return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "checkuser",
                new SqlParameter("@uname", model.GlobalUserName),
                new SqlParameter("@pwd", model.UserPassword),
                new SqlParameter("@unit", model.GlobalUnitName)
            );

            var resut = new LoginResultModel()
            {
                GlobalUserName = model.GlobalUserName,
                GlobalUnitName = model.GlobalUnitName,
            };
            await FillValidationAsync(resut);
            return resut;
        }

        public string GenerateJwtToken(LoginResultModel user)
        {
            // 1. Setup the Secret Key (Should be in appsettings.json)
            var jwtSettings = _config.GetSection("JWTConfigs");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 2. Define Claims (This is what your BaseController properties will read)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.GlobalUserId.ToString()),
                new Claim(ClaimTypes.Name, user.GlobalUserName),
                new Claim(ClaimTypes.Role, user.GlobalUserGroup),
                new Claim(ClaimTypes.Sid, user.GlobalUnitId.ToString()),
                new Claim("UnitName", user.GlobalUnitName ?? "")
            };

            // 3. Create the Token
            var token = new JwtSecurityToken(
                issuer: jwtSettings["ValidIssuer"],
                audience: jwtSettings["ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddSeconds(Convert.ToDouble(jwtSettings["ExpirySeconds"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<CompanyModel?> UpdateUserPassword(
            UpdateUserPasswordRequest model)
        {
            if (model == null)
                return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "UpdateuserPassword",
                new SqlParameter("@userid", model.UserId),
                new SqlParameter("@Password", model.NewPassword),
                new SqlParameter("@Globalusername", model.GlobalUserName)
            );

            var companyModel = new CompanyModel()
            {
                UserId = model.UserId,
                GlobalUserName = model.GlobalUserName,
                GlobalUnitName = model.GlobalUnitName ?? string.Empty,
            };
            await FillValidationAsync(companyModel);

            return companyModel;
        }

        public async Task<CompanyModel?> UpdateUserPasswordAsync(
            string username,
            string oldPassword,
            string newPassword)
        {

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "UpdateuserPasswordcheck",
                new SqlParameter("@Globalusername", username),
                new SqlParameter("@Opassword", oldPassword),
                new SqlParameter("@Npassword", newPassword)
            );

            var companyModel = new CompanyModel()
            {
                GlobalUserName = username
            };
            await FillValidationAsync(companyModel);

            return companyModel;
        }


    }
}
