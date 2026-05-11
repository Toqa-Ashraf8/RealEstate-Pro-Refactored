using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using WebApp1.Core.DTO;
using WebApp1.Core.Interfaces;
using WebApp1.Core.Models;

namespace WebApp1.Repositories
{
    public class BookingRepository :IBookingRepository
    {
        private readonly DbConnection _db;
        public BookingRepository(DbConnection db)
        {
            _db = db;
        }
        public async Task<(IEnumerable<Negotiation> negotiations, IEnumerable<ClientExtraDetails> ClientPersonalInform)> GetBookingClientData(BookingClient cl)
        {
            string sql = @"select * from Negotiations where ClientID=@ClientID AND ProjectCode=@ProjectCode AND UnitID=@UnitID
                          select * from ClientExtraDetails where ClientID=@ClientID";
            using var multi = await _db.QueryMultipleAsync(sql, 
                new { 
                    ClientID= cl.ClientID, 
                    ProjectCode=cl.ProjectCode,
                    UnitID=cl.UnitID 
                });
            var negotiations = await multi.ReadAsync<Negotiation>();
            var ClientPersonalInform = await multi.ReadAsync<ClientExtraDetails>();
            return (negotiations, ClientPersonalInform);
        }

    }
}
