using ColdStoreManagement.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface IChamberService
    {
        Task<CompanyModel?> AddNewChamber(CompanyModel model);
        Task<CompanyModel?> AddSlot(CompanyModel model);
        Task<List<CompanyModel>> GetallChambers();

    }
}
