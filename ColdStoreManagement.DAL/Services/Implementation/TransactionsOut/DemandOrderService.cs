using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ColdStoreManagement.BLL.Models.DTOs;
using ColdStoreManagement.DAL.Helper;
using ColdStoreManagement.DAL.Services.Interface.TransactionsOut;
using Microsoft.Data.SqlClient;

namespace ColdStoreManagement.DAL.Services.Implementation.TransactionsOut
{
    public class DemandOrderService(SQLHelperCore sql) : BaseService(sql), IDemandOrderService
    {

        public async Task<bool> UpdateLotOrderQuantity(DemandOrderDto EditModel)
        {
            const string query = "update LotOutTranstemp set OrderQty=@oqty where lotno=@Lotno and Tempid=@Tempid";
            var parameters = new[]
            {
                new SqlParameter("@oqty", EditModel.OrderQty),
                new SqlParameter("@Lotno", EditModel.Lotno),
                new SqlParameter("@Tempid", EditModel.TempOrderId)
            };

            await _sql.ExecuteNonQueryAsync(CommandType.Text, query, parameters);
            return true;
        }

        public async Task<List<DemandOrderDto>> GenerateTempLotReport(DemandOrderDto EditModel, int unit, int Tempid)
        {
            const string query = @"SELECT 
    g.LOTNO as Lotno, g.lotirn as LotIrn,
    g.VerifiedQty AS VerfiedQty, 
    c.partytypeid + '-' + c.partyname as GrowerGroupName, 
    CONVERT(varchar(max), ps.partyid) + '-' + ps.partyname AS GrowerName,
    g.orderqty AS OrderQty, 
    g.verifiedQty - g.OrderQty as Netqty,
    (g.VerifiedQty - COALESCE(SUM(o.QTY), 0)) AS Alotbal,
    prt.name as ItemName, pt.name as Prodetails, g.khataname as PreInwardKhata, pq.name as VarietyId,
    lf.FinalLocationText as Templocation, cr.Chambername as ChamberName, cm.ChallanName, st.sname as ServiceId
FROM LotOutTranstemp g
LEFT JOIN LOTOUTTRANS o ON g.LOTNO = o.LOTNO
LEFT JOIN PARTY C ON G.PARTYID = C.PARTYID
LEFT JOIN PARTYsub ps ON G.GrowerId = PS.PARTYID
LEFT JOIN prodtype prt ON g.itemid = prt.id
LEFT JOIN PTYPE pt ON g.packageid = pt.id
LEFT JOIN prodqaul pq ON g.varietyid = pq.id
LEFT JOIN challanmaster cm ON g.challanid = cm.id  
LEFT JOIN servicetypes st ON g.schemeid = st.id
LEFT JOIN chamber cr ON g.locationchamberid = cr.chamberid
LEFT JOIN locationFinal lf ON g.lotno = lf.lotno
WHERE g.tempid = @Tempid and g.unitid = @unit
GROUP BY g.orderqty, g.LOTNO, g.VerifiedQty, c.partytypeid + '-' + c.partyname, CONVERT(varchar(max), ps.partyid) + '-' + ps.partyname, prt.name, pt.name, g.khataname, pq.name, cm.ChallanName, st.sname, FinalLocationText, cr.Chambername, g.lotirn
ORDER BY g.LOTNO";

            var parameters = new[]
            {
                new SqlParameter("@Tempid", Tempid),
                new SqlParameter("@unit", unit)
            };

            return await _sql.ExecuteReaderAsync<DemandOrderDto>(query, CommandType.Text, parameters);
        }

