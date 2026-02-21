using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColdStoreManagement.BLL.Data
{
    public class Party
    {
        [Key]
        public int Partyid { get; set; }

        [Column("partyname")]
        public string? PartyName { get; set; }
        public Agreement? Agreement { get; set; }

        public ICollection<Allocation>? Allocation { get; set; }
        public ICollection<SubGrower>? SubGrowers { get; set; }
    }

}
