using System.Data;
using System.Data.Common;
using System.Globalization;
using WebApp1.Core.Interfaces;
using Dapper;


namespace WebApp1.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly DbConnection _db;
        public DashboardRepository(DbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<dynamic>> GetProjectsUnitsStats()
        {
            string sql = @"SELECT p.ProjectName, COUNT(u.UnitID) as TotalUnitsCount
                       FROM Projects p
                       LEFT JOIN Units u ON p.ProjectCode = u.ProjectCode
                       GROUP BY p.ProjectName";
            return await _db.QueryAsync<dynamic>(sql);
        }

        public async Task<IEnumerable<dynamic>> GetMonthlyBookingStats()
        {
            string sql = @"SELECT MONTH(BookingDate) AS MonthNumber, COUNT(*) AS BookingCount
                       FROM reserved_clients_details
                       WHERE YEAR(BookingDate) = YEAR(GETDATE())
                       GROUP BY MONTH(BookingDate)";

            var dbResults = await _db.QueryAsync<dynamic>(sql);

            var finalResult = Enumerable.Range(1, 12).Select(i => {
                var row = dbResults.FirstOrDefault(r => r.MonthNumber == i);
                return new
                {
                    MonthName = CultureInfo.GetCultureInfo("ar-EG").DateTimeFormat.GetMonthName(i),
                    BookingCount = row != null ? row.BookingCount : 0,
                    MonthNumber = i
                };
            });

            return finalResult;
        }

        public async Task<int> GetCount(string sql)
        {
            return await _db.ExecuteScalarAsync<int>(sql);
        }
    }
}
