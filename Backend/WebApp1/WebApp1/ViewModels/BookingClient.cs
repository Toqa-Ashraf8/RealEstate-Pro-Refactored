namespace WebApp1.ViewModels
{
    public class BookingClient
    {
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public int ProjectCode  { get; set; }
        public string ProjectName { get; set; }
        public int UnitID { get; set; }
        public string unitName { get; set; }
       
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
}
