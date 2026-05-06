

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using WebApp1.Models;
namespace WebApp1.Models
{ 
    public class Unit
     {
        [Key]
             public int UnitID { get; set; }
             public int? serial { get; set; }
             public string? unitName{ get; set; }
             public string? Floor { get; set; }
             public float? TotalArea { get; set; }
             public int? MeterPrice { get; set; }
             public float? TotalPrice { get; set; }
             public string? unitImage { get; set; }
             [ForeignKey("ProjectCode")] 
             public int? ProjectCode { get; set; }
             public string? ProjectName { get; set; }
             public Boolean? ReservedStatus { get; set; }
            

    }
}

