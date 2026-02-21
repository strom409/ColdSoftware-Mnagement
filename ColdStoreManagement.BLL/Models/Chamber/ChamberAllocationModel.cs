
namespace ColdStoreManagement.BLL.Models.Chamber
{
    public class ChamberAllocationModel
    {
        public int ChamberId { get; set; }       
        public string? ChamberName { get; set; }
        public string? GrowerGroupName { get; set; } = string.Empty;

        /// <summary>
        /// Maps to 'TotalQuantity' - use decimal for weight/units
        /// </summary>
        public decimal ChamberAllocation { get; set; } 
        public int ChamberCapcity { get; set; }
        public string? GlobalUserName { get; set; }
        public int AllocationNo { get; set; }

        /// <summary>
        /// Maps to 'status' - True if locked/active
        /// </summary>
        public bool IsLocked { get; set; }
 
    }

    public sealed class SaveChamberAllocationRequest
    {
        public int ChamberId { get; set; }
        public string GrowerGroupName { get; set; } = string.Empty;
        public decimal ChamberAllocation { get; set; }
    }

}