        public async Task<List<DemandOrderDto>> GenerateTempLotRawReportEdit(DemandOrderDto EditModel, int unit, int Tempid)
        {
            await _sql.ExecuteNonQueryAsync(CommandType.StoredProcedure, "ProcessEditDemandRaw", new SqlParameter("@Tempid", Tempid));

            const string query = @"SELECT g.Tempid as TempOrderId,
    g.LOTNO as Lotno, g.lotirn as LotIrn,
    g.VerifiedQty AS VerfiedQty, 
    c.partytypeid + '-' + c.partyname as GrowerGroupName, 
    CONVERT(varchar(max), ps.partyid) + '-' + ps.partyname AS GrowerName,
    g.orderqty AS OrderQty, 
    g.verifiedQty - g.OrderQty as Netqty,
    (g.VerifiedQty - COALESCE(SUM(o.OrderQty), 0)) AS Alotbal,
    prt.name as ItemName, pt.name as Prodetails, g.khataname as PreInwardKhata, pq.name as VarietyId,
    isnull(FloorName + space(1) + lf.MatrixName + lf.RowName + space(1) + lf.ColumnName, 'NA') as Templocation,
    g.locationchamberid as ChamberId, cm.ChallanName, st.sname as ServiceId
FROM LotOutTranstemp g
LEFT JOIN LOTOUTTRANS o ON g.LOTNO = o.LOTNO
LEFT JOIN PARTY C ON G.PARTYID = C.PARTYID
LEFT JOIN PARTYsub ps ON G.GrowerId = PS.PARTYID
LEFT JOIN prodtype prt ON g.itemid = prt.id
LEFT JOIN PTYPE pt ON g.packageid = pt.id
LEFT JOIN prodqaul pq ON g.varietyid = pq.id
LEFT JOIN challanmaster cm ON g.challanid = cm.id  
LEFT JOIN servicetypes st ON g.schemeid = st.id
LEFT JOIN locationFinal lf ON g.lotno = lf.lotno
where g.tempid in (select tempid from LotOutTranstemp where outid = @Tempid) and g.unitid = @unit and g.flagdeleted = 0
GROUP BY g.Tempid, g.orderqty, g.LOTNO, g.VerifiedQty, c.partytypeid + '-' + c.partyname, CONVERT(varchar(max), ps.partyid) + '-' + ps.partyname, prt.name, pt.name, g.khataname, pq.name, cm.ChallanName, st.sname, FloorName + space(1) + lf.MatrixName + lf.RowName + space(1) + lf.ColumnName, g.locationchamberid, g.lotirn
ORDER BY g.LOTNO";

            var parameters = new[]
            {
                new SqlParameter("@Tempid", Tempid),
                new SqlParameter("@unit", unit)
            };

            return await _sql.ExecuteReaderAsync<DemandOrderDto>(query, CommandType.Text, parameters);
        }

        public async Task<List<DemandOrderDto>> generateDemandReport(DemandOrderDto EditModel, int unit)
        {
            var procParameters = new[]
            {
                new SqlParameter("@d1", EditModel.OrderDatefrom ?? (object)DBNull.Value),
                new SqlParameter("@d2", EditModel.OrderDateto ?? (object)DBNull.Value),
                new SqlParameter("@Partyid", EditModel.Partyid),
                new SqlParameter("@Groowerid", EditModel.Growerid),
                new SqlParameter("@OrderBy", EditModel.OrderBy ?? (object)DBNull.Value),
                new SqlParameter("@Status", EditModel.DemandStatus ?? (object)DBNull.Value),
                new SqlParameter("@Demandirn", EditModel.DemandIrn ?? (object)DBNull.Value),
                new SqlParameter("@unit", unit)
            };

            await _sql.ExecuteNonQueryAsync(CommandType.StoredProcedure, "generateDemandReport", procParameters);

            const string query = @"SELECT
    ci.OutId as DemandNo,
    ci.demandirn as DemandIrn, ci.orderDate as OrderDate, ci.DeliveryDate, p.partytypeid + '-' + p.partyname AS GrowerGroupName,
    sum(ci.OrderQty) as OrderQty,
    ci.OrderByName as OrderBy,
    ci.otype as OrderType,
    ci.DemandStatus 
FROM LotOutTransReport ci
LEFT JOIN party p ON ci.partyid = p.partyid
group by ci.OutId, ci.demandirn, ci.orderDate, ci.DeliveryDate, p.partytypeid + '-' + p.partyname, ci.OrderByName, ci.otype, ci.DemandStatus 
ORDER BY ci.OutId desc";

            return await _sql.ExecuteReaderAsync<DemandOrderDto>(query, CommandType.Text);
        }

