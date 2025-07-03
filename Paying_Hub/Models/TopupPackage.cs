namespace Paying_Hub.Models
{
	public class TopupPackage
	{

	}

	public class packageviewModel
	{

		public int PackageId { get; set; }
		public string PackageName { get; set; }
		public decimal PackagePrice { get; set; }
		public string UserId { get; set; }



	}

    public class MemberROIPackageLedger
    {
        public int SrNo { get; set; }
        public string UserID { get; set; }
        public decimal CrAmount { get; set; }
        public decimal DrAmount { get; set; }
        public DateTime? ROIDate { get; set; } 
        public string Description { get; set; }
        public DateTime EntryDate { get; set; }
        public string FromMid { get; set; }
        public string PackageName { get; set; }
    }

}
