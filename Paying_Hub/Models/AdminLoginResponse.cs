namespace Paying_Hub.Models
{
    public class AdminLoginResponse
    {

        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DOB { get; set; }
        public string? CompanyName { get; set; }
        public string? Email { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Password { get; set; } // maps from PasswordHash
        public string PasswordHash { get; set; }  // ✅ Must match SQL column name
        public bool IsActive { get; set; }
        public int RecoredStatusId { get; set; }
        public string? Role { get; set; }
    }
}
