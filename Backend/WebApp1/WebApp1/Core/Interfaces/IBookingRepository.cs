using WebApp1.Core.DTO;
using WebApp1.Core.Models;

namespace WebApp1.Core.Interfaces
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Negotiation>> GetBookingClientData(BookingClient cl);
        Task<string> UploadBookingImages(IFormFile file, string folderName);
        List<InstallmentViewModel> GenerateInstallments(InstallmentDetails request);
        Task<(int id, bool savedBooking, bool updatedBooking)> ConfirmFullBooking(FullBookingRequest request);
        Task<bool> ConfirmReservation(NegotiationViewModel neg);
        Task<IEnumerable<BookingClient>> GetAllReservedClients();
        Task<ReservedClientDto> GetReservedClientById(int bookingId);
    }
}