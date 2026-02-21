using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.DAL.Helper;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public class UserService : BaseService, IUserService
    {
        private readonly IConfiguration _configuration;

        public UserService(SQLHelperCore sql, IConfiguration configuration) : base(sql)
        {
            _configuration = configuration;
        }


        #region ---------- Account ----------

        public async Task<CompanyModel?> AddAccountName(CompanyModel model)
        {
            if (model == null) return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "AddAccountName",
                new SqlParameter("@GrowerGrp", model.AccountGroup),
                new SqlParameter("@GrowerName", model.AccountName),
                new SqlParameter("@GrowerAddress", model.AccountAddress),
                new SqlParameter("@GrowerCity", model.AccountCity),
                new SqlParameter("@GrowerPincode", model.AccountPincode),
                new SqlParameter("@GrowerState", model.AccountState),
                new SqlParameter("@GrowerStatecode", model.AccountStatecode),
                new SqlParameter("@GrowerEmail", model.AccountEmail),
                new SqlParameter("@GrowerPan", model.AccountPan),
                new SqlParameter("@GrowerGst", model.AccountGst),
                new SqlParameter("@GrowerContact", model.AccountContact),
                new SqlParameter("@GrowerStatus", model.AccountActive),
                new SqlParameter("@Createdby", 1),
                new SqlParameter("@Updatedby", 1),
                new SqlParameter("@GrowerVillage", model.AccountVillage),
                new SqlParameter("@graceperiod", "1"),
                new SqlParameter("@country", model.AccountCountry),
                new SqlParameter("@crateQtyLimit", 0),
                new SqlParameter("@crateQtypercent", 0),
                new SqlParameter("@GrowerisLock", model.AccountLock),
                new SqlParameter("@Growerisflexi", "0"),
                new SqlParameter("@growerRemarks", model.AccountRemarks),
                new SqlParameter("@growerapproval", model.AccountApproval)
            );

            await FillValidationAsync(model);
            return model;
        }
        public async Task<CompanyModel?> GetAccountType(int Partyid)
        {
            CompanyModel? companyModel = null;

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();

                var sql = "select type,partyid from party where flagdeleted=0 AND  Idno = @growerid";

                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@growerid", Partyid);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            companyModel = new CompanyModel
                            {
                                AccountType = reader["type"] as string ?? string.Empty,
                                Growerid = Convert.ToInt32(reader["partyid"])
                            };
                        }
                    }
                }
                con.Close();

            }

            return companyModel;

        }


        public async Task<CompanyModel?> GetAccountIdAsync(int growerid)
        {
            CompanyModel? companyModel = null;

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();

                var sql = "SELECT party.*,b.subname FROM party, AccountSubGroups b WHERE party.masterid=b.id and type='V' AND  partycode = @Id";

                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Id", growerid);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            companyModel = new CompanyModel
                            {
                                // companyModel.Growerid = reader.GetInt32(reader.GetOrdinal("id")),




                                Accountid = reader.GetInt32(reader.GetOrdinal("partycode")),

                                AccountName = reader["partyname"] as string,
                                AccountAddress = reader["address"] as string,
                                AccountCity = reader["city"] as string,
                                AccountPincode = reader["pincode"] as string,
                                AccountState = reader["state"] as string,
                                AccountStatecode = reader["statecode"] as string,
                                AccountEmail = reader["email"] as string,
                                AccountContact = reader["phone"] as string,
                                AccountGst = reader["GSTNo"] as string,
                                AccountPan = reader["panno"] as string,
                                AccountRemarks = reader["remarks"] as string,
                                AccountCountry = reader["country"] as string,
                                AccountVillage = reader["village"] as string,
                                AccountRetLock = Convert.ToBoolean(reader["islock"]),
                                AccountGroup = reader["subname"] as string,

                                AccountRetActive = Convert.ToBoolean(reader["Status"])
                            };
                        }
                    }
                }
                con.Close();

            }

            return companyModel;
        }



        public async Task<List<CompanyModel>> GetallAccountlist()
        {
            const string query = "select rtrim(subname) as subname from AccountSubGroups";
            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                query);

            var result = new List<CompanyModel>();

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return result;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                result.Add(new CompanyModel
                {
                    AccountGroup = row["subname"]?.ToString()
                });
            }

            return result;
        }
        public async Task<List<CompanyModel>> GetallStatus()
        {
            List<CompanyModel> Getstatuslist = new List<CompanyModel>();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                const string query = "select rtrim(statname) as status from allstat";
                SqlCommand cmd = new SqlCommand(query, con)
                {
                    CommandType = CommandType.Text
                };

                con.Open();
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    CompanyModel companyModel = new CompanyModel
                    {
                        GrowerActive = rdr["status"].ToString()



                    };
                    Getstatuslist.Add(companyModel);
                }
                con.Close();
                cmd.Dispose();
            }
            return Getstatuslist;
        }


        public async Task<CompanyModel?> Getassignuser(int selectedcrn)
        {
            CompanyModel? companyModel = null;

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();

                var sql = @"SELECT ISNULL((
    SELECT TOP 1 u.username
    FROM packingorderitems poi
    LEFT JOIN users u ON poi.userid = u.userid
    WHERE poi.packingorderid = @selectedcrn
), '.') as username
            ";
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@selectedcrn", selectedcrn);
                    using (var rdr = await cmd.ExecuteReaderAsync())
                    {
                        if (await rdr.ReadAsync())
                        {
                            companyModel = new CompanyModel
                            {

                                UserName = rdr["username"] as string,
                            };
                        }
                    }
                }
                con.Close();
            }

            return companyModel;
        }




        #endregion

        #region ---------- User ----------

        public async Task<List<CompanyModel>> GetUserlist()
        {
            const string query = "select rtrim(username) as username from users where userid in(select distinct userid from outward)";
            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                query);

            var result = new List<CompanyModel>();

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return result;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                result.Add(new CompanyModel
                {
                    UserName = row["username"]?.ToString()
                });
            }

            return result;
        }

        public async Task<CompanyModel?> AdduserName(CompanyModel model)
        {
            if (model == null) return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "AdduserName",
                new SqlParameter("@Username", model.UserName),
                new SqlParameter("@Password", model.UserPassword),
                new SqlParameter("@Useremail", model.Useremail),
                new SqlParameter("@UserGroup", model.GlobalUserGroup),
                new SqlParameter("@Userstatus", model.UserStatus),
                new SqlParameter("@Unit1", model.Unit1),
                new SqlParameter("@Unit2", model.Unit2),
                new SqlParameter("@Unit3", model.Unit3),
                new SqlParameter("@Unit4", model.Unit4),
                new SqlParameter("@Unit5", model.Unit5),
                new SqlParameter("@Globalusername", model.GlobalUserName)
            );

            await FillValidationAsync(model);
            return model;
        }

        public async Task<CompanyModel?> UpdateuserName(CompanyModel model)
        {
            if (model == null) return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "UpdateuserName",
                new SqlParameter("@userid", model.Userid),
                new SqlParameter("@Username", model.UserName),
                new SqlParameter("@Useremail", model.Useremail),
                new SqlParameter("@UserGroup", model.GlobalUserGroup),
                new SqlParameter("@Userstatus", model.UserStatus),
                new SqlParameter("@Unit1", model.Unit1),
                new SqlParameter("@Unit2", model.Unit2),
                new SqlParameter("@Unit3", model.Unit3),
                new SqlParameter("@Unit4", model.Unit4),
                new SqlParameter("@Unit5", model.Unit5),
                new SqlParameter("@Globalusername", model.GlobalUserName)
            );

            await FillValidationAsync(model);
            return model;
        }
       
        public async Task<CompanyModel?> UpdateUserPasswordAsync(
            CompanyModel model,
            string username,
            string oldPassword,
            string newPassword)
        {
            if (model == null) return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "UpdateuserPasswordcheck",
                new SqlParameter("@Globalusername", username),
                new SqlParameter("@Opassword", oldPassword),
                new SqlParameter("@Npassword", newPassword)
            );

            await FillValidationAsync(model);
            return model;
        }

        #endregion

        public async Task<CompanyModel?> AdduserGroupName(CompanyModel EditModel)
        {
            if (EditModel == null)
                return null;

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("AdduserGroupName", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Make sure there are no extra spaces in parameter names!
                    cmd.Parameters.AddWithValue("@Usergroup", EditModel.UserGroupName);
                    cmd.Parameters.AddWithValue("@UsergroupRemarks", EditModel.UserGroupRemarks);


                    cmd.Parameters.AddWithValue("@Globalusername", EditModel.GlobalUserName);



                    await cmd.ExecuteNonQueryAsync();
                }

                // After stored procedure, retrieve message from dbo.svalidate
                using (SqlCommand cmd2 = new SqlCommand("SELECT TOP 1 flag,remarks FROM dbo.svalidate", con))
                {
                    using (SqlDataReader rdr = await cmd2.ExecuteReaderAsync())
                    {
                        if (rdr.Read())
                        {
                            EditModel.RetMessage = rdr["remarks"]?.ToString();
                            EditModel.RetFlag = rdr["flag"]?.ToString();
                        }
                    }
                }

                return EditModel;
            }
        }


        public async Task<CompanyModel?> UpdateuserPassword(CompanyModel companyModel, string apassoword)
        {
            if (companyModel == null)
                return null;

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("UpdateuserPassword", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@userid", companyModel.Userid);
                    cmd.Parameters.AddWithValue("@Password", apassoword);



                    cmd.Parameters.AddWithValue("@Globalusername", companyModel.GlobalUserName);



                    await cmd.ExecuteNonQueryAsync();
                }

                // After stored procedure, retrieve message from dbo.svalidate
                using (SqlCommand cmd2 = new SqlCommand("SELECT TOP 1 flag,remarks FROM dbo.svalidate", con))
                {
                    using (SqlDataReader rdr = await cmd2.ExecuteReaderAsync())
                    {
                        if (rdr.Read())
                        {
                            companyModel.RetMessage = rdr["remarks"]?.ToString();
                            companyModel.RetFlag = rdr["flag"]?.ToString();
                        }
                    }
                }

                return companyModel;
            }
        }
        public async Task<CompanyModel?> CheckUser(CompanyModel loginModel)
        {
            if (loginModel == null)
                return null;

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("checkuser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@uname", loginModel.GlobalUserName);
                    cmd.Parameters.AddWithValue("@pwd", loginModel.UserPassword);
                    cmd.Parameters.AddWithValue("@unit", loginModel.GlobalUnitName);





                    await cmd.ExecuteNonQueryAsync();
                }

                // After stored procedure, retrieve message from dbo.svalidate
                using (SqlCommand cmd2 = new SqlCommand("SELECT TOP 1 flag,remarks,unitid,usergroup FROM dbo.svalidate", con))
                {
                    using (SqlDataReader rdr = await cmd2.ExecuteReaderAsync())
                    {
                        if (rdr.Read())
                        {
                            loginModel.RetMessage = rdr["remarks"]?.ToString();
                            loginModel.RetFlag = rdr["flag"]?.ToString();
                            loginModel.GlobalUnitId = Convert.ToInt32(rdr["unitid"]);
                            loginModel.GlobalUserGroup = rdr["usergroup"]?.ToString();

                        }
                    }
                }

                return loginModel;
            }
        }

        public async Task<bool> DeleteEmployee(string id)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                "delete dbo.Employees where Id=@Id",
                new SqlParameter("@Id", id)
            );
            return true;
        }


        public async Task<CompanyModel?> GetUserIdAsync(int Userid)
        {
            CompanyModel? companyModel = null;

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();

                // First query: user info
                var userInfoSql = @"SELECT ci.userid, ci.username, ci.email, ci.status, 
                                   ug.name AS gname 
                            FROM USERS ci 
                            LEFT JOIN usergroup ug ON ci.usergroupid = ug.usergroupid 
                            WHERE ci.userid = @Id";

                using (var cmd = new SqlCommand(userInfoSql, con))
                {
                    cmd.Parameters.AddWithValue("@Id", Userid);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            companyModel = new CompanyModel
                            {
                                //Userid = reader.GetInt32(reader.GetOrdinal("userid")),
                                UserName = reader["username"] as string,
                                Useremail = reader["email"] as string,
                                UserStatus = reader["status"] as string,
                                GlobalUserGroup = reader["gname"] as string
                            };
                        }
                    }
                }

                if (companyModel != null)
                {
                    // Second query: unit IDs
                    var unitSql = "SELECT UnitID FROM UserUnitMap WHERE UserID = @UserId";

                    using (var unitCmd = new SqlCommand(unitSql, con))
                    {
                        unitCmd.Parameters.AddWithValue("@UserId", Userid);

                        using (var unitReader = await unitCmd.ExecuteReaderAsync())
                        {
                            while (await unitReader.ReadAsync())
                            {
                                int unitId = unitReader.GetInt32(0);
                                switch (unitId)
                                {
                                    case 1: companyModel.Unit1 = true; break;
                                    case 2: companyModel.Unit2 = true; break;
                                    case 3: companyModel.Unit3 = true; break;
                                    case 4: companyModel.Unit4 = true; break;
                                    case 5: companyModel.Unit5 = true; break;
                                }
                            }
                        }
                    }
                }

                con.Close();
            }

            return companyModel;
        }

        public async Task<bool> UpdateuserStatus(int id, CompanyModel companyModel)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))

            {
                const string query = "UpdateuserStatus";
                //const string query = "update dbo.company set Regno=@regno,website=@website,Pan=@pan ,cname = @Name, gst = @Gst, addr = @Caddress where cid=@Id";

                SqlCommand cmd = new SqlCommand(query, con)
                {
                    CommandType = CommandType.StoredProcedure,
                };
                cmd.Parameters.AddWithValue("@Growerid", id);




                con.Open();
                await cmd.ExecuteNonQueryAsync();

                con.Close();
                cmd.Dispose();
            }
            return true;
        }

        public async Task<bool> UpdateuserGroupStatus(int id, CompanyModel companyModel)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))

            {
                const string query = "UpdateuserGroupStatus";
                //const string query = "update dbo.company set Regno=@regno,website=@website,Pan=@pan ,cname = @Name, gst = @Gst, addr = @Caddress where cid=@Id";

                SqlCommand cmd = new SqlCommand(query, con)
                {
                    CommandType = CommandType.StoredProcedure,
                };
                cmd.Parameters.AddWithValue("@Growerid", id);




                con.Open();
                await cmd.ExecuteNonQueryAsync();

                con.Close();
                cmd.Dispose();
            }
            return true;
        }

        public async Task<bool> UpdateGroupStatus(int id, CompanyModel companyModel)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))

            {
                const string query = "UpdateGroupStatus";
                //const string query = "update dbo.company set Regno=@regno,website=@website,Pan=@pan ,cname = @Name, gst = @Gst, addr = @Caddress where cid=@Id";

                SqlCommand cmd = new SqlCommand(query, con)
                {
                    CommandType = CommandType.StoredProcedure,
                };
                cmd.Parameters.AddWithValue("@Mid", companyModel.Mid);




                con.Open();
                await cmd.ExecuteNonQueryAsync();

                con.Close();
                cmd.Dispose();
            }
            return true;
        }
        public async Task<bool> UpdatesubGroupStatus(int id, CompanyModel companyModel)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))

            {
                const string query = "updatesubgroupstatus";
                //const string query = "update dbo.company set Regno=@regno,website=@website,Pan=@pan ,cname = @Name, gst = @Gst, addr = @Caddress where cid=@Id";

                SqlCommand cmd = new SqlCommand(query, con)
                {
                    CommandType = CommandType.StoredProcedure,
                };
                cmd.Parameters.AddWithValue("@Mid", companyModel.SubGroupId);




                con.Open();
                await cmd.ExecuteNonQueryAsync();

                con.Close();
                cmd.Dispose();
            }
            return true;
        }


        public async Task<CompanyModel?> UpdateSubGroup(CompanyModel EditModel)
        {
            if (EditModel == null)
                return null;

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("UpdateSubGroup", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Make sure there are no extra spaces in parameter names!
                    cmd.Parameters.AddWithValue("@MaccGrp", EditModel.SubaccGrp);
                    cmd.Parameters.AddWithValue("@MaccDetails", EditModel.SubGroupDetails);
                    cmd.Parameters.AddWithValue("@UserId", EditModel.UserId);
                    cmd.Parameters.AddWithValue("@Mid", EditModel.SubGroupId);
                    cmd.Parameters.AddWithValue("@SubAcgrp", EditModel.SubgroupName);












                    await cmd.ExecuteNonQueryAsync();
                }

                // After stored procedure, retrieve message from dbo.svalidate
                using (SqlCommand cmd2 = new SqlCommand("SELECT TOP 1 flag,remarks FROM dbo.svalidate", con))
                {
                    using (SqlDataReader rdr = await cmd2.ExecuteReaderAsync())
                    {
                        if (rdr.Read())
                        {
                            EditModel.RetMessage = rdr["remarks"]?.ToString();
                            EditModel.RetFlag = rdr["flag"]?.ToString();
                        }
                    }
                }

                return EditModel;
            }
        }
        public async Task<bool> UpdateSubGrowerStatus(int id, CompanyModel companyModel)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))

            {
                const string query = "UpdateSubGrowerStatus";
                //const string query = "update dbo.company set Regno=@regno,website=@website,Pan=@pan ,cname = @Name, gst = @Gst, addr = @Caddress where cid=@Id";

                SqlCommand cmd = new SqlCommand(query, con)
                {
                    CommandType = CommandType.StoredProcedure,
                };
                cmd.Parameters.AddWithValue("@Growerid", companyModel.SubGrowerId);




                con.Open();
                await cmd.ExecuteNonQueryAsync();

                con.Close();
                cmd.Dispose();
            }
            return true;
        }



    }
}
