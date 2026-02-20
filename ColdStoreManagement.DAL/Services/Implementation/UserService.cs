using ColdStoreManagement.BLL.Models;
using ColdStoreManagement.DAL.Helper;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public class UserService(SQLHelperCore sql) : BaseService(sql), IUserService
    {
        #region ---------- Account ----------

        public async Task<CompanyModel?> AddAccountName(CompanyModel model)
        {
            if (model == null) return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "AddAccountName",
                new SqlParameter("@GrowerGrp", model.AccountGroup),
                new SqlParameter("@GrowerName", model.AccountName),
                new SqlParameter("@GrowerAddress", model.AccountAddress),
                new SqlParameter("@GrowerCity", model.AccountCity),
                new SqlParameter("@GrowerPincode", model.AccountPincode),
                new SqlParameter("@GrowerState", model.AccountState),
                new SqlParameter("@GrowerStatecode", model.AccountStatecode),
                new SqlParameter("@GrowerEmail", model.AccountEmail),
                new SqlParameter("@GrowerPan", model.AccountPan),
                new SqlParameter("@GrowerGst", model.AccountGst),
                new SqlParameter("@GrowerContact", model.AccountContact),
                new SqlParameter("@GrowerStatus", model.AccountActive),
                new SqlParameter("@Createdby", 1),
                new SqlParameter("@Updatedby", 1),
                new SqlParameter("@GrowerVillage", model.AccountVillage),
                new SqlParameter("@graceperiod", "1"),
                new SqlParameter("@country", model.AccountCountry),
                new SqlParameter("@crateQtyLimit", 0),
                new SqlParameter("@crateQtypercent", 0),
                new SqlParameter("@GrowerisLock", model.AccountLock),
                new SqlParameter("@Growerisflexi", "0"),
                new SqlParameter("@growerRemarks", model.AccountRemarks),
                new SqlParameter("@growerapproval", model.AccountApproval)
            );

            await FillValidationAsync(model);
            return model;
        }

        public async Task<List<CompanyModel>> GetallAccountlist()
        {
            const string query = "select rtrim(subname) as subname from AccountSubGroups";
            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                query);

            var result = new List<CompanyModel>();

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return result;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                result.Add(new CompanyModel
                {
                    AccountGroup = row["subname"]?.ToString()
                });
            }

            return result;
        }


        #endregion

        #region ---------- User ----------

        public async Task<List<CompanyModel>> GetUserlist()
        {
            const string query = "select rtrim(username) as username from users where userid in(select distinct userid from outward)";
            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                query);

            var result = new List<CompanyModel>();

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return result;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                result.Add(new CompanyModel
                {
                    UserName = row["username"]?.ToString()
                });
            }

            return result;
        }

        public async Task<CompanyModel?> AdduserName(CompanyModel model)
        {
            if (model == null) return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "AdduserName",
                new SqlParameter("@Username", model.UserName),
                new SqlParameter("@Password", model.UserPassword),
                new SqlParameter("@Useremail", model.Useremail),
                new SqlParameter("@UserGroup", model.GlobalUserGroup),
                new SqlParameter("@Userstatus", model.UserStatus),
                new SqlParameter("@Unit1", model.Unit1),
                new SqlParameter("@Unit2", model.Unit2),
                new SqlParameter("@Unit3", model.Unit3),
                new SqlParameter("@Unit4", model.Unit4),
                new SqlParameter("@Unit5", model.Unit5),
                new SqlParameter("@Globalusername", model.GlobalUserName)
            );

            await FillValidationAsync(model);
            return model;
        }

        public async Task<CompanyModel?> UpdateuserName(CompanyModel model)
        {
            if (model == null) return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "UpdateuserName",
                new SqlParameter("@userid", model.Userid),
                new SqlParameter("@Username", model.UserName),
                new SqlParameter("@Useremail", model.Useremail),
                new SqlParameter("@UserGroup", model.GlobalUserGroup),
                new SqlParameter("@Userstatus", model.UserStatus),
                new SqlParameter("@Unit1", model.Unit1),
                new SqlParameter("@Unit2", model.Unit2),
                new SqlParameter("@Unit3", model.Unit3),
                new SqlParameter("@Unit4", model.Unit4),
                new SqlParameter("@Unit5", model.Unit5),
                new SqlParameter("@Globalusername", model.GlobalUserName)
            );

            await FillValidationAsync(model);
            return model;
        }
       
        public async Task<CompanyModel?> UpdateUserPasswordAsync(
            CompanyModel model,
            string username,
            string oldPassword,
            string newPassword)
        {
            if (model == null) return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "UpdateuserPasswordcheck",
                new SqlParameter("@Globalusername", username),
                new SqlParameter("@Opassword", oldPassword),
                new SqlParameter("@Npassword", newPassword)
            );

            await FillValidationAsync(model);
            return model;
        }

        #endregion

    }
}
