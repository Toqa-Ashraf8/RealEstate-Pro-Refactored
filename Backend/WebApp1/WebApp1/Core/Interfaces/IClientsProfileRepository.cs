namespace WebApp1.Core.Interfaces
{
    public interface IClientsProfileRepository
    {
        Task<IEnumerable<dynamic>> GetAllClients();
        Task<dynamic> GetClientProfile(int clientId);
    }
}
