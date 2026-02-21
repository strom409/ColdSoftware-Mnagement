using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.DAL.Helper;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public class CompanyService(SQLHelperCore sql) : BaseService(sql), ICompanyService
    {
        #region ---------Company---------
        public async Task<CompanyModel?> GetCompanyByIdAsync(int companyId = 1)
        {
            return await _sql.ExecuteSingleAsync<CompanyModel>(
                @"SELECT 
                        cid       AS Id,
                        cname     AS Name,
                        gst,
                        regno,
                        pan,
                        website,
                        addr      AS Caddress,
                        city,
                        Contact1  AS Phone,
                        Contact1  AS Mobile,
                        State,
                        Pincode,
                        email
                      FROM company 
                      WHERE cid = @Id",
                CommandType.Text,
                new SqlParameter("@Id", companyId)
            );
        }
        public async Task<bool> EditCompany(int id, CompanyModel companyModel)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                @"UPDATE dbo.company 
                      SET email=@Email,
                          State=@State,
                          City=@City,
                          Contact1=@Phone,
                          contact2=@Mobile,
                          Pincode=@Pincode,
                          Regno=@Regno,
                          website=@Website,
                          Pan=@Pan,
                          cname=@Name,
                          gst=@Gst,
                          addr=@Caddress
                      WHERE cid=@Id",
                new SqlParameter("@Id", id),
                new SqlParameter("@Name", companyModel.Name),
                new SqlParameter("@Gst", companyModel.Gst),
                new SqlParameter("@Regno", companyModel.Regno),
                new SqlParameter("@Pan", companyModel.Pan),
                new SqlParameter("@Website", companyModel.Website),
                new SqlParameter("@Caddress", companyModel.Caddress),
                new SqlParameter("@City", companyModel.City),
                new SqlParameter("@State", companyModel.State),
                new SqlParameter("@Pincode", companyModel.Pincode),
                new SqlParameter("@Email", companyModel.Email),
                new SqlParameter("@Phone", companyModel.Phone),
                new SqlParameter("@Mobile", companyModel.Mobile)
            );

            return true;
        }

        #endregion ---------Company---------

        #region -----------Buiding-----------
        public async Task<List<BuildingModel>> GetAllBuildingsAsync()
        {
            using var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                "SELECT * FROM dbo.building_master"
            );

            var buildings = new List<BuildingModel>();

            if (ds.Tables.Count == 0)
                return buildings;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                buildings.Add(new BuildingModel
                {
                    Id = Convert.ToInt32(row["id"]),
                    Buildname = row["Buname"]?.ToString() ?? string.Empty,
                    Bcode = row["Bcode"]?.ToString() ?? string.Empty,
                    Bstat = row["Bstat"]?.ToString() ?? string.Empty,
                    BuildDetails = row["BuildDetails"]?.ToString()
                });
            }

            return buildings;
        }
        public async Task<BuildingModel?> GetBuildingById(int id)
        {
            var building = await _sql.ExecuteSingleAsync<BuildingModel>(
                @"SELECT id, Buname as Buildname, Bcode, Bstat, BuildDetails
                    FROM dbo.building_master where id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", id)
            );

            return building;
        }
        public async Task<BuildingModel?> GetBuildingByName(string buildingName)
        {
            return await _sql.ExecuteSingleAsync<BuildingModel>(
                @"SELECT id, Buname as Buildname, Bcode, Bstat, BuildDetails 
                    FROM Buildings 
                    WHERE BuildingName = @BuildingName",
                CommandType.Text,
                new SqlParameter("@BuildingName", buildingName)
            );
        }

        public async Task<bool> AddBuildingAsync(BuildingModel model)
        {
            const string query = @"
                INSERT INTO dbo.Building_master (Id, Bcode, Buname, Bstat, BuildDetails)
                VALUES (
                    (SELECT ISNULL(MAX(id) + 1, 1) FROM building_master),
                    @Bucode, @Buildname, @Bustat, @Budetails
                )";

            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                query,
                new SqlParameter("@Bucode", model.Bcode),
                new SqlParameter("@Buildname", model.Buildname),
                new SqlParameter("@Bustat", model.Bstat),
                new SqlParameter("@Budetails", model.BuildDetails)
            );

            return true;
        }
        public async Task<bool> UpdateBuildingAsync(int id, CompanyModel model)
        {
            const string query = @"
                UPDATE dbo.building_master
                SET
                    Bcode = @Bucode,
                    Buname = @Buildname,
                    Bstat = @Bustat,
                    BuildDetails = @Budetails
                WHERE id = @Id";

            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                query,
                new SqlParameter("@Id", id),
                new SqlParameter("@Bucode", model.Bucode),
                new SqlParameter("@Buildname", model.Buildname),
                new SqlParameter("@Bustat", model.Bustat),
                new SqlParameter("@Budetails", model.Budetails)
            );

            return true;
        }
        public async Task<bool> DeleteBuildingAsync(int id)
        {
            const string query = "DELETE FROM dbo.building_master WHERE id = @Id";

            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                query,
                new SqlParameter("@Id", id)
            );

            return true;
        }
        #endregion

        

    }
}
