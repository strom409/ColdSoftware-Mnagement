using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ColdStoreManagement.BLL.Models.DTOs;
using ColdStoreManagement.DAL.Helper;
using ColdStoreManagement.DAL.Services.Interface.TransactionsOut;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ColdStoreManagement.DAL.Services.Implementation.TransactionsOut
{
    public class StoreOutService(SQLHelperCore sql) : BaseService(sql), IStoreOutService
    {
        public async Task<List<StoreOutDto>> GetStoreOutStatus(string stat, int UnitId, string demandirn, string avuser)
        {
            // Handle Swagger string "null" vs actual null
            if (demandirn == "null") demandirn = null;
            if (stat == "null") stat = null;

            const string query = @"SELECT 
    l.isAssigned AS IsuserAssigned,
    ps.partytypeid + '-' + ps.partyname AS GrowerGroupName,
    l.demandirn AS DemandIrn,
    l.outid AS DemandNo,
    MAX(l.otype) AS OrderType,
    SUM(COALESCE(l.orderqty, 0)) AS TotalOrderQty,
    ISNULL((
        SELECT SUM(COALESCE(s.orderqty, 0)) 
        FROM StoreOutTrans s 
        WHERE s.outid = l.outid 
    ), 0) AS TotalStoreOut,
    ISNULL((
        SELECT SUM(COALESCE(df.orderqty, 0)) 
        FROM DraftOutTrans df 
        WHERE df.outid = l.outid 
    ), 0) AS DraftedQty,
    CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM StoreOutTrans s 
            WHERE s.outid = l.outid 
            AND s.PreLotPrefix = 'FORCE'
        ) THEN 'Completed'
        WHEN SUM(COALESCE(l.orderqty, 0)) = ISNULL((
            SELECT SUM(COALESCE(s.orderqty, 0)) 
            FROM StoreOutTrans s 
            WHERE s.outid = l.outid
        ), 0) THEN 'Completed'
        ELSE 'Pending'
    END AS StoreStat
FROM LotOutTrans l
LEFT OUTER JOIN party ps ON ps.partyid = l.PartyId 
WHERE 
    (
        (@status = 'Completed' AND (
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
        ))
        OR 
        (@status = 'Pending' AND (
            l.outid NOT IN (
                SELECT s.outid 
                FROM StoreOutTrans s 
                GROUP BY s.outid
                HAVING SUM(COALESCE(s.orderqty, 0)) = (
                    SELECT SUM(COALESCE(l2.orderqty, 0))
                    FROM LotOutTrans l2
                    WHERE l2.outid = s.outid
                )
            )
            AND NOT EXISTS (
                SELECT 1 
                FROM StoreOutTrans s 
                WHERE s.outid = l.outid 
                AND s.PreLotPrefix = 'FORCE'
            )
        ))
        OR (@status IS NULL OR @status = '')
    )
    AND l.UnitId = @unit 
    AND l.DemandStatus = 'Approved'
    AND (@demandirn IS NULL OR @demandirn = '' OR l.demandirn = @demandirn)
GROUP BY 
    l.outid,
    l.isAssigned,
    l.demandirn,
    ps.partytypeid + '-' + ps.partyname
ORDER BY l.outid desc";

            var parameters = new[]
            {
                new SqlParameter("@status", stat ?? (object)DBNull.Value),
                new SqlParameter("@unit", UnitId),
                new SqlParameter("@demandirn", demandirn ?? (object)DBNull.Value),
                new SqlParameter("@username", avuser ?? (object)DBNull.Value)
            };

            return await _sql.ExecuteReaderAsync<StoreOutDto>(query, CommandType.Text, parameters);
        }

        public async Task<bool> UpdateDraftQuantity(StoreOutDto EditModel)
        {
            const string query = "update StoreOutTransrep set count=1,OrderQty=@oqty where lotno=@Lotno and TempDraftId=@Tempid";
            await _sql.ExecuteNonQueryAsync(CommandType.Text, query,
                new SqlParameter("@oqty", EditModel.GradingQty),
                new SqlParameter("@Lotno", EditModel.Lotno),
                new SqlParameter("@Tempid", EditModel.TempDraftId));
            return true;
        }

        public async Task<bool> ForceUpload(int id, string Frems)
        {
            const string query = "update StoreOutTrans set PreLotPrefix=@fix,remarks=@Pkname where outid=@Id";
            await _sql.ExecuteNonQueryAsync(CommandType.Text, query,
                new SqlParameter("@Id", id),
                new SqlParameter("@Pkname", Frems),
                new SqlParameter("@fix", "FORCE"));
            return true;
        }

        public async Task<StoreOutDto?> ValidateStoreOutTransQty(StoreOutDto companyModel)
        {
            if (companyModel == null) return null;

            await _sql.ExecuteNonQueryAsync(CommandType.StoredProcedure, "ValidateStoreOutTransQty",
                new SqlParameter("@OutID", companyModel.DemandNo),
                new SqlParameter("@Lotno", companyModel.Lotno),
                new SqlParameter("@NewQty", companyModel.TotalStoreOut));

            await FillValidationAsync(companyModel);
            return companyModel;
        }

        public async Task<StoreOutDto?> AddStoreOut(StoreOutDto companyModel)
        {
            if (companyModel == null) return null;

            await _sql.ExecuteNonQueryAsync(CommandType.StoredProcedure, "AddStoreOut",
                new SqlParameter("@OutID", companyModel.DemandNo),
                new SqlParameter("@Lotno", companyModel.Lotno),
                new SqlParameter("@NewQty", companyModel.TotalStoreOut),
                new SqlParameter("@Createdby", companyModel.GlobalUserName));

            await FillValidationAsync(companyModel);
            return companyModel;
        }
    }
}
