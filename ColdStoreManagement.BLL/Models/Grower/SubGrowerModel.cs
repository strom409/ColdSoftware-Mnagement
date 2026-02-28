namespace ColdStoreManagement.BLL.Models.Grower
{
    public class SubGrowerModel : GrowerModel
    {
        public string? GrowerGroupSelectName { get; set; }
        public int SubGrowerId { get; set; }
        public bool GrowerRetAcitve { get; set; }
    }
}
