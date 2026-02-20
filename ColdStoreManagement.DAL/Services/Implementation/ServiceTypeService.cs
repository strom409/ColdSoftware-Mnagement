using ColdStoreManagement.BLL.Models;
using ColdStoreManagement.DAL.Helper;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public class ServiceTypeService(SQLHelperCore sql) : BaseService(sql), IServiceTypeService
    {
        public async Task<bool> DoesServiceExistAsync(string serviceName)
        {
            var result = await _sql.ExecuteScalarAsync(
                "SELECT COUNT(1) FROM servicetypes WHERE sname = @Name",
                CommandType.Text,
                new SqlParameter("@Name", serviceName));

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> AddService(CompanyModel model)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                @"INSERT INTO dbo.servicetypes (id, sname, sdescrip)
              VALUES (
                (SELECT ISNULL(MAX(id) + 1, 1) FROM servicetypes),
                @Stname,
                @Stdetails)",
                new SqlParameter("@Stname", model.Stname),
                new SqlParameter("@Stdetails", model.Stdetails));

            return true;
        }        
        public async Task<bool> UpdateService(int id, CompanyModel model)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                @"UPDATE dbo.servicetypes
              SET sname = @Stname,
                  sdescrip = @Stdetails
              WHERE id = @Id",
                new SqlParameter("@Id", id),
                new SqlParameter("@Stname", model.Stname),
                new SqlParameter("@Stdetails", model.Stdetails));

            return true;
        }
        public async Task<bool> DeleteService(int id)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                "DELETE dbo.Servicetypes WHERE Id = @Id",
                new SqlParameter("@Id", id));

            return true;
        }


        public async Task<List<CompanyModel>> GetServices()
        {
            const string query = "SELECT RTRIM(sname) AS sname FROM servicetypes";

            var ds = await _sql.ExecuteDatasetAsync(CommandType.Text, query);
            var list = new List<CompanyModel>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(new CompanyModel
                {
                    Atype = row["sname"]?.ToString()
                });
            }

            return list;
        }
        public async Task<List<CompanyModel>> GetServicesFromAgreement(string selectedPurchase)
        {
            const string query = @"
            SELECT RTRIM(sname) AS sname
            FROM servicetypes
            WHERE id IN (
                SELECT [service]
                FROM growerAgreement
                WHERE mainid IN (
                    SELECT partyid
                    FROM party
                    WHERE partytypeid + '-' + partyname = @Partyid
                )
            )";

            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                query,
                new SqlParameter("@Partyid", selectedPurchase));

            var list = new List<CompanyModel>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(new CompanyModel
                {
                    Atype = row["sname"]?.ToString()
                });
            }

            return list;
        }
        public async Task<List<CompanyModel>> GetAllServices()
        {
            const string query = "SELECT id, sname, sdescrip FROM dbo.servicetypes";

            var ds = await _sql.ExecuteDatasetAsync(CommandType.Text, query);
            var list = new List<CompanyModel>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(new CompanyModel
                {
                    Skid = Convert.ToInt32(row["id"]),
                    Stname = row["sname"]?.ToString(),
                    Stdetails = row["sdescrip"]?.ToString()
                });
            }

            return list;
        }
    }

}