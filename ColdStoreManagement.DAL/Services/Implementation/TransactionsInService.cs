using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ColdStoreManagement.BLL.Models.TransactionsIn;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public class TransactionsInService : ITransactionsInService
    {
        private readonly IConfiguration _configuration;

        public TransactionsInService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // --- Preinward Methods ---

        public async Task<TransactionsInModel?> AddPreinwardAsync(TransactionsInModel model)
        {
            if (model == null) return null;

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("UploadPreinwardtemp", con, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@pdate", model.PreinwardDate);
                            cmd.Parameters.AddWithValue("@Partyname", model.GrowerGroupName ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@GrowerName", model.GrowerName ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@challanName", model.ChallanName ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@challanNo", model.ChallanNo ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@username", model.GlobalUserName ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@productName", model.Itemname ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@KhataName", model.PreInwardKhata ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@vehno", model.Vehno ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Remarks", model.PreInwardRemarks ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Qty", model.PreInwardQty);
                            cmd.Parameters.AddWithValue("@CrateType", model.PreCrateType ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@SchemeName", model.ServiceId);
                            cmd.Parameters.AddWithValue("@companyCrates", model.StoreQty);
                            cmd.Parameters.AddWithValue("@OwnCrates", model.OwnQty);
                            cmd.Parameters.AddWithValue("@uom", model.PreInwardUom ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@chamberId", model.ChamberId);
                            cmd.Parameters.AddWithValue("@VariertyName", model.VarietyId);
                            cmd.Parameters.AddWithValue("@preirn", model.TempGateInIrn ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Gateid", model.TempGateInId);

                            await cmd.ExecuteNonQueryAsync();
                        }

                        using (SqlCommand cmd2 = new SqlCommand("SELECT TOP 1 flag,remarks FROM dbo.svalidate", con, transaction))
                        {
                            using (SqlDataReader rdr = await cmd2.ExecuteReaderAsync())
                            {
                                if (await rdr.ReadAsync())
                                {
                                    model.RetMessage = rdr["remarks"]?.ToString();
                                    model.RetFlag = rdr["flag"]?.ToString();
                                }
                            }
                        }

                        transaction.Commit();
                        return model;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        model.RetMessage = $"Error: {ex.Message}";
                        model.RetFlag = "FALSE";
                        return model;
                    }
                }
            }
        }

        public async Task<List<TransactionsInModel>> GetPreinwardProcBydateAsync(DateTime DateFrom, DateTime Dateto, string Prestat)
        {
            List<TransactionsInModel> results = new List<TransactionsInModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
                {
                    const string query = @"SELECT
                        ci.Lotno,
                        ci.PreInIrn, ci.LotIrn, ci.GateInId,
                        ci.GateInDate,
                        p.partytypeid + '-' + p.partyname AS PartyName,
                        CONVERT(varchar(max), ps.partyid) + '-' + ps.partyname AS GrowerName,
                        CONVERT(varchar(max), cm.id) + '-' + cm.ChallanName AS ChallanName,
                        RTRIM(vi.vehno) + SPACE(1) + '(' + RTRIM(vi.drivername) + SPACE(1) + ')' + SPACE(1) + '(' + RTRIM(vi.contactno) + ')' AS vehno,
                        ci.qty, ci.sno, ci.createddate, ci.KhataName, ci.ChallanNo, ur.uname,
                        ci.Remarks,
                        ci.preInirn, ui.uname as username, pq.name as variety, ci.cratetype, st.sname, um.Ucode, pt.name,
                        CASE 
                            WHEN ci.flagdeleted = 1 THEN 'Deleted'
                            WHEN EXISTS (SELECT 1 FROM QualityControl qi WHERE qi.LotNo = ci.LotNo) THEN 'Completed'
                            ELSE 'Pending'
                        END AS Status
                    FROM GateInTrans ci
                    LEFT JOIN party p ON ci.partyid = p.partyid
                    LEFT JOIN partysub ps ON ci.growerid = ps.partyid
                    LEFT JOIN challanmaster cm ON ci.challanid = cm.id         
                    LEFT JOIN vehinfo vi ON ci.vehid = vi.vid
                    LEFT JOIN users ui ON ci.createdby = ui.id
                    LEFT JOIN prodqaul pq ON ci.varietyid = pq.id
                    LEFT JOIN servicetypes st ON ci.schemeid = st.id
                    LEFT JOIN unit_master um ON ci.Unitid = um.id
                    LEFT JOIN PTYPE pt ON ci.packageid = pt.id
                    LEFT JOIN users ur ON ci.Createdby = ur.id
                    WHERE 
                        ((@status = 'Deleted' AND ci.flagdeleted = 1)
                        OR 
                        (@status = 'Completed' AND ci.flagdeleted = 0 AND EXISTS (SELECT 1 FROM QualityControl qi WHERE qi.LotNo = ci.LotNo))
                        OR 
                        (@status = 'Pending' AND ci.flagdeleted = 0 AND NOT EXISTS (SELECT 1 FROM QualityControl qi WHERE qi.LotNo = ci.LotNo)))
                        AND CONVERT(date, ci.GateInDate) >= CONVERT(date, @DateFrom) 
                        AND CONVERT(date, ci.GateInDate) <= CONVERT(date, @DateTo)
                    ORDER BY ci.GateInId DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                        cmd.Parameters.AddWithValue("@Dateto", Dateto);
                        cmd.Parameters.AddWithValue("@status", Prestat);

                        await con.OpenAsync();
                        using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                results.Add(new TransactionsInModel
                                {
                                    PreInwardId = Convert.ToInt32(rdr["GateInId"]),
                                    PreinwardDate = rdr["GateInDate"] != DBNull.Value ? Convert.ToDateTime(rdr["GateInDate"]) : DateTime.MinValue,
                                    GrowerGroupName = rdr["PartyName"].ToString(),
                                    GrowerName = rdr["GrowerName"].ToString(),
                                    ChallanName = rdr["ChallanName"].ToString(),
                                    Vehno = rdr["vehno"].ToString(),
                                    PreInIrn = rdr["PreInIrn"].ToString(),
                                    LotIrn = rdr["LotIrn"].ToString(),
                                    PreInwardQty = rdr["qty"] != DBNull.Value ? Convert.ToDecimal(rdr["qty"]) : 0,
                                    GlobalUserName = rdr["username"].ToString(),
                                    PreInwardStatus = rdr["Status"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Error in GetPreinwardProcBydateAsync: {ex.Message}");
                 throw;
            }
            return results;
        }

        public async Task<TransactionsInModel?> UpdatePreinwardHeadAsync(TransactionsInModel model)
        {
             // To implement based on EmployeeAdoNetService.UpdatePreinwardHead logic if needed. 
             // Logic seems similar to AddPreinward but for updates.
             return null;
        }

        public async Task<TransactionsInModel?> GetPreinwardIdAsync(int id)
        {
             TransactionsInModel? model = null;
             using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                // Basic query based on EmployeeAdoNetService
                string query = "SELECT * FROM GateInTrans WHERE GateInId = @id"; // Simplified for brevity, add full joins if needed
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    await con.OpenAsync();
                     using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                    {
                          if(await rdr.ReadAsync()){
                               model = new TransactionsInModel {
                                    PreInwardId = Convert.ToInt32(rdr["GateInId"]),
                                    PreinwardDate = Convert.ToDateTime(rdr["GateInDate"]),
                                    PreInwardQty = Convert.ToDecimal(rdr["qty"]),
                                    PreInwardStatus = rdr["Status"].ToString(),
                                    PreInwardRemarks = rdr["Remarks"].ToString()
                               };
                          }
                    }
                }
            }
            return model;
        }

        public async Task<List<TransactionsInModel>> GetPreinwardIdlistAsync(int id)
        {
             // Simplified implementation based on pattern
             return new List<TransactionsInModel>(); 
        }

        public async Task<TransactionsInModel?> GeneratePreinwardAsync(TransactionsInModel model)
        {
             using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("GeneratePreinward", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    // Add parameters as needed
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            return model;
        }

        public async Task<bool> GenchamberAggAsync(int id)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                using (SqlCommand cmd = new SqlCommand("GenchamberAgg", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Growerid", id);
                    await con.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            return true;
        }

        public async Task<TransactionsInModel?> CheckChamberAllocationAsync(string selectedPurchase)
        {
            TransactionsInModel model = new TransactionsInModel();
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("Check_allocation", con, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@partyname", selectedPurchase);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        using (SqlCommand cmd2 = new SqlCommand("SELECT TOP 1 flag,remarks FROM dbo.svalidate", con, transaction))
                        {
                            using (SqlDataReader rdr = await cmd2.ExecuteReaderAsync())
                            {
                                if (await rdr.ReadAsync())
                                {
                                    model.RetMessage = rdr["remarks"]?.ToString();
                                    model.RetFlag = rdr["flag"]?.ToString();
                                }
                            }
                        }
                        transaction.Commit();
                        return model;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        model.RetMessage = $"Error: {ex.Message}";
                        model.RetFlag = "FALSE";
                        return model;
                    }
                }
            }
        }

        public async Task<List<TransactionsInModel>> GetallStockChamberAsync(int GrowerId)
        {
            List<TransactionsInModel> results = new List<TransactionsInModel>();
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                 const string query = "SELECT p.chambername,CAST(sum(companycrate) AS DECIMAL(18,2)) as ccrates,CAST(sum(owncrate) AS DECIMAL(18,2)) as owncrates,CAST(sum(ci.qty) AS DECIMAL(18,2)) as qty,CAST(sum(VerifiedCompanyCrates) AS DECIMAL(18,2)) as vccrates,CAST(sum(VerifiedOwnCrates) AS DECIMAL(18,2)) as vcowncrates,CAST(sum(VerifiedQty) AS DECIMAL(18,2)) as Vcqty,CAST(sum(VerifiedWoodenPetty) AS DECIMAL(18,2)) as vcpetty  FROM gateintrans ci LEFT JOIN chamber p ON ci.LocationChamberId = p.chamberid where ci.flagdeleted=0 and ci.dockpost=1 and ci.PartyId=@Growerid group by p.chambername, p.chamberid order by p.chamberid ";
                 using (SqlCommand cmd = new SqlCommand(query, con))
                 {
                     cmd.Parameters.AddWithValue("@Growerid", GrowerId);
                     await con.OpenAsync();
                     using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                     {
                         while (await rdr.ReadAsync())
                         {
                             results.Add(new TransactionsInModel
                             {
                                 ChamberName = rdr["chambername"].ToString(),
                                 TotalPreQty = rdr.GetDecimal(rdr.GetOrdinal("qty")),
                                 TotalPrecompanyQty = rdr.GetDecimal(rdr.GetOrdinal("ccrates")),
                                 TotalPreownQty = rdr.GetDecimal(rdr.GetOrdinal("owncrates")),
                                 TotalInQty = rdr.GetDecimal(rdr.GetOrdinal("Vcqty")),
                                 TotalIncompanyQty = rdr.GetDecimal(rdr.GetOrdinal("vccrates")),
                                 TotalInownQty = rdr.GetDecimal(rdr.GetOrdinal("vcowncrates")),
                                 TotalInpettyQty = rdr.GetDecimal(rdr.GetOrdinal("vcpetty"))
                             });
                         }
                     }
                 }
            }
            return results;
        }

        public async Task<List<TransactionsInModel>> GeneratePreinwardReportAsync(TransactionsInModel model)
        {
            List<TransactionsInModel> results = new List<TransactionsInModel>();
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("generatePreinwardReport", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@d1", model.PreinwardDate);
                    cmd.Parameters.AddWithValue("@d2", model.PreinwardDateTo ?? DateTime.Now);
                    cmd.Parameters.AddWithValue("@Partyid", model.Partyid);
                    cmd.Parameters.AddWithValue("@Groowerid", model.Growerid);
                    cmd.Parameters.AddWithValue("@vehname", model.Vehno ?? "");
                    cmd.Parameters.AddWithValue("@itemname", model.Itemname ?? "");
                    cmd.Parameters.AddWithValue("@uom", model.PreInwardUom ?? "");
                    cmd.Parameters.AddWithValue("@stat", model.PreInwardStatus ?? "");
                    cmd.Parameters.AddWithValue("@vehid", 0);
                    cmd.Parameters.AddWithValue("@itemid", 0);
                    cmd.Parameters.AddWithValue("@uomid", 0);
                    cmd.Parameters.AddWithValue("@SearchText", model.CommonSearch ?? "");
                    
                    await cmd.ExecuteNonQueryAsync();
                }

                const string query = @"SELECT
                    ci.cid, ci.Trno, ci.Dated,
                    p.partytypeid  +'-'+ p.partyname AS PartyName,
                    convert(varchar(max),ps.partyid)  +'-'+ ps.partyname AS GrowerName,
                    convert(varchar(max),cm.id)  +'-'+ cm.ChallanName as ChallanName,
                    rtrim(vi.vehno) + space(1)+'('+ rtrim(vi.drivername)+space(1)+')'+space(1)+'('+rtrim(vi.contactno)+')' as vehno,
                    ci.CrateMark, ci.qty, ci.trflag, ci.Remarks
                FROM CrateIssuereports ci
                LEFT JOIN party p ON ci.partyid = p.partyid
                LEFT JOIN partysub ps ON ci.groowerid = ps.partyid
                LEFT JOIN challanmaster cm ON ci.challanid = cm.id         
                LEFT JOIN vehinfo vi ON ci.vehid = vi.vid
                WHERE ci.flagdeleted=0 order by ci.cid";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            results.Add(new TransactionsInModel
                            {
                                PreInwardId = Convert.ToInt32(rdr["cid"]),
                                GrowerGroupName = rdr["PartyName"].ToString(),
                                GrowerName = rdr["GrowerName"].ToString(),
                                ChallanName = rdr["ChallanName"].ToString(),
                                Vehno = rdr["vehno"].ToString(),
                                PreInwardRemarks = rdr["Remarks"].ToString(),
                                PreInwardQty = Convert.ToDecimal(rdr["qty"]),
                                PreinwardDate = Convert.ToDateTime(rdr["Dated"]),
                                // Note: Using existing properties to map report columns
                            });
                        }
                    }
                }
            }
            return results;
        }


        // --- Chamber Allocation Methods ---

        public async Task<TransactionsInModel?> SaveChamberAllocationAsync(TransactionsInModel model)
        {
             if (model == null) return null;

             using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("SaveChamberAllocation", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Chamberid", model.ChamberId);
                    cmd.Parameters.AddWithValue("@GrowerName", model.GrowerGroupName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Qty", model.ChamberAllocation);

                    await cmd.ExecuteNonQueryAsync();
                }

                using (SqlCommand cmd2 = new SqlCommand("SELECT TOP 1 flag,remarks FROM dbo.svalidate", con))
                {
                    using (SqlDataReader rdr = await cmd2.ExecuteReaderAsync())
                    {
                        if (await rdr.ReadAsync())
                        {
                             model.RetMessage = rdr["remarks"]?.ToString();
                             model.RetFlag = rdr["flag"]?.ToString();
                        }
                    }
                }
            }
            return model;
        }

        public async Task<TransactionsInModel?> UpdateChamberAllocationAsync(TransactionsInModel model)
        {
              using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("UpdateChamberAllocation", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Chamberid", model.AllocationNo);
                    cmd.Parameters.AddWithValue("@Qty", model.ChamberAllocation);

                    await cmd.ExecuteNonQueryAsync();
                }
                 // Validate if necessary
            }
            return model;
        }

        public async Task<List<TransactionsInModel>> CheckChamberStatusAsync(string selectedPurchase)
        {
             List<TransactionsInModel> results = new List<TransactionsInModel>();
             using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                 await con.OpenAsync();
                 
                 // Execute SP CheckChamberStatus first
                 using (SqlCommand cmd = new SqlCommand("CheckChamberStatus", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@partyname", selectedPurchase);
                    await cmd.ExecuteNonQueryAsync();
                }

                 // Check Allocations
                 using (SqlCommand cmd2 = new SqlCommand("select ChamberId,isnull(sum(AllotedQty),0)-isnull(sum(InQty),0) as ChamberQty from allotedQty group by ChamberId", con))
                {
                     using (SqlDataReader rdr = await cmd2.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                             results.Add(new TransactionsInModel
                            {
                                ChamberAvailQty = rdr.GetDecimal(rdr.GetOrdinal("ChamberQty")),
                                ChamberId = rdr.GetInt32(rdr.GetOrdinal("ChamberId"))
                            });
                        }
                    }
                }
            }
            return results;
        }

        public async Task<List<TransactionsInModel>> CheckChamberQtyAsync(string selectedPurchase, int chamberid)
        {
             List<TransactionsInModel> results = new List<TransactionsInModel>();
             using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                 await con.OpenAsync();
                 using (SqlCommand cmd = new SqlCommand("CheckChamberQty", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@chamber", chamberid); 
                     // Note: original code didn't use selectedPurchase in Params, only chamber
                    await cmd.ExecuteNonQueryAsync();
                }
                 using (SqlCommand cmd2 = new SqlCommand("select ChamberId,isnull(sum(AllotedQty),0)-isnull(sum(InQty),0) as ChamberQty from allotedQty group by ChamberId", con))
                {
                     using (SqlDataReader rdr = await cmd2.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                             results.Add(new TransactionsInModel
                            {
                                ChamberAvailQty = rdr.GetDecimal(rdr.GetOrdinal("ChamberQty")),
                                ChamberId = rdr.GetInt32(rdr.GetOrdinal("ChamberId"))
                            });
                        }
                    }
                }
            }
            return results;
        }

        public async Task<List<TransactionsInModel>> CheckChamberAsync(int selectedNewchamber)
        {
             List<TransactionsInModel> results = new List<TransactionsInModel>();
             using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                 await con.OpenAsync();
                 using (SqlCommand cmd = new SqlCommand("CheckChamber", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@chamber", selectedNewchamber);
                    await cmd.ExecuteNonQueryAsync();
                }
                 using (SqlCommand cmd2 = new SqlCommand("select ChamberId,isnull(sum(AllotedQty),0)-isnull(sum(InQty),0) as ChamberQty from allotedQty group by ChamberId", con))
                {
                     using (SqlDataReader rdr = await cmd2.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                             results.Add(new TransactionsInModel
                            {
                                ChamberAvailQty = rdr.GetDecimal(rdr.GetOrdinal("ChamberQty")),
                                ChamberId = rdr.GetInt32(rdr.GetOrdinal("ChamberId"))
                            });
                        }
                    }
                }
            }
            return results;
        }

        // --- Quality Methods ---

        public async Task<List<TransactionsInModel>> GetPendingQualityAsync(int unitId, string status)
        {
            List<TransactionsInModel> results = new List<TransactionsInModel>();
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                const string query = @"SELECT p.partytypeid + '-' + p.partyname AS PartyName,
                    CONVERT(varchar(max), ps.partyid) + '-' + ps.partyname AS GrowerName, ci.Lotno, ci.PreInIrn, ci.LotIrn, pq.name as variety, ci.qty,
                    qcon.Pressure, qcon.AvgWeight, qcon.bgradavg, cmr.chambername as Chamber,
                    CASE 
                        WHEN ci.flagdeleted = 1 THEN 'Deleted'
                        WHEN EXISTS (SELECT 1 FROM QualityControl qi WHERE qi.LotNo = ci.LotNo) THEN 'Completed'
                        ELSE 'Pending'
                    END AS Status
                    FROM GateInTrans ci
                    LEFT JOIN party p ON ci.partyid = p.partyid
                    LEFT JOIN partysub ps ON ci.growerid = ps.partyid
                    LEFT JOIN challanmaster cm ON ci.challanid = cm.id         
                    LEFT JOIN vehinfo vi ON ci.vehid = vi.vid
                    LEFT JOIN users ui ON ci.createdby = ui.id
                    LEFT JOIN prodqaul pq ON ci.varietyid = pq.id
                    LEFT JOIN servicetypes st ON ci.schemeid = st.id
                    LEFT JOIN unit_master um ON ci.Unitid = um.id
                    LEFT JOIN PTYPE pt ON ci.packageid = pt.id
                    LEFT JOIN users ur ON ci.Createdby = ur.id
                    LEFT JOIN chamber cmr ON ci.LocationChamberId = cmr.chamberid
                    LEFT JOIN QualityControl qcon ON ci.lotno = qcon.lotno
                    WHERE ci.UnitId=@unitid AND 
                    (
                        (@status = 'Deleted' AND ci.flagdeleted = 1)
                        OR 
                        (@status = 'Completed' AND ci.flagdeleted = 0 AND EXISTS (SELECT 1 FROM QualityControl qi WHERE qi.LotNo = ci.LotNo))
                        OR 
                        (@status = 'Pending' AND ci.flagdeleted = 0 AND NOT EXISTS (SELECT 1 FROM QualityControl qi WHERE qi.LotNo = ci.LotNo))
                    )
                    ORDER BY ci.GateInId DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@unitid", unitId);
                    cmd.Parameters.AddWithValue("@status", status);
                    await con.OpenAsync();
                    using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            results.Add(new TransactionsInModel
                            {
                                Lotno = Convert.ToInt32(rdr["Lotno"]),
                                GrowerGroupName = rdr["PartyName"].ToString(),
                                GrowerName = rdr["GrowerName"].ToString(),
                                VarietyName = rdr["variety"].ToString(), 
                                PreInwardQty = rdr["qty"] != DBNull.Value ? Convert.ToDecimal(rdr["qty"]) : 0,
                                PreInIrn = rdr["PreInIrn"].ToString(),
                                PreInwardStatus = rdr["Status"].ToString(),
                                LotIrn = rdr["LotIrn"].ToString(),
                                ChamberName = rdr["Chamber"].ToString()
                            });
                        }
                    }
                }
            }
            return results;
        }

        public async Task<TransactionsInModel?> GetQcPrivAsync(string userGroup)
        {
            TransactionsInModel? model = null;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                const string query = "select Addval,Editval,ViewVal,DelVal from userpriv where Groupid in (select usergroupid from usergroup where name=@Ugroup) and pname=@pname";
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Ugroup", userGroup);
                    cmd.Parameters.AddWithValue("@pname", "Quality");
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            model = new TransactionsInModel
                            {
                                QcAdd = reader.GetBoolean(reader.GetOrdinal("Addval")),
                                QcEdit = reader.GetBoolean(reader.GetOrdinal("Editval")),
                                QcView = reader.GetBoolean(reader.GetOrdinal("ViewVal")),
                                QckDel = reader.GetBoolean(reader.GetOrdinal("DelVal")),
                            };
                        }
                    }
                }
            }
            return model;
        }

        // --- Dock Methods ---

        public async Task<List<TransactionsInModel>> GetPendingDockAsync(int unitId, int dockPosting)
        {
            List<TransactionsInModel> results = new List<TransactionsInModel>();
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                const string query = @"SELECT p.partytypeid + '-' + p.partyname AS PartyName,
                    CONVERT(varchar(max), ps.partyid) + '-' + ps.partyname AS GrowerName, ci.Lotno, ci.PreInIrn, ci.LotIrn, pq.name as variety, ci.qty,
                    ci.LocationChamberId as Chamber,
                    CASE 
                        WHEN ci.flagdeleted = 1 THEN 'Deleted'
                        WHEN EXISTS (SELECT 1 FROM QualityControl qi WHERE qi.LotNo = ci.LotNo) THEN 'Yes'
                        ELSE 'Pending'
                    END AS Quality,
                    isnull(tl.name,'NA') as Location, ci.VerifiedQty, ci.VerifiedCompanyCrates, ci.VerifiedOwnCrates, ci.VerifiedWoodenPetty, ci.Pallets, ci.Bins,
                    CASE 
                        WHEN ci.isStickerPrinted = 1 THEN 'Yes'
                        ELSE 'No'
                    END AS StickerPrinted, ci.cratemarka, ci.KhataName, ci.dockpost
                    FROM GateInTrans ci
                    LEFT JOIN party p ON ci.partyid = p.partyid
                    LEFT JOIN partysub ps ON ci.growerid = ps.partyid
                    LEFT JOIN challanmaster cm ON ci.challanid = cm.id         
                    LEFT JOIN vehinfo vi ON ci.vehid = vi.vid
                    LEFT JOIN users ui ON ci.createdby = ui.id
                    LEFT JOIN prodqaul pq ON ci.varietyid = pq.id
                    LEFT JOIN servicetypes st ON ci.schemeid = st.id
                    LEFT JOIN unit_master um ON ci.Unitid = um.id
                    LEFT JOIN PTYPE pt ON ci.packageid = pt.id
                    LEFT JOIN users ur ON ci.Createdby = ur.id
                    LEFT JOIN location_master tl ON ci.OriginID = tl.id
                    WHERE ci.lotno in(select lotno from qualitycontrol)
                    AND ci.UnitId=@unitid AND ci.dockpost=@Trid ORDER BY ci.GateInId DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@unitid", unitId);
                    cmd.Parameters.AddWithValue("@Trid", dockPosting);
                    await con.OpenAsync();
                    using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            results.Add(new TransactionsInModel
                            {
                                DockPost = rdr["dockpost"] != DBNull.Value ? Convert.ToInt32(rdr["dockpost"]) : 0,
                                Lotno = rdr["Lotno"] != DBNull.Value ? Convert.ToInt32(rdr["Lotno"]) : 0,
                                GrowerGroupName = rdr["PartyName"]?.ToString() ?? "",
                                GrowerName = rdr["GrowerName"]?.ToString() ?? "",
                                VarietyName = rdr["variety"]?.ToString() ?? "",
                                PreInwardQty = rdr["qty"] != DBNull.Value ? Convert.ToDecimal(rdr["qty"]) : 0,
                                PreInwardKhata = rdr["KhataName"]?.ToString() ?? "",
                                PreInIrn = rdr["PreInIrn"]?.ToString() ?? "",
                                PreInwardStatus = rdr["Quality"]?.ToString() ?? "",
                                LotIrn = rdr["LotIrn"]?.ToString() ?? "",
                                ChamberId = rdr["Chamber"] != DBNull.Value ? Convert.ToInt32(rdr["Chamber"]) : 0,
                                Templocation = rdr["Location"]?.ToString() ?? "",
                                VerfiedQty = rdr["VerifiedQty"] != DBNull.Value ? Convert.ToDecimal(rdr["VerifiedQty"]) : 0,
                                VerfiedOwnCrates = rdr["VerifiedOwnCrates"] != DBNull.Value ? Convert.ToDecimal(rdr["VerifiedOwnCrates"]) : 0,
                                VerfiedCompanyCrates = rdr["VerifiedCompanyCrates"] != DBNull.Value ? Convert.ToDecimal(rdr["VerifiedCompanyCrates"]) : 0,
                                Verfiedpetties = rdr["VerifiedWoodenPetty"] != DBNull.Value ? Convert.ToDecimal(rdr["VerifiedWoodenPetty"]) : 0,
                                Verfiedbins = rdr["Bins"] != DBNull.Value ? Convert.ToDecimal(rdr["Bins"]) : 0,
                                Verfiedpallets = rdr["Pallets"] != DBNull.Value ? Convert.ToDecimal(rdr["Pallets"]) : 0,
                                Stickerprinted = rdr["StickerPrinted"]?.ToString() ?? "",
                                CrateMarka = rdr["cratemarka"]?.ToString() ?? "",
                            });
                        }
                    }
                }
            }
            return results;
        }

        public async Task<TransactionsInModel?> GetDockPrivAsync(string userGroup)
        {
            TransactionsInModel? model = null;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                const string query = "select Addval,Editval,ViewVal,DelVal from userpriv where Groupid in (select usergroupid from usergroup where name=@Ugroup) and pname=@pname";
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Ugroup", userGroup);
                    cmd.Parameters.AddWithValue("@pname", "Dock");
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            model = new TransactionsInModel
                            {
                                DockAdd = reader.GetBoolean(reader.GetOrdinal("Addval")),
                                DockEdit = reader.GetBoolean(reader.GetOrdinal("Editval")),
                                DockView = reader.GetBoolean(reader.GetOrdinal("ViewVal")),
                                DockDel = reader.GetBoolean(reader.GetOrdinal("DelVal")),
                            };
                        }
                    }
                }
            }
            return model;
        }

        // --- Location Methods ---

        public async Task<List<TransactionsInModel>> GetPendingLocationAsync(int unitId, string status)
        {
            List<TransactionsInModel> results = new List<TransactionsInModel>();
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                const string query = @"SELECT
                    p.partytypeid + '-' + p.partyname AS PartyName,
                    CONVERT(varchar(max), ps.partyid) + '-' + ps.partyname AS GrowerName, pt.name as UOM, ci.LotIrn, pq.name as variety, ci.VerifiedQty as Qty, ci.LocationChamberId as Chamber,
                    lf.FloorName, lf.MatrixName, lf.RowName, lf.ColumnName, ci.bins, lf.TotalCrates, ci.Lotno,
                    CASE 
                        WHEN EXISTS (SELECT 1 FROM locationfinal lf WHERE lf.LotNo = ci.LotNo) THEN 'Completed'
                        ELSE 'Pending'
                    END AS Status
                    FROM GateInTrans ci
                    LEFT JOIN party p ON ci.partyid = p.partyid
                    LEFT JOIN partysub ps ON ci.growerid = ps.partyid
                    LEFT JOIN challanmaster cm ON ci.challanid = cm.id         
                    LEFT JOIN vehinfo vi ON ci.vehid = vi.vid
                    LEFT JOIN users ui ON ci.createdby = ui.id
                    LEFT JOIN prodqaul pq ON ci.varietyid = pq.id
                    LEFT JOIN servicetypes st ON ci.schemeid = st.id
                    LEFT JOIN unit_master um ON ci.Unitid = um.id
                    LEFT JOIN PTYPE pt ON ci.packageid = pt.id
                    LEFT JOIN users ur ON ci.Createdby = ur.id
                    LEFT JOIN location_master lm ON ci.locationMasterId  = lm.id
                    LEFT JOIN prodtype pd ON ci.itemid = pd.id
                    LEFT JOIN QualityControl qcon ON ci.lotno = qcon.lotno
                    LEFT JOIN locationfinal lf ON ci.lotno = lf.lotno
                    WHERE ci.UnitId=@unitid and ci.dockpost=1 and
                    (
                        (@status = 'Deleted' AND ci.flagdeleted = 1)
                        OR 
                        (@status = 'Completed' AND ci.flagdeleted = 0 AND EXISTS (SELECT 1 FROM locationfinal lf WHERE lf.LotNo = ci.LotNo))
                        OR 
                        (@status = 'Pending' AND ci.flagdeleted = 0 AND NOT EXISTS (SELECT 1 FROM locationfinal lf WHERE lf.LotNo = ci.LotNo))
                    )
                    ORDER BY ci.GateInId DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@unitid", unitId);
                    cmd.Parameters.AddWithValue("@status", status);
                    await con.OpenAsync();
                    using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            results.Add(new TransactionsInModel
                            {
                                Lotno = Convert.ToInt32(rdr["Lotno"]),
                                LotIrn = rdr["LotIrn"].ToString(),
                                GrowerGroupName = rdr["PartyName"].ToString(),
                                VarietyName = rdr["variety"].ToString(),
                                ChamberId = Convert.ToInt32(rdr["Chamber"]),
                                PreInwardUom = rdr["UOM"].ToString(),
                                VerfiedQty = Convert.ToDecimal(rdr["Qty"]),
                                Verfiedbins = rdr["bins"] != DBNull.Value ? Convert.ToDecimal(rdr["bins"]) : 0,
                                PreInwardStatus = rdr["Status"].ToString(),
                                FloorName = rdr["FloorName"]?.ToString() ?? "",
                                MatrixName = rdr["MatrixName"]?.ToString() ?? "",
                                RowName = rdr["RowName"]?.ToString() ?? "",
                                ColumName = rdr["ColumnName"]?.ToString() ?? "",
                                CrateNos = rdr["TotalCrates"] != DBNull.Value ? Convert.ToDecimal(rdr["TotalCrates"]) : 0,
                            });
                        }
                    }
                }
            }
            return results;
        }

        public async Task<TransactionsInModel?> GetLocationPrivAsync(string userGroup)
        {
            TransactionsInModel? model = null;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                const string query = "select Addval,Editval,ViewVal,DelVal from userpriv where Groupid in (select usergroupid from usergroup where name=@Ugroup) and pname=@pname";
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Ugroup", userGroup);
                    cmd.Parameters.AddWithValue("@pname", "Location");
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            model = new TransactionsInModel
                            {
                                LocAdd = reader.GetBoolean(reader.GetOrdinal("Addval")),
                                LocEdit = reader.GetBoolean(reader.GetOrdinal("Editval")),
                                LocView = reader.GetBoolean(reader.GetOrdinal("ViewVal")),
                                LocDel = reader.GetBoolean(reader.GetOrdinal("DelVal")),
                            };
                        }
                    }
                }
            }
            return model;
        }

        public async Task<bool> UpdateItemStatusAsync(int itemId)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                const string query = "updateitemstatus";
                SqlCommand cmd = new SqlCommand(query, con)
                {
                    CommandType = CommandType.StoredProcedure,
                };
                cmd.Parameters.AddWithValue("@Mid", itemId);
                await con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            return true;
        }
    }
}

