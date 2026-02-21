using System.ComponentModel.DataAnnotations.Schema;

namespace ColdStoreManagement.BLL.Data
{
    [Table("GrowerAgreement")]
    public class Agreement
    {
        [Column("AgreementId")]
        public int Id { get; set; }

        [ForeignKey(nameof(Party))] // this makes the FK relationship clear
        [Column("MainId")]
        public int Partyid { get; set; }

        public Party? Party { get; set; }

        [Column("qty")]
        public int Allotment { get; set; }
    }

}