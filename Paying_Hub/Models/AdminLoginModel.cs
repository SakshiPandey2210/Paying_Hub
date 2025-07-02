using System.ComponentModel.DataAnnotations;

namespace Paying_Hub.Models
{
    public class AdminLoginModel
    {
 
            [Required(ErrorMessage = "Username is required")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "Password is required")]
            public string Password { get; set; }

    }
}
