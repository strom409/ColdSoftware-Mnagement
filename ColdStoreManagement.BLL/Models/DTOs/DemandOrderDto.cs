using System;

namespace ColdStoreManagement.BLL.Models.DTOs
{
    public class DemandOrderDto
    {
        // Search Filter Properties
        public DateTime? OrderDatefrom { get; set; }
        public DateTime? OrderDateto { get; set; }
        public int Partyid { get; set; }
        public int Growerid { get; set; }
        public string? OrderBy { get; set; }
        public string? DemandStatus { get; set; }
        public string? DemandIrn { get; set; }

        // Demand Detail Properties
        public int DemandNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? GrowerGroupName { get; set; }
        public int? OrderQty { get; set; }
        public string? OrderType { get; set; }

        // Privilege Properties
        public bool DemAdd { get; set; }
        public bool DemEdit { get; set; }
        public bool DemView { get; set; }
        public bool DemDel { get; set; }
        public bool DemApp { get; set; }

        // Status/Result Properties
        public string? RetMessage { get; set; }
        public string? RetFlag { get; set; }
        public string? GlobalUserName { get; set; }

        // Report Related Properties
        public int Lotno { get; set; }
        public string? LotIrn { get; set; }
        public string? GrowerName { get; set; }
        public string? GrowerCombine { get; set; }
        public int? Netqty { get; set; }
        public int? VerfiedQty { get; set; }
        public int? Alotbal { get; set; }
        public string? ItemName { get; set; }
        public string? Prodetails { get; set; }
        public string? PreInwardKhata { get; set; }
        public string? VarietyId { get; set; }
        public string? ChamberName { get; set; }
        public int ChamberId { get; set; }
        public string? Templocation { get; set; }
        public string? ChallanName { get; set; }
        public string? ServiceId { get; set; }
        public int TempOrderId { get; set; }
        public int? DraftedQty { get; set; }
        public string? DemandContact { get; set; }
        public string? DemandRemarks { get; set; }
        public string? TransferType { get; set; }
        public string? TransferTo { get; set; }
    }
}

