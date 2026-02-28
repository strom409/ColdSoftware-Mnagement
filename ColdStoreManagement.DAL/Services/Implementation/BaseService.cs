using ColdStoreManagement.BLL.Models.Auth;
using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.BLL.Models.DTOs;
using ColdStoreManagement.DAL.Helper;
using System.Data;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public abstract class BaseService(SQLHelperCore sql)
    {
        protected readonly SQLHelperCore _sql = sql;

        protected async Task FillValidationWithGroupAsync(CompanyModel model)
        {
            if (model == null) return;

            var result = await _sql.ExecuteSingleAsync<CompanyModel>(                
                @"SELECT TOP 1 
                    flag        AS RetFlag, 
                    remarks     AS RetMessage, 
                    unitid      AS GlobalUnitId, 
                    usergroup   AS GlobalUserGroup 
                FROM dbo.svalidate",
                CommandType.Text);
            if (result == null)
                return;

            // copy values back to input model
            model.RetFlag = result.RetFlag;
            model.RetMessage = result.RetMessage;
            model.GlobalUnitId = result.GlobalUnitId;
            model.GlobalUserGroup = result.GlobalUserGroup;
        }

        protected async Task FillValidationAsync(CompanyModel model)
        {
            var validation = await _sql.ExecuteSingleAsync<CompanyModel>(
                @"SELECT TOP 1 
                    flag        AS RetFlag,
                    remarks     AS RetMessage
                FROM dbo.svalidate",
                CommandType.Text);

            if (validation == null) return;

            model.RetFlag = validation.RetFlag;
            model.RetMessage = validation.RetMessage;
        }

        protected async Task FillValidationAsync(ChamberDto model)
        {
            var validation = await _sql.ExecuteSingleAsync<ChamberDto>(
                @"SELECT TOP 1 
                    flag        AS RetFlag,
                    remarks     AS RetMessage
                FROM dbo.svalidate",
                CommandType.Text);

            if (validation == null) return;

            model.RetFlag = validation.RetFlag;
            model.RetMessage = validation.RetMessage;
        }

        protected async Task FillValidationAsync(ChamberUpdateDto model)
        {
            var validation = await _sql.ExecuteSingleAsync<ChamberUpdateDto>(
                @"SELECT TOP 1 
                    flag        AS RetFlag,
                    remarks     AS RetMessage
                FROM dbo.svalidate",
                CommandType.Text);

            if (validation == null) return;

            model.RetFlag = validation.RetFlag;
            model.RetMessage = validation.RetMessage;
        }

        protected async Task FillValidationAsync(VehicleDto model)
        {
            var validation = await _sql.ExecuteSingleAsync<VehicleDto>(
                @"SELECT TOP 1 
                    flag        AS RetFlag,
                    remarks     AS RetMessage
                FROM dbo.svalidate",
                CommandType.Text);

            if (validation == null) return;

            model.RetFlag = validation.RetFlag;
            model.RetMessage = validation.RetMessage;
        }

        protected async Task FillValidationAsync(ItemDto model)
        {
            var validation = await _sql.ExecuteSingleAsync<ItemDto>(
                @"SELECT TOP 1 
                    flag        AS RetFlag,
                    remarks     AS RetMessage
                FROM dbo.svalidate",
                CommandType.Text);

            if (validation == null) return;

            model.RetFlag = validation.RetFlag;
            model.RetMessage = validation.RetMessage;
        }

        protected async Task FillValidationAsync(LoginResultModel model)
        {
            var result = await _sql.ExecuteSingleAsync<LoginResultModel>(
               @"SELECT TOP 1 
                    flag        AS RetFlag,
                    remarks     AS RetMessage,
                    unitid      AS GlobalUnitId,
                    usergroup   AS GlobalUserGroup 
                FROM dbo.svalidate",
               CommandType.Text);

            if (result == null)
                return;

            // copy values back to input model
            model.RetFlag = result.RetFlag;
            model.RetMessage = result.RetMessage;
            // model.GlobalUserId = result.GlobalUserId;
            model.GlobalUnitId = result.GlobalUnitId;
            model.GlobalUserGroup = result.GlobalUserGroup;

        }

        protected async Task FillValidationAsync(StoreOutDto model)
        {
            var validation = await _sql.ExecuteSingleAsync<StoreOutDto>(
                @"SELECT TOP 1 
                    flag        AS RetFlag,
                    remarks     AS RetMessage
                FROM dbo.svalidate",
                CommandType.Text);

            if (validation == null) return;

            model.RetFlag = validation.RetFlag;
            model.RetMessage = validation.RetMessage;
        }

        protected async Task FillValidationAsync(DemandOrderDto model)
        {
            var validation = await _sql.ExecuteSingleAsync<DemandOrderDto>(
                @"SELECT TOP 1 
                    flag        AS RetFlag,
                    remarks     AS RetMessage
                FROM dbo.svalidate",
                CommandType.Text);

            if (validation == null) return;

            model.RetFlag = validation.RetFlag;
            model.RetMessage = validation.RetMessage;
        }

    }
}
