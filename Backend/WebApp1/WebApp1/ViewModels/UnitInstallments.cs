using WebApp1.Models;

namespace WebApp1.ViewModels
{
    public class UnitInstallments
    {
        public int BookingID { get; set; }
        public int UnitID { get; set; }
        public string unitName { get; set; }
        public string ProjectName { get; set; }
        public DateTime? BookingDate { get; set; }
     
        public List<dynamic> Installments { get; set; } = new List<dynamic>();
    }
}
