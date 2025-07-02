using Microsoft.AspNetCore.Mvc;
using Paying_Hub.Interface;
using Paying_Hub.Models;
using Paying_Hub.Repository;

namespace Paying_Hub.Controllers
{
    public class MemberController : Controller
    {
        private readonly IMember _Member;


        public MemberController(IMember Member)
        {
            _Member = Member;
        }


        [HttpGet]
        //Admin RegistrationForm
        public async Task<IActionResult> Registration()
        {
            return View();
        }


        //Admin submission form
        [HttpPost]

        public async Task<IActionResult> Registration(MemberMaster model)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    return View(model);
                //}

                // Check if Sponsor ID is provided and exists
                if (!string.IsNullOrEmpty(model.SponserId))
                {
                    bool isSponserValid = _Member.DoesSponserIdExist(model.SponserId);
                    if (!isSponserValid)
                    {
                        ModelState.AddModelError("SponserId", "Sponsor ID does not exist. Please check and try again.");
                        return View(model);
                    }
                }

                // Generate unique referral code
                model.ReferralCode = GenerateUniqueReferralCode(model.Name);

                // Save the member
                string result = _Member.AddMember(model);

                if (result.StartsWith("SQL Error"))
                {
                    TempData["Error"] = result;
                    return View(model);
                }

                TempData["Success"] = "Member Registered Successfully!";
                return RedirectToAction("Registration");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Something went wrong. Please try again later.");
                return View(model);
            }
        }


        //  GenerateUniqueReferralCode method
        private string GenerateUniqueReferralCode(string userName)
        {
            string referralCode;
            bool isUnique = false;

            do
            {
                referralCode = userName.Substring(0, 4).ToUpper() + new Random().Next(100, 999);
                isUnique = !_Member.DoesReferralCodeExist(referralCode);
            }
            while (!isUnique);

            return referralCode;
        }

        [HttpPost]
        public JsonResult GetSponserName(string sponserId)
        {
            string sponserName = _Member.GetSponserNameById(sponserId);
            return Json(new { sponserName });
        }

        public async Task<IActionResult> AllUser()
        {
            var allMembers = _Member.GetAllMembers();
            return View(allMembers);
        }


		[HttpGet]
		public IActionResult AllUserSearch(DateTime? fromDate, DateTime? toDate, string loginId, string mobileNumber)
		{
			try
			{
				var allMembers = _Member.GetAllMembers(); // Replace with your actual data access

				// ✅ Date filter logic
				if (fromDate.HasValue && toDate.HasValue)
				{
					allMembers = allMembers.Where(m =>
						m.CreatedOn.HasValue &&
						m.CreatedOn.Value.Date >= fromDate.Value.Date &&
						m.CreatedOn.Value.Date <= toDate.Value.Date
					).ToList();
				}
				else if (fromDate.HasValue)
				{
					allMembers = allMembers.Where(m =>
						m.CreatedOn.HasValue &&
						m.CreatedOn.Value.Date >= fromDate.Value.Date
					).ToList();
				}
				else if (toDate.HasValue)
				{
					allMembers = allMembers.Where(m =>
						m.CreatedOn.HasValue &&
						m.CreatedOn.Value.Date <= toDate.Value.Date
					).ToList();
				}

				// ✅ Other filters
				if (!string.IsNullOrEmpty(loginId))
					allMembers = allMembers.Where(m => m.ReferralCode == loginId).ToList();

				if (!string.IsNullOrEmpty(mobileNumber))
					allMembers = allMembers.Where(m => m.MobileNumber.Contains(mobileNumber)).ToList();

				// ✅ Return filtered list
				return View("AllUser", allMembers);
			}
			catch (Exception ex)
			{
				// Optionally log the error (e.g., to a logger or file)
				// _logger.LogError(ex, "Error filtering AllUser list");

				ViewBag.ErrorMessage = "An error occurred while processing your request.";
				return View("AllUser", new List<Paying_Hub.Models.MemberMaster>()); // Return empty list on error
			}
		}


		public async Task<IActionResult> BankDetails()
		{
			return View();
		}
		public async Task<IActionResult> ApproveKYC()
		{
			return View();
		}
		public async Task<IActionResult> Password(DateTime? fromDate, DateTime? toDate, string loginId, string mobileNumber)
		{
			var members = _Member.GetAllMemberPasswords(fromDate, toDate, loginId, mobileNumber);
			return View(members);
		}
		public async Task<IActionResult> ManageWithdralStatus()
		{
			return View();
		}
		public async Task<IActionResult> WithdrawalStatusReport()
		{
			return View();
		}
		public async Task<IActionResult> ManageNews()
		{
			return View();
		}
		public async Task<IActionResult> ManageCity()
		{
			return View();
		}

        public async Task<IActionResult> view_fund_request_report()
        {
            return View();
        }
        public async Task<IActionResult> view_fund_transfer()
        {
            return View();
        }
        public async Task<IActionResult> view_fund_transfer_report()
        {
            return View();
        }
        public async Task<IActionResult> member_topup()
        {
            return View();
        }

        public async Task<IActionResult> ViewTopupReport()
        {

			var data =  _Member.GetTopupReport();
			return View(data);
        }
        public async Task<IActionResult> view_direct_referral()
        {
            return View();
        }

        public async Task<IActionResult> ViewUserDownLine()
        {
            return View();
        }
        public async Task<IActionResult> view_withdrawal_report()
        {
            return View();
        }

        public async Task<IActionResult> DirectIncome()
        {
			var lsdirectIncomedetails = _Member.GetDirectIncomeLedgerReport();

			return View(lsdirectIncomedetails);
        }
        public async Task<IActionResult> WithdrawalReport()
        {
            return View();
        }
        public async Task<IActionResult> QueryBox()
        {
            return View();
        }

    }
}
