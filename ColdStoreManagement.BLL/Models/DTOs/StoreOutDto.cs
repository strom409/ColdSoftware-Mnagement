using System;

namespace ColdStoreManagement.BLL.Models.DTOs
{
    public class StoreOutDto
    {
        public bool IsuserAssigned { get; set; }
        public string? DemandIrn { get; set; }
        public string? GrowerGroupName { get; set; }
        public string? OrderType { get; set; }
        public int? TotalOrderQty { get; set; }
        public int? TotalStoreOut { get; set; }
        public int? DraftedQty { get; set; }
        public int DemandNo { get; set; }
        public string? StoreStat { get; set; }
        public int? GradingQty { get; set; }
        public int Lotno { get; set; }
        public int TempDraftId { get; set; }
        public string? RetMessage { get; set; }
        public string? RetFlag { get; set; }
        public string? GlobalUserName { get; set; }
    }
}
