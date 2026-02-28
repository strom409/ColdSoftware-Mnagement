using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ColdStoreManagement.BLL.Models.DTOs;
using ColdStoreManagement.DAL.Services.Interface.TransactionsOut;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ColdStoreManagement.DAL.Services.Implementation.TransactionsOut
{
    public class CalendarService : ICalendarService
    {
        private readonly IConfiguration _configuration;

        public CalendarService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<CalendarSlotDto>> GetallSlots()
        {
            List<CalendarSlotDto> Getallslot = new List<CalendarSlotDto>();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                const string query = "select sdate,sum(qty) as Qty from Grading_calendar WHERE flagdeleted=0 group by sdate order by sdate";
                SqlCommand cmd = new SqlCommand(query, con)
                {
                    CommandType = CommandType.Text
                };

                con.Open();
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    CalendarSlotDto companyModel = new CalendarSlotDto
                    {
                        calendardate = rdr["sdate"] as DateTime? ?? DateTime.MinValue,
                        SlotQty = rdr.IsDBNull(rdr.GetOrdinal("Qty")) ? 0 : rdr.GetInt32(rdr.GetOrdinal("Qty")),
                    };
                    Getallslot.Add(companyModel);
                }
                con.Close();
                cmd.Dispose();
            }
            return Getallslot;
        }

        public async Task<List<CalendarSlotDto>> GetSlotbydate(DateTime Slotdate)
        {
            List<CalendarSlotDto> GetGroweralslot = new List<CalendarSlotDto>();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                const string query = @"SELECT
      ci.sdate,p.partytypeid + '-' + p.partyname AS PartyName,
    CONVERT(varchar(max), ps.partyid) + '-' + ps.partyname AS GrowerName,ci.sdate,ci.Qty, ttime,ci.contact,ci.id
    
FROM Grading_calendar ci
LEFT JOIN party p ON ci.partyid = p.partyid
LEFT JOIN partysub ps ON ci.growerid = ps.partyid

WHERE 
 ci.sdate=@Slotdate and ci.flagdeleted=0

ORDER BY id";
                SqlCommand cmd = new SqlCommand(query, con)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@Slotdate", Slotdate);
                con.Open();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    CalendarSlotDto companyModel = new CalendarSlotDto
                    {
                        Slotno = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                        calendardate = reader["sdate"] as DateTime? ?? DateTime.MinValue,
                        GrowerGroupName = reader["PartyName"] as string,
                        GrowerName = reader["GrowerName"] as string,
                        SlotQty = reader.IsDBNull(reader.GetOrdinal("Qty")) ? 0 : reader.GetInt32(reader.GetOrdinal("Qty")),
                        GrowerContact = reader["contact"] as string,
                        calendartime = reader["ttime"] as DateTime? ?? DateTime.MinValue,
                    };
                    GetGroweralslot.Add(companyModel);
                }
                con.Close();
                cmd.Dispose();
            }
            return GetGroweralslot;
        }

        public async Task<CalendarSlotDto?> GetSlotdet(int selectedGrowerId)
        {
            CalendarSlotDto? companyModel = null;

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                const string sql = @"SELECT
      ci.sdate,p.partytypeid + '-' + p.partyname AS PartyName,
    CONVERT(varchar(max), ps.partyid) + '-' + ps.partyname AS GrowerName,ci.sdate,ci.Qty, ttime,ci.contact,ci.id
    
FROM Grading_calendar ci
LEFT JOIN party p ON ci.partyid = p.partyid
LEFT JOIN partysub ps ON ci.growerid = ps.partyid

WHERE 
 ci.id=@orderid and ci.flagdeleted=0

ORDER BY id";

                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@orderid", selectedGrowerId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            companyModel = new CalendarSlotDto
                            {
                                Slotno = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                                calendardate = reader["sdate"] as DateTime? ?? DateTime.MinValue,
                                GrowerGroupName = reader["PartyName"] as string,
                                GrowerName = reader["GrowerName"] as string,
                                SlotQty = reader.IsDBNull(reader.GetOrdinal("Qty")) ? 0 : reader.GetInt32(reader.GetOrdinal("Qty")),
                                GrowerContact = reader["contact"] as string,
                                calendartime = reader["ttime"] as DateTime? ?? DateTime.MinValue,
                            };
                        }
                    }
                }
                con.Close();
            }
            return companyModel;
        }

        public async Task<bool> DeleteSlot(int STrid)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                const string query = "update Grading_calendar set flagdeleted=1 where id=@Id";
                SqlCommand cmd = new SqlCommand(query, con)
                {
                    CommandType = CommandType.Text,
                };

                cmd.Parameters.AddWithValue("@Id", STrid);

                con.Open();
                await cmd.ExecuteNonQueryAsync();

                con.Close();
                cmd.Dispose();
            }
            return true;
        }
    }
}
