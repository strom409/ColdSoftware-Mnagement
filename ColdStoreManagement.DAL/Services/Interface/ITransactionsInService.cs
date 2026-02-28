using ColdStoreManagement.BLL.Models.Chamber;
using ColdStoreManagement.BLL.Models.TransactionsIn;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface ITransactionsInService
    {
        // Preinward Methods
        Task<TransactionsInModel?> AddPreinwardAsync(TransactionsInModel model);
        Task<List<TransactionsInModel>> GetPreinwardProcBydateAsync(DateTime DateFrom, DateTime Dateto, string Prestat);
        Task<TransactionsInModel?> UpdatePreinwardHeadAsync(TransactionsInModel model);
        Task<TransactionsInModel?> GetPreinwardIdAsync(int id);
        Task<List<TransactionsInModel>> GetPreinwardIdlistAsync(int id);
        Task<TransactionsInModel?> GeneratePreinwardAsync(TransactionsInModel model);
        Task<List<TransactionsInModel>> GeneratePreinwardReportAsync(TransactionsInModel model);

        // Chamber Allocation Methods
        Task<bool> GenchamberAggAsync(int id);
        Task<TransactionsInModel?> CheckChamberAllocationAsync(string selectedPurchase);
        Task<List<TransactionsInModel>> GetallStockChamberAsync(int GrowerId);
        Task<TransactionsInModel?> SaveChamberAllocationAsync(TransactionsInModel model);
        Task<TransactionsInModel?> UpdateChamberAllocationAsync(TransactionsInModel model);
        Task<List<TransactionsInModel>> CheckChamberStatusAsync(string selectedPurchase);
        Task<List<TransactionsInModel>> CheckChamberQtyAsync(string selectedPurchase, int chamberid);
        Task<List<TransactionsInModel>> CheckChamberAsync(int selectedNewchamber);

        // Quality, Dock, Location Methods
        Task<List<TransactionsInModel>> GetPendingQualityAsync(int unitId, string status);
        Task<TransactionsInModel?> GetQcPrivAsync(string userGroup);
        Task<List<QualityModel>> GetallQuality();
        Task<LotDetailModel?> GetLotFullDet(int selectedGrowerId);
        Task<List<RunningChamberModel>> GetRunningChambers(int selectedid);

        Task<List<TransactionsInModel>> GetPendingDockAsync(int unitId, int dockPosting);
        Task<DocPrivModel?> GetDockPrivAsync(string userGroup);

        Task<List<TransactionsInModel>> GetPendingLocationAsync(int unitId, string status);
        Task<TransactionsInModel?> GetLocationPrivAsync(string userGroup);

        Task<bool> UpdateItemStatusAsync(int itemId);
    }
}
