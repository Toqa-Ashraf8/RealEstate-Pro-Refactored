using System.ComponentModel.DataAnnotations;

namespace WebApp1.Models
{
    public class Client
    {
        [Key]
        public int? ClientID { get; set; }
        public string? ClientName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ClientStatus { get; set; }
        public string? Notes { get; set; }
        public List<Negotiation> negotiations { get; set; }
    }
}
