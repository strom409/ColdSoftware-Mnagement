using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColdStoreManagement.BLL.Data
{
    public class Chamber
    {
        [Key]
        public  int chamberid { get; set; }
        public string chambername { get; set; } = null!;

        public int MaxQuantity { get; set; } 
        public int? QuantityConsumed { get; set; } 
        public int? QuantityBalanced { get; set; } 

        public bool IsLocked  { get; set; }

        // Foreign key to Building
        public int? BuildingId { get; set; }
        public Building? Building { get; set; }


        // Foreign key to Units
        public int? UnitId { get; set; }
        [ForeignKey(nameof(UnitId))]
        public UnitMaster? UnitMaster { get; set; }

        // Newly added properties
        public string? ChamberType { get; set; }
        public bool FlagDeleted { get; set; }
        public bool Status { get; set; }

        public float? Length { get; set; }
        public float? Width { get; set; }
        public float? Height { get; set; }
        public float? CFT { get; set; }
        public float? ActualSize { get; set; }
        public float? UsableSize { get; set; }
        public float? Floor { get; set; }
        public float? SquareFeet { get; set; }

        public bool IsMeznineChamber { get; set; }
        public bool IsActualChamber { get; set; }

        public ICollection<Allocation>? Allocation { get; set; }
    }
}
