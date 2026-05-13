namespace WebApp1.Core.Interfaces
{
    public interface IDashboardRepository
    {
        Task<IEnumerable<dynamic>> GetProjectsUnitsStats();
        Task<IEnumerable<dynamic>> GetMonthlyBookingStats(); 
        Task<int> GetCount(string sql);
    }
}
