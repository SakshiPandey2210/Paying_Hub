using System.ComponentModel.DataAnnotations;

namespace Paying_Hub.Models
{
    
    public class UserLogin
    {
        [Required(ErrorMessage = "Login ID is required.")]
        public string LoginId { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; } 
    }

}
