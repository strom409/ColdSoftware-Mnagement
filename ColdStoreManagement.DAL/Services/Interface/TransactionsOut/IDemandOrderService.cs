using System.Collections.Generic;
using System.Threading.Tasks;
using ColdStoreManagement.BLL.Models.DTOs;

namespace ColdStoreManagement.DAL.Services.Interface.TransactionsOut
{
    public interface IDemandOrderService
    {
        Task<bool> UpdateLotOrderQuantity(DemandOrderDto EditModel);
        Task<List<DemandOrderDto>> GenerateTempLotReport(DemandOrderDto EditModel, int unit, int Tempid);
        Task<List<DemandOrderDto>> GenerateTempLotRawReportEdit(DemandOrderDto EditModel, int unit, int Tempid);
        Task<List<DemandOrderDto>> generateDemandReport(DemandOrderDto EditModel, int unit);
        Task<List<DemandOrderDto>> GetAllDemands(int UnitId);
        Task<List<DemandOrderDto>> GetAllOrderby(int UnitId);
        Task<DemandOrderDto?> GetDemandGrower(string demandirn);
        Task<DemandOrderDto?> GetDemandbyAsync(int outid);
        Task<DemandOrderDto?> GetRawDemandbyAsync(int outid);
        Task<List<DemandOrderDto>> GetDemand(int outid);
        Task<List<DemandOrderDto>> GetDemandwithstore(int outid);
        Task<List<DemandOrderDto>> GetDraft(int draftid);
        Task<bool> SaveDemandsToDatabase(List<string> demandIRNs, DemandOrderDto EditModel, int Unit);
        Task<DemandOrderDto?> GetDemandPriv(string Ugroup);
        Task<DemandOrderDto?> ValidatedemandStatus(DemandOrderDto companyModel, int outid);
        Task<DemandOrderDto?> GenerateDemandPreview(int selectedGrowerId, DemandOrderDto companyModel);
        Task<bool> UpdateOrderStatus(int id, DemandOrderDto companyModel);
        Task<bool> UpdateOrderStatusraw(int id, DemandOrderDto companyModel);
        Task<DemandOrderDto?> DeleteDemandOrder(int selectedGrowerId, DemandOrderDto companyModel);
        Task<DemandOrderDto?> DeleteDemandOrderRaw(int selectedGrowerId, DemandOrderDto companyModel);
        Task<List<DemandOrderDto>> GetAllunitDemands();
        Task<List<DemandOrderDto>> GetAllDemandsapprove(int UnitId);
        Task<List<DemandOrderDto>> GetAllGrowerDemands(int unitid, string GrowerName);
        Task<DemandOrderDto?> AddFinalDemand(DemandOrderDto EditModel);
        Task<DemandOrderDto?> AddFinalRaw(DemandOrderDto EditModel);
    }
}