        public async Task<List<DemandOrderDto>> GetAllDemands(int UnitId)
        {
            const string query = "SELECT distinct demandirn as DemandIrn FROM LOTOUTTRANS WHERE flagdeleted = 0 and unitid = @Unitid";
            return await _sql.ExecuteReaderAsync<DemandOrderDto>(query, CommandType.Text, new SqlParameter("@Unitid", UnitId));
        }

        public async Task<List<DemandOrderDto>> GetAllOrderby(int UnitId)
        {
            const string query = "SELECT distinct OrderByName as OrderBy FROM LOTOUTTRANS WHERE flagdeleted = 0 and unitid = @Unitid";
            return await _sql.ExecuteReaderAsync<DemandOrderDto>(query, CommandType.Text, new SqlParameter("@Unitid", UnitId));
        }

        public async Task<DemandOrderDto?> GetDemandPriv(string Ugroup)
        {
            const string query = @"SELECT 
    Addval as DemAdd,
    Editval as DemEdit,
    ViewVal as DemView,
    DelVal as DemDel,
    Approval as DemApp
FROM userpriv 
WHERE Groupid in (select usergroupid from usergroup where name = @Ugroup) 
  AND pname = @pname";

            var parameters = new[]
            {
                new SqlParameter("@Ugroup", Ugroup),
                new SqlParameter("@pname", "Demand Order")
            };

            return await _sql.ExecuteSingleAsync<DemandOrderDto>(query, CommandType.Text, parameters);
        }

        public async Task<DemandOrderDto?> ValidatedemandStatus(DemandOrderDto companyModel, int outid)
        {
            if (companyModel == null) return null;

            await _sql.ExecuteNonQueryAsync(CommandType.StoredProcedure, "ValidatedemandStatus", new SqlParameter("@OutID", outid));
            await FillValidationAsync(companyModel);
            return companyModel;
        }

        public async Task<DemandOrderDto?> GenerateDemandPreview(int selectedGrowerId, DemandOrderDto companyModel)
        {
            if (companyModel == null) return null;
            await _sql.ExecuteNonQueryAsync(CommandType.StoredProcedure, "Demandproc", new SqlParameter("@lotno", selectedGrowerId));
            return companyModel;
        }

        public async Task<bool> UpdateOrderStatus(int id, DemandOrderDto companyModel)
        {
            await _sql.ExecuteNonQueryAsync(CommandType.StoredProcedure, "UpdateOrderStatus", new SqlParameter("@Growerid", companyModel.DemandNo));
            return true;
        }

        public async Task<bool> UpdateOrderStatusraw(int id, DemandOrderDto companyModel)
        {
            await _sql.ExecuteNonQueryAsync(CommandType.StoredProcedure, "UpdateOrderStatusraw", new SqlParameter("@Growerid", companyModel.DemandNo));
            return true;
        }

        public async Task<DemandOrderDto?> DeleteDemandOrder(int selectedGrowerId, DemandOrderDto companyModel)
        {
            if (companyModel == null) return null;

            try
            {
                await _sql.ExecuteNonQueryAsync(CommandType.StoredProcedure, "DeleteDemandOrder", 
                    new SqlParameter("@Mid", selectedGrowerId),
                    new SqlParameter("@User", companyModel.GlobalUserName ?? (object)DBNull.Value));

                await FillValidationAsync(companyModel);
                return companyModel;
            }
            catch (Exception ex)
            {
                companyModel.RetMessage = $"Error: {ex.Message}";
                companyModel.RetFlag = "FALSE";
                return companyModel;
            }
        }

        public async Task<DemandOrderDto?> DeleteDemandOrderRaw(int selectedGrowerId, DemandOrderDto companyModel)
        {
            if (companyModel == null) return null;

            try
            {
                await _sql.ExecuteNonQueryAsync(CommandType.StoredProcedure, "DeleteDemandOrderRaw", 
                    new SqlParameter("@Mid", selectedGrowerId),
                    new SqlParameter("@User", companyModel.GlobalUserName ?? (object)DBNull.Value));

                await FillValidationAsync(companyModel);
                return companyModel;
            }
            catch (Exception ex)
            {
                companyModel.RetMessage = $"Error: {ex.Message}";
                companyModel.RetFlag = "FALSE";
                return companyModel;
            }
        }
        public async Task<List<DemandOrderDto>> GetAllunitDemands()
        {
            const string query = "SELECT distinct demandirn as DemandIrn FROM LOTOUTTRANS WHERE flagdeleted = 0";
            return await _sql.ExecuteReaderAsync<DemandOrderDto>(query, CommandType.Text);
        }

