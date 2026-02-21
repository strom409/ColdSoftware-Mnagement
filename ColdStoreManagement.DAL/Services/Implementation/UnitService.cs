using ColdStoreManagement.BLL.Models;
using ColdStoreManagement.DAL.Helper;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public class UnitService(SQLHelperCore sql) : BaseService(sql), IUnitService
    {
        public async Task<List<UnitMasterModel>> GetAllAsync()
        {
            const string query = "SELECT * FROM dbo.Unit_master";
            using var ds = await _sql.ExecuteDatasetAsync(CommandType.Text, query);
            var units = new List<UnitMasterModel>();

            if (ds.Tables.Count == 0)
                return units;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                units.Add(new UnitMasterModel
                {
                    Id = Convert.ToInt32(row["id"]),
                    UnitCode = row["Ucode"]?.ToString() ?? string.Empty,
                    UnitName = row["UnitName"]?.ToString() ?? string.Empty,
                    Stat = row["Stat"]?.ToString(),
                    Details = row["details"]?.ToString()
                });
            }

            return units;
        }
        public async Task<UnitMasterModel?> GetByIdAsync(int id)
        {
            return await _sql.ExecuteSingleAsync<UnitMasterModel>(
              @"select id,	
                UnitName,
                Ucode as UnitCode,
                Stat, details
                from dbo.Unit_master where Id=@Id",
              CommandType.Text,
              new SqlParameter("@Id", id)
           );
        }
        public async Task<UnitMasterModel?> GetByNameAsync(string unitName)
        {
            return await _sql.ExecuteSingleAsync<UnitMasterModel>(
                 @"select id,	
                    UnitName,
                    Ucode as UnitCode,
                    Stat, details
                    from dbo.Unit_master where UnitName=@UnitName",
                 CommandType.Text,
                 new SqlParameter("@UnitName", unitName)
              );
        }
        public async Task<bool> AddAsync(UnitMasterModel model)
        {
            const string query = @"
            INSERT INTO dbo.Unit_master (Id, Ucode, UnitName, Stat, details)
            VALUES (
                (SELECT ISNULL(MAX(id) + 1, 1) FROM Unit_master),
                @Ucode, @UnitName, @Stat, @Details
            )";

            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                query,
                new SqlParameter("@Ucode", model.UnitCode),
                new SqlParameter("@UnitName", model.UnitName),
                new SqlParameter("@Stat", model.Stat),
                new SqlParameter("@Details", model.Details)
            );

            return true;
        }
        public async Task<bool> UpdateAsync(int id, UnitMasterModel model)
        {
            const string query = @"
            UPDATE dbo.Unit_master
            SET
                Ucode = @Ucode,
                UnitName = @UnitName,
                Stat = @Stat,
                details = @Details
            WHERE id = @Id";

            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                query,
                new SqlParameter("@Id", id),
                new SqlParameter("@Ucode", model.UnitCode),
                new SqlParameter("@UnitName", model.UnitName),
                new SqlParameter("@Stat", model.Stat),
                new SqlParameter("@Details", model.Details)
            );

            return true;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM dbo.Unit_master WHERE id = @Id";
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                query,
                new SqlParameter("@Id", id)
            );

            return true;
        }

    }
}
