
namespace ColdStoreManagement.BLL.Models.Chamber
{
    public class ChamberModel
    {
        // Primary Identifiers
        public int ChamberId { get; set; }
        public string? ChamberName { get; set; } = string.Empty;

        // Capacity and Units
        public decimal Capacity { get; set; }   //qty
        public int UnitId { get; set; }
        public string? ChamberType { get; set; } // Ctype 
        public string? UnitName { get; set; } // A

        // Status and Logic
        public Boolean Status { get; set; }
        public int? Cid { get; set; }
        public bool IsBin { get; set; } // Mapping 0/1 to Boolean

        // Audit Fields
        public int UserId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }


        //public int MaxQuantity { get; set; }
        //public int? QuantityConsumed { get; set; }
        //public int? QuantityBalanced { get; set; }

        //public bool IsLocked { get; set; }
        //public int? BuildingId { get; set; }


        //public bool FlagDeleted { get; set; }
        //public float? Length { get; set; }
        //public float? Width { get; set; }
        //public float? Height { get; set; }
        //public float? CFT { get; set; }
        //public float? ActualSize { get; set; }
        //public float? UsableSize { get; set; }
        //public float? Floor { get; set; }
        //public float? SquareFeet { get; set; }

        //public bool IsMeznineChamber { get; set; }
        //public bool IsActualChamber { get; set; }


    }

    public class ChamberModelVM
    {
        public int ChamberId { get; set; }
        public string ChamberName { get; set; } = string.Empty;

        // Mapped from ChamberType with "NULL" fallback
        public string Type { get; set; } = "NULL";

        public int Floor { get; set; }
        public decimal CFT { get; set; }
        public decimal ActualSize { get; set; }
        public decimal UsableSize { get; set; }
        public decimal Square { get; set; }

        public bool isLocked { get; set; }
        public int Status { get; set; }
        public int BuildingId { get; set; }

        // Added this based on your .Include(a => a.Building) 
        // usually you want the name in the VM
        public string? BuildingName { get; set; }
    }

    public sealed class ChamberEditModel
    {
        public string ChamberType { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string UnitName { get; set; } = string.Empty;
    }

    public sealed class AllocationInfo
    {
        public int AllocationId { get; set; }
        public int Quantity { get; set; }
    }

}
