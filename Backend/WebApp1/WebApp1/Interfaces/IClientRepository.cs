using System.Data;
using WebApp1.Models;

namespace WebApp1.Interfaces
{
    public interface IClientRepository
    {
        Task<DataTable> GetAllProjects();
        Task<DataTable> GetUnitsByProject(int projectid);
        Task<DataTable> GetUnitPrice(int unitid);
        Task<(int id, bool saved, bool updated)> UpsertClient(Client cl);
        Task<bool> DeleteClient(int id);
        Task<DataTable> GetAllClients();
        Task<DataTable> GetClientNegotiations(int clientid);
        Task<(DataTable dt, List<Negotiation> negotiations, bool isnull)> GetFirstClient();
         Task<(DataTable dt, List<Negotiation> negotiations, bool isnull)> GetLastClient();
        Task<(DataTable dt, List<Negotiation> negotiations, bool isLast, bool isEmpty)> GetNextClient(int currentId);
        Task<(DataTable dt, List<Negotiation> negotiations, bool isFirst, bool isEmpty)> GetPreviousClient(int currentId);
    }
}
