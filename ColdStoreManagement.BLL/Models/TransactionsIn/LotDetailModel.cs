namespace ColdStoreManagement.BLL.Models.TransactionsIn
{
    public class LotDetailModel
    {
        public string? GrowerGroupName { get; set; }
        public string? GrowerName { get; set; }
        public string? ChallanName { get; set; }
        public string? ChallanNo { get; set; }
        public int Lotno { get; set; }
        public string? Itemname { get; set; }
        public string? Prodname { get; set; }
        public string? PreInwardUom { get; set; }
        public string? PreInwardKhata { get; set; }
        public string? VarietyId { get; set; }
        public string? PreCrateType { get; set; }
        public string? ServiceId { get; set; }
        public decimal PreInwardQty { get; set; }
        public int? GateInId { get; set; }
        public string? PreInIrn { get; set; }
        public string? LotIrn { get; set; }
        public int Partyid { get; set; }
        public int ChamberId { get; set; }
        public string? PreInwardRemarks { get; set; }
        public string? PreinwardStatus { get; set; }
    }
}
