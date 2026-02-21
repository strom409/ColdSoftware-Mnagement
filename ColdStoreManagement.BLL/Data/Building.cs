
namespace ColdStoreManagement.BLL.Data
{
    public class Building
    {
        public int Id { get; set; }
        public string BuildingName { get; set; } = null!;

        public ICollection<Chamber>? chamber { get; set; }
    }
}
