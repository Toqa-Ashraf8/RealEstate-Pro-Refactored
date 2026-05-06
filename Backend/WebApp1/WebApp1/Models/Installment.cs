using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp1.Models
{
    public class Installment
    {
        
        [Key]
        public int InstallmentID { get; set; }
        public int? InstallmentNumber { get; set; }
        public DateTime? DueDate { get; set; }
        public int? Months { get; set; }
        public int? MonthlyAmount { get; set; }
        public Boolean? Paid { get; set; }
        public string? PaymentType { get; set; }
        public string? CheckImage { get; set; }  
        public int? BookingID { get; set; }
       
    }
}
