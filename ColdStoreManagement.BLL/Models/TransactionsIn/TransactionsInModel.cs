using System;

namespace ColdStoreManagement.BLL.Models.TransactionsIn
{
    public class TransactionsInModel
    {
        // Common / System
        public string? RetFlag { get; set; }
        public string? RetMessage { get; set; }
        public string? GlobalUserName { get; set; }

        // Preinward (Gate In) Properties
        public int PreInwardId { get; set; }
        public DateTime? PreinwardDate { get; set; } = DateTime.Today;
        public string? GrowerGroupName { get; set; }
        public string? GrowerName { get; set; }
        public string? ChallanName { get; set; }
        public string? ChallanNo { get; set; }
        public string? Itemname { get; set; } // Product Name
        public string? PreInwardKhata { get; set; }
        public string? Vehno { get; set; }
        public string? PreInwardRemarks { get; set; }
        public decimal PreInwardQty { get; set; }
        public string? PreCrateType { get; set; }
        public int ServiceId { get; set; }
        public decimal StoreQty { get; set; } // Company Crates
        public decimal OwnQty { get; set; }   // Own Crates
        public string? PreInwardUom { get; set; }
        public int VarietyId { get; set; }
        public string? VarietyName { get; set; }
        public string? TempGateInIrn { get; set; }
        public int TempGateInId { get; set; }
        public string? PreInIrn { get; set; }
        public string? LotIrn { get; set; }
        public string? PreInwardStatus { get; set; }

        // Chamber Allocation Properties
        public int ChamberId { get; set; }
        public string? ChamberName { get; set; }
        public decimal ChamberAllocation { get; set; } // Qty
        public int AllocationNo { get; set; } // For Updates
        public decimal ChamberAvailQty { get; set; }

        // Additional Stats (GetallStockChamber)
        public decimal TotalPrecompanyQty { get; set; }
        public decimal TotalPreownQty { get; set; }
        public decimal TotalPreQty { get; set; }
        public decimal TotalIncompanyQty { get; set; }
        public decimal TotalInownQty { get; set; }
        public decimal TotalInpettyQty { get; set; }
        public decimal TotalInQty { get; set; }
        
        // Report / View Properties
        public int Partyid { get; set; }
        public int Growerid { get; set; }
        public int ChallanId { get; set; }
        public string? CommonSearch { get; set; }
        public DateTime? PreinwardDateTo { get; set; }
        
        // Permissions (GrowerPriv - reused here if needed or separate)
        public bool GroAdd { get; set; }
        public bool GroEdit { get; set; }
        public bool GroView { get; set; }
        public bool GroDel { get; set; }

        // Unit info
        public string? Unitname { get; set; }
        public int Capacity { get; set; }
        public bool chamberstatus { get; set; }

        // Quality Properties
        public int Lotno { get; set; }
        public bool QcAdd { get; set; }
        public bool QcEdit { get; set; }
        public bool QcView { get; set; }
        public bool QckDel { get; set; }
        
        // Dock Properties
        public int DockPost { get; set; }
        public string? Templocation { get; set; }
        public decimal VerfiedQty { get; set; }
        public decimal VerfiedOwnCrates { get; set; }
        public decimal VerfiedCompanyCrates { get; set; }
        public decimal Verfiedpetties { get; set; }
        public decimal Verfiedbins { get; set; }
        public decimal Verfiedpallets { get; set; }
        public string? Stickerprinted { get; set; }
        public string? CrateMarka { get; set; }
        public bool DockAdd { get; set; }
        public bool DockEdit { get; set; }
        public bool DockView { get; set; }
        public bool DockDel { get; set; }

        // Location Properties
        public string? FloorName { get; set; }
        public string? MatrixName { get; set; }
        public string? RowName { get; set; }
        public string? ColumName { get; set; }
        public decimal CrateNos { get; set; } 
        public bool LocAdd { get; set; }
        public bool LocEdit { get; set; }
        public bool LocView { get; set; }
        public bool LocDel { get; set; }
    }
}
