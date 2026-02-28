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
        Task<GrowerDeleteModel?> DeleteGrowerGroup(int id, GrowerDeleteModel deleteModel);
        Task<GrowerModel?> GrowerPriv(string Ugroup);
        Task<List<SubGrowerModel>> GetallSubGrowers(int id);

        // New Missing Endpoints
        Task<SubGrowerModel?> AddGrowerSub(SubGrowerModel subGrowerModel);
        Task<GrowerDeleteModel?> DeleteSubGrowerGroup(int id, GrowerDeleteModel deleteModel);
        Task<ChallanModel?> AddChallanGrower(ChallanModel challanModel);
        Task<List<ChallanModel>> GetallChallanlist(string growerGroup);
        Task<GrowerDeleteModel?> DeleteChallanGroup(int id, GrowerDeleteModel deleteModel);
        Task<bool> GenchamberAgg(int growerId);
        Task<bool> GenGrowerAgg(int growerId);
        Task<GrowerModel?> UpdateGrowerGroup(GrowerModel growerModel);
        Task<GrowerModel?> GetGrowerIdAsync(int id);

        // Agreement Methods
        Task<List<LookUpModel>> GetFinyear();
        Task<List<LookUpModel>> GetServices();
        Task<List<LookUpModel>> GetItemname();
        Task<GrowerAgreementModel?> GetAgreementGroupName(int id);
        Task<List<GrowerAgreementModel>> GetAgreementByGrowerId(int id);
        Task<GrowerAgreementModel?> GetAgreementId(int id);
        Task<List<InstallmentModel>> GetInstallmentsByGrowerAsync(int id);
        Task<GrowerAgreementModel?> AddAgreement(GrowerAgreementModel model);
        Task<GrowerAgreementModel?> UpdateAgreement(GrowerAgreementModel model, int agreementId);
        Task BulkInsertInstallmentsAsync(List<InstallmentModel> installments, GrowerAgreementModel agreement);
    }
}
