using System.ComponentModel.DataAnnotations;

namespace Paying_Hub.Models
{
    public class Cities
    {
        [Key]
        public int id { get; set; }
        public string? city_name { get; set; }
        public int? stateid { get; set; }
        public string? state_name { get; set; }
    }
}
