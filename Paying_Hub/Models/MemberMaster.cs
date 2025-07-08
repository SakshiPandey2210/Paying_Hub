using System.ComponentModel.DataAnnotations;

namespace Paying_Hub.Models
{


	public class MemberMaster
	{

		public string? MemberId { get; set; }
		[Required(ErrorMessage = "Sponsor Id is required.")]
		[StringLength(20, ErrorMessage = "Sponsor Id cannot exceed 20 characters.")]
		public string SponserId { get; set; }

		//[Required(ErrorMessage = "Sponsor Name is required.")]
		//[StringLength(50, ErrorMessage = "Sponsor Name cannot exceed 50 characters.")]
		public string? SponserName { get; set; }

		[Required(ErrorMessage = "Name is required.")]
		[StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Mobile Number is required.")]
		[RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile Number must be 10 digits.")]
		public string MobileNumber { get; set; }

		[Required(ErrorMessage = "Email Id is required.")]
		[EmailAddress(ErrorMessage = "Invalid Email Address.")]
		public string EmailId { get; set; }

		[Required(ErrorMessage = "State selection is required.")]
		public string State { get; set; }

		[Required(ErrorMessage = "City selection is required.")]
		public string City { get; set; }
		public string PinCode { get; set; }
		public string Address { get; set; }
		public string? NomineeName { get; set; }
		public DateTime? NomineeAge { get; set; }
		public string? NomineeRelation { get; set; }
		public string? ReferralCode { get; set; }
		public string CreatedBy { get; set; }
		public DateTime? CreatedOn { get; set; }
		public DateTime? UpdatedOn { get; set; }
		public decimal? Amount { get; set; }
		public string Password { get; set; }
		public int IsActive { get; set; }
	}

	public class TopUPReportModel
	{
		public int SrNo { get; set; }
		public string UserID { get; set; }
		public string UserName { get; set; }
		public string PackageName { get; set; }
		public decimal Amount { get; set; }
		public DateTime PackageDate { get; set; }
	}

	public class SponserInfo
	{
		public string SponserName { get; set; }
		public string SponserId { get; set; }
	}

	public class LevelIncomeLedgerModel
	{
		public int SrNo { get; set; }
		public string UserID { get; set; }
		public string UserName { get; set; }
		public string TransactionID { get; set; }
		public string Description { get; set; }
		public decimal Amount { get; set; }
		public DateTime? Date { get; set; }
	}
	public class DirectIncomeLedgerModel
	{
		public int SrNo { get; set; }
		public string UserID { get; set; }
		public string UserName { get; set; }
		public string TransactionID { get; set; }
		public string Description { get; set; }
		public decimal Amount { get; set; }
		public DateTime? Date { get; set; }
	}
	public class DirectRefferalView
	{
		public int SrNo { get; set; }
		public string UserID { get; set; }
		public string UserName { get; set; }
		public string MobileNo { get; set; }
		public string Package { get; set; }
		public DateTime Date { get; set; }
	}

	public class MemberTreeViewModel
	{
		public string MemberId { get; set; }
		public string ReferralCode { get; set; }
		public string SponserId { get; set; }
		public string Name { get; set; }
		public DateTime? TopupDate { get; set; }
		public decimal Amount { get; set; }
		public List<MemberTreeViewModel> DirectReferrals { get; set; } = new List<MemberTreeViewModel>();
	}
}