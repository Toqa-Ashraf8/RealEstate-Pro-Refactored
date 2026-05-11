using WebApp1.Core.DTO;
using WebApp1.Core.Models;

namespace WebApp1.Core.Interfaces
{
    public interface IBookingRepository
    {
        Task<(IEnumerable<Negotiation> negotiations, IEnumerable<ClientExtraDetails> ClientPersonalInform)> GetBookingClientData(BookingClient cl);
    }
}