using WebApp1.Core.Models;

namespace WebApp1.Core.Interfaces
{
    public interface INegotiationRepository
    {
        Task<IEnumerable<int>> GetPendingNegotiationsCount();
        Task<IEnumerable<Negotiation>> GetPendingNegotiations();
    }
}
