using System.ComponentModel.DataAnnotations;

namespace Paying_Hub.Models
{
    public class States
    {
        [Key]
        public int Id { get; set; }
        public string State_name { get; set; }
        public string Type { get; set; }

    }
}
