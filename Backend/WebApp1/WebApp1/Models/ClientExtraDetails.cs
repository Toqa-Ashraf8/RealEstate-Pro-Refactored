using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp1.Models
{
    public class ClientExtraDetails
    {
        [Key]
        public int Code { get; set; }
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
