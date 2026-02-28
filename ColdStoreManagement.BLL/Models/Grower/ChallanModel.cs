namespace ColdStoreManagement.BLL.Models.Grower
{
    public class ChallanModel
    {
        public int ChallanId { get; set; }
        public int SubGrowerId { get; set; }
        public string? GrowerName { get; set; }
        public string? ChallanName { get; set; }
        public string? ChallanAddress { get; set; }
        public string? ChallanVillage { get; set; }
        public string? ChallanCity { get; set; }
        public string? ChallanPhone1 { get; set; }
        public string? ChallanPhone2 { get; set; }
        public string? ChallanSMS1 { get; set; }
        public string? ChallanSMS2 { get; set; }
        public string? RetMessage { get; set; }
        public string? RetFlag { get; set; }
    }
}
