using WebApp1.Core.Models;

namespace WebApp1.Core.DTO
{
    public class FullBookingRequest
    {
        public ClientExtraDetails ClientExtraDetails { get; set; }
        public UnitBooking UnitBooking { get; set; }
    }
}
