using WebApp1.Core.Models;

namespace WebApp1.Core.DTO
{
    public class ReservedClientDto
    {
        public Negotiation InitialClientData { get; set; }
        public ClientDetails ClientExDetails { get; set; }
        public UnitBooking BookingDetails { get; set; }
        public List<Installment> installments { get; set; }
    }
}
