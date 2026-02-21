using System.Data;
using Microsoft.Data.SqlClient;
using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.BLL.Models.Crate;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.Extensions.Configuration;
using ColdStoreManagement.DAL.Helper;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public class CrateService: BaseService, ICrateService
    {
        private readonly IConfiguration _configuration;

        public CrateService(SQLHelperCore sql, IConfiguration configuration) : base(sql)
        {
            _configuration = configuration;
        }

        // ================== EXISTS ==================

        public async Task<bool> DoesCrateTypeExistAsync(string name)
            => (await _sql.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM crtypes WHERE name=@Name",
                CommandType.Text,
                new SqlParameter("@Name", name))) > 0;

        public async Task<bool> DoesCrateFlagExistAsync(string name)
            => (await _sql.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM CrateFlags WHERE name=@Name",
                CommandType.Text,
                new SqlParameter("@Name", name))) > 0;


        // ================== CRUD ==================

        public async Task<bool> AddCrateTypeAsync(CompanyModel m)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                @"insert into dbo.crtypes (id,name,Crqty) 
                        values((select isnull(max(id+1),1) from crtypes),@Crname,@Crqty)",
                new SqlParameter("@Crname", m.Crname),
                new SqlParameter("@Crqty", m.Crqty));

            return true;
        }

        public async Task<bool> UpdateCrateTypeAsync(CompanyModel m)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                "update dbo.crtypes set name=@Crname,Crqty=@Crqty where  id=@Id",
                new SqlParameter("@id", m.Crid),
                new SqlParameter("@Crname", m.Crname),
                new SqlParameter("@Crqty", m.Crqty));
            return true;
        }

        public async Task<bool> DeleteCrateTypeAsync(int id)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                "DELETE crtypes WHERE id=@id",
                new SqlParameter("@id", id));
            return true;
        }

        public async Task<bool> AddCrateFlagAsync(CrateFlags model)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                @"insert into dbo.CrateFlags (id,name,ftype,srtype)  
                    values((select isnull(max(id+1),1) from CrateFlags),@Cfname,@Cfstat,@Cflag)",

                new SqlParameter("@Cfname", model.Name),
                new SqlParameter("@Cfstat", model.Ftype),
                new SqlParameter("@Cflag", model.Srtype));

            return true;
        }
        public async Task<bool> UpdateCrateFlagAsync(CrateFlags model)
        {
            const string query = "update dbo.CrateFlags set name=@Cfname,ftype=@Cfstat,srtype=@Cflag where id=@Id";
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                query,
                new SqlParameter("@id", model.Id),
                new SqlParameter("@Cfname", model.Name),
                new SqlParameter("@Cfstat", model.Ftype),
                new SqlParameter("@Cflag", model.Srtype));

            return true;
        }
        public async Task<bool> DeleteCrateFlagAsync(int id)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                "DELETE CrateFlags WHERE id=@id",
                new SqlParameter("@id", id));

            return true;
        }
        public async Task<bool> DeleteCrateTypes(int id)
        {
            await _sql.ExecuteNonQueryAsync(
               CommandType.Text,
               "DELETE dbo.crtypes WHERE id=@id",
               new SqlParameter("@id", id));

            return true;
        }
        public async Task<bool> UpdateCratetypes(CrateType crateType)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                @"update dbo.crtypes set name=@Crname,Crqty=@Crqty where id=@Id",
                new SqlParameter("@Id", crateType.Id),
                new SqlParameter("@Crname", crateType.Name),
                new SqlParameter("@Crqty", crateType.Crqty));

            return true;
        }


        public async Task<List<CompanyModel>> GetDailyCratesByDateAsync(DateTime from, DateTime to)
        {
            using var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                @"SELECT cid,Trno,Dated,CrateMark,qty,trflag,Remarks
                  FROM crateissue
                  WHERE trflag='Crate Issue' AND flagdeleted=0
                  AND CONVERT(date,Dated) BETWEEN @f AND @t
                  ORDER BY cid DESC",
                new SqlParameter("@f", from),
                new SqlParameter("@t", to));

            return MapCrateIssue(ds);
        }

        public async Task<List<CompanyModel>> GetDailyCratesOutByDateAsync(DateTime from, DateTime to)
        {
            using var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                @"SELECT cid,Trno,Dated,CrateMark,qty,trflag,Remarks
                  FROM crateissue
                  WHERE trflag IN ('Empty Returned','Petty Returned')
                  AND flagdeleted=0
                  AND CONVERT(date,Dated) BETWEEN @f AND @t
                  ORDER BY cid DESC",
                new SqlParameter("@f", from),
                new SqlParameter("@t", to));

            return MapCrateIssue(ds);
        }

        public async Task<List<CompanyModel>> GetDailyCratesEmptyByDateAsync(DateTime from, DateTime to)
        {
            using var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                @"SELECT cid,Trno,Dated,CrateMark,qty,trflag,Remarks
                  FROM crateissue
                  WHERE trflag='Empty Receive'
                  AND flagdeleted=0
                  AND CONVERT(date,Dated) BETWEEN @f AND @t
                  ORDER BY cid DESC",
                new SqlParameter("@f", from),
                new SqlParameter("@t", to));

            return MapCrateIssue(ds);
        }
        public async Task<List<CompanyModel>> GetDailyCratesAdjustmentAsync()
        {
            using var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                @"SELECT cid,Trno,Dated,qty,remarks
                  FROM crateadjustment
                  WHERE flagdeleted=0
                  ORDER BY cid DESC");

            if (ds.Tables.Count == 0)
                return new List<CompanyModel>();

            return ds.Tables[0].AsEnumerable()
                .Select(r => new CompanyModel
                {
                    CrissueId = r.Field<int>("cid"),
                    CrateCrn = r["Trno"]?.ToString(),
                    CrateIssueDated = r.Field<DateTime>("Dated"),
                    CrissueQty = r.Field<decimal>("qty"),
                    CrissueRemarks = r["remarks"]?.ToString()
                })
                .ToList();
        }

        public async Task<List<CompanyModel>> GetDailyCratesAdjByDateAsync(DateTime from, DateTime to)
        {
            using var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                @"SELECT cid,Trno,Dated,qty,remarks
                  FROM crateadjustment
                  WHERE flagdeleted=0
                  AND CONVERT(date,Dated) BETWEEN @f AND @t
                  ORDER BY cid DESC",
                new SqlParameter("@f", from),
                new SqlParameter("@t", to));

            if (ds.Tables.Count == 0)
                return new List<CompanyModel>();

            return ds.Tables[0].AsEnumerable()
                .Select(r => new CompanyModel
                {
                    CrissueId = r.Field<int>("cid"),
                    CrateCrn = r["Trno"]?.ToString(),
                    CrateIssueDated = r.Field<DateTime>("Dated"),
                    CrissueQty = r.Field<decimal>("qty"),
                    CrissueRemarks = r["remarks"]?.ToString()
                })
                .ToList();
        }
        public async Task<List<CompanyModel>> GetCrateSummarySubGrowerAsync(string party)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "CratesummarySub",
                new SqlParameter("@partyname", party));

            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                "SELECT Agreement,crateissue,Cratereceive,EmptyReceive FROM CrateAnalysissummary");

            if (ds.Tables.Count == 0)
                return new List<CompanyModel>();
            return ds.Tables[0].AsEnumerable()
                .Select(r => new CompanyModel
                {
                    CrateAgreement = r.Field<decimal>("Agreement"),
                    CrateIssue = r.Field<decimal>("crateissue"),
                    CrateReceive = r.Field<decimal>("Cratereceive"),
                    EmptyReceive = r.Field<decimal>("EmptyReceive")
                })
                .ToList();
        }
        public async Task<List<CompanyModel>> GenerateCrateReportAsync(CompanyModel filter)
        {
            using var ds = await _sql.ExecuteDatasetAsync(
                CommandType.StoredProcedure,
                "GenerateCrateReport",
                new SqlParameter("@FromDate", filter.CrateDatefrom),
                new SqlParameter("@ToDate", filter.CrateDateto),
                new SqlParameter("@PartyId", filter.Partyid),
                new SqlParameter("@GrowerId", filter.Growerid),
                new SqlParameter("@CrateMark", filter.CrissueMark),
                new SqlParameter("@Flag", filter.CrissueFlag));

            return MapCrateIssue(ds);
        }
        public async Task<List<CompanyModel>> CheckCratesAgreementOnVQtyAsync(string party)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "CheckCratesAgreementvqty",
                new SqlParameter("@partyname", party));

            using var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                "SELECT SUM(AgreeQty)-SUM(receiveQty) AS Bookqty FROM CHECKCRATEQTY");

            if (ds.Tables.Count == 0)
                return new List<CompanyModel>();

            return ds.Tables[0].AsEnumerable()
                .Select(r => new CompanyModel
                {
                    PrebookQty = r.Field<decimal>("Bookqty")
                })
                .ToList();
        }

        public async Task<List<CompanyModel>> CheckCratesPartyOutAsync(string party, string flag)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "CheckCratesPartyout",
                new SqlParameter("@partyname", party),
                new SqlParameter("@trflag", flag));

            using var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                "SELECT AGREEMENT,Receive,returned,Balance FROM CrateAnalysis");
            if (ds.Tables.Count == 0)
                return new List<CompanyModel>();

            return ds.Tables[0].AsEnumerable()
                .Select(r => new CompanyModel
                {
                    CrateAgreement = r.Field<decimal>("AGREEMENT"),
                    CrateReceive = r.Field<decimal>("Receive"),
                    EmptyReceive = r.Field<decimal>("returned"),
                    CrateAvailable = r.Field<decimal>("Balance")
                })
                .ToList();
        }

        public async Task<List<CompanyModel>> CheckCratesPartyEmptyAsync(string party)
            => await CheckCratesPartyAsync(party);

        public async Task<List<CompanyModel>> CheckCratesPartySubAsync(string party, string grower)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "CheckCratesPartysub",
                new SqlParameter("@partyname", party),
                new SqlParameter("@growername", grower));

            return await CheckCratesPartyAsync(party);
        }

        public async Task<List<CompanyModel>> CheckCratesPartySubEmptyAsync(string party, string grower)
            => await CheckCratesPartySubAsync(party, grower);

        public async Task<List<CompanyModel>> CheckCratesPartySubOutAsync(string party, string grower, string flag)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "CheckCratesPartysubout",
                new SqlParameter("@partyname", party),
                new SqlParameter("@growername", grower),
                new SqlParameter("@trflag", flag));

            return await CheckCratesPartyAsync(party);
        }



        // ================== ISSUE ==================

        public async Task<CompanyModel?> AddCrateIssueAsync(CompanyModel m)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "addCrateIssueproc",
                new SqlParameter("@Cdate", m.CrateIssueDated),
                new SqlParameter("@Partyname", m.GrowerGroupName),
                new SqlParameter("@GrowerName", m.GrowerName),
                new SqlParameter("@challanName", m.ChallanName),
                new SqlParameter("@userid", m.UserId),
                new SqlParameter("@crateMark", m.CrissueMark),
                new SqlParameter("@Vehno", m.Vehno),
                new SqlParameter("@Remarks", m.CrissueRemarks),
                new SqlParameter("@Qty", m.CrissueQty));

            await FillValidationAsync(m);
            return m;
        }

        public async Task<int> GetMaxCrateIssueIdAsync()
        {
            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                "SELECT MAX(cid) AS cid FROM crateissue");

            if (ds.Tables.Count == 0)
                return 0;

            return ds.Tables[0].Rows.Count == 0
                ? 0
                : Convert.ToInt32(ds.Tables[0].Rows[0]["cid"]);
        }
        public async Task<List<CompanyModel>> GetAllCrateMarksAsync()
        {
           using  var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                "SELECT DISTINCT CrateMark FROM Crateissue");
            if (ds.Tables.Count == 0)
                return new List<CompanyModel>();

            return ds.Tables[0].AsEnumerable()
                .Select(r => new CompanyModel
                {
                    CrissueMark = r["CrateMark"]?.ToString()
                })
                .ToList();
        }
        public async Task<List<CompanyModel>> GetDailyCratesAsync()
        {
            using var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                @"SELECT cid,Trno,Dated,CrateMark,qty,trflag,Remarks
                  FROM crateissue
                  WHERE trflag='Crate Issue' AND flagdeleted=0
                  AND CONVERT(date,Dated)=CONVERT(date,GETDATE())
                  ORDER BY cid DESC");

            return MapCrateIssue(ds);
        }
        public async Task<List<CompanyModel>> GetDailyCratesOutAsync()
        {
            using var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                @"SELECT cid,Trno,Dated,CrateMark,qty,trflag,Remarks
                  FROM crateissue
                  WHERE trflag IN ('Empty Returned','Petty Returned')
                  AND flagdeleted=0
                  AND CONVERT(date,Dated)=CONVERT(date,GETDATE())
                  ORDER BY cid DESC");

            return MapCrateIssue(ds);
        }
        public async Task<List<CompanyModel>> GetDailyCratesEmptyAsync()
        {
            using var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                @"SELECT cid,Trno,Dated,CrateMark,qty,trflag,Remarks
                  FROM crateissue
                  WHERE trflag='Empty Receive'
                  AND flagdeleted=0
                  AND CONVERT(date,Dated)=CONVERT(date,GETDATE())
                  ORDER BY cid DESC");

            return MapCrateIssue(ds);
        }

        public async Task<List<CompanyModel>> GetDailyCratesByDateAsync(DateTime from, DateTime to, string flag)
        {
            using var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                @"SELECT cid,Trno,Dated,CrateMark,qty,trflag,Remarks
                      FROM crateissue
                      WHERE trflag=@flag AND flagdeleted=0
                      AND CONVERT(date,Dated) BETWEEN @f AND @t
                      ORDER BY cid DESC",
                new SqlParameter("@flag", flag),
                new SqlParameter("@f", from),
                new SqlParameter("@t", to));

            return MapCrateIssue(ds);
        }
        public async Task<List<CompanyModel>> GetCrateSummaryMainAsync()
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "CratesummaryMain");

            using var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                @"SELECT partyid,Agreement,crateissue,Cratereceive,EmptyReceive,
                  (crateissue)-(Cratereceive+EmptyReceive) AS Balance
                  FROM CrateAnalysissummary");

            if (ds.Tables.Count == 0)
                return new List<CompanyModel>();

            return ds.Tables[0].AsEnumerable()
                .Select(r => new CompanyModel
                {
                    Partyid = r.Field<int>("partyid"),
                    CrateAgreement = r.Field<decimal>("Agreement"),
                    CrateIssue = r.Field<decimal>("crateissue"),
                    CrateReceive = r.Field<decimal>("Cratereceive"),
                    EmptyReceive = r.Field<decimal>("EmptyReceive"),
                    CrateBalance = r.Field<decimal>("Balance")
                })
                .ToList();
        }
        public async Task<List<CompanyModel>> CheckCratesPartyAsync(string party)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "CheckCratesParty",
                new SqlParameter("@partyname", party));

            using var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                "SELECT agreement,crateissue,CrateReceive,EmptyReceive,availqty FROM CrateAnalysis");

            if (ds.Tables.Count == 0)
                return new List<CompanyModel>();

            return ds.Tables[0].AsEnumerable()
                .Select(r => new CompanyModel
                {
                    CrateAgreement = r.Field<decimal>("agreement"),
                    CrateIssue = r.Field<decimal>("crateissue"),
                    CrateReceive = r.Field<decimal>("CrateReceive"),
                    EmptyReceive = r.Field<decimal>("EmptyReceive"),
                    CrateAvailable = r.Field<decimal>("availqty")
                })
                .ToList();
        }
        public async Task<List<CompanyModel>> CheckCratesAgreementAsync(string party)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "CheckCratesAgreement",
                new SqlParameter("@partyname", party));

            using var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                "SELECT SUM(AgreeQty)-SUM(receiveQty) AS Bookqty FROM CHECKCRATEQTY");

            if (ds.Tables.Count == 0)
                return new List<CompanyModel>();

            return ds.Tables[0].AsEnumerable()
                .Select(r => new CompanyModel
                {
                    PrebookQty = r.Field<decimal>("Bookqty")
                })
                .ToList();
        }


        private static List<CompanyModel> MapCrateIssue(DataSet ds)
        {
            if (ds.Tables.Count == 0)
                return new List<CompanyModel>();

            return ds.Tables[0].AsEnumerable()
                .Select(r => new CompanyModel
                {
                    CrissueId = r.Field<int>("cid"),
                    CrateCrn = r["Trno"]?.ToString(),
                    CrateIssueDated = r.Field<DateTime>("Dated"),
                    CrissueMark = r["CrateMark"]?.ToString(),
                    CrissueQty = r.Field<decimal>("qty"),
                    CrissueFlag = r["trflag"]?.ToString(),
                    CrissueRemarks = r["Remarks"]?.ToString()
                })
                .ToList();
        }



        public async Task<CrateModel?> GetCrateIssueDetAsync(int selectedGrowerId)

        {
            CrateModel? crateModel = null;
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
                {
                    await con.OpenAsync();
                    var sql = @"SELECT
                                ci.cid,
                                ci.Dated,
                                p.partytypeid +'-'+ p.partyname AS PartyName,
                                convert(varchar(max),ps.partyid) +'-'+ ps.partyname AS GrowerName,
                                convert(varchar(max),cm.id) +'-'+ cm.ChallanName as challanname,
                                rtrim(vi.vehno) + space(1)+'('+ rtrim(vi.drivername)+space(1)+')'+space(1)+'('+rtrim(vi.contactno)+')' as vehno,
                                ci.CrateMark,
                                ci.qty,
                                ci.trflag,
                                ci.Remarks
                            FROM crateissue ci
                            LEFT JOIN party p ON ci.partyid = p.partyid
                            LEFT JOIN partysub ps ON ci.groowerid = ps.partyid
                            LEFT JOIN challanmaster cm ON ci.challanid = cm.id         
                            LEFT JOIN vehinfo vi ON ci.vehid = vi.vid
                            WHERE ci.cid=@orderid";

                    using (var cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@orderid", selectedGrowerId);

                        using (var rdr = await cmd.ExecuteReaderAsync())
                        {
                            if (await rdr.ReadAsync())
                            {
                                crateModel = new CrateModel
                                {
                                    CrissueId = rdr["cid"] != DBNull.Value ? Convert.ToInt32(rdr["cid"]) : 0,
                                    GrowerGroupName = rdr["PartyName"].ToString(),
                                    GrowerName = rdr["GrowerName"].ToString(),
                                    ChallanName = rdr["challanname"].ToString(),
                                    Vehno = rdr["vehno"].ToString(),
                                    CrissueMark = rdr["CrateMark"].ToString(),
                                    CrissueQty = rdr["qty"] != DBNull.Value ? Convert.ToDecimal(rdr["qty"]) : 0,
                                    CrateIssueDated = rdr["Dated"] != DBNull.Value ? Convert.ToDateTime(rdr["Dated"]) : DateTime.MinValue,
                                    CrissueFlag = rdr["trflag"].ToString(),
                                    CrissueRemarks = rdr["Remarks"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCrateIssueDetAsync: {ex.Message}");
                throw;
            }
            return crateModel;
        }

        public async Task<CrateModel?> GetCratePrivs2Async(string Ugroup)
        {
            CrateModel? crateModel = null;
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
                {
                    await con.OpenAsync();
                    const string query = "select Addval,Editval,ViewVal,DelVal from userpriv where Groupid in (select usergroupid from usergroup where name=@Ugroup) and pname=@pname";

                    using (var cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Ugroup", Ugroup ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@pname", "Crates");

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                crateModel = new CrateModel
                                {
                                    CrateAdd = reader.GetBoolean(reader.GetOrdinal("Addval")),
                                    CrateEdit = reader.GetBoolean(reader.GetOrdinal("Editval")),
                                    CrateView = reader.GetBoolean(reader.GetOrdinal("ViewVal")),
                                    CrateDel = reader.GetBoolean(reader.GetOrdinal("DelVal"))
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCratePrivs2Async: {ex.Message}");
                throw;
            }
            return crateModel;
        }

        public async Task<List<CrateModel>> GetallCrateflagallAsync()
        {
            List<CrateModel> Cflaglist = new List<CrateModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
                {
                    const string query = "select distinct trflag as name from dbo.crateissue";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        await con.OpenAsync();
                        using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                Cflaglist.Add(new CrateModel
                                {
                                    CrissueFlag = rdr["name"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetallCrateflagallAsync: {ex.Message}");
                throw;
            }
            return Cflaglist;
        }

        public async Task<List<CrateModel>> GetCrateordersEmptyAsync()
        {
            List<CrateModel> Purchaseorders = new List<CrateModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
                {
                    const string query = "select distinct cid from crateissue where flagdeleted=0 and Trflag='Empty Receive'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        await con.OpenAsync();
                        using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                Purchaseorders.Add(new CrateModel
                                {
                                    CrissueId = rdr["cid"] != DBNull.Value ? Convert.ToInt32(rdr["cid"]) : 0
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCrateordersEmptyAsync: {ex.Message}");
                throw;
            }
            return Purchaseorders;
        }

        public async Task<CrateModel?> GenerateCrateReportPdfAsync(CrateModel model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
                {
                    await con.OpenAsync();
                    var whereConditions = new List<string>();
                    var parameters = new List<SqlParameter>();

                    whereConditions.Add("ci.flagdeleted = 0");

                    // Date range filter
                    if (model.CrateDatefrom != null && model.CrateDateto != null)
                    {
                        whereConditions.Add("ci.Dated BETWEEN @d1 AND @d2");
                        parameters.Add(new SqlParameter("@d1", model.CrateDatefrom));
                        parameters.Add(new SqlParameter("@d2", model.CrateDateto));
                    }
                    else if (model.CrateDatefrom != null)
                    {
                        whereConditions.Add("ci.Dated >= @d1");
                        parameters.Add(new SqlParameter("@d1", model.CrateDatefrom));
                    }
                    else if (model.CrateDateto != null)
                    {
                        whereConditions.Add("ci.Dated <= @d2");
                        parameters.Add(new SqlParameter("@d2", model.CrateDateto));
                    }

                    // Party ID filter
                    if (model.Partyid > 0)
                    {
                        whereConditions.Add("ci.partyid = @Partyid");
                        parameters.Add(new SqlParameter("@Partyid", model.Partyid));
                    }

                    // Grower ID filter
                    if (model.Growerid > 0)
                    {
                        whereConditions.Add("ci.groowerid = @Groowerid");
                        parameters.Add(new SqlParameter("@Groowerid", model.Growerid));
                    }

                    // Challan ID filter
                    if (model.ChallanId > 0)
                    {
                        whereConditions.Add("ci.challanid = @challanId");
                        parameters.Add(new SqlParameter("@challanId", model.ChallanId));
                    }

                    // Crate Mark filter
                    if (!string.IsNullOrWhiteSpace(model.CrissueMark))
                    {
                        whereConditions.Add("ci.CrateMark = @cratemark");
                        parameters.Add(new SqlParameter("@cratemark", model.CrissueMark));
                    }
                    else
                    {
                        whereConditions.Add("(ci.CrateMark IS NOT NULL OR ci.CrateMark IS NULL)");
                    }

                    // Transaction Flag filter
                    if (!string.IsNullOrWhiteSpace(model.CrissueFlag))
                    {
                        whereConditions.Add("ci.trflag = @trflag");
                        parameters.Add(new SqlParameter("@trflag", model.CrissueFlag));
                    }
                    else
                    {
                        whereConditions.Add("(ci.trflag IS NOT NULL OR ci.trflag IS NULL)");
                    }

                    // Vehicle Name filter
                    if (!string.IsNullOrWhiteSpace(model.Vehno))
                    {
                        whereConditions.Add("vi.vehno LIKE '%' + @vehname + '%'");
                        parameters.Add(new SqlParameter("@vehname", model.Vehno));
                    }
                    else
                    {
                        whereConditions.Add("(vi.vehno IS NOT NULL OR vi.vehno IS NULL)");
                    }

                    string whereClause = string.Join(" AND ", whereConditions);

                    // Delete existing REPORT data (assuming this is a temp reporting table based on logic)
                    using (SqlCommand cmd2 = new SqlCommand(@"DELETE FROM crateissuereports", con))
                    {
                        cmd2.CommandType = CommandType.Text;
                        await cmd2.ExecuteNonQueryAsync();
                    }

                    // Insert new data
                    var insertSql = $@"
                        INSERT INTO crateissuereports(cid, Trno, Dated, PartyName, GrowerName, ChallanName, vehid, vehname,CrateMark, qty, trflag, Remarks, cidval)
                        SELECT
                            ci.cid,
                            ci.Trno,
                            ci.Dated,
                            p.partytypeid + '-' + p.partyname AS PartyName,
                            CONVERT(VARCHAR(MAX), ps.partyid) + '-' + ps.partyname AS GrowerName,
                            CONVERT(VARCHAR(MAX), cm.id) + '-' + cm.ChallanName as ChallanName,
                            ci.vehid,
                            RTRIM(vi.vehno) + SPACE(1) + '(' + RTRIM(vi.drivername) + SPACE(1) + ')' + SPACE(1) + '(' + RTRIM(vi.contactno) + ')' as vehno,
                            ci.CrateMark,
                            ci.qty,
                            ci.trflag,
                            ci.Remarks,
                            1
                        FROM CrateIssue ci
                        LEFT JOIN party p ON ci.partyid = p.partyid
                        LEFT JOIN partysub ps ON ci.groowerid = ps.partyid
                        LEFT JOIN challanmaster cm ON ci.challanid = cm.id         
                        LEFT JOIN vehinfo vi ON ci.vehid = vi.vid
                        WHERE {whereClause}
                        ORDER BY ci.cid";

                    using (SqlCommand cmd = new SqlCommand(insertSql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.Add(new SqlParameter(param.ParameterName, param.Value ?? DBNull.Value));
                        }
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GenerateCrateReportPdfAsync: {ex.Message}");
                throw;
            }
            return model;
        }

        public async Task<List<CrateModel>> GenerateCrateReportModelAsync(CrateModel model)
        {
            List<CrateModel> reportData = new List<CrateModel>();

            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
                {
                    await con.OpenAsync();

                    var whereConditions = new List<string>();
                    var parameters = new List<SqlParameter>();

                    whereConditions.Add("ci.flagdeleted = 0");

                    // Date range filter
                    if (model.CrateDatefrom != null && model.CrateDateto != null)
                    {
                        whereConditions.Add("ci.Dated BETWEEN @d1 AND @d2");
                        parameters.Add(new SqlParameter("@d1", model.CrateDatefrom));
                        parameters.Add(new SqlParameter("@d2", model.CrateDateto));
                    }
                    else if (model.CrateDatefrom != null)
                    {
                        whereConditions.Add("ci.Dated >= @d1");
                        parameters.Add(new SqlParameter("@d1", model.CrateDatefrom));
                    }
                    else if (model.CrateDateto != null)
                    {
                        whereConditions.Add("ci.Dated <= @d2");
                        parameters.Add(new SqlParameter("@d2", model.CrateDateto));
                    }

                    // Party ID filter
                    if (model.Partyid > 0)
                    {
                        whereConditions.Add("ci.partyid = @Partyid");
                        parameters.Add(new SqlParameter("@Partyid", model.Partyid));
                    }

                    // Grower ID filter
                    if (model.Growerid > 0)
                    {
                        whereConditions.Add("ci.groowerid = @Groowerid");
                        parameters.Add(new SqlParameter("@Groowerid", model.Growerid));
                    }

                    // Challan ID filter
                    if (model.ChallanId > 0)
                    {
                        whereConditions.Add("ci.challanid = @challanId");
                        parameters.Add(new SqlParameter("@challanId", model.ChallanId));
                    }

                    // Crate Mark filter
                    if (!string.IsNullOrWhiteSpace(model.CrissueMark))
                    {
                        whereConditions.Add("ci.CrateMark = @cratemark");
                        parameters.Add(new SqlParameter("@cratemark", model.CrissueMark));
                    }
                    else
                    {
                        whereConditions.Add("(ci.CrateMark IS NOT NULL OR ci.CrateMark IS NULL)");
                    }

                    // Transaction Flag filter
                    if (!string.IsNullOrWhiteSpace(model.CrissueFlag))
                    {
                        whereConditions.Add("ci.trflag = @trflag");
                        parameters.Add(new SqlParameter("@trflag", model.CrissueFlag));
                    }
                    else
                    {
                        whereConditions.Add("(ci.trflag IS NOT NULL OR ci.trflag IS NULL)");
                    }

                    // Vehicle Name filter
                    if (!string.IsNullOrWhiteSpace(model.Vehno))
                    {
                        whereConditions.Add("vi.vehno LIKE '%' + @vehname + '%'");
                        parameters.Add(new SqlParameter("@vehname", model.Vehno));
                    }
                    else
                    {
                        whereConditions.Add("(vi.vehno IS NOT NULL OR vi.vehno IS NULL)");
                    }

                    string whereClause = string.Join(" AND ", whereConditions);

                    string query = $@"
                        SELECT
                            ci.cid,
                            ci.Trno,
                            ci.Dated,
                            p.partytypeid + '-' + p.partyname AS PartyName,
                            CONVERT(VARCHAR(MAX), ps.partyid) + '-' + ps.partyname AS GrowerName,
                            CONVERT(VARCHAR(MAX), cm.id) + '-' + cm.ChallanName as ChallanName,
                            RTRIM(vi.vehno) + SPACE(1) + '(' + RTRIM(vi.drivername) + SPACE(1) + ')' + SPACE(1) + '(' + RTRIM(vi.contactno) + ')' as vehno,
                            ci.CrateMark,
                            ci.qty,
                            ci.trflag,
                            ci.Remarks
                        FROM CrateIssue ci
                        LEFT JOIN party p ON ci.partyid = p.partyid
                        LEFT JOIN partysub ps ON ci.groowerid = ps.partyid
                        LEFT JOIN challanmaster cm ON ci.challanid = cm.id         
                        LEFT JOIN vehinfo vi ON ci.vehid = vi.vid
                        WHERE {whereClause}
                        ORDER BY ci.cid";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;

                        foreach (var param in parameters)
                        {
                            cmd.Parameters.Add(param);
                        }

                        using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                CrateModel crateModel = new CrateModel
                                {
                                    CrissueId = Convert.ToInt32(rdr["cid"]),
                                    GrowerGroupName = rdr["PartyName"].ToString(),
                                    GrowerName = rdr["GrowerName"].ToString(),
                                    ChallanName = rdr["challanname"].ToString(),
                                    Vehno = rdr["vehno"].ToString(),
                                    CrissueMark = rdr["CrateMark"].ToString(),
                                    CrissueQty = Convert.ToDecimal(rdr["qty"]),
                                    CrateIssueDated = Convert.ToDateTime(rdr["Dated"]),
                                    CrissueFlag = rdr["trflag"].ToString(),
                                    CrissueRemarks = rdr["Remarks"].ToString(),
                                    CrateCrn = rdr["Trno"].ToString(),
                                };
                                reportData.Add(crateModel);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine($"Error in GenerateCrateReportAsync: {ex.Message}");
                throw;
            }

            return reportData;
        }

        public async Task<List<CrateModel>> GetCrateSummaryMainModelAsync()
        {
            List<CrateModel> summaryData = new List<CrateModel>();

            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd3 = new SqlCommand("CratesummaryMain", con))
                    {
                        cmd3.CommandType = CommandType.StoredProcedure;
                        await cmd3.ExecuteNonQueryAsync();
                    }

                    const string query = @"select b.partyid,
                                           b.partytypeid +'-'+ b.partyname as Partyname,
                                           a.Agreement,
                                           a.crateissue,
                                           a.Cratereceive as Filled,
                                           a.EmptyReceive as Empty,
                                           adjgiven as Adjustmentto,
                                           adjreceived as Adjustmentreceive,
                                           (crateissue+adjreceived)-(crateReceive+emptyReceive+adjgiven) as CrBalance,
                                           GrowerReceive,
                                           GrowerReturn,
                                           GrowerReceive-GrowerReturn as SelfBalance,
                                           pettyReceive,
                                           PettyReturn,
                                           pettyReceive-PettyReturn as Pbalance
                                           from CrateAnalysissummary a,party b where a.partyid=b.partyid order by a.partyid";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                CrateModel crateModel = new CrateModel
                                {
                                    Partyid = Convert.ToInt32(rdr["partyid"]),
                                    GrowerGroupName = rdr["Partyname"].ToString(),
                                    CrateAgreement = rdr.GetDecimal(rdr.GetOrdinal("Agreement")),
                                    CrateIssue = rdr.GetDecimal(rdr.GetOrdinal("crateissue")),
                                    CrateReceive = rdr.GetDecimal(rdr.GetOrdinal("Filled")),
                                    EmptyReceive = rdr.GetDecimal(rdr.GetOrdinal("Empty")),
                                    CrateAdjustmentGiven = rdr.GetDecimal(rdr.GetOrdinal("Adjustmentto")),
                                    CrateAdjustmentTaken = rdr.GetDecimal(rdr.GetOrdinal("Adjustmentreceive")),
                                    CrateBalance = rdr.GetDecimal(rdr.GetOrdinal("CrBalance")),
                                    SelfCrateReceive = rdr.GetDecimal(rdr.GetOrdinal("GrowerReceive")),
                                    EmptyReturn = rdr.GetDecimal(rdr.GetOrdinal("GrowerReturn")),
                                    SelfCrateBalance = rdr.GetDecimal(rdr.GetOrdinal("SelfBalance")),
                                    PettyReceive = rdr.GetDecimal(rdr.GetOrdinal("pettyReceive")),
                                    PettyReturn = rdr.GetDecimal(rdr.GetOrdinal("PettyReturn")),
                                    PettyBalance = rdr.GetDecimal(rdr.GetOrdinal("Pbalance"))
                                };
                                summaryData.Add(crateModel);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCrateSummaryMainAsync: {ex.Message}");
                throw;
            }

            return summaryData;
        }

        public async Task<List<CrateModel>> GetDailyCratesProcEmptyAsync()
        {
            List<CrateModel> getDailyCrates = new List<CrateModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
                {
                    const string query = @"SELECT
                        ci.cid,ci.Trno,
                        ci.Dated,
                        p.partytypeid  +'-'+ p.partyname AS PartyName,
                        convert(varchar(max),ps.partyid)  +'-'+ ps.partyname AS GrowerName,
                        convert(varchar(max),cm.id)  +'-'+ cm.ChallanName as ChallanName,
                        rtrim(vi.vehno) + space(1)+'('+ rtrim(vi.drivername)+space(1)+')'+space(1)+'('+rtrim(vi.contactno)+')' as vehno,
                        ci.CrateMark,
                        ci.qty,
                        ci.trflag,
                        ci.Remarks
                    FROM crateissue ci
                    LEFT JOIN party p ON ci.partyid = p.partyid
                    LEFT JOIN partysub ps ON ci.groowerid = ps.partyid
                    LEFT JOIN challanmaster cm ON ci.challanid = cm.id         
                    LEFT JOIN vehinfo vi ON ci.vehid = vi.vid
                    WHERE  ci.trflag='Empty Receive' and ci.flagdeleted=0 and CONVERT(date, ci.Dated) = CONVERT(date, GETDATE()) order by ci.cid desc";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        await con.OpenAsync();
                        using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                CrateModel crateModel = new CrateModel
                                {
                                    CrissueId = Convert.ToInt32(rdr["cid"]),
                                    GrowerGroupName = rdr["PartyName"].ToString(),
                                    GrowerName = rdr["GrowerName"].ToString(),
                                    ChallanName = rdr["challanname"].ToString(),
                                    Vehno = rdr["vehno"].ToString(),
                                    CrissueMark = rdr["CrateMark"].ToString(),
                                    CrissueQty = Convert.ToDecimal(rdr["qty"]),
                                    CrateIssueDated = Convert.ToDateTime(rdr["Dated"]),
                                    CrissueFlag = rdr["trflag"].ToString(),
                                    CrissueRemarks = rdr["Remarks"].ToString(),
                                    CrateCrn = rdr["Trno"].ToString(),
                                };
                                getDailyCrates.Add(crateModel);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetDailyCratesProcEmptyAsync: {ex.Message}");
                throw;
            }
            return getDailyCrates;
        }

        public async Task<List<CrateModel>> GetDailyCratesProcOutAsync()
        {
            List<CrateModel> getDailyCrates = new List<CrateModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
                {
                    const string query = @"SELECT
                        ci.cid,ci.Trno,
                        ci.Dated,
                        p.partytypeid  +'-'+ p.partyname AS PartyName,
                        convert(varchar(max),ps.partyid)  +'-'+ ps.partyname AS GrowerName,
                        convert(varchar(max),cm.id)  +'-'+ cm.ChallanName as ChallanName,
                        rtrim(vi.vehno) + space(1)+'('+ rtrim(vi.drivername)+space(1)+')'+space(1)+'('+rtrim(vi.contactno)+')' as vehno,
                        ci.CrateMark,
                        ci.qty,
                        ci.trflag,
                        ci.Remarks
                    FROM crateissue ci
                    LEFT JOIN party p ON ci.partyid = p.partyid
                    LEFT JOIN partysub ps ON ci.groowerid = ps.partyid
                    LEFT JOIN challanmaster cm ON ci.challanid = cm.id         
                    LEFT JOIN vehinfo vi ON ci.vehid = vi.vid
                    WHERE  ci.trflag in('Empty Returned','Petty Returned') and ci.flagdeleted=0 and CONVERT(date, ci.Dated) = CONVERT(date, GETDATE()) order by ci.cid desc";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        await con.OpenAsync();
                        using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                CrateModel crateModel = new CrateModel
                                {
                                    CrissueId = Convert.ToInt32(rdr["cid"]),
                                    GrowerGroupName = rdr["PartyName"].ToString(),
                                    GrowerName = rdr["GrowerName"].ToString(),
                                    ChallanName = rdr["challanname"].ToString(),
                                    Vehno = rdr["vehno"].ToString(),
                                    CrissueMark = rdr["CrateMark"].ToString(),
                                    CrissueQty = Convert.ToDecimal(rdr["qty"]),
                                    CrateIssueDated = Convert.ToDateTime(rdr["Dated"]),
                                    CrissueFlag = rdr["trflag"].ToString(),
                                    CrissueRemarks = rdr["Remarks"].ToString(),
                                    CrateCrn = rdr["Trno"].ToString(),
                                };
                                getDailyCrates.Add(crateModel);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetDailyCratesProcOutAsync: {ex.Message}");
                throw;
            }
            return getDailyCrates;
        }

        public async Task<List<CrateModel>> CheckCratesPartyOutModelAsync(string growerGroupName, string flag)
        {
            List<CrateModel> result = new List<CrateModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("CheckCratesPartyout", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@partyname", growerGroupName);
                        cmd.Parameters.AddWithValue("@trflag", flag);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    using (SqlCommand cmd2 = new SqlCommand("SELECT partyid ,AGREEMENT,CrateReceive+PettyReceive AS Receive ,GrowerReturn+PettyReturn as returned,((CrateReceive+PettyReceive)-(GrowerReturn+PettyReturn)) as Balance FROM CrateAnalysis ", con))
                    {
                        using (SqlDataReader rdr = await cmd2.ExecuteReaderAsync())
                        {
                            if (await rdr.ReadAsync())
                            {
                                result.Add(new CrateModel
                                {
                                    CrateAgreement = rdr.GetDecimal(rdr.GetOrdinal("agreement")),
                                    CrateIssue = 0,
                                    CrateReceive = rdr.GetDecimal(rdr.GetOrdinal("Receive")),
                                    EmptyReceive = rdr.GetDecimal(rdr.GetOrdinal("returned")),
                                    CrateAvailable = rdr.GetDecimal(rdr.GetOrdinal("Balance"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CheckCratesPartyOutAsync: {ex.Message}");
                throw;
            }
            return result;
        }

        public async Task<List<CrateModel>> CheckCratesPartySubOutModelAsync(string growerName, string growerGroupName, string flag)
        {
            List<CrateModel> result = new List<CrateModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("CheckCratesPartysubout", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@partyname", growerGroupName);
                        cmd.Parameters.AddWithValue("@growername", growerName); // Note: Original parameter name might be confusing, keeping procedure param names
                        cmd.Parameters.AddWithValue("@trflag", flag);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    using (SqlCommand cmd2 = new SqlCommand("SELECT partyid ,AGREEMENT,CrateReceive+PettyReceive AS Receive ,GrowerReturn+PettyReturn as returned,((CrateReceive+PettyReceive)-(GrowerReturn+PettyReturn)) as Balancenew FROM CrateAnalysis", con))
                    {
                        using (SqlDataReader rdr = await cmd2.ExecuteReaderAsync())
                        {
                            if (await rdr.ReadAsync())
                            {
                                result.Add(new CrateModel
                                {
                                    CrateAgreement = rdr.GetDecimal(rdr.GetOrdinal("AGREEMENT")),
                                    CrateIssue = 0,
                                    CrateReceive = rdr.GetDecimal(rdr.GetOrdinal("Receive")),
                                    EmptyReceive = rdr.GetDecimal(rdr.GetOrdinal("returned")),
                                    CrateAvailable = rdr.GetDecimal(rdr.GetOrdinal("Balancenew"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CheckCratesPartySubOutAsync: {ex.Message}");
                throw;
            }
            return result;
        }

        public async Task<CrateModel> AddCrateOutAsync(CrateModel model)
        {
           // if (model == null) return null;

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("addCrateOut", con, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@Cdate", model.CrateIssueDated);
                            cmd.Parameters.AddWithValue("@Partyname", model.GrowerGroupName);
                            cmd.Parameters.AddWithValue("@GrowerName", model.GrowerName);
                            cmd.Parameters.AddWithValue("@challanName", model.ChallanName);
                            cmd.Parameters.AddWithValue("@userid", model.UserId);
                            cmd.Parameters.AddWithValue("@crateMark", model.CrissueMark);
                            cmd.Parameters.AddWithValue("@Vehno", model.Vehno);
                            cmd.Parameters.AddWithValue("@Remarks", model.CrissueRemarks);
                            cmd.Parameters.AddWithValue("@Qty", model.CrissueQty);
                            cmd.Parameters.AddWithValue("@trflag", model.CrissueFlag);

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

        public async Task<CrateModel> UpdateCrateIssueOutAsync(CrateModel model)
        {
            //if (model == null) return null;
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("UpdateCrateIssueout", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Cid", model.CrissueId);
                        cmd.Parameters.AddWithValue("@Cdate", model.CrateIssueDated);
                        cmd.Parameters.AddWithValue("@Partyname", model.GrowerGroupName);
                        cmd.Parameters.AddWithValue("@GrowerName", model.GrowerName);
                        cmd.Parameters.AddWithValue("@challanName", model.ChallanName);
                        cmd.Parameters.AddWithValue("@userid ", model.UserId);
                        cmd.Parameters.AddWithValue("@crateMark", model.CrissueMark);
                        cmd.Parameters.AddWithValue("@Vehno", model.Vehno);
                        cmd.Parameters.AddWithValue("@Remarks", model.CrissueRemarks);
                        cmd.Parameters.AddWithValue("@Qty", model.CrissueQty);
                        cmd.Parameters.AddWithValue("@trflag", model.CrissueFlag);

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
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.RetMessage = $"Error: {ex.Message}";
                model.RetFlag = "FALSE";
                return model;
            }
        }

        public async Task<CrateModel> DeleteCrateIssueAsync(int id, CrateModel model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("DeleteCrateIssue", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Cid", id);
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
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.RetMessage = $"Error: {ex.Message}";
                model.RetFlag = "FALSE";
                return model;
            }
        }

        public async Task<List<CrateModel>> GenerateCratePreviewAsync(int id, CrateModel model)
        {
            List<CrateModel> result = new List<CrateModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("crateissueprint", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@cid", id);
                        cmd.Parameters.AddWithValue("@crflag", model.CrissueFlag);
                        await cmd.ExecuteNonQueryAsync();
                    }
                    // The original method returns 'companyModel' (single), but typically preview generation might not return a list.
                    // It seems it just executes the SP (maybe filling a temp table for reporting?).
                    // Returing list with 1 item or empty to satisfy interface.
                    result.Add(model);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GenerateCratePreviewAsync: {ex.Message}");
                throw;
            }
            return result;
        }

        public async Task<List<CrateModel>> GenerateCrateReportDelAsync(CrateModel model)
        {
            List<CrateModel> GetDailyCrates = new List<CrateModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd3 = new SqlCommand("generateCrateReportdel", con))
                    {
                        cmd3.CommandType = CommandType.StoredProcedure;
                        cmd3.Parameters.AddWithValue("@d1", model.CrateDatefrom ?? (object)DBNull.Value);
                        cmd3.Parameters.AddWithValue("@d2", model.CrateDateto ?? (object)DBNull.Value);
                        cmd3.Parameters.AddWithValue("@Partyid", model.Partyid);
                        cmd3.Parameters.AddWithValue("@Groowerid", model.Growerid);
                        cmd3.Parameters.AddWithValue("@challanId", model.ChallanId);
                        cmd3.Parameters.AddWithValue("@cratemark", model.CrissueMark ?? (object)DBNull.Value);
                        cmd3.Parameters.AddWithValue("@vehid", 0);
                        cmd3.Parameters.AddWithValue("@trflag", model.CrissueFlag ?? (object)DBNull.Value);
                        cmd3.Parameters.AddWithValue("@vehname", model.Vehno ?? (object)DBNull.Value);

                        await cmd3.ExecuteNonQueryAsync();
                    }

                    const string query = @"SELECT
                                            ci.cid,
                                            ci.Trno,
                                            ci.Dated,
                                            p.partytypeid  +'-'+ p.partyname AS PartyName,
                                            convert(varchar(max),ps.partyid)  +'-'+ ps.partyname AS GrowerName,
                                            convert(varchar(max),cm.id)  +'-'+ cm.ChallanName as ChallanName,
                                            rtrim(vi.vehno) + space(1)+'('+ rtrim(vi.drivername)+space(1)+')'+space(1)+'('+rtrim(vi.contactno)+')' as vehno,
                                            ci.CrateMark,
                                            ci.qty,
                                            ci.trflag,
                                            ci.Remarks
                                        FROM CrateIssuereports ci
                                        LEFT JOIN party p ON ci.partyid = p.partyid
                                        LEFT JOIN partysub ps ON ci.groowerid = ps.partyid
                                        LEFT JOIN challanmaster cm ON ci.challanid = cm.id         
                                        LEFT JOIN vehinfo vi ON ci.vehid = vi.vid
                                        WHERE ci.flagdeleted=1 order by ci.cid";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                GetDailyCrates.Add(new CrateModel
                                {
                                    CrissueId = rdr["cid"] != DBNull.Value ? Convert.ToInt32(rdr["cid"]) : 0,
                                    GrowerGroupName = rdr["PartyName"].ToString(),
                                    GrowerName = rdr["GrowerName"].ToString(),
                                    ChallanName = rdr["challanname"].ToString(),
                                    Vehno = rdr["vehno"].ToString(),
                                    CrissueMark = rdr["CrateMark"].ToString(),
                                    CrissueQty = rdr["qty"] != DBNull.Value ? Convert.ToDecimal(rdr["qty"]) : 0,
                                    CrateIssueDated = rdr["Dated"] != DBNull.Value ? Convert.ToDateTime(rdr["Dated"]) : DateTime.MinValue,
                                    CrissueFlag = rdr["trflag"].ToString(),
                                    CrissueRemarks = rdr["Remarks"].ToString(),
                                    CrateCrn = rdr["Trno"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GenerateCrateReportDelAsync: {ex.Message}");
                throw;
            }
            return GetDailyCrates;
        }

        public async Task<List<CrateModel>> GenerateRawCrateReportAsync(CrateModel model, int unit)
        {
            List<CrateModel> GetDailyPreinward = new List<CrateModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd3 = new SqlCommand("generateRawCrateReport", con))
                    {
                        cmd3.CommandType = CommandType.StoredProcedure;
                        cmd3.Parameters.AddWithValue("@d1", model.OrderDatefrom ?? (object)DBNull.Value);
                        cmd3.Parameters.AddWithValue("@d2", model.OrderDateto ?? (object)DBNull.Value);
                        cmd3.Parameters.AddWithValue("@Partyid", model.Partyid);
                        cmd3.Parameters.AddWithValue("@Groowerid", model.Growerid);
                        cmd3.Parameters.AddWithValue("@OrderBy", model.OrderBy ?? (object)DBNull.Value);
                        cmd3.Parameters.AddWithValue("@Status", model.DemandStatus ?? (object)DBNull.Value);
                        cmd3.Parameters.AddWithValue("@Demandirn", model.DemandIrn ?? (object)DBNull.Value);
                        cmd3.Parameters.AddWithValue("@unit", unit);

                        await cmd3.ExecuteNonQueryAsync();
                    }

                    const string query = @"SELECT
                                            ci.OutId,
                                            ci.demandirn,ci.orderDate,ci.DeliveryDate,p.partytypeid  +'-'+ p.partyname AS PartyName,
                                            CONVERT(VARCHAR(MAX), ps.partyid) + '-' + ps.partyname as GrowerName,
                                            sum(ci.OrderQty) as Qty,
                                            ci.OrderByName,
                                            ci.otype ,
                                            ci.DemandStatus 
                                        FROM TransferOutTransreport ci
                                        LEFT JOIN party p ON ci.partyid = p.partyid
                                        LEFT JOIN partysub ps ON ci.ToGrowerId = ps.partyid
                                        group by ci.OutId,ci.demandirn,ci.orderDate,ci.DeliveryDate,p.partytypeid  +'-'+ p.partyname,ci.OrderByName,ps.partyid , ps.partyname,
                                            ci.otype ,
                                            ci.DemandStatus order by ci.OutId desc";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                GetDailyPreinward.Add(new CrateModel
                                {
                                    DemandNo = rdr["OutId"] != DBNull.Value ? Convert.ToInt32(rdr["OutId"]) : 0,
                                    DemandIrn = rdr["demandirn"].ToString(),
                                    OrderDate = rdr["orderDate"] != DBNull.Value ? Convert.ToDateTime(rdr["orderDate"]) : DateTime.MinValue,
                                    DeliveryDate = rdr["DeliveryDate"] != DBNull.Value ? Convert.ToDateTime(rdr["DeliveryDate"]) : DateTime.MinValue,
                                    GrowerGroupName = rdr["PartyName"].ToString(),
                                    OrderQty = rdr["Qty"] != DBNull.Value ? Convert.ToInt32(rdr["Qty"]) : 0,
                                    OrderBy = rdr["OrderByName"].ToString(),
                                    TransferType = rdr["otype"].ToString(),
                                    DemandStatus = rdr["DemandStatus"].ToString(),
                                    TransferTo = rdr["GrowerName"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GenerateRawCrateReportAsync: {ex.Message}");
                throw;
            }
            return GetDailyPreinward;
        }

        public async Task<List<CrateModel>> GetCrateOrderNoAsync()
        {
            List<CrateModel> result = new List<CrateModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
                {
                    await con.OpenAsync();
                    var sql = "select max(cid) as cid from crateissue";
                    using (var cmd = new SqlCommand(sql, con))
                    {
                        using (var rdr = await cmd.ExecuteReaderAsync())
                        {
                            if (await rdr.ReadAsync())
                            {
                                result.Add(new CrateModel
                                {
                                    CrissueId = rdr["cid"] != DBNull.Value ? Convert.ToInt32(rdr["cid"]) : 0
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCrateOrderNoAsync: {ex.Message}");
                throw;
            }
            return result;
        }

        public async Task<List<CrateModel>> GetallCrateMarksAsync()
        {
            List<CrateModel> getCrateMark = new List<CrateModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
                {
                    const string query = "select distinct CrateMark from Crateissue";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        await con.OpenAsync();
                        using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                getCrateMark.Add(new CrateModel
                                {
                                    CrissueMark = rdr["CrateMark"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetallCrateMarksAsync: {ex.Message}");
                throw;
            }
            return getCrateMark;
        }



    }
}
