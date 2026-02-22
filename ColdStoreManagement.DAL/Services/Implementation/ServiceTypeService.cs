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
        public async Task<List<ServiceTypesModel>> GetServices()
        {
            const string query = "SELECT RTRIM(sname) AS Name FROM servicetypes";
            return await _sql.ExecuteReaderAsync<ServiceTypesModel>(
                query,
                CommandType.Text
            );
        }

        public async Task<List<ServiceTypesModel>> GetServicesFromAgreement(string selectedPurchase)
        {
            const string query = @"
                    SELECT 
                        s.id AS Id,
                        RTRIM(s.sname) AS ServiceName,
                        s.sdescrip AS Stdetails,
                        s.flag AS Flag
                    FROM servicetypes s
                    INNER JOIN growerAgreement g ON s.id = g.service
                    INNER JOIN party p ON g.mainid = p.partyid
                    WHERE p.partytypeid + '-' + p.partyname = @Partyid";

            return await _sql.ExecuteReaderAsync<ServiceTypesModel>(
                query,
                CommandType.Text,
                new SqlParameter("@Partyid", selectedPurchase)
            );
        }
        //public async Task<List<ServiceTypesModel>> GetServicesFromAgreement(string selectedPurchase)
        //{
        //    const string query = @"
        //    SELECT RTRIM(sname) AS Name
        //    FROM servicetypes
        //    WHERE id IN (
        //        SELECT [service]
        //        FROM growerAgreement
        //        WHERE mainid IN (
        //            SELECT partyid
        //            FROM party
        //            WHERE partytypeid + '-' + partyname = @Partyid
        //        )
        //    )";
        //    return await _sql.ExecuteReaderAsync<ServiceTypesModel>(
        //        query,
        //        CommandType.Text,
        //        new SqlParameter("@Partyid", selectedPurchase)
        //    );
        //}


        public async Task<List<ServiceTypesModel>> GetAllServices()
        {
            const string query = "SELECT id, sname, sdescrip FROM dbo.servicetypes";
            using var ds = await _sql.ExecuteDatasetAsync(CommandType.Text, query);

            var list = new List<ServiceTypesModel>();
            if (ds.Tables[0].Rows.Count == 0)
                return list;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(new ServiceTypesModel
                {
                    Id = Convert.ToInt32(row["id"]),
                    ServiceName = row["sname"]?.ToString() ?? string.Empty,
                    Stdetails = row["sdescrip"]?.ToString()
                });
            }

            return list;
        }
        public async Task<ServiceTypesModel?> GetServiceById(int id)
        {
            return await _sql.ExecuteSingleAsync<ServiceTypesModel>(
               "SELECT id, sname as Name, sdescrip as Stdetails FROM dbo.servicetypes WHERE id = @Id",
               CommandType.Text,
               new SqlParameter("@Id", id)
           );
        }

        public async Task<bool> AddService(ServiceTypesModel model)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                @"INSERT INTO dbo.servicetypes (id, sname, sdescrip)
                VALUES (
                    (SELECT ISNULL(MAX(id) + 1, 1) FROM servicetypes),
                    @Stname,
                    @Stdetails)",
                new SqlParameter("@Stname", model.ServiceName),
                new SqlParameter("@Stdetails", model.Stdetails));

            return true;
        }        
        public async Task<bool> UpdateService(int id, ServiceTypesModel model)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                @"UPDATE dbo.servicetypes
                  SET sname = @Stname,
                      sdescrip = @Stdetails
                  WHERE id = @Id",
                new SqlParameter("@Id", id),
                new SqlParameter("@Stname", model.ServiceName),
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
        
    }

}