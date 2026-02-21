using System;

namespace ColdStoreManagement.BLL.Data
{
    public class Allocation
    {
        public int Id { get; set; }

        public int Chamberid { get; set; }

        public Chamber? Chamber { get; set; }

        public int Partyid { get; set; }

        public Party? Party { get; set; }

        public int? SubGrowerId { get; set; }

        public SubGrower? SubGrower { get; set; }

        public int Quantity { get; set; }
        public int? Series { get; set; }

        public bool FlagDeleted { get; set; }
        public bool FlagIsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

    }
}
