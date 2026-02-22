using ColdStoreManagement.BLL.Models;
using ColdStoreManagement.BLL.Models.Chamber;
using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.BLL.Models.DTOs;
using ColdStoreManagement.DAL.Helper;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public class ChamberService(SQLHelperCore sql,
        IConfiguration configuration) : BaseService(sql), IChamberService
    {
        private readonly IConfiguration _config = configuration;

        // Chamber Retrieval Methods 
        public async Task<List<ChamberModel>> GetAllChambersAsync()
        {
            const string sql = @"SELECT 
                    chamberid     AS ChamberId,
                    chambername   AS ChamberName,
                    ctype         AS ChamberType,
                    unitname      AS UnitName,
                    Qty           AS Capacity,
                    status        AS Status
                FROM dbo.Chamber
                ORDER BY chamberid ";

            return await _sql.ExecuteReaderAsync<ChamberModel>(sql, CommandType.Text);
        }
        public async Task<List<ChamberModelVM>> GetAllChamberDetailsAsync()
        {
            string sql = @"SELECT 
                c.chamberid AS ChamberId,
                c.chambername AS ChamberName,
                ISNULL(c.ChamberType, 'NULL') AS [Type],
                ISNULL(c.Floor, 0) AS Floor,
                ISNULL(c.CFT, 0) AS CFT,
                ISNULL(c.ActualSize, 0) AS ActualSize,
                ISNULL(c.UsableSize, 0) AS UsableSize,
                ISNULL(c.SquareFeet, 0) AS Square,
                c.IsLocked AS isLocked,
                c.Status AS Status,
                ISNULL(c.BuildingId, 0) AS BuildingId
            FROM Chamber AS c
            LEFT JOIN Building AS b ON c.BuildingId = b.Id
            WHERE c.FlagDeleted = 0;";

            var records = new List<ChamberModelVM>();
            var ds = await _sql.ExecuteDatasetAsync(sql, CommandType.Text);
            if (ds.Tables.Count == 0)
                return records;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                records.Add(new ChamberModelVM
                {
                    ChamberId = row["chamberid"] != DBNull.Value ? Convert.ToInt32(row["chamberid"]) : 0,
                    ChamberName = row["chambername"]?.ToString() ?? "",
                    Type = row["ChamberType"]?.ToString() ?? "NULL",
                    Floor = row["Floor"] != DBNull.Value ? Convert.ToInt32(row["Floor"]) : 0,
                    CFT = row["CFT"] != DBNull.Value ? Convert.ToDecimal(row["CFT"]) : 0,
                    ActualSize = row["ActualSize"] != DBNull.Value ? Convert.ToDecimal(row["ActualSize"]) : 0,
                    UsableSize = row["UsableSize"] != DBNull.Value ? Convert.ToDecimal(row["UsableSize"]) : 0,
                    Square = row["SquareFeet"] != DBNull.Value ? Convert.ToDecimal(row["SquareFeet"]) : 0,

                    // Booleans in DataRows need careful casting if they are bits in SQL
                    isLocked = row["IsLocked"] != DBNull.Value && Convert.ToBoolean(row["IsLocked"]),

                    Status = row["Status"] != DBNull.Value ? Convert.ToInt32(row["Status"]) : 0,
                    BuildingId = row["BuildingId"] != DBNull.Value ? Convert.ToInt32(row["BuildingId"]) : 0
                });
            }

            return records;
        }
        public async Task<List<ChamberStockModel>> GetChambersInAsync()
        {
            const string query = @"SELECT 
                    c.status,
                    c.ChamberId as Id,
                    c.ChamberName as Name,
                    c.Status,
                    ISNULL(SUM(ca.VerifiedQty), 0) as InQty,
                    ISNULL((SELECT SUM(OrderQty) FROM StoreOutTrans WHERE LocationChamberId = c.ChamberId), 0) as OutQty,
                    (ISNULL(SUM(ca.VerifiedQty), 0) - ISNULL((SELECT SUM(OrderQty) FROM StoreOutTrans WHERE LocationChamberId = c.ChamberId), 0)) as CurrentStock,
                   ISNULL(convert(varchar(30), min(ca.Gateindate), 104), '') as mindate,
                    ISNULL(convert(varchar(30), (SELECT min(DeliveryDate) FROM StoreOutTrans WHERE LocationChamberId = c.ChamberId), 104), '') as maxdate
                FROM Chamber c 
                LEFT JOIN GateInTrans ca ON c.ChamberId = ca.LocationChamberId 
                GROUP BY c.ChamberId, c.ChamberName, c.status 
                ORDER BY c.ChamberId";

            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                query
            );

            var records = new List<ChamberStockModel>();

            if (ds.Tables.Count == 0)
                return records;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                records.Add(new ChamberStockModel
                {
                    ChamberId = Convert.ToInt32(row["Id"]),
                    IsLocked = Convert.ToBoolean(row["Status"]),
                    ChamberName = row["Name"].ToString() ?? string.Empty,
                    ChamberinDate = row["mindate"].ToString(),
                    ChamberOutDate = row["maxdate"].ToString(),

                    InQty = Convert.ToInt32(row["InQty"]),
                    OutQty = Convert.ToInt32(row["OutQty"]),
                    ChamberBalance = Convert.ToInt32(row["CurrentStock"]),
                });
            }

            return records;
        }
        public async Task<ChamberModel?> GetChamberDetailByIdAsync(int id)
        {
            const string query = @"SELECT 
                    chamberid     AS ChamberId,
                    chambername   AS ChamberName,
                    ctype         AS ChamberType,
                    unitname      AS UnitName,
                    Qty           AS Capacity,
                    status        AS Status
                FROM dbo.Chamber
                WHERE chamberid = @chamberId ";

            var ds = await _sql.ExecuteSingleAsync<ChamberModel>(
                 query,
                 CommandType.Text,
                 new SqlParameter("@chamberId", id)
            );
            return ds;
        }
        public async Task<List<ChamberModel>> GetAllChambersList(int unitId)
        {
            // cmd.Parameters.AddWithValue("@Unitid", unitid);
            // unitId kept for future filtering
            return await GetAllChambersAsync();
        }
        public async Task<ChamberEditModel?> GetChamberByIdAsync(int chamberId)
        {
            const string sql = @"
            SELECT 
                ctype    AS ChamberType,
                Qty      AS Capacity,
                unitname AS UnitName
            FROM Chamber
            WHERE chamberid = @Id ";

            return await _sql.ExecuteSingleAsync<ChamberEditModel>(
                sql,
                CommandType.Text,
                new SqlParameter("@Id", chamberId)
            );
        }
        public async Task<List<ChamberAllocationViewModel>> GetChambersAsync()
        {
            const string sql = @"
                SELECT
                    c.ChamberId,
                    c.ChamberName,
                    c.Status as IsLocked,
                    c.Qty as Capacity,
                    c.Ctype as ChamberType,
                    ISNULL(SUM(ca.qty), 0) as AllocatedQty,
                    (c.qty - ISNULL(SUM(ca.qty), 0)) as RemainingQty 
                FROM Chamber c
                LEFT JOIN Challocation ca ON c.ChamberId = ca.Chamber
                GROUP BY c.ChamberId, c.ChamberName, c.qty, c.Status,  c.Status,c.ctype
                ORDER BY c.ChamberId";

            return await _sql.ExecuteReaderAsync<ChamberAllocationViewModel>(sql, CommandType.Text);
        }

        // Allocation Management
        public async Task ReleaseAllocationAsync(
            int growerId,
            int chamberId)
        {
            var allocation = await GetAllocationAsync(growerId, chamberId);
            if (allocation == null)
                return;

            var dtransId = await GetDtransIdAsync(
                growerId,
                chamberId,
                allocation.AllocationId);

            if (!dtransId.HasValue)
                return;

            await UpdateChamberQuantityAsync(
                chamberId,
                allocation.Quantity);

            await SoftDeleteDtransAsync(dtransId.Value);
            await SoftDeleteAllocationAsync(allocation.AllocationId);
        }
        private async Task<AllocationInfo?> GetAllocationAsync(int growerId,int chamberId)
        {
            const string sql = @"
                SELECT TOP 1 Id as AllocationId, Quantity
                FROM Allocation
                WHERE partyid = @growerId
                  AND chamberid = @chamberId
                  AND FlagDeleted = 0";

            return await _sql.ExecuteSingleAsync<AllocationInfo>(
               sql,
               CommandType.Text,
               new SqlParameter("@growerId", growerId),
               new SqlParameter("@chamberId", chamberId)
           );
        }
        private async Task<int?> GetDtransIdAsync(
            int growerId,
            int chamberId,
            int allocationId)
        {
            const string sql = @"
                SELECT TOP 1 Id
                FROM Dtrans
                WHERE partyid = @growerId
                  AND chamberid = @chamberId
                  AND AllocationId = @allocationId
                  AND LotNumber IS NULL
                  AND FlagDeleted = 0";

            return await _sql.ExecuteScalarAsync<int>(
               sql,
               CommandType.Text,
               new SqlParameter("@growerId", growerId),
               new SqlParameter("@chamberId", chamberId),
               new SqlParameter("@allocationId", allocationId)
           );
        }
        private async Task<bool> UpdateChamberQuantityAsync(int chamberId, int quantity)
        {
            const string sql = @"
                UPDATE chamber
                SET QuantityConsumed = QuantityConsumed - @qty,
                    QuantityBalanced = QuantityBalanced + @qty
                WHERE chamberid = @chamberId";

            await _sql.ExecuteNonQueryAsync(
                 CommandType.Text,
                 sql,
                 new SqlParameter("@qty", quantity),
                 new SqlParameter("@chamberId", chamberId)
             );
            return true;
        }
        private async Task SoftDeleteDtransAsync(int dtransId)
        {
            const string sql = @"UPDATE Dtrans SET FlagDeleted = 1 WHERE Id = @id";

            await _sql.ExecuteNonQueryAsync(
                 CommandType.Text,
                 sql,
                 new SqlParameter("@id", dtransId)
             );
        }
        private async Task SoftDeleteAllocationAsync(int allocationId)
        {
            const string sql = @"UPDATE Allocation SET FlagDeleted = 1 WHERE Id = @id";
            await _sql.ExecuteNonQueryAsync(
                 CommandType.Text,
                 sql,
                 new SqlParameter("@id", allocationId)
             );
        }


        public async Task<CompanyModel?> SaveChamberAllocationAsync(
            SaveChamberAllocationRequest request,
            string currentUser)
        {
            if (request == null)
                return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "SaveChamberAllocation",
                new SqlParameter("@Chamberid", request.ChamberId),
                new SqlParameter("@GrowerName", request.GrowerGroupName),
                new SqlParameter("@Qty", request.ChamberAllocation)
            );

            var result = new CompanyModel
            {
                ChamberId = request.ChamberId,
                GrowerGroupName = request.GrowerGroupName,
                ChamberAllocation = request.ChamberAllocation,
                GlobalUserName = currentUser
            };

            await FillValidationAsync(result);
            return result;
        }
        public async Task<CompanyModel?> UpdateChamberAllocationAsync(
            int allocationId,
            decimal chamberAllocation)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "UpdateChamberAllocation",
                new SqlParameter("@Chamberid", allocationId),
                new SqlParameter("@Qty", chamberAllocation)
            );

            var result = new CompanyModel
            {
                AllocationNo = allocationId,
                ChamberAllocation = chamberAllocation
            };

            await FillValidationAsync(result);
            return result;
        }

        // Dashboard and Permissions
        public async Task<DashboardTotalsModel?> GetDashboardTotalsAsync()
        {
            const string sql = @"SELECT
                verified_total.total_verified_qty,
                order_total.total_order_qty,
                verified_total.total_verified_qty - order_total.total_order_qty AS balance_qty
            FROM
                (SELECT COALESCE(SUM(verifiedqty), 0) AS total_verified_qty
                 FROM gateintrans
                 WHERE flagdeleted = 0) AS verified_total
            CROSS JOIN
                (SELECT COALESCE(SUM(orderqty), 0) AS total_order_qty
                 FROM lotouttrans
                 WHERE flagdeleted = 0) AS order_total";

            var record = new DashboardTotalsModel();
            var ds = await _sql.ExecuteDatasetAsync(sql, CommandType.Text);
            if (ds.Tables.Count == 0)
                return record;

            // Safety check: Ensure the table exists and has at least one row
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];

                // Map the columns to your model properties
                // Use 'as' or Convert to handle potential DBNull values safely
                record.TotalInQty = row["total_verified_qty"] != DBNull.Value ? Convert.ToInt32(row["total_verified_qty"]) : 0;
                record.TotalOutQty = row["total_order_qty"] != DBNull.Value ? Convert.ToInt32(row["total_order_qty"]) : 0;
                record.TotalBalQty = row["balance_qty"] != DBNull.Value ? Convert.ToInt32(row["balance_qty"]) : 0;
            }

            return record;
        }
        public async Task<UserPrivModel?> GetAllocationpriv(string userGroup)
        {
            const string sql = @"
                SELECT Addval, Editval, ViewVal, DelVal
                FROM userpriv
                WHERE Groupid IN (
                    SELECT usergroupid FROM usergroup WHERE name = @Ugroup
                )
                AND pname = @pname";

            return await _sql.ExecuteSingleAsync<UserPrivModel>(
                sql,
                CommandType.Text,
                new SqlParameter("@Ugroup", userGroup),
                new SqlParameter("@pname", "Allocation")
            );
        }
        public async Task<UserPrivModel?> GrowerPriv(string Ugroup)
        {
            var dt = await _sql.ExecuteDatasetAsync(
                @"select Addval,Editval,ViewVal,DelVal 
                  from userpriv 
                  where Groupid in (select usergroupid from usergroup where name=@Ugroup) 
                  and pname=@pname",
                CommandType.Text,
                new SqlParameter("@Ugroup", Ugroup),
                new SqlParameter("@pname", "Grower Group")
            );

            if (dt.Tables.Count == 0)
                return null;

            return new UserPrivModel
            {
                AddVal = Convert.ToBoolean(dt.Tables[0].Rows[0]["Addval"]),
                EditVal = Convert.ToBoolean(dt.Tables[0].Rows[0]["Editval"]),
                ViewVal = Convert.ToBoolean(dt.Tables[0].Rows[0]["ViewVal"]),
                DelVal = Convert.ToBoolean(dt.Tables[0].Rows[0]["DelVal"])
            };
        }

        // Chamber & Grower CRUD/Status
        public async Task<ChamberDto?> AddNewChamber(ChamberDto model)
        {
            if (model == null) return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "AddNewChamber",
                new SqlParameter("@ctype", model.ChamberType),
                new SqlParameter("@unit", model.UnitName),
                new SqlParameter("@Capacity", model.Capacity),
                new SqlParameter("@User", model.GlobalUserName)
            );

            await FillValidationAsync(model);
            return model;
        }
        public async Task<ChamberUpdateDto?> UpdateChamber(int chamberid, ChamberUpdateDto model)
        {
            if (model == null) return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "UpdateChamber",
                new SqlParameter("@chamberid", chamberid),
                new SqlParameter("@ctype", model.ChamberType),
                new SqlParameter("@unit", model.UnitName),
                new SqlParameter("@Capacity", model.Capacity),
                new SqlParameter("@User", model.GlobalUserName)
            );

            await FillValidationAsync(model);
            return model;
        }
        //Getchamberdet
        public async Task<List<ChamberPartyStockModel>> GetChamberDetailsAsync(int chamberid)
        {
            const string query = @"WITH InTrans AS (
                SELECT 
                    LocationChamberId,
                    partyid,
                    SUM(verifiedQty) AS TotalInQty
                FROM GateInTrans
                WHERE LocationChamberId = @chamberid
                    AND verifiedQty > 0
                GROUP BY LocationChamberId, partyid
            ),
            OutTrans AS (
                SELECT 
                    LocationChamberId,
                    partyid,
                    SUM(OrderQty) AS TotalOutQty
                FROM lotouttrans
                WHERE LocationChamberId = @chamberid
                GROUP BY LocationChamberId, partyid
            )
            SELECT 
                ch.chamberid,
                i.partyid,
                ch.chambername, 
                p.partytypeid + '-' + p.partyname AS GrowerGroupName,
                ISNULL(i.TotalInQty, 0) AS InQty,
                ISNULL(o.TotalOutQty, 0) AS OutQty,
                ISNULL(i.TotalInQty, 0) - ISNULL(o.TotalOutQty, 0) AS BalanceQty
            FROM InTrans i
            LEFT JOIN OutTrans o ON i.LocationChamberId = o.LocationChamberId 
                AND i.partyid = o.partyid
            LEFT JOIN chamber ch ON i.LocationChamberId = ch.chamberid
            LEFT JOIN party p ON i.partyid = p.partyid
            ORDER BY 
                ch.chamberid, i.partyid";

            return await _sql.ExecuteReaderAsync<ChamberPartyStockModel>(
                query,
                CommandType.Text,
                new SqlParameter("@chamberid", chamberid)
            );
        }
        //Getchamberdetgrowers
        public async Task<List<ChamberGrowerStockModel>> GetChamberGrowerDetailsAsync(
            int partyId,
            int chamberId)
        {
            const string sql = @"
                WITH InTrans AS (
                    SELECT LocationChamberId, growerid, SUM(verifiedQty) AS TotalInQty
                    FROM GateInTrans
                    WHERE LocationChamberId = @chamberid
                      AND partyid = @partyid
                      AND verifiedQty > 0
                    GROUP BY LocationChamberId, growerid
                ),
                OutTrans AS (
                    SELECT LocationChamberId, growerid, SUM(OrderQty) AS TotalOutQty
                    FROM lotouttrans
                    WHERE LocationChamberId = @chamberid
                      AND partyid = @partyid
                    GROUP BY LocationChamberId, growerid
                )
                SELECT 
                    ch.chamberid,
                    ch.chambername,
                    p.partytypeid + '-' + p.partyname AS GrowerName,
                    ISNULL(i.TotalInQty, 0) AS InQty,
                    ISNULL(o.TotalOutQty, 0) AS OutQty,
                    ISNULL(i.TotalInQty, 0) - ISNULL(o.TotalOutQty, 0) AS BalanceQty
                FROM InTrans i
                LEFT JOIN OutTrans o 
                    ON i.LocationChamberId = o.LocationChamberId
                   AND i.growerid = o.growerid
                LEFT JOIN chamber ch ON i.LocationChamberId = ch.chamberid
                LEFT JOIN partysub p ON i.growerid = p.partyid
                ORDER BY ch.chamberid";

            return await _sql.ExecuteReaderAsync<ChamberGrowerStockModel>(
                sql,
                CommandType.Text,
                new SqlParameter("@partyid", partyId),
                new SqlParameter("@chamberid", chamberId)
            );
        }

        public async Task<bool> LockUnlockChamber(int chamberId)
        {
            var result = await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "UpdatechamberStatus",
                new SqlParameter("@Growerid", chamberId)
            );

            return (result > 0);
        }
        public async Task<bool> GenchamberAgg(int id)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "GenchamberAgg",
                new SqlParameter("@Growerid", id)
            );
            return true;
        }
        public async Task<CompanyModel?> DeleteGrowerGroup(int id, CompanyModel companyModel)
        {
            if (companyModel == null)
                return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "DeleteGrowerGroup",
                new SqlParameter("@Growerid", id)
            );

            await FillValidationAsync(companyModel);
            return companyModel;
        }
        public async Task<bool> UpdateGrowerStatus(int id)
        {
            await _sql.ExecuteNonQueryAsync(
               CommandType.StoredProcedure,
               "UpdateGrowerStatus",
               new SqlParameter("@Growerid", id)
            );
            return true;
        }

        public async Task<CompanyModel?> DeleteAllocation(int selectedGrowerId, CompanyModel companyModel)
        {
            if (companyModel == null)
                return null;

            await _sql.ExecuteNonQueryAsync(
              CommandType.StoredProcedure,
              "DeleteAllocation",
              new SqlParameter("@Chamberid", selectedGrowerId)
            );

            await FillValidationAsync(companyModel);
            return companyModel;
        }

        public async Task<CompanyModel?> AddGrowerSubdirect(CompanyModel EditModel)
        {
            if (EditModel == null)
                return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "AddGrowerSub",
                new SqlParameter("@GrowerGrp", EditModel.GrowerGroupName),
                new SqlParameter("@GrowerName", EditModel.GrowerName),
                new SqlParameter("@GrowerAddress", EditModel.GrowerAddress),
                new SqlParameter("@GrowerCity", "NA"),
                new SqlParameter("@GrowerPincode", "NA"),
                new SqlParameter("@GrowerState", "Jammu and Kashmir"),
                new SqlParameter("@GrowerStatecode", "01"),
                new SqlParameter("@GrowerEmail", "NA"),
                new SqlParameter("@GrowerPan", "NA"),
                new SqlParameter("@GrowerGst", "NA"),
                new SqlParameter("@GrowerContact", EditModel.GrowerContact),
                new SqlParameter("@GrowerStatus", 1),
                new SqlParameter("@Createdby", 1),
                new SqlParameter("@Updatedby", 1),
                new SqlParameter("@GrowerVillage", "NA"),
                new SqlParameter("@graceperiod", 0),
                new SqlParameter("@country", "India"),
                new SqlParameter("@crateQtyLimit", EditModel.GrowerCrateissue),
                new SqlParameter("@crateQtypercent", EditModel.GrowerCratelimit),
                new SqlParameter("@GrowerisLock", 0),
                new SqlParameter("@Growerisflexi", 0),
                new SqlParameter("@growerRemarks", "na")
            );


            await FillValidationAsync(EditModel);
            return EditModel;
        }

        // Grower and Party Lookups
        public async Task<List<GrowerModel>> GetallGrowers()
        {
            var list = new List<GrowerModel>();

            var dt = await _sql.ExecuteDatasetAsync(
                @"select *, partytypeid + '-' + partyname AS PartyGroupName 
                    from party where type='C' and flagdeleted=0 order by partycode desc",
                CommandType.Text
            );

            foreach (DataRow rdr in dt.Tables[0].Rows)
            {
                list.Add(new GrowerModel
                {
                    Growerid = Convert.ToInt32(rdr["Partycode"]),
                    GrowerAddress = rdr["address"].ToString(),
                    GrowerName = rdr["PartyGroupName"].ToString(),
                    Cdated = rdr["createdon"].ToString(),
                    Grstatus = rdr["status"].ToString()
                });
            }

            return list;
        }
        public async Task<List<CompanyModel>> GetallStockChamber(int GrowerId)
        {
            var list = new List<CompanyModel>();

            var dt = await _sql.ExecuteDatasetAsync(
                @"SELECT p.chambername,
                     CAST(sum(companycrate) AS DECIMAL(18,2)) as ccrates,
                     CAST(sum(owncrate) AS DECIMAL(18,2)) as owncrates,
                     CAST(sum(ci.qty) AS DECIMAL(18,2)) as qty,
                     CAST(sum(VerifiedCompanyCrates) AS DECIMAL(18,2)) as vccrates,
                     CAST(sum(VerifiedOwnCrates) AS DECIMAL(18,2)) as vcowncrates,
                     CAST(sum(VerifiedQty) AS DECIMAL(18,2)) as Vcqty,
                     CAST(sum(VerifiedWoodenPetty) AS DECIMAL(18,2)) as vcpetty
                  FROM gateintrans ci
                  LEFT JOIN chamber p ON ci.LocationChamberId = p.chamberid
                  WHERE ci.flagdeleted=0 AND ci.dockpost=1 AND ci.PartyId=@Growerid
                  GROUP BY p.chambername, p.chamberid
                  ORDER BY p.chamberid",
                CommandType.Text,
                new SqlParameter("@Growerid", GrowerId)
            );

            if (dt.Tables.Count == 0)
                return new List<CompanyModel>();

            foreach (DataRow rdr in dt.Tables[0].Rows)
            {
                list.Add(new CompanyModel
                {
                    ChamberName = rdr["chambername"].ToString(),
                    TotalPreQty = Convert.ToDecimal(rdr["qty"]),
                    TotalPrecompanyQty = Convert.ToDecimal(rdr["ccrates"]),
                    TotalPreownQty = Convert.ToDecimal(rdr["owncrates"]),
                    TotalInQty = Convert.ToDecimal(rdr["Vcqty"]),
                    TotalIncompanyQty = Convert.ToDecimal(rdr["vccrates"]),
                    TotalInownQty = Convert.ToDecimal(rdr["vcowncrates"]),
                    TotalInpettyQty = Convert.ToDecimal(rdr["vcpetty"])
                });
            }

            return list;
        }
        public async Task<List<CompanyModel>> GetallStockGrowerlots(int GrowerId)
        {
            var list = new List<CompanyModel>();

            var dt = await _sql.ExecuteDatasetAsync(
                @"SELECT convert(varchar(20),ci.GateInDate,104) as GDated,
                         ci.lotno,ci.PreInIrn,ci.lotirn,
                         p.partytypeid + '-' + p.partyname AS PartyName,
                         CONVERT(varchar(max), ps.partyid) + '-' + ps.partyname AS GrowerName,
                         cm.challanName,ci.challanNo,prt.name as item,
                         pt.name as package,ci.khataName,ci.cratetype,
                         pq.name as variety,cs.chambername,
                         CAST(ci.qty AS DECIMAL(18,0)) as preinwardQty,
                         CAST(ci.VerifiedCompanyCrates AS DECIMAL(18,0)) as VerifiedCompanyCrates,
                         CAST(ci.VerifiedOwnCrates AS DECIMAL(18,0)) as VerifiedOwnCrates,
                         CAST(ci.VerifiedWoodenPetty AS DECIMAL(18,0)) as VerifiedWoodenPetty,
                         CAST(ci.verifiedqty AS DECIMAL(18,0)) as verifiedqty,
                         CAST(ci.bins AS DECIMAL(18,0)) as bins
                  FROM GateInTrans ci
                  LEFT JOIN party p ON ci.partyid = p.partyid
                  LEFT JOIN partysub ps ON ci.growerid = ps.partyid
                  LEFT JOIN challanmaster cm ON ci.challanid = cm.id
                  LEFT JOIN prodqaul pq ON ci.varietyid = pq.id
                  LEFT JOIN PTYPE pt ON ci.packageid = pt.id
                  LEFT JOIN chamber cs ON ci.LocationChamberId = cs.chamberid
                  LEFT JOIN prodtype prt ON ci.itemid = prt.id
                  WHERE VerifiedQty > 0 AND GrowerId=@Growerid
                  ORDER BY lotno",
                CommandType.Text,
                new SqlParameter("@Growerid", GrowerId)
            );

            if (dt.Tables.Count == 0)
                return new List<CompanyModel>();

            foreach (DataRow rdr in dt.Tables[0].Rows)
            {
                list.Add(new CompanyModel
                {
                    PreInwardKhata = rdr["khataName"].ToString(),
                    Lotno = Convert.ToInt32(rdr["lotno"]),
                    EntryDate = rdr["GDated"].ToString(),
                    LotIrn = rdr["lotirn"].ToString(),
                    PreInIrn = rdr["PreInIrn"].ToString(),
                    GrowerGroupName = rdr["PartyName"].ToString(),
                    GrowerName = rdr["GrowerName"].ToString(),
                    ChallanName = rdr["challanName"].ToString(),
                    ChallanNo = rdr["challanNo"].ToString(),
                    VarietyId = rdr["variety"].ToString(),
                    ChamberName = rdr["chambername"].ToString(),
                    PreCrateType = rdr["cratetype"].ToString(),
                    PreInwardQty = Convert.ToDecimal(rdr["preinwardQty"]),
                    VerfiedCompanyCrates = Convert.ToDecimal(rdr["VerifiedCompanyCrates"]),
                    VerfiedOwnCrates = Convert.ToDecimal(rdr["VerifiedOwnCrates"]),
                    Verfiedpetties = Convert.ToDecimal(rdr["VerifiedWoodenPetty"]),
                    VerfiedQty = Convert.ToDecimal(rdr["verifiedqty"]),
                    Verfiedbins = Convert.ToDecimal(rdr["bins"])
                });
            }

            return list;
        }
        public async Task<CompanyModel?> Getchsummary(int chamberid)
        {
            var dt = await _sql.ExecuteDatasetAsync(
                @"SELECT 
                    (SELECT COALESCE(SUM(qty),0) FROM chamber WHERE chamberid=@chamberid) AS capacity,
                    (SELECT COALESCE(SUM(qty),0) FROM challocation WHERE chamber=@chamberid) AS TotalQuantity",
                CommandType.Text,
                new SqlParameter("@chamberid", chamberid)
            );

            if (dt.Tables[0].Rows.Count == 0)
                return null;

            var rdr = dt.Tables[0].Rows[0];

            return new CompanyModel
            {
                chambercapacitytotal = Convert.ToInt32(rdr["capacity"]),
                chamberallotedtotal = Convert.ToInt32(rdr["TotalQuantity"])
            };
        }
        public async Task<List<ChamberAllocationModel>> GetchsummaryGrowerdet(int chamberid)
        {
            List<ChamberAllocationModel> GetGrowerallocation = new List<ChamberAllocationModel>();

            var query = "SELECT c.qty as capacity,a.chamber,a.ser,c.status" +
                    ",a.chamber as id,a.wid,partytypeid  +'-'+ partyname as PartyName " +
                    ",SUM(a.qty) as TotalQuantity " +
                    "FROM challocation a " +
                    "LEFT JOIN party p ON a.wid = p.partyid" +
                    " LEFT JOIN chamber c ON a.chamber = c.chamberid " +
                    "WHERE a.wid = @chamberid " +
                    "GROUP BY a.chamber, a.ser, c.status,A.idno,a.wid,c.qty, p. partytypeid  +'-'+ partyname" +
                    " ORDER BY a.ser";
            var dt = await _sql.ExecuteDatasetAsync(
               query,
               CommandType.Text,
               new SqlParameter("@chamberid", chamberid)
           );

            if (dt.Tables.Count == 0)
                return new List<ChamberAllocationModel>();

            foreach (DataRow rdr in dt.Tables[0].Rows)
            {
                GetGrowerallocation.Add(new ChamberAllocationModel
                {
                    ChamberId = Convert.ToInt32(rdr["chamber"]),
                    AllocationNo = Convert.ToInt32(rdr["id"]),
                    GrowerGroupName = rdr["PartyName"] as string,
                    ChamberAllocation = Convert.ToDecimal(rdr["TotalQuantity"]),
                    ChamberCapcity = Convert.ToInt32(rdr["capacity"]),
                    ChamberName = rdr["chamber"] as string,
                    IsLocked = Convert.ToBoolean(rdr["status"])                   
                });
            } 
                  
            return GetGrowerallocation;
        }
        public async Task<CompanyModel?> GetchsummaryGrower(int chamberid)
        {
            var query = @"SELECT 
                (SELECT COALESCE(SUM(qty), 0) FROM groweragreement WHERE mainid =@chamberid) AS TotalAgreementQty,
                (SELECT COALESCE(SUM(qty), 0) FROM challocation WHERE wid = @chamberid) AS TotalAllocationQty;";
            
            var dt = await _sql.ExecuteDatasetAsync(
               query,
               CommandType.Text,
               new SqlParameter("@chamberid", chamberid)
            );

            if (dt.Tables[0].Rows.Count == 0)
                return null;

            var rdr = dt.Tables[0].Rows[0];

            return new CompanyModel
            {
                chambercapacitytotalgrower = Convert.ToInt32(rdr["TotalAgreementQty"]),
                chamberallotedtotalgrower = Convert.ToInt32(rdr["TotalAllocationQty"])
            };
        }
        public async Task<List<ChamberAllocationModel>> GetChallocation(int chamberid)
        {
            List<ChamberAllocationModel> Getallocation = new List<ChamberAllocationModel>();

            var query = @"SELECT c.qty as capacity,a.chamber,a.ser,c.status,a.idno as id,a.wid,
                        partytypeid  +'-'+ partyname as PartyName ,SUM(a.qty) as TotalQuantity 
                        FROM challocation a 
                    LEFT JOIN party p ON a.wid = p.partyid 
                    LEFT JOIN chamber c ON a.chamber = c.chamberid 
                    WHERE a.chamber = @chamberid 
                    GROUP BY a.chamber, a.ser, c.status,A.idno,a.wid,c.qty, p. partytypeid  +'-'+ partyname 
                    ORDER BY a.ser";
            var dt = await _sql.ExecuteDatasetAsync(
                 query,
                 CommandType.Text,
                 new SqlParameter("@chamberid", chamberid)
              );

            if (dt.Tables[0].Rows.Count == 0)
                return Getallocation;

            foreach (DataRow rdr in dt.Tables[0].Rows)
            {
                Getallocation.Add(new ChamberAllocationModel
                {
                    ChamberId = Convert.ToInt32(rdr["chamber"]),
                    AllocationNo = Convert.ToInt32(rdr["id"]),
                    GrowerGroupName = rdr["PartyName"] as string,
                    ChamberAllocation = Convert.ToDecimal(rdr["TotalQuantity"]),
                    ChamberCapcity = Convert.ToInt32(rdr["capacity"]),
                    ChamberName = rdr["chamber"] as string,
                    IsLocked = Convert.ToBoolean(rdr["status"])
                });
            }
               
            return Getallocation;
        }
        public async Task<ChamberAllocationModel?> Getallocationdet(int selectedno)
        {
            var dt = await _sql.ExecuteDatasetAsync(
                @"SELECT a.chamber,a.idno as id,c.status,
                     partytypeid + '-' + partyname as PartyName,
                     SUM(a.qty) as TotalQuantity
                  FROM challocation a
                  LEFT JOIN party p ON a.wid = p.partyid
                  LEFT JOIN chamber c ON a.chamber = c.chamberid
                  WHERE a.idno=@id
                  GROUP BY a.chamber,a.idno,c.status,partytypeid,partyname",
                CommandType.Text,
                new SqlParameter("@id", selectedno)
            );

            if (dt.Tables[0].Rows.Count == 0)
                return null;

            var rdr = dt.Tables[0].Rows[0];

            return new ChamberAllocationModel
            {
                ChamberId = Convert.ToInt32(rdr["chamber"]),
                AllocationNo = Convert.ToInt32(rdr["id"]),
                GrowerGroupName = rdr["PartyName"].ToString(),
                ChamberAllocation = Convert.ToDecimal(rdr["TotalQuantity"]),
                IsLocked = Convert.ToBoolean(rdr["status"])
            };
        }
        public async Task<CompanyModel?> GetGrowerIdAsync(int Growerid)
        {
            var dt = await _sql.ExecuteDatasetAsync(
                "SELECT * FROM party WHERE type='C' AND partycode=@Id",
                CommandType.Text,
                new SqlParameter("@Id", Growerid)
            );

            if (dt.Tables[0].Rows.Count == 0)
                return null;

            var rdr = dt.Tables[0].Rows[0];

            return new CompanyModel
            {
                Growerid = Convert.ToInt32(rdr["partycode"]),
                GrowerGroupName = rdr["partyname"].ToString(),
                GrowerAddress = rdr["address"].ToString(),
                GrowerCity = rdr["city"].ToString(),
                GrowerPincode = rdr["pincode"].ToString(),
                GrowerState = rdr["state"].ToString(),
                GrowerStatecode = rdr["statecode"].ToString(),
                GrowerEmail = rdr["email"].ToString(),
                GrowerContact = rdr["phone"].ToString(),
                GrowerGst = rdr["GSTNo"].ToString(),
                GrowerPan = rdr["panno"].ToString(),
                GrowerRemarks = rdr["remarks"].ToString(),
                GrowerCrateissue = Convert.ToDecimal(rdr["CrateQtyLimit"]),
                GrowerCratelimit = Convert.ToDecimal(rdr["CrateIssueLimitPer"]),
                GrowerGrace = rdr["greaseperiod"].ToString(),
                GrowerCountry = rdr["country"].ToString(),
                GrowerVillage = rdr["village"].ToString(),
                GrowerRetLock = Convert.ToBoolean(rdr["islock"]),
                GrowerRetFlexi = Convert.ToBoolean(rdr["IsFlexibleChamber"]),
                GrowerRetAcitve = Convert.ToBoolean(rdr["Status"])

                // companyModel.Growerid = reader.GetInt32(reader.GetOrdinal("id")),

                //Gst = reader["gst"] as string,
                //Regno = reader["regno"] as string,
                //Pan = reader["pan"] as string,
                //Website = reader["website"] as string,
                //Caddress = reader["addr"] as string,

                //City = reader["city"] as string,
                //Phone = reader["Contact1"] as string,
                //Mobile = reader["Contact1"] as string,
                //State = reader["State"] as string,
                //Pincode = reader["Pincode"] as string,
                //Email = reader["email"] as string


            };
        }

        public async Task<List<ChamberModel>> GetallChambersParty(int unitid, string GrowerName)
        {
            List<ChamberModel> banks = new List<ChamberModel>();

            const string query = @"SELECT * FROM dbo.Chamber 
                WHERE unitid = @Unitid 
                  AND chamberid IN (
                    SELECT locationchamberid FROM gateintrans 
                    WHERE flagdeleted = 0 
                      AND partyid IN (
                        SELECT partyid FROM party 
                        WHERE partytypeid + '-' + RTRIM(partyname) = @Growername
                      )
                    UNION
                    SELECT chamberid FROM TransferinTrans 
                    WHERE flagdeleted = 0 
                      AND partyid IN (
                        SELECT partyid FROM party 
                        WHERE partytypeid + '-' + RTRIM(partyname) = @Growername
                      )
                  )
                ORDER BY chamberid";

            var dt = await _sql.ExecuteDatasetAsync(
                query,
                CommandType.Text,
                new SqlParameter("@Unitid", unitid),
                new SqlParameter("@Growername", GrowerName)
            );

            if (dt.Tables[0].Rows.Count == 0)
                return banks;

            foreach (DataRow rdr in dt.Tables[0].Rows)
            {
                banks.Add(new ChamberModel
                {
                    ChamberId = Convert.ToInt32(rdr["chamberid"]),
                    ChamberName = rdr["chamber"] as string,
                    ChamberType = rdr["ctype"].ToString(),
                    UnitName = rdr["unitname"].ToString(),
                    Capacity = Convert.ToInt32(rdr["Qty"]),
                    Status = Convert.ToBoolean(rdr["status"])
                });
            }
           
            return banks;
        }

        public async Task<List<ChamberModel>> GetallChambersIn()
        {
            List<ChamberModel> banks = new List<ChamberModel>();
            
            const string query = @"select * from dbo.Chamber 
                        where chamberid in(select locationchamberid from gateintrans where  flagdeleted=0)
                        order by chamberid";

            var dt = await _sql.ExecuteDatasetAsync(
                query,
                CommandType.Text
             );

            if (dt.Tables[0].Rows.Count == 0)
                return banks;

            foreach (DataRow rdr in dt.Tables[0].Rows)
            {
                banks.Add(new ChamberModel
                {
                    ChamberId = Convert.ToInt32(rdr["chamberid"]), 
                    ChamberName = rdr["chambername"] as string,
                    ChamberType = rdr["ctype"].ToString(),
                    UnitName = rdr["unitname"].ToString(),
                    Capacity = Convert.ToInt32(rdr["Qty"]),

                    Status = Convert.ToBoolean(rdr["status"])
                });
            }
            return banks;
        }
        public async Task<List<ChamberModel>> GetallChambersPartydemand(int unitid, string GrowerName)
        {
            List<ChamberModel> banks = new List<ChamberModel>();

            const string query = @"SELECT DISTINCT ca.chambername
                FROM LotOutTrans l
                LEFT OUTER JOIN party ps ON ps.partyid = l.PartyId 
                LEFT OUTER JOIN chamber ca ON  l.LocationChamberId= ca.chamberid 

                WHERE 
                    -- Filter only completed records
                    (
                        l.outid IN (
                            SELECT s.outid 
                            FROM StoreOutTrans s 
                            GROUP BY s.outid
                            HAVING SUM(COALESCE(s.orderqty, 0)) = (
                                SELECT SUM(COALESCE(l2.orderqty, 0))
                                FROM LotOutTrans l2
                                WHERE l2.outid = s.outid
                            )
                        )
                        OR EXISTS (
                            SELECT 1 
                            FROM StoreOutTrans s 
                            WHERE s.outid = l.outid 
                            AND s.PreLotPrefix = 'FORCE'
                        )
                    )
                    AND l.UnitId = @unitid
                    AND l.DemandStatus = 'Approved'
                    -- Mandatory conditions for partyid and chambername
                    AND ps.partytypeid + '-' + ps.partyname = @partyid  -- Replace @partyid with actual value
   
                    AND l.demandirn IS NOT NULL";

            var dt = await _sql.ExecuteDatasetAsync(
                query,
                CommandType.Text,
                new SqlParameter("@Unitid", unitid),
                new SqlParameter("@partyid", GrowerName)
             );

            if (dt.Tables[0].Rows.Count == 0)
                return banks;

            foreach (DataRow rdr in dt.Tables[0].Rows)
            {
                banks.Add(new ChamberModel
                {
                    ChamberName = rdr["chambername"].ToString()
                });
            }
            return banks;
        }
        public async Task<List<ChamberModel>> GetallChambersSub(int unitid, string GrowerName, string subgrower)
        {
            List<ChamberModel> banks = new List<ChamberModel>();
            const string query = @"select * from dbo.Chamber 
                    where unitid=@Unitid  
                    AND chamberid in(select locationchamberid from gateintrans 
                        where GrowerId in(select partyid from partysub where convert(varchar(max),partyid)  +'-'+ partyname=@Subgrower) and flagdeleted=0 
                        and partyid in(select partyid from party where  partytypeid+'-'+ rtrim(partyname)=@Growername))
                    order by chamberid";

            var dt = await _sql.ExecuteDatasetAsync(
               query,
               CommandType.Text,
               new SqlParameter("@Unitid", unitid),
               new SqlParameter("@Growername", subgrower),
               new SqlParameter("@Subgrower", GrowerName)
            );

            if (dt.Tables[0].Rows.Count == 0)
                return banks;

            foreach (DataRow rdr in dt.Tables[0].Rows)
            {
                banks.Add(new ChamberModel
                {
                    ChamberId = Convert.ToInt32(rdr["chamberid"]),
                    ChamberName = rdr["chambername"].ToString(),
                    ChamberType = rdr["ctype"].ToString(),
                    UnitName = rdr["unitname"].ToString(),
                    Capacity = Convert.ToInt32(rdr["Qty"]),

                    Status = Convert.ToBoolean(rdr["status"])
                });
            }
            return banks;
        }
        public async Task<List<GrowerModel>> GetallchamberGrowerlist(string GrowerName)
        {
            List<GrowerModel> GetgrowerList = new List<GrowerModel>();
            const string query = @"select partytypeid+'-'+ rtrim(partyname) as Groupname 
                        from party 
                        WHERE status=1 and flagdeleted=0 and type='C' 
                            and partyid in(select partyid from gateintrans where flagdeleted=0 and locationchamberid in(select chamberid from chamber where chambername=@Growername))";
           
            var dt = await _sql.ExecuteDatasetAsync(
               query,
               CommandType.Text,
               new SqlParameter("@Growername", GrowerName)
            );

            if (dt.Tables[0].Rows.Count == 0)
                return GetgrowerList;

            foreach (DataRow rdr in dt.Tables[0].Rows)
            {
                GetgrowerList.Add(new GrowerModel
                {
                    GrowerName = rdr["Groupname"].ToString()
                });
            }
            return GetgrowerList;
        }
        
        public async Task<List<GrowerModel>> GetallGrowerlist()
        {
            List<GrowerModel> GetgrowerList = new List<GrowerModel>();

            const string query = @"select partytypeid+'-'+ rtrim(partyname) as Groupname 
                                    from party WHERE status=1 and flagdeleted=0 and type='C'";

            var dt = await _sql.ExecuteDatasetAsync(
              query,
              CommandType.Text
            );

            if (dt.Tables[0].Rows.Count == 0)
                return GetgrowerList;

            foreach (DataRow rdr in dt.Tables[0].Rows)
            {
                GetgrowerList.Add(new GrowerModel
                {
                    GrowerName = rdr["Groupname"].ToString(),
                    OutwardGrowerGroup = rdr["Groupname"].ToString()
                });
            }
            return GetgrowerList;
        }
        public async Task<List<GrowerModel>> GetSalesGrower()
        {
            List<GrowerModel> GetgrowerList = new List<GrowerModel>();
            const string query = @"select partytypeid+'-'+ rtrim(partyname) as Groupname
                                   from party WHERE status=1 and flagdeleted=0 and partyid=39  and type='C'";

            var dt = await _sql.ExecuteDatasetAsync(
              query,
              CommandType.Text
            );

            if (dt.Tables[0].Rows.Count == 0)
                return GetgrowerList;

            foreach (DataRow rdr in dt.Tables[0].Rows)
            {
                GetgrowerList.Add(new GrowerModel
                {
                    GrowerName = rdr["Groupname"].ToString(),
                    OutwardGrowerGroup = rdr["Groupname"].ToString()

                });
            }
            return GetgrowerList;
        }
        public async Task<List<GrowerModel>> GetallGrowerlistwithin()
        {
            List<GrowerModel> GetgrowerList = new List<GrowerModel>();
            const string query = @"select partytypeid+'-'+ rtrim(partyname) as Groupname 
                    from party 
                    WHERE status=1  
                        and partyid in(select partyid from gateintrans where flagdeleted=0 ) 
                        and flagdeleted=0 and type='C'";
               
            var dt = await _sql.ExecuteDatasetAsync(
             query,
             CommandType.Text
            );

            if (dt.Tables[0].Rows.Count == 0)
                return GetgrowerList;

            foreach (DataRow rdr in dt.Tables[0].Rows)
            {
                GetgrowerList.Add(new GrowerModel
                {
                    GrowerName = rdr["Groupname"].ToString()
                });
            }

            return GetgrowerList;
        }
        public async Task<GrowerModel?> GetDemandGrower(string demandirn)
        {
            GrowerModel? model = null;

            var query = @"select distinct partytypeid+'-'+ rtrim(partyname) as Groupname 
                        from party WHERE partyid in(select partyid from LotOutTrans where demandirn=@Dirn)";
                
            var dt = await _sql.ExecuteDatasetAsync(
             query,
             CommandType.Text
            );

            if (dt.Tables[0].Rows.Count == 0)
                return model;

            model = new GrowerModel
            {
                GrowerGroupName = dt.Tables[0].Rows[0]["Groupname"] as string,
            };

            return model;

        }
        public async Task<List<GrowerModel>> GetallGrowerlistnew()
        {
            List<GrowerModel> GetgrowerList = new List<GrowerModel>();
            const string query = "select partytypeid+'-'+ rtrim(partyname) as Groupname from party WHERE type='C'";
                
            var dt = await _sql.ExecuteDatasetAsync(
                query,
                CommandType.Text
               );

            if (dt.Tables[0].Rows.Count == 0)
                return GetgrowerList;

            foreach (DataRow rdr in dt.Tables[0].Rows)
            {
                GetgrowerList.Add(new GrowerModel
                {
                    GrowerGroupName = rdr["Groupname"].ToString()
                });
            }
            return GetgrowerList;
        }
        public async Task<bool> GenGrowerAgg(int id)
        {
            var result = await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "GenGrowerAgg",               
                 new SqlParameter("@Growerid", id)
               );

            return (result > 0);
        }

        // Demand Processing
        public async Task<bool> SaveDemandsToDatabase(
            List<string> demandIRNs,
            CompanyModel EditModel,
            int Unit)
        {
            SqlConnection? con = null;
            SqlTransaction? transaction = null;

            try
            {
                con = new SqlConnection(_config.GetConnectionString("SqlDbContext"));
                await con.OpenAsync();

                transaction = con.BeginTransaction();

                try
                {
                    // Delete all existing records first
                    await _sql.ExecuteNonQueryAsync(
                        CommandType.Text,
                        "DELETE FROM ProcessedDemands",
                        transaction
                    );

                    // Insert new selected demands
                    var insertSql = @"
                    INSERT INTO ProcessedDemands
                    (DemandIRN,billdate,partyid,growerid,billtype,chamberid,Unitid,createdby)
                    VALUES
                    (@DemandIRN,@Billdate,@Partyid,@GrowerId,@Billtype,@Chamber,@Unitid,@userid)";

                    foreach (var irn in demandIRNs)
                    {
                        await _sql.ExecuteNonQueryAsync(
                            CommandType.Text,
                            insertSql,
                            transaction,
                            new SqlParameter("@DemandIRN", irn),
                            new SqlParameter("@Billdate", EditModel.Billdate),
                            new SqlParameter("@Partyid", EditModel.GrowerGroupName),
                            new SqlParameter("@GrowerId", EditModel.GrowerName),
                            new SqlParameter("@Billtype", EditModel.Billtype),
                            new SqlParameter("@Chamber", EditModel.ChamberName),
                            new SqlParameter("@Unitid", Unit),
                            new SqlParameter("@userid", EditModel.GlobalUserName)
                        );
                    }

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    // Rollback transaction if any error occurs
                    transaction.Rollback();
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                // Ensure connection is closed
                transaction?.Dispose();
                con?.Close();
                con?.Dispose();
            }
        }

        // Challan Management
        public async Task<CompanyModel?> AddChallanGrower(CompanyModel EditModel)
        {
            if (EditModel == null)
                return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "AddChallanGroup",                
                new SqlParameter("@GrowerId", EditModel.SubGrowerId),
                new SqlParameter("@GrowerName", EditModel.GrowerName),
                new SqlParameter("@Chname", EditModel.ChallanName),
                new SqlParameter("@Chaddress", EditModel.ChallanAddress),
                new SqlParameter("@Chvillage", EditModel.ChallanVillage),
                new SqlParameter("@Chcity", EditModel.ChallanCity),
                new SqlParameter("@Chphone1", EditModel.ChallanPhone1),
                new SqlParameter("@Chphone2", EditModel.ChallanPhone2),
                new SqlParameter("@Chsms1", EditModel.ChallanSMS1),
                new SqlParameter("@Chsms2", EditModel.ChallanSMS2)
            );

            await FillValidationAsync(EditModel);
            return EditModel;
        }
        public async Task<List<CompanyModel>> GetallChallanlist(string ChallanGroup)
        {
            var list = new List<CompanyModel>();

            var dt = await _sql.ExecuteDatasetAsync(
                @"SELECT id,
                     ChallanName,
                     Address,
                     Phone1 + SPACE(1) + CAST(IsSMSPhone1 AS VARCHAR) AS Phone1,
                     Phone2 + SPACE(2) + CAST(IsSMSPhone2 AS VARCHAR) AS Phone2
                  FROM challanmaster
                  WHERE flagdeleted = 0
                    AND mainid IN (
                    SELECT partycode
                    FROM party
                    WHERE partytypeid + '-' + RTRIM(partyname) = @GrowerGroup
                )",
                CommandType.Text,
                new SqlParameter("@GrowerGroup", ChallanGroup)
            );

            foreach (DataRow rdr in dt.Tables[0].Rows)
            {
                list.Add(new CompanyModel
                {
                    ChallanId = Convert.ToInt32(rdr["id"]),
                    ChallanName = rdr["ChallanName"].ToString(),
                    ChallanAddress = rdr["Address"].ToString(),
                    ChallanPhone1 = rdr["Phone1"].ToString(),
                    ChallanPhone2 = rdr["Phone2"].ToString()

                    //,
                    //  ChallanName,
                    //  Address,
                    //  Phone1 + space(1) & IsSMSPhone1,
                    //  Phone2 + space(2) & IsSMSPhone2
                });
            }

            return list;
        }


    }
}
