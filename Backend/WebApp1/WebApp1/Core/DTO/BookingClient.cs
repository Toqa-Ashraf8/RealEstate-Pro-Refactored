using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp1.Core.DTO
{
    public class BookingClient
    {
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public int ProjectCode  { get; set; }
        public string ProjectName { get; set; }
        public int UnitID { get; set; }
        public string unitName { get; set; }
        public int BookingID { get; set; }
        public int NegotiaitonPrice { get; set; }
       

    }
    public class InstallmentData
    {
        public int BookingID { get; set; }
        public int? ReservationAmount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? CheckImagePath { get; set; }
        public int? DownPayment { get; set; }
        public DateTime? FirstInstallmentDate { get; set; }
        public int? InstallmentYears { get; set; }
        public DateTime? BookingDate { get; set; }
      
    }
    public class ClientDetails{
        public int Code { get; set; }
        public int BookingID { get; set; }
        public string? NationalID { get; set; }
        public string? NationalIdImagePath { get; set; }
        public string? SecondaryPhone { get; set; }
        public string? Address { get; set; }
        public string? Job { get; set; }
        [ForeignKey("ClientID")]
        public int? ClientID { get; set; }
        public string? ClientName { get; set; }
    }
}