        public async Task<List<DemandOrderDto>> GetAllDemandsapprove(int UnitId)
        {
            const string query = @"SELECT 
    d.demandirn as DemandIrn
FROM StoreOutTrans d 
WHERE flagdeleted = 0
GROUP BY d.demandirn
HAVING 
    SUM(d.orderqty) > 
    COALESCE((SELECT SUM(o.OrderQty) 
              FROM (SELECT DISTINCT OutId, unitid FROM StoreOutTrans WHERE demandirn = d.demandirn AND flagdeleted = 0) s
              INNER JOIN DraftOutTrans o ON s.OutId = o.OutId AND s.unitid = o.unitid AND o.flagdeleted = 0), 0)
ORDER BY d.demandirn";

            return await _sql.ExecuteReaderAsync<DemandOrderDto>(query, CommandType.Text, new SqlParameter("@Unitid", UnitId));
        }

        public async Task<List<DemandOrderDto>> GetAllGrowerDemands(int unitid, string GrowerName)
        {
            const string query = @"SELECT
    d.demandirn as DemandIrn
FROM StoreOutTrans d
WHERE flagdeleted = 0 and partyid in (select partyid from party where partytypeid + '-' + partyname = @Party)
GROUP BY d.demandirn
HAVING
    SUM(d.orderqty) >
    COALESCE((SELECT SUM(o.OrderQty)
              FROM (SELECT DISTINCT OutId, unitid FROM StoreOutTrans WHERE demandirn = d.demandirn AND flagdeleted = 0) s
              INNER JOIN DraftOutTrans o ON s.OutId = o.OutId AND s.unitid = o.unitid AND o.flagdeleted = 0), 0)
ORDER BY d.demandirn";

            var parameters = new[]
            {
                new SqlParameter("@Unitid", unitid),
                new SqlParameter("@Party", GrowerName)
            };

            return await _sql.ExecuteReaderAsync<DemandOrderDto>(query, CommandType.Text, parameters);
        }
        public async Task<DemandOrderDto?> GetDemandGrower(string demandirn)
        {
            const string query = "select distinct partytypeid + '-' + rtrim(partyname) as GrowerGroupName from party WHERE partyid in (select partyid from LotOutTrans where demandirn = @Dirn)";
            return await _sql.ExecuteSingleAsync<DemandOrderDto>(query, CommandType.Text, new SqlParameter("@Dirn", demandirn));
        }

        public async Task<DemandOrderDto?> GetDemandbyAsync(int outid)
        {
            const string query = @"SELECT 
    g.tempid as TempOrderId,
    g.demandirn as DemandIrn,
    g.otype as DemandStatus,
    g.Orderbyname as OrderBy,
    c.partytypeid + '-' + c.partyname as GrowerGroupName
FROM LotOutTrans g
LEFT JOIN PARTY C ON G.PARTYID = C.PARTYID
WHERE g.outid = @outid
GROUP BY g.tempid, g.demandirn, g.otype, g.Orderbyname, c.partytypeid + '-' + c.partyname";

            return await _sql.ExecuteSingleAsync<DemandOrderDto>(query, CommandType.Text, new SqlParameter("@outid", outid));
        }

        public async Task<DemandOrderDto?> GetRawDemandbyAsync(int outid)
        {
            const string query = @"SELECT 
    g.tempid as TempOrderId,
    g.demandirn as DemandIrn,
    g.Orderbyname as OrderBy,
    g.DemandStatus as OrderType,
    c.partytypeid + '-' + c.partyname as GrowerGroupName
FROM TransferInTrans g
LEFT JOIN PARTY C ON G.PARTYID = C.PARTYID
WHERE g.outid = @outid
GROUP BY g.tempid, g.demandirn, g.Orderbyname, g.DemandStatus, c.partytypeid + '-' + c.partyname";

            return await _sql.ExecuteSingleAsync<DemandOrderDto>(query, CommandType.Text, new SqlParameter("@outid", outid));
        }

