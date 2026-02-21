using ColdStoreManagement.BLL.Validators;
using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.BLL.Models.Chamber
{
    public class AddChamberVM
    {
        [Required(ErrorMessage = "Building Name is required")]
        public string BuildingName { get; set; } = null!;

        [Required(ErrorMessage = "Unit Name is required")]
        public string UnitName { get; set; } = null!;

        [Required(ErrorMessage = "Chamber Name is required")]
        [UniqueChamberName(ErrorMessage = "This chamber name already exists.")]
        public string ChamberName { get; set; } = null!;
        public int ChamberId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Chamber Capacity must be a positive number")]
        [QuantityCheck]
        public int ChamberCapacity { get; set; }
        public int ChamberCapacityBalanced { get; set; }    //for db purpose

        public bool ChamberLocked { get; set; }

        [Required(ErrorMessage = "BinOrMeznineChamber Name is required")]
        public string BinOrMeznineChamber { get; set; } = null!;

        [Required(ErrorMessage = "Type is required")]
        public string Type { get; set; } = null!;

        public bool Status { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Length must be a greater than zero")]
        public float? Length { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Height must be a greater than zero")]
        public float? Height { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Width must be a greater than zero")]
        public float? Width { get; set; }

        public float? CFT { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "ActualSize must be a greater than zero")]
        public float? ActualSize { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "UsableSize must be a greater than zero")]
        public float? UsableSize { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Floor must be a greater than zero")]
        public float? Floor { get; set; }

        public float? SquareFeet { get; set; }
    }

    
}
