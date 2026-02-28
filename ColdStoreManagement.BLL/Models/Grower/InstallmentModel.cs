using System;

namespace ColdStoreManagement.BLL.Models.Grower
{
    public class InstallmentModel
    {
        public int Id { get; set; }
        public int PersistedId { get; set; }
        public bool IsPersisted => PersistedId > 0;
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
    }
}
