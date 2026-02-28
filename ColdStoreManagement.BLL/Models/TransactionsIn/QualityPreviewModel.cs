namespace ColdStoreManagement.BLL.Models.TransactionsIn
{
    public class QualityPreviewModel
    {
        public string? GrowerName { get; set; }
        public string? LotNo { get; set; }
        public string? Variety { get; set; }
        public decimal AvgWeight { get; set; }
        public decimal AvgPressure { get; set; }
        public string? Remarks { get; set; }
        public List<string>? Defects { get; set; }
    }
}
