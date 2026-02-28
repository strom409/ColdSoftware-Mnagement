using System;

namespace ColdStoreManagement.BLL.Models.DTOs
{
    public class CalendarSlotDto
    {
        public int Slotno { get; set; }
        public DateTime calendardate { get; set; }
        public DateTime calendartime { get; set; }
        public string? GrowerGroupName { get; set; }
        public string? GrowerName { get; set; }
        public int SlotQty { get; set; }
        public string? GrowerContact { get; set; }
    }
}
