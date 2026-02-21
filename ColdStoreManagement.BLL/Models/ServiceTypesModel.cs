using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.BLL.Models
{
    public class ServiceTypesModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Service type name is required")]
        [StringLength(40, ErrorMessage = "Service type name cannot exceed 40 characters")]
        public string ServiceName { get; set; } = string.Empty;
        public string? Stdetails { get; set; }
        public string? Flag { get; set; }
    }
}
