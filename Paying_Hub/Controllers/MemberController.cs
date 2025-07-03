
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
        public async Task<IActionResult> Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(MemberMaster model)
        {
            try
            {

                if (!string.IsNullOrEmpty(model.SponserId))
                {
                    bool isSponserValid = _Member.DoesSponserIdExist(model.SponserId);
                    if (!isSponserValid)
                    {
                        ModelState.AddModelError("SponserId", "Sponsor ID does not exist. Please check and try again.");
                        return View(model);
                    }
                }

                string refcode = GenerateUniqueReferralCode();
                model.ReferralCode = refcode;

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
        private string GenerateUniqueReferralCode()
        {
            string referralCode;
            bool isUnique = false;

            do
            {

                Random rnd = new Random();
                int randomNumber = rnd.Next(1000, 9999);


                int totalMembers = _Member.GetTotalMemberCount();
                int newMemberNumber = totalMembers + 1;


                string lastTwoDigits = (newMemberNumber % 100).ToString("D2");

                referralCode = "Pay" + randomNumber + lastTwoDigits;

                isUnique = !_Member.DoesReferralCodeExist(referralCode);

            } while (!isUnique);

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
                var allMembers = _Member.GetAllMembers();


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


                if (!string.IsNullOrEmpty(loginId))
                    allMembers = allMembers.Where(m => m.ReferralCode == loginId).ToList();

                if (!string.IsNullOrEmpty(mobileNumber))
                    allMembers = allMembers.Where(m => m.MobileNumber.Contains(mobileNumber)).ToList();


                return View("AllUser", allMembers);
            }
            catch (Exception ex)
            {


                ViewBag.ErrorMessage = "An error occurred while processing your request.";
                return View("AllUser", new List<Paying_Hub.Models.MemberMaster>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeactivateMember(string id)
        {
            string memId = id;
            if (!string.IsNullOrEmpty(memId))
            {

                var member = _Member.GetAllMembers().FirstOrDefault(m => m.ReferralCode == memId);
                if (member != null)
                {
                    int IsActive = member.IsActive = 0;
                    _Member.UpdateStatus(member);
                    TempData["Success"] = "Member deactivated successfully!";
                }
                else
                {
                    TempData["Error"] = "Member not found!";
                }
            }
            else
            {
                TempData["Error"] = "Invalid Member ID!";
            }

            return RedirectToAction("AllUser", "Member");
        }

        [HttpPost]
        public async Task<IActionResult> ActivateMember(string id)
        {
            string memId = id;
            if (!string.IsNullOrEmpty(memId))
            {

                var member = _Member.GetAllMembers().FirstOrDefault(m => m.ReferralCode == memId);
                if (member != null)
                {
                    int IsActive = member.IsActive == 1 ? 0 : 1;
                    _Member.UpdateStatus(member);
                    TempData["Success"] = "Member Activated successfully!";
                }
                else
                {
                    TempData["Error"] = "Member not found!";
                }
            }
            else
            {
                TempData["Error"] = "Invalid Member ID!";
            }

            return RedirectToAction("AllUser", "Member");
        }

        public async Task<IActionResult> BankDetails()
        {
            return View();
        }
        public async Task<IActionResult> ApproveKYC()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Password()

        {
            return View();
        }

        [HttpPost]
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

        [HttpPost]
        public JsonResult GetPackageByUserId(string UserId)
        {
            var resultList = _Member.GetPackageByUserId(UserId);
            var result = resultList.FirstOrDefault();
            return Json(result);
        }

        [HttpPost]
        public IActionResult member_topup(packageviewModel Data)
        {
            try
            {
                if (!string.IsNullOrEmpty(Data.UserId))
                {
                    _Member.PackageRegistration(Data);
                    return Json(new { success = true, message = "Package Registered Successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "User Id not found!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred!" });
            }
        }

        public async Task<IActionResult> ViewTopupReport()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ViewTopupReport(DateTime? fromDate, DateTime? toDate, string userName, string packageName)
        {
            try
            {
                var allTopups = _Member.GetTopupReport();
                if (fromDate.HasValue && toDate.HasValue)
                {
                    allTopups = allTopups.Where(x =>
                        x.PackageDate.Date >= fromDate.Value.Date &&
                        x.PackageDate.Date <= toDate.Value.Date
                    ).ToList();
                }
                else if (fromDate.HasValue)
                {
                    allTopups = allTopups.Where(x =>
                        x.PackageDate.Date >= fromDate.Value.Date
                    ).ToList();
                }
                else if (toDate.HasValue)
                {
                    allTopups = allTopups.Where(x =>
                        x.PackageDate.Date <= toDate.Value.Date
                    ).ToList();
                }

                if (!string.IsNullOrWhiteSpace(userName))
                {
                    allTopups = allTopups
                        .Where(x => !string.IsNullOrEmpty(x.UserName) && x.UserName.ToLower().Contains(userName.ToLower()))
                        .ToList();
                }

                if (!string.IsNullOrWhiteSpace(packageName))
                {
                    allTopups = allTopups
                        .Where(x => !string.IsNullOrEmpty(x.PackageName) && x.PackageName.ToLower().Contains(packageName.ToLower()))
                        .ToList();
                }

                return View(allTopups);
            }
            catch (Exception)
            {

                ViewBag.ErrorMessage = "Something went wrong.";
                return View(new List<TopUPReportModel>());
            }
        }

        public async Task<IActionResult> view_direct_referral()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> view_direct_referral(string userid)
        {
            try
            {
                var DirectRefferal = _Member.GetDirectRefferalReport();
                if (!string.IsNullOrWhiteSpace(userid))
                {
                    DirectRefferal = DirectRefferal
                        .Where(x => !string.IsNullOrEmpty(x.UserName) && x.UserName.ToLower().Contains(userid.ToLower()))
                        .ToList();
                }

                return View(DirectRefferal);
            }
            catch (Exception ex)
            {

                ViewBag.ErrorMessage = "Something went wrong.";
                return View(new List<DirectRefferalView>());
            }
        }
        public async Task<IActionResult> ViewUserDownLine()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ViewUserDownLine(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Please enter a valid User ID.";
                return View();
            }

            var tree = _Member.GetMemberTree(userId);

            if (tree == null)
            {
                TempData["Error"] = "No user found with this referral code.";
                return View();
            }

            return View(tree);
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

        public async Task<IActionResult> LevelIncome()
        {
            var lsleveldetails = _Member.GetLevelIncomeLedgerReport();

            return View(lsleveldetails);
        }

        public async Task<IActionResult> CashBackIncome()
        {
            var AllledgerDetails = _Member.GetMemberROIPackageLedger();
            return View(AllledgerDetails);
        }

        [HttpPost]
        public IActionResult CashBackIncome(DateTime? fromDate, DateTime? toDate, string loginId)
        {
            try
            {
                var AllledgerDetails = _Member.GetMemberROIPackageLedger();


                if (fromDate.HasValue && toDate.HasValue)
                {
                    AllledgerDetails = AllledgerDetails.Where(m =>
                        m.ROIDate.HasValue &&
                        m.ROIDate.Value.Date >= fromDate.Value.Date &&
                        m.ROIDate.Value.Date <= toDate.Value.Date
                    ).ToList();
                }
                else if (fromDate.HasValue)
                {
                    AllledgerDetails = AllledgerDetails.Where(m =>
                        m.ROIDate.HasValue &&
                        m.ROIDate.Value.Date >= fromDate.Value.Date
                    ).ToList();
                }
                else if (toDate.HasValue)
                {
                    AllledgerDetails = AllledgerDetails.Where(m =>
                        m.ROIDate.HasValue &&
                        m.ROIDate.Value.Date <= toDate.Value.Date
                    ).ToList();
                }


                if (!string.IsNullOrEmpty(loginId))
                    AllledgerDetails = AllledgerDetails.Where(m => m.UserID == loginId).ToList();


                return View("CashBackIncome", AllledgerDetails);
            }
            catch (Exception ex)
            {


                ViewBag.ErrorMessage = "An error occurred while processing your request.";
                return View("CashBackIncome", new List<Paying_Hub.Models.MemberROIPackageLedger>());
            }
        }


    }
}
