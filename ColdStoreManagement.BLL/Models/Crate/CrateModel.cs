using System;
using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.BLL.Models.Crate
{
    public class CrateModel
    {
        public int CrissueId { get; set; }
        public string? CrateCrn { get; set; }
        public DateTime? CrateIssueDated { get; set; } = DateTime.Today;

        public string? GrowerGroupName { get; set; }
        public string? GrowerName { get; set; }
        public string? ChallanName { get; set; }
        public string? Vehno { get; set; }

        public string? CrissueMark { get; set; }
        public decimal? CrissueQty { get; set; }
        public string? CrissueFlag { get; set; }
        public string? CrissueRemarks { get; set; }

        public int Partyid { get; set; }
        public int Growerid { get; set; }
        public int ChallanId { get; set; }

        // Summary / Analysis Properties
        public decimal? CrateAgreement { get; set; }
        public decimal? CrateIssue { get; set; }
        public decimal? CrateReceive { get; set; }
        public decimal? EmptyReceive { get; set; }
        public decimal? CrateAvailable { get; set; }

        public decimal? SelfCrateReceive { get; set; }
        public decimal? EmptyReturn { get; set; }
        public decimal? SelfCrateBalance { get; set; }

        public decimal? PettyReceive { get; set; }
        public decimal? PettyReturn { get; set; }
        public decimal? PettyBalance { get; set; }

        public decimal? CrateAdjustmentTaken { get; set; }
        public decimal? CrateAdjustmentGiven { get; set; }
        public decimal? TotalInwardQty { get; set; }
        public decimal? CrateBalance { get; set; }


        // Filtering
        public DateTime? CrateDatefrom { get; set; } = new DateTime(2025, 09, 1);
        public DateTime? CrateDateto { get; set; } = DateTime.Today;

        public DateTime? OrderDatefrom { get; set; } = DateTime.Today;
        public DateTime? OrderDateto { get; set; } = DateTime.Today;

        // Raw Crate Report Properties
        public int DemandNo { get; set; }
        public string? DemandIrn { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int? OrderQty { get; set; }
        public string? OrderBy { get; set; }
        public string? TransferType { get; set; }
        public string? DemandStatus { get; set; }
        public string? TransferTo { get; set; }
        
        // Validation / Return Status
        public string? RetFlag { get; set; }
        public string? RetMessage { get; set; }
        
        // User Privileges
        public bool CrateAdd { get; set; }
        public bool CrateEdit { get; set; }
        public bool CrateView { get; set; }
        public bool CrateDel { get; set; }

        // Additional properties that might be needed based on usage
        public int? UserId { get; set; } = 1;
    }
}
