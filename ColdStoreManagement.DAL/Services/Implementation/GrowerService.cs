using ColdStoreManagement.BLL.Models.Grower;
using ColdStoreManagement.DAL.Helper;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public class GrowerService(SQLHelperCore sql) : BaseService(sql), IGrowerService
    {
        public async Task<List<GrowerModel>> GetallGrowers()
        {
            // Mapping columns to GrowerModel properties
            const string query = @"
                SELECT 
                    Partycode AS Growerid, 
                    address AS GrowerAddress, 
                    partytypeid + '-' + partyname AS GrowerGroupName, 
                    createdon AS Cdated, 
                    status AS Grstatus 
                FROM party 
                WHERE type='C' AND flagdeleted=0 
                ORDER BY partycode DESC";

            return await _sql.ExecuteReaderAsync<GrowerModel>(query, CommandType.Text);
        }

        public async Task<GrowerModel?> AddGrowerGroup(GrowerModel growerModel)
        {
            if (growerModel == null) return null;

            try
            {
                await _sql.ExecuteNonQueryAsync(
                    CommandType.StoredProcedure,
                    "AddGrowerGroup",
                    new SqlParameter("@GrowerGrp", growerModel.GrowerGroupName),
                    new SqlParameter("@GrowerName", growerModel.GrowerName),
                    new SqlParameter("@GrowerAddress", growerModel.GrowerAddress),
                    new SqlParameter("@GrowerCity", growerModel.GrowerCity),
                    new SqlParameter("@GrowerPincode", growerModel.GrowerPincode),
                    new SqlParameter("@GrowerState", growerModel.GrowerState),
                    new SqlParameter("@GrowerStatecode", growerModel.GrowerStatecode),
                    new SqlParameter("@GrowerEmail", growerModel.GrowerEmail),
                    new SqlParameter("@GrowerPan", growerModel.GrowerPan),
                    new SqlParameter("@GrowerGst", growerModel.GrowerGst),
                    new SqlParameter("@GrowerContact", growerModel.GrowerContact),
                    new SqlParameter("@GrowerStatus", growerModel.GrowerActive),
                    new SqlParameter("@Createdby", 1), // Hardcoded as in original
                    new SqlParameter("@Updatedby", 1), // Hardcoded as in original
                    new SqlParameter("@GrowerVillage", growerModel.GrowerVillage),
                    new SqlParameter("@graceperiod", growerModel.GrowerGrace),
                    new SqlParameter("@country", growerModel.GrowerCountry),
                    new SqlParameter("@crateQtyLimit", growerModel.GrowerCrateissue),
                    new SqlParameter("@crateQtypercent", growerModel.GrowerCratelimit),
                    new SqlParameter("@GrowerisLock", growerModel.GrowerLock),
                    new SqlParameter("@Growerisflexi", growerModel.GrowerFlexi),
                    new SqlParameter("@growerRemarks", growerModel.GrowerRemarks)
                );

                await FillValidationAsync(growerModel);
                return growerModel;
            }
            catch (Exception ex)
            {
                // Log exception here if logger was available
                growerModel.RetFlag = "FALSE";
                growerModel.RetMessage = ex.Message;
                return growerModel;
            }
        }

        public async Task<bool> UpdateGrowerStatus(int id, GrowerModel growerModel)
        {
            try
            {
                // Original code used "UpdateGrowerStatus" as query but the user mentioned it's a Stored Procedure
                // Line 3058 in EmployeeAdoNetService: const string query = "UpdateGrowerStatus";
                await _sql.ExecuteNonQueryAsync(
                    CommandType.StoredProcedure,
                    "UpdateGrowerStatus",
                    new SqlParameter("@Growerid", growerModel.Growerid)
                );
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<GrowerModel?> DeleteGrowerGroup(int id, GrowerModel growerModel)
        {
             if (growerModel == null) return null;

            try
            {
                await _sql.ExecuteNonQueryAsync(
                    CommandType.StoredProcedure,
                    "DeleteGrowerGroup",
                    new SqlParameter("@Growerid", growerModel.Growerid)
                );

                await FillValidationAsync(growerModel);
                return growerModel;
            }
            catch (Exception ex)
            {
                growerModel.RetFlag = "FALSE";
                growerModel.RetMessage = ex.Message;
                return growerModel;
            }
        }

        private async Task FillValidationAsync(GrowerModel model)
        {
            if (model == null) return;

             var result = await _sql.ExecuteSingleAsync<GrowerModel>(
                @"SELECT TOP 1 
                    flag        AS RetFlag,
                    remarks     AS RetMessage
                FROM dbo.svalidate",
                CommandType.Text);

            if (result == null) return;

            model.RetFlag = result.RetFlag;
            model.RetMessage = result.RetMessage;
        }

        public async Task<GrowerModel?> GrowerPriv(string Ugroup)
        {
            const string mappedQuery = @"
                select 
                    Addval as GroAdd,
                    Editval as GroEdit,
                    ViewVal as GroView,
                    DelVal as GroDel 
                from userpriv 
                where Groupid in (select usergroupid from usergroup where name=@Ugroup) 
                and pname=@pname";
            
            return await _sql.ExecuteSingleAsync<GrowerModel>(mappedQuery, CommandType.Text,
                new SqlParameter("@Ugroup", Ugroup),
                new SqlParameter("@pname", "Grower Group"));
        }
    }
}