        public async Task<List<DemandOrderDto>> GetDemand(int outid)
        {
            const string query = @"SELECT 
    g.DemandStatus, g.LOTNO, g.lotirn as LotIrn, g.orderdate, g.deliverydate, g.ordercontact, g.demandremarks, g.otype, g.Orderbyname,
    g.VerifiedQty AS VerfiedQty, 
    c.partytypeid + '-' + c.partyname as GrowerGroupName, 
    CONVERT(varchar(max), ps.partyid) + '-' + ps.partyname AS GrowerName,
    g.verifiedQty, G.ORDERQTY as OrderQty, prt.name as ItemName, pt.name as Prodetails, g.khataname as PreInwardKhata, pq.name as VarietyId, g.locationchamberid as ChamberId,
    ur.username, ISNULL(FloorName + space(1) + lf.MatrixName + lf.RowName + space(1) + lf.ColumnName, 'NA') as Templocation,
    g.locationchamberid as chamber, cm.ChallanName, st.sname as ServiceId
FROM LotOutTrans g
LEFT JOIN users ur on g.Createdby = ur.userid
LEFT JOIN PARTY C ON G.PARTYID = C.PARTYID
LEFT JOIN PARTYsub ps ON G.GrowerId = PS.PARTYID
LEFT JOIN prodtype prt ON g.itemid = prt.id
LEFT JOIN PTYPE pt ON g.packageid = pt.id
LEFT JOIN prodqaul pq ON g.varietyid = pq.id
LEFT JOIN challanmaster cm ON g.challanid = cm.id
LEFT JOIN servicetypes st ON g.schemeid = st.id
LEFT JOIN locationFinal lf ON g.lotno = lf.lotno
WHERE g.outid = @outid
GROUP BY g.DemandStatus,g.Orderbyname, g.otype, g.demandirn, g.LOTNO, g.VerifiedQty, c.partytypeid + '-' + c.partyname, convert(varchar(max), ps.partyid) + '-' + ps.partyname, prt.name, pt.name, g.khataname, pq.name, cm.ChallanName, st.sname, FloorName + space(1) + lf.MatrixName + lf.RowName + space(1) + lf.ColumnName, g.locationchamberid, g.lotirn, G.ORDERQTY, ur.username, g.orderdate, g.deliverydate, g.ordercontact, g.demandremarks
ORDER BY g.LOTNO";

            return await _sql.ExecuteReaderAsync<DemandOrderDto>(query, CommandType.Text, new SqlParameter("@outid", outid));
        }
    
