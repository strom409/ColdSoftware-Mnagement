using ColdStoreManagement.BLL.Models;
using ColdStoreManagement.DAL.Helper;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public class ChamberService(SQLHelperCore sql, IConfiguration _configuration) : BaseService(sql), IChamberService
    {
        #region ---------- Chamber / Slot ----------

        public async Task<CompanyModel?> AddNewChamber(CompanyModel model)
        {
            if (model == null) return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "AddNewChamber",
                new SqlParameter("@ctype", model.ChamberType),
                new SqlParameter("@unit", model.Unitname),
                new SqlParameter("@Capacity", model.Capacity),
                new SqlParameter("@User", model.GlobalUserName)
            );

            await FillValidationAsync(model);
            return model;
        }

        public async Task<CompanyModel?> AddSlot(CompanyModel model)
        {
            if (model == null) return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "addslot",
                new SqlParameter("@Sdate", model.calendardate),
                new SqlParameter("@partyid", model.GrowerGroupName),
                new SqlParameter("@growerid", model.GrowerName),
                new SqlParameter("@Contact", model.GrowerContact),
                new SqlParameter("@Qty", model.SlotQty),
                new SqlParameter("@ttime", model.calendartime)
            );

            await FillValidationAsync(model);
            return model;
        }

        public async Task<List<CompanyModel>> GetallChambers()
        {
            List<CompanyModel> banks = new List<CompanyModel>();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                const string query = "select * from dbo.Chamber order by chamberid";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = CommandType.Text;
                    await con.OpenAsync();
                    using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            banks.Add(new CompanyModel
                            {
                                ChamberId = rdr.GetInt32(rdr.GetOrdinal("chamberid")),
                                ChamberName = rdr["chambername"].ToString(),
                                ChamberType = rdr["ctype"].ToString(),
                                Unitname = rdr["unitname"].ToString(),
                                Capacity = rdr.GetInt32(rdr.GetOrdinal("Qty")),
                                chamberstatus = rdr.GetBoolean(rdr.GetOrdinal("status"))
                            });
                        }
                    }
                }
            }
            return banks;
        }
        #endregion
    }
}
