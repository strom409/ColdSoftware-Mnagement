using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.BLL.Models
{
    public class VehInfoModel
    {
        public int Vehid { get; set; }
        [Required(ErrorMessage = "Please Select valid vehicle No")]
        public string? Vehno { get; set; }
        [Required(ErrorMessage = "Please Select valid driver Name")]
        public string? VehDriver { get; set; }
        [Required(ErrorMessage = "Please Select valid Contact no")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string? VehContact { get; set; }

        public string? Vehtype { get; set; }

        public bool VehStatus { get; set; }

        public int? Flagdeleted { get; set; }

        public int? Userid { get; set; }

        //public DateTime? Createddate { get; set; } = DateTime.Now;
    }
}
