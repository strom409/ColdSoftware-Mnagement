using ColdStoreManagement.BLL.Models;
using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.DAL.Helper;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public class VehicleInfoService(SQLHelperCore sql) : BaseService(sql), IVehicleInfoService
    {
        public async Task<List<VehInfoModel>> GetAllVehGroup()
        {
            const string query = @"select vid as Id,Vehno as Vehno,drivername as Driver,
                                    contactno as Contact,status,vehtype 
                                from vehinfo where flagdeleted=0 order by vid ";

            var list = new List<VehInfoModel>();

            using (var ds = await _sql.ExecuteDatasetAsync(CommandType.Text, query))
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new VehInfoModel
                    {
                        Vehid = Convert.ToInt32(row["Id"]),
                        Vehno = row["Vehno"]?.ToString(),
                        VehDriver = row["Driver"]?.ToString(),
                        VehContact = row["Contact"]?.ToString(),
                        Vehtype = row["vehtype"]?.ToString(),
                        VehStatus = Convert.ToBoolean(row["status"])
                    });
                }
            }

            return list;
        }
        public async Task<VehInfoModel?> Getvehid(int vehid)
        {
            const string sql = @"select 
                Vehno, drivername as Driver,
                contactno as Contact,
                status, vehtype
              from vehinfo
              where flagdeleted = 0
                and vid = @Mid";

            using var ds = await _sql.ExecuteDatasetAsync(CommandType.Text, sql, new SqlParameter("@Mid", vehid));
            if (ds.Tables[0].Rows.Count == 0)
                return null;

            var row = ds.Tables[0].Rows[0];

            return new VehInfoModel
            {
                Vehno = row["Vehno"]?.ToString(),
                VehDriver = row["Driver"]?.ToString(),
                VehContact = row["Contact"]?.ToString(),
                Vehtype = row["vehtype"]?.ToString()
            };
        }
        public async Task<CompanyModel?> Addveh(VehInfoModel model)
        {
            if (model == null)
                return null;

            SqlParameter[] parameters =
            {
                new SqlParameter("@Vehno", model.Vehno),
                new SqlParameter("@Drname", model.VehDriver),
                new SqlParameter("@drcontact", model.VehContact),
                new SqlParameter("@Vrtype", model.Vehtype),
                new SqlParameter("@Userid", model.Userid)
            };
            await _sql.ExecuteNonQueryAsync(
                 CommandType.StoredProcedure,
                 "Addveh",
                 parameters);

            // Read validation result
            var result = new CompanyModel()
            {
                Vehno = model.Vehno,
                VehDriver = model.VehDriver,
                VehContact = model.VehContact,
                Vehntype = model.Vehtype,
                Userid = model.Userid ?? 0
            };
            await FillValidationAsync(result);
            return result;
        }
        public async Task<CompanyModel?> UpdateVeh(VehInfoModel model)
        {
            if (model == null)
                return null;

            SqlParameter[] parameters =
            {
                new SqlParameter("@Vehno", model.Vehno),
                new SqlParameter("@Drname", model.VehDriver),
                new SqlParameter("@drcontact", model.VehContact),
                new SqlParameter("@vrtype", model.Vehtype),
                new SqlParameter("@Userid", model.Userid),
                new SqlParameter("@vid", model.Vehid)
            };
            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "UpdateVeh",
                parameters);

            // Read validation result
            var result = new CompanyModel()
            {
                Vehno = model.Vehno,
                VehDriver = model.VehDriver,
                VehContact = model.VehContact,
                Vehntype = model.Vehtype,
                Userid = model.Userid ?? 0
            };
            await FillValidationAsync(result);
            return result;
        }
        public async Task<bool> UpdatevehStatus(int id)
        {
            await _sql.ExecuteNonQueryAsync(
               CommandType.StoredProcedure,
               "UpdatevehStatus",
               new SqlParameter("@Mid", id));

            return true;
        }
        public async Task<CompanyModel?> DeleteVeh(int id, CompanyModel model)
        {
            if (model == null)
                return null;

            await _sql.ExecuteNonQueryAsync(
               CommandType.StoredProcedure,
               "DeleteVehGroup",
               new SqlParameter("@Mid", model.Vehid));

            // Read validation result
            await FillValidationAsync(model);
            return model;
        }
        public async Task<List<CompanyModel>> GetallItemGroup()
        {
            const string sql = @"select 
                    b.mastername,
                    a.itemid,
                    a.itemname,
                    a.itemuom,
                    a.itemgst,
                    a.itemhsn,
                    a.status,
                    a.createdon
                from masterItems a
                join PurchaseGroup b on a.prGroup = b.id
                where a.flagdeleted = 0
                order by a.itemid";

            var list = new List<CompanyModel>();
            using (var ds = await _sql.ExecuteDatasetAsync(CommandType.Text, sql))
            {
                if (ds.Tables[0].Rows.Count == 0)
                    return list;

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new CompanyModel
                    {
                        PurchGrp = row["mastername"]?.ToString(),
                        Itemid = Convert.ToInt32(row["itemid"]),
                        PurchaseItemName = row["itemname"]?.ToString(),
                        ItemUom = row["itemuom"]?.ToString(),
                        ItemGst = row["itemgst"]?.ToString(),
                        ItemHsn = row["itemhsn"]?.ToString(),
                        ItemStatus = Convert.ToBoolean(row["status"]),
                        ItemCreatedDate = row["createdon"] as DateTime? ?? DateTime.MinValue
                    });
                }
            }

            return list;
        }
        
    }
}
