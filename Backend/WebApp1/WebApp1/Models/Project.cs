using System.ComponentModel.DataAnnotations;

namespace WebApp1.Models
{
    public class Project
    {
        [Key]
        public int ProjectCode { get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectType { get; set; }
        public string? Location { get; set; }
        public int? TotalUnits { get; set; }
        public string? ProjectStatus { get; set; }
        public string? ProjectImage { get; set; }
        public List<Unit> units { get; set; }
     

    }
}