        public async Task<List<DemandOrderDto>> GetDemandwithstore(int outid)
        {
            const string query = @"SELECT 
    g.DemandStatus, g.LOTNO, g.lotirn, g.demandirn, g.orderdate, g.deliverydate, g.ordercontact, g.demandremarks, g.otype, g.Orderbyname,
    ISNULL(SUM(df.orderQty), 0) as DraftedQty, 
    g.VerifiedQty AS VerfiedQty, 
    ISNULL(SO.ORDERQTY, 0) AS Netqty,
    c.partytypeid + '-' + c.partyname as GrowerGroupName, 
    CONVERT(varchar(max), ps.partyid) + '-' + ps.partyname AS GrowerName,
    g.verifiedQty, G.ORDERQTY as OrderQty, prt.name as ItemName, pt.name as Prodetails, g.khataname as PreInwardKhata, pq.name as VarietyId, g.locationchamberid as ChamberId,
    ur.username, ISNULL(FloorName + space(1) + lf.MatrixName + lf.RowName + space(1) + lf.ColumnName, 'NA') as Templocation,
    g.locationchamberid as chamber, cm.ChallanName, st.sname as ServiceId, SO.Remarks as frem
FROM LotOutTrans g
LEFT JOIN StoreOutTrans SO ON g.lotno = SO.lotno and g.OutId = SO.OutId 
LEFT JOIN DraftOutTrans df ON g.lotno = df.lotno and df.OutId = SO.OutId 
LEFT JOIN users ur on g.Createdby = ur.userid
LEFT JOIN PARTY C ON G.PARTYID = C.PARTYID
LEFT JOIN PARTYsub ps ON G.GrowerId = PS.PARTYID
LEFT JOIN prodtype prt ON g.itemid = prt.id
LEFT JOIN PTYPE pt ON g.packageid = pt.id
LEFT JOIN prodqaul pq ON g.varietyid = pq.id
LEFT JOIN challanmaster cm ON g.challanid = cm.id
LEFT JOIN servicetypes st ON g.schemeid = st.id
LEFT JOIN locationFinal lf ON g.lotno = lf.lotno
WHERE g.outid = @outid and g.flagdeleted = 0
GROUP BY g.DemandStatus, g.LOTNO, SO.Remarks, g.lotirn, g.demandirn, g.orderdate, g.deliverydate, g.ordercontact, g.demandremarks, g.otype, g.Orderbyname, g.VerifiedQty, SO.ORDERQTY, c.partytypeid + '-' + c.partyname, CONVERT(varchar(max), ps.partyid) + '-' + ps.partyname, g.verifiedQty, G.ORDERQTY, prt.name, pt.name, g.khataname, pq.name, g.locationchamberid, ur.username, ISNULL(FloorName + space(1) + lf.MatrixName + lf.RowName + space(1) + lf.ColumnName, 'NA'), g.locationchamberid, cm.ChallanName, st.sname
ORDER BY g.LOTNO";

            return await _sql.ExecuteReaderAsync<DemandOrderDto>(query, CommandType.Text, new SqlParameter("@outid", outid));
        }

        public async Task<List<DemandOrderDto>> GetDraft(int draftid)
        {
            const string query = @"SELECT 
    g.id, g.LOTNO, g.lotirn as LotIrn, g.demandirn as DemandIrn, g.orderdate, g.deliverydate, g.ordercontact, g.demandremarks, g.otype, g.Orderbyname as OrderBy, 
    g.VerifiedQty AS VerfiedQty, 
    c.partytypeid + '-' + c.partyname as GrowerGroupName, 
    CONVERT(varchar(max), ps.partyid) + '-' + ps.partyname AS GrowerName, 
    g.verifiedQty, G.ORDERQTY as OrderQty, so.orderQty as Netqty, so.orderQty as DraftedQty, 
    prt.name as ItemName, pt.name as Prodetails, g.khataname as PreInwardKhata, pq.name as VarietyId, g.locationchamberid as ChamberId, ur.username as GlobalUserName, 
    ISNULL(FloorName + space(1) + lf.MatrixName + lf.RowName + space(1) + lf.ColumnName, 'NA') as Templocation, 
    g.locationchamberid as chamber, cm.ChallanName, st.sname as ServiceId
FROM DraftOutTrans g
LEFT JOIN StoreOutTrans so on so.demandirn = g.demandirn and so.LotNo=g.lotno
LEFT JOIN LotOutTrans lt on lt.demandirn = g.demandirn and lt.LotNo=g.lotno
LEFT JOIN users ur on g.Createdby = ur.userid
LEFT JOIN PARTY C ON G.PARTYID = C.PARTYID
LEFT JOIN PARTYsub ps ON G.GrowerId = PS.PARTYID
LEFT JOIN prodtype prt ON g.itemid = prt.id
LEFT JOIN PTYPE pt ON g.packageid = pt.id
LEFT JOIN prodqaul pq ON g.varietyid = pq.id
LEFT JOIN challanmaster cm ON g.challanid = cm.id
LEFT JOIN servicetypes st ON g.schemeid = st.id
LEFT JOIN locationFinal lf ON g.lotno = lf.lotno
WHERE g.Draftid = @outid and g.flagdeleted=0
ORDER BY g.LOTNO";

            return await _sql.ExecuteReaderAsync<DemandOrderDto>(query, CommandType.Text, new SqlParameter("@outid", draftid));
        }

