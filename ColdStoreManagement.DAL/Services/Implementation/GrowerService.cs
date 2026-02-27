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

        public async Task<GrowerDeleteModel?> DeleteGrowerGroup(int id, GrowerDeleteModel deleteModel)
        {
             if (deleteModel == null) return null;
 
            try
            {
                await _sql.ExecuteNonQueryAsync(
                    CommandType.StoredProcedure,
                    "DeleteGrowerGroup",
                    new SqlParameter("@Growerid", deleteModel.Growerid)
                );
 
                await FillValidationAsync(deleteModel);
                return deleteModel;
            }
            catch (Exception ex)
            {
                deleteModel.RetFlag = "FALSE";
                deleteModel.RetMessage = ex.Message;
                return deleteModel;
            }
        }

        private async Task FillValidationAsync<T>(T model) where T : class
        {
            if (model == null) return;

             var result = await _sql.ExecuteSingleAsync<dynamic>(
                @"SELECT TOP 1 
                    flag        AS RetFlag,
                    remarks     AS RetMessage
                FROM dbo.svalidate",
                CommandType.Text);

            if (result == null) return;

            var retFlagProp = typeof(T).GetProperty("RetFlag");
            var retMessageProp = typeof(T).GetProperty("RetMessage");

            retFlagProp?.SetValue(model, (string)result.RetFlag);
            retMessageProp?.SetValue(model, (string)result.RetMessage);
        }

        public async Task<List<SubGrowerModel>> GetallSubGrowers(int id)
        {
            const string query = @"
                SELECT 
                    b.partytypeid + '-' + rtrim(b.partyname) AS GrowerGroupName,
                    a.partycode AS SubGrowerId,
                    a.GrowerName,
                    a.address AS GrowerAddress,
                    CAST(a.status AS BIT) AS GrowerRetAcitve,
                    a.Crateqtylimit AS GrowerCrateissue,
                    a.crateissuelimitper AS GrowerCratelimit
                FROM partysub a
                JOIN party b ON a.type = b.type AND a.mainid = b.partycode
                WHERE a.type = 'C' 
                  AND a.flagdeleted = 0 
                  AND a.Mainid = @GrowerId";

            return await _sql.ExecuteReaderAsync<SubGrowerModel>(query, CommandType.Text,
                new SqlParameter("@GrowerId", id));
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

        public async Task<SubGrowerModel?> AddGrowerSub(SubGrowerModel subGrowerModel)
        {
            if (subGrowerModel == null) return null;

            try
            {
                await _sql.ExecuteNonQueryAsync(
                    CommandType.StoredProcedure,
                    "AddGrowerSub",
                    new SqlParameter("@GrowerGrp", subGrowerModel.GrowerGroupSelectName),
                    new SqlParameter("@GrowerName", subGrowerModel.GrowerName),
                    new SqlParameter("@GrowerAddress", subGrowerModel.GrowerAddress),
                    new SqlParameter("@GrowerCity", subGrowerModel.GrowerCity),
                    new SqlParameter("@GrowerPincode", subGrowerModel.GrowerPincode),
                    new SqlParameter("@GrowerState", subGrowerModel.GrowerState),
                    new SqlParameter("@GrowerStatecode", subGrowerModel.GrowerStatecode),
                    new SqlParameter("@GrowerEmail", subGrowerModel.GrowerEmail),
                    new SqlParameter("@GrowerPan", subGrowerModel.GrowerPan),
                    new SqlParameter("@GrowerGst", subGrowerModel.GrowerGst),
                    new SqlParameter("@GrowerContact", subGrowerModel.GrowerContact),
                    new SqlParameter("@GrowerStatus", subGrowerModel.GrowerActive),
                    new SqlParameter("@Createdby", 1),
                    new SqlParameter("@Updatedby", 1),
                    new SqlParameter("@GrowerVillage", subGrowerModel.GrowerVillage),
                    new SqlParameter("@graceperiod", subGrowerModel.GrowerGrace),
                    new SqlParameter("@country", subGrowerModel.GrowerCountry),
                    new SqlParameter("@crateQtyLimit", subGrowerModel.GrowerCrateissue),
                    new SqlParameter("@crateQtypercent", subGrowerModel.GrowerCratelimit),
                    new SqlParameter("@GrowerisLock", subGrowerModel.GrowerLock),
                    new SqlParameter("@Growerisflexi", subGrowerModel.GrowerFlexi),
                    new SqlParameter("@growerRemarks", subGrowerModel.GrowerRemarks)
                );

                await FillValidationAsync(subGrowerModel);
                return subGrowerModel;
            }
            catch (Exception ex)
            {
                subGrowerModel.RetFlag = "FALSE";
                subGrowerModel.RetMessage = ex.Message;
                return subGrowerModel;
            }
        }

        public async Task<GrowerDeleteModel?> DeleteSubGrowerGroup(int id, GrowerDeleteModel deleteModel)
        {
            if (deleteModel == null) return null;

            try
            {
                await _sql.ExecuteNonQueryAsync(
                    CommandType.StoredProcedure,
                    "DeleteSubGrowerGroup",
                    new SqlParameter("@Growerid", deleteModel.Growerid)
                );

                await FillValidationAsync(deleteModel);
                return deleteModel;
            }
            catch (Exception ex)
            {
                deleteModel.RetFlag = "FALSE";
                deleteModel.RetMessage = ex.Message;
                return deleteModel;
            }
        }

        public async Task<ChallanModel?> AddChallanGrower(ChallanModel challanModel)
        {
            if (challanModel == null) return null;

            try
            {
                await _sql.ExecuteNonQueryAsync(
                    CommandType.StoredProcedure,
                    "AddChallanGroup",
                    new SqlParameter("@GrowerId", challanModel.SubGrowerId),
                    new SqlParameter("@GrowerName", challanModel.GrowerName),
                    new SqlParameter("@Chname", challanModel.ChallanName),
                    new SqlParameter("@Chaddress", challanModel.ChallanAddress),
                    new SqlParameter("@Chvillage", challanModel.ChallanVillage),
                    new SqlParameter("@Chcity", challanModel.ChallanCity),
                    new SqlParameter("@Chphone1", challanModel.ChallanPhone1),
                    new SqlParameter("@Chphone2", challanModel.ChallanPhone2),
                    new SqlParameter("@Chsms1", challanModel.ChallanSMS1),
                    new SqlParameter("@Chsms2", challanModel.ChallanSMS2)
                );

                await FillValidationAsync(challanModel);
                return challanModel;
            }
            catch (Exception ex)
            {
                challanModel.RetFlag = "FALSE";
                challanModel.RetMessage = ex.Message;
                return challanModel;
            }
        }

        public async Task<List<ChallanModel>> GetallChallanlist(string growerGroup)
        {
            const string query = @"
                SELECT 
                    id          AS ChallanId, 
                    ChallanName, 
                    Address     AS ChallanAddress, 
                    Phone1      AS ChallanPhone1, 
                    Phone2      AS ChallanPhone2 
                FROM challanmaster 
                WHERE flagdeleted = 0 
                AND mainid IN (SELECT partycode FROM party WHERE partytypeid + '-' + RTRIM(partyname) = @GrowerGroup)";

            return await _sql.ExecuteReaderAsync<ChallanModel>(query, CommandType.Text,
                new SqlParameter("@GrowerGroup", growerGroup));
        }

        public async Task<GrowerDeleteModel?> DeleteChallanGroup(int id, GrowerDeleteModel deleteModel)
        {
            if (deleteModel == null) return null;

            try
            {
                await _sql.ExecuteNonQueryAsync(
                    CommandType.StoredProcedure,
                    "DeleteChallanGroup",
                    new SqlParameter("@ChallanId", deleteModel.Growerid)
                );

                await FillValidationAsync(deleteModel);
                return deleteModel;
            }
            catch (Exception ex)
            {
                deleteModel.RetFlag = "FALSE";
                deleteModel.RetMessage = ex.Message;
                return deleteModel;
            }
        }

        public async Task<bool> GenchamberAgg(int growerId)
        {
            try
            {
                await _sql.ExecuteNonQueryAsync(
                    CommandType.StoredProcedure,
                    "GenchamberAgg",
                    new SqlParameter("@Growerid", growerId)
                );
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> GenGrowerAgg(int growerId)
        {
            try
            {
                await _sql.ExecuteNonQueryAsync(
                    CommandType.StoredProcedure,
                    "GenGrowerAgg",
                    new SqlParameter("@Growerid", growerId)
                );
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<GrowerModel?> UpdateGrowerGroup(GrowerModel growerModel)
        {
            if (growerModel == null) return null;

            try
            {
                await _sql.ExecuteNonQueryAsync(
                    CommandType.StoredProcedure,
                    "UpdateGrowerGroup",
                    new SqlParameter("@Growerid", growerModel.Growerid),
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
                    new SqlParameter("@Createdby", 1),
                    new SqlParameter("@Updatedby", 1),
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
                growerModel.RetFlag = "FALSE";
                growerModel.RetMessage = ex.Message;
                return growerModel;
            }
        }

        public async Task<GrowerModel?> GetGrowerIdAsync(int id)
        {
            const string query = @"
                SELECT partytypeid + '-' + partyname as GrowerGroupName, 
                       rtrim(partyname) as GrowerName, 
                       status as GrowerActive, 
                       islock as GrowerLock, 
                       IsFlexibleChamber as GrowerFlexi,
                       partycode as Growerid,
                       pincode as GrowerPincode,
                       statecode as GrowerStatecode,
                       rtrim(state) as GrowerState,
                       rtrim(city) as GrowerCity,
                       email as GrowerEmail,
                       rtrim(village) as GrowerVillage,
                       rtrim(remarks) as GrowerRemarks,
                       rtrim(phone) as GrowerContact,
                       rtrim(address) as GrowerAddress,
                       convert(varchar, createdon, 103) as Cdated,
                       status as Grstatus,
                       rtrim(GSTNo) as GrowerGst,
                       rtrim(panno) as GrowerPan,
                       CrateQtyLimit as GrowerCrateissue,
                       CrateIssueLimitPer as GrowerCratelimit
                FROM party 
                WHERE type = 'C' AND partycode = @GrowerId";

            return await _sql.ExecuteSingleAsync<GrowerModel>(query, CommandType.Text,
                new SqlParameter("@GrowerId", id));
        }

        public async Task<GrowerAgreementModel?> GetAgreementGroupName(int id)
        {
            const string query = "select partytypeid + '-' + partyname as GrowerGroupName, rtrim(partyname) as GrowerGroupSelectName, status as GrowerRetAcitve, islock as GrowerRetLock, IsFlexibleChamber as GrowerRetFlexi from Party where TYPE='C' AND partycode=@GrowerId";
            return await _sql.ExecuteSingleAsync<GrowerAgreementModel>(query, CommandType.Text, new SqlParameter("@GrowerId", id));
        }

        public async Task<List<GrowerAgreementModel>> GetAgreementByGrowerId(int id)
        {
            const string query = "select rtrim(a.partyname) as GrowerGroupSelectName, a.MainId as Growerid, AgreementiD as AgreementId, FyeariD as Fyear, Dated as Adated, RateType as RentType, StoreRent, BinRate as BinRent, Craterental as CrateRate, LabourRate, GradingRate as GradingRent, PackingRate as PackingRent, Qty as Aqty, b.sname as Atype, c.name as Itemname, a.Investment as Investmentflag from GrowerAgreement a, servicetypes b, prodtype c where a.service=b.id and a.itemid=c.id and a.MainId =@GrowerId";
            return await _sql.ExecuteReaderAsync<GrowerAgreementModel>(query, CommandType.Text, new SqlParameter("@GrowerId", id));
        }

        public async Task<GrowerAgreementModel?> GetAgreementId(int id)
        {
            const string query = "select a.AgreementId, a.MainId as Growerid, a.qty as Aqty, fyearid as Fyear, RateType as RentType, b.name as Itemname, rtrim(c.sname) as Atype, Dated as Adated, inwarddate as Idated, Investment as Investmentflag, invqty as InvestQty, InvRate as InvestRate, Commpercent as Invpercent, StoreRent, BinRate as BinRent, Craterental as CrateRate, LabourRate, GradingRate as GradingRent, PackingRate as PackingRent from GrowerAgreement a, prodtype b, servicetypes c where a.itemID=b.id and a.service=c.id and a.flagdeleted=0 AND a.AgreementId = @AgreementId";
            return await _sql.ExecuteSingleAsync<GrowerAgreementModel>(query, CommandType.Text, new SqlParameter("@AgreementId", id));
        }

        public async Task<List<InstallmentModel>> GetInstallmentsByGrowerAsync(int id)
        {
            const string query = "SELECT Id, DueDate, Amount FROM installmentnew WHERE Agreementid = @Agreementid ORDER BY DueDate";
            return await _sql.ExecuteReaderAsync<InstallmentModel>(query, CommandType.Text, new SqlParameter("@Agreementid", id));
        }

        public async Task<List<LookUpModel>> GetFinyear()
        {
            const string query = "select rtrim(fname) as Name from Finyear";
            return await _sql.ExecuteReaderAsync<LookUpModel>(query, CommandType.Text);
        }

        public async Task<List<LookUpModel>> GetServices()
        {
            const string query = "select rtrim(sname) as Name from servicetypes";
            return await _sql.ExecuteReaderAsync<LookUpModel>(query, CommandType.Text);
        }

        public async Task<List<LookUpModel>> GetItemname()
        {
            const string query = "select rtrim(name) as Name from prodtype";
            return await _sql.ExecuteReaderAsync<LookUpModel>(query, CommandType.Text);
        }

        public async Task<GrowerAgreementModel?> AddAgreement(GrowerAgreementModel model)
        {
            if (model == null) return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "AddGrowerAgreement",
                new SqlParameter("@GrowerGrp", model.GrowerGroupSelectName),
                new SqlParameter("@Fyear", model.Fyear),
                new SqlParameter("@Adate", model.Adated),
                new SqlParameter("@InwardDate", model.Idated),
                new SqlParameter("@aqty", model.Aqty),
                new SqlParameter("@createdby", "1"),
                new SqlParameter("@item", model.Itemname),
                new SqlParameter("@bins", "1"),
                new SqlParameter("@StoreRent", SqlDbType.Decimal) { Precision = 10, Scale = 2, Value = model.StoreRent ?? (object)DBNull.Value },
                new SqlParameter("@BinRate ", model.BinRent ?? (object)DBNull.Value),
                new SqlParameter("@CrateRental", model.CrateRate ?? (object)DBNull.Value),
                new SqlParameter("@LabourRate", model.LabourRate ?? (object)DBNull.Value),
                new SqlParameter("@GradingRate ", model.GradingRent ?? (object)DBNull.Value),
                new SqlParameter("@PackingRate", model.PackingRent ?? (object)DBNull.Value),
                new SqlParameter("@Service", model.Atype),
                new SqlParameter("@RateType", model.RentType),
                new SqlParameter("@chamber", model.Achambers ?? (object)DBNull.Value),
                new SqlParameter("@InvFlag", model.InvestFlag),
                new SqlParameter("@saleFlag", model.SalesType),
                new SqlParameter("@invQty", model.InvestQty ?? (object)DBNull.Value),
                new SqlParameter("@invrate", model.InvestRate ?? (object)DBNull.Value),
                new SqlParameter("@Commpercent", model.Invpercent ?? (object)DBNull.Value),
                new SqlParameter("@Growerid", model.Growerid)
            );

            await FillValidationAsync(model);
            return model;
        }

        public async Task<GrowerAgreementModel?> UpdateAgreement(GrowerAgreementModel model, int agreementId)
        {
            if (model == null) return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "UpdateGrowerAgreement",
                new SqlParameter("@AgreementId", agreementId),
                new SqlParameter("@GrowerGrp", model.GrowerGroupSelectName),
                new SqlParameter("@Fyear", model.Fyear),
                new SqlParameter("@Adate", model.Adated),
                new SqlParameter("@InwardDate", model.Idated),
                new SqlParameter("@aqty", model.Aqty),
                new SqlParameter("@updatedby", "1"),
                new SqlParameter("@item", model.Itemname),
                new SqlParameter("@bins", "1"),
                new SqlParameter("@StoreRent", SqlDbType.Decimal) { Precision = 10, Scale = 2, Value = model.StoreRent ?? (object)DBNull.Value },
                new SqlParameter("@BinRate ", model.BinRent ?? (object)DBNull.Value),
                new SqlParameter("@CrateRental", model.CrateRate ?? (object)DBNull.Value),
                new SqlParameter("@LabourRate", model.LabourRate ?? (object)DBNull.Value),
                new SqlParameter("@GradingRate ", model.GradingRent ?? (object)DBNull.Value),
                new SqlParameter("@PackingRate", model.PackingRent ?? (object)DBNull.Value),
                new SqlParameter("@Service", model.Atype),
                new SqlParameter("@RateType", model.RentType),
                new SqlParameter("@chamber", model.Achambers ?? (object)DBNull.Value),
                new SqlParameter("@InvFlag", model.InvestFlag),
                new SqlParameter("@saleFlag", model.SalesType),
                new SqlParameter("@invQty", model.InvestQty ?? (object)DBNull.Value),
                new SqlParameter("@invrate", model.InvestRate ?? (object)DBNull.Value),
                new SqlParameter("@Commpercent", model.Invpercent ?? (object)DBNull.Value),
                new SqlParameter("@Growerid", model.Growerid)
            );

            await FillValidationAsync(model);
            return model;
        }

        public async Task BulkInsertInstallmentsAsync(List<InstallmentModel> installments, GrowerAgreementModel agreement)
        {
            DataTable installmentTable = new();
            installmentTable.Columns.Add("Id", typeof(int));
            installmentTable.Columns.Add("DueDate", typeof(DateTime));
            installmentTable.Columns.Add("Amount", typeof(decimal));
            installmentTable.Columns.Add("Atype", typeof(string));
            installmentTable.Columns.Add("GrowerGrp", typeof(string));
            installmentTable.Columns.Add("GrowerId", typeof(int));

            foreach (var installment in installments)
            {
                installmentTable.Rows.Add(
                    installment.Id,
                    installment.DueDate,
                    installment.Amount,
                    agreement.Atype,
                    agreement.GrowerGroupSelectName,
                    agreement.Growerid
                );
            }

            await _sql.ExecuteNonQueryAsync("sp_InsertInstallments", "@Installments", installmentTable, "dbo.InstallmentType");
        }
    }
}
