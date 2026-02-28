using System.Collections.Generic;
using System.Threading.Tasks;
using ColdStoreManagement.BLL.Models.DTOs;

namespace ColdStoreManagement.DAL.Services.Interface.TransactionsOut
{
    public interface IStoreOutService
    {
        Task<List<StoreOutDto>> GetStoreOutStatus(string stat, int UnitId, string demandirn, string avuser);
        Task<bool> UpdateDraftQuantity(StoreOutDto EditModel);
        Task<bool> ForceUpload(int id, string Frems);
        Task<StoreOutDto?> ValidateStoreOutTransQty(StoreOutDto companyModel);
        Task<StoreOutDto?> AddStoreOut(StoreOutDto companyModel);
    }
}
