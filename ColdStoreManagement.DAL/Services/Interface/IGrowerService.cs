using System.Collections.Generic;
using System.Threading.Tasks;
using ColdStoreManagement.BLL.Models.Grower;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface IGrowerService
    {
        Task<List<GrowerModel>> GetallGrowers();
        Task<GrowerModel?> AddGrowerGroup(GrowerModel growerModel);
        Task<bool> UpdateGrowerStatus(int id, GrowerModel growerModel);
        Task<GrowerModel?> DeleteGrowerGroup(int id, GrowerModel growerModel);
        Task<GrowerModel?> GrowerPriv(string Ugroup);
    }
}
