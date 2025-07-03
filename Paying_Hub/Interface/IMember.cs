using Paying_Hub.Models;
namespace Paying_Hub.Interface
{
	public interface IMember
	{
		bool DoesReferralCodeExist(string referralCode);
		bool DoesSponserIdExist(string sponserId);
		public string AddMember(MemberMaster member);
		public int GetTotalMemberCount();
		string GetSponserNameById(string sponserId);
		List<MemberMaster> GetAllMembers();
		List<packageviewModel> GetPackageByUserId(string userid);
		string PackageRegistration(packageviewModel Data);
		List<MemberMaster> GetAllMemberPasswords(DateTime? fromDate, DateTime? toDate, string loginId, string mobileNumber);
		List<TopUPReportModel> GetTopupReport();
		public string UpdateStatus(MemberMaster member);
		List<LevelIncomeLedgerModel> GetLevelIncomeLedgerReport();
		List<DirectIncomeLedgerModel> GetDirectIncomeLedgerReport();

		List<DirectRefferalView> GetDirectRefferalReport();

		MemberTreeViewModel GetMemberTree(string referralCode);

        List<MemberROIPackageLedger> GetMemberROIPackageLedger();

    }
}