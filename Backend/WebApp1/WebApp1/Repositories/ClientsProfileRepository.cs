using Dapper;
using System.Data;
using System.Data.Common;
using WebApp1.Core.Interfaces;

namespace WebApp1.Repositories
{
    public class ClientsProfileRepository: IClientsProfileRepository
    {
        private readonly DbConnection _db;
        public ClientsProfileRepository(DbConnection db)
        {
            _db = db;
        }
        public async Task<IEnumerable<dynamic>> GetAllClients()
        {
            return await _db.QueryAsync<dynamic>("SELECT * FROM Clients");
        }
        public async Task<dynamic> GetClientProfile(int clientId)
        {
            // 1. جلب بيانات العميل الأساسية
            var clientData = await _db.QueryFirstOrDefaultAsync<dynamic>(
                "SELECT * FROM ClientFullDetails WHERE ClientID = @ClientID", new { ClientID = clientId });

            string sql = "SELECT * FROM ClientUnitsBookings WHERE ClientID = @ClientID";
            var rawData = await _db.QueryAsync<dynamic>(sql, new { ClientID = clientId });
            var bookedUnits = rawData.GroupBy(r => r.BookingID).Select(group => new
            {
                BookingID = group.Key,
                UnitID = group.First().UnitID,
                UnitName = group.First().unitName,
                ProjectName = group.First().ProjectName,
                BookingDate = group.First().BookingDate,
                Installments = group.Select(i => new
                {
                    i.InstallmentID,
                    i.DueDate,
                    i.MonthlyAmount,
                    i.Paid
                }).ToList()
            }).ToList();

            return new { clientData, bookedUnitsData = bookedUnits };
        }
    
    }
}