        public async Task<bool> SaveDemandsToDatabase(List<string> demandIRNs, DemandOrderDto EditModel, int Unit)
        {
            try
            {
                await _sql.ExecuteNonQueryAsync(CommandType.Text, "DELETE FROM ProcessedDemands");

                const string insertSql = @"INSERT INTO ProcessedDemands (DemandIRN, billdate, partyid, growerid, billtype, chamberid, Unitid, createdby) 
                                         VALUES (@DemandIRN, @Billdate, @Partyid, @GrowerId, @Billtype, @Chamber, @Unitid, @userid)";

                foreach (var irn in demandIRNs)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@DemandIRN", irn),
                        new SqlParameter("@Billdate", EditModel.OrderDate ?? (object)DBNull.Value),
                        new SqlParameter("@Partyid", EditModel.GrowerGroupName ?? (object)DBNull.Value),
                        new SqlParameter("@GrowerId", EditModel.GrowerName ?? (object)DBNull.Value),
                        new SqlParameter("@Chamber", EditModel.ChamberName ?? (object)DBNull.Value),
                        new SqlParameter("@Billtype", EditModel.OrderType ?? (object)DBNull.Value),
                        new SqlParameter("@Unitid", Unit),
                        new SqlParameter("@userid", EditModel.GlobalUserName ?? (object)DBNull.Value)
                    };
                    await _sql.ExecuteNonQueryAsync(CommandType.Text, insertSql, parameters);
                }
                return true;
            }
            catch { return false; }
        }
        public async Task<DemandOrderDto?> AddFinalDemand(DemandOrderDto EditModel)
        {
            if (EditModel == null) return null;
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@tempid", EditModel.TempOrderId),
                    new SqlParameter("@Orderby", EditModel.OrderBy ?? (object)DBNull.Value),
                    new SqlParameter("@OrderContact", EditModel.DemandContact ?? (object)DBNull.Value),
                    new SqlParameter("@OrderType", EditModel.DemandStatus ?? (object)DBNull.Value),
                    new SqlParameter("@Dremarks", EditModel.DemandRemarks ?? (object)DBNull.Value),
                    new SqlParameter("@Dated", EditModel.OrderDate ?? (object)DBNull.Value),
                    new SqlParameter("@DelDated", EditModel.DeliveryDate ?? (object)DBNull.Value),
                    new SqlParameter("@Createdby", EditModel.GlobalUserName ?? (object)DBNull.Value)
                };

                await _sql.ExecuteNonQueryAsync(CommandType.StoredProcedure, "AddFinalDemand", parameters);
                await FillValidationAsync(EditModel);
                return EditModel;
            }
            catch (Exception ex)
            {
                EditModel.RetMessage = $"Error: {ex.Message}";
                EditModel.RetFlag = "FALSE";
                return EditModel;
            }
        }

        public async Task<DemandOrderDto?> AddFinalRaw(DemandOrderDto EditModel)
        {
            if (EditModel == null) return null;
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@tempid", EditModel.TempOrderId),
                    new SqlParameter("@Orderby", EditModel.OrderBy ?? (object)DBNull.Value),
                    new SqlParameter("@OrderContact", EditModel.DemandContact ?? (object)DBNull.Value),
                    new SqlParameter("@OrderType", EditModel.TransferType ?? (object)DBNull.Value),
                    new SqlParameter("@Dremarks", EditModel.DemandRemarks ?? (object)DBNull.Value),
                    new SqlParameter("@Dated", EditModel.OrderDate ?? (object)DBNull.Value),
                    new SqlParameter("@DelDated", EditModel.DeliveryDate ?? (object)DBNull.Value),
                    new SqlParameter("@Createdby", EditModel.GlobalUserName ?? (object)DBNull.Value),
                    new SqlParameter("@Transferto", EditModel.TransferTo ?? (object)DBNull.Value)
                };

                await _sql.ExecuteNonQueryAsync(CommandType.StoredProcedure, "AddFinalRaw", parameters);
                await FillValidationAsync(EditModel);
                return EditModel;
            }
            catch (Exception ex)
            {
                EditModel.RetMessage = $"Error: {ex.Message}";
                EditModel.RetFlag = "FALSE";
                return EditModel;
            }
        }
    }
}


