
using Paying_Hub.Models;
namespace Paying_Hub.Interface
{
    public interface IMember
    {
        bool DoesReferralCodeExist(string referralCode);


        bool DoesSponserIdExist(string sponserId);

        public string AddMember(MemberMaster member);
        string GetSponserNameById(string sponserId);
        List<MemberMaster> GetAllMembers();

        List<MemberMaster> GetAllMemberPasswords(DateTime? fromDate, DateTime? toDate, string loginId, string mobileNumber);

        List<TopUPReportModel> GetTopupReport();

        List<DirectIncomeLedgerModel> GetDirectIncomeLedgerReport();

	}
}
