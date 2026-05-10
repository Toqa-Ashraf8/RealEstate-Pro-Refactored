using WebApp1.Core.Models;

namespace WebApp1.Core.Interfaces
{
    public interface INegotiationRepository
    {
        Task<int> GetPendingNegotiationsCount();
        Task<IEnumerable<Negotiation>> GetPendingNegotiations();
        Task<bool> ProcessNegotiationReview(Rejected_negotiations_phase phase);
        Task<(bool Re_Approved, bool Re_Rejected)> UpdateNegotiationReview(Rejected_negotiations_phase phase);
        Task<(int count, IEnumerable<Negotiation> negotiations)> GetRejectedNegotiations();
        Task<(int count, IEnumerable<Negotiation> negotiations)> GetApprovedNegotiations();
    }
}
