using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColdStoreManagement.BLL.Models.DTOs;

namespace ColdStoreManagement.DAL.Services.Interface.TransactionsOut
{
    public interface ICalendarService
    {
        Task<List<CalendarSlotDto>> GetallSlots();
        Task<List<CalendarSlotDto>> GetSlotbydate(DateTime Slotdate);
        Task<CalendarSlotDto?> GetSlotdet(int selectedGrowerId);
        Task<bool> DeleteSlot(int STrid);
    }
}
