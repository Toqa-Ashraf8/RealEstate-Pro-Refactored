using System.Data;
using WebApp1.Models;

namespace WebApp1.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<Project>> GetAllProjects();
        Task<IEnumerable<Unit>> GetUnitsByProject(int projectid);
        Task<IEnumerable<Unit>> GetUnitPrice(int unitid);
        Task<(int id, bool saved, bool updated)> UpsertClient(Client cl);
        Task<bool> DeleteClient(int id);
        Task<IEnumerable<Client>> GetAllClients();
        Task<IEnumerable<Negotiation>> GetClientNegotiations(int clientid);
        Task<(Client? client, IEnumerable<Negotiation> negotiations, bool isnull)> GetFirstClient();
        Task<(Client? client, IEnumerable<Negotiation> negotiations_l, bool isnull)> GetLastClient();
        Task<(Client? client, IEnumerable<Negotiation> negotiations, bool isLast, bool isEmpty)> GetNextClient(int currentId);
        Task<(Client? client, IEnumerable<Negotiation> negotiations, bool isFirst, bool isEmpty)> GetPreviousClient(int currentId);
    }
}
