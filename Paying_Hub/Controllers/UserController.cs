using Microsoft.AspNetCore.Mvc;
using Paying_Hub.Interface;
using Paying_Hub.Models;
using Paying_Hub.Repository;

namespace Paying_Hub.Controllers
{
    public class UserController : Controller
    {
        private readonly IUser _User;


        public UserController(IUser User)
        {
            _User = User;
        }

        public IActionResult Registration()
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
                    bool isSponserValid = _User.DoesSponserIdExist(model.SponserId);
                    if (!isSponserValid)
                    {
                        ModelState.AddModelError("SponserId", "Sponsor ID does not exist. Please check and try again.");
                        return View(model);
                    }
                }

                string refcode = GenerateUniqueReferralCode();
                model.ReferralCode = refcode;

                string result = _User.AddMember(model);

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


                int totalMembers = _User.GetTotalMemberCount();
                int newMemberNumber = totalMembers + 1;


                string lastTwoDigits = (newMemberNumber % 100).ToString("D2");

                referralCode = "Pay" + randomNumber + lastTwoDigits;

                isUnique = !_User.DoesReferralCodeExist(referralCode);

            } while (!isUnique);

            return referralCode;
        }
        public async Task<IActionResult> AllUser()
        {

            return View();
        }

        [HttpGet]
        public IActionResult AllUserSearch(DateTime? fromDate, DateTime? toDate, string loginId, string mobileNumber)
        {
            try
            {
                var allMembers = _User.GetAllMembers();


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
                    allMembers = allMembers.Where(m => m.SponserId == loginId).ToList();

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

                var member = _User.GetAllMembers().FirstOrDefault(m => m.ReferralCode == memId);
                if (member != null)
                {
                    int IsActive = member.IsActive = 0;
                    _User.UpdateStatus(member);
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

                var member = _User.GetAllMembers().FirstOrDefault(m => m.ReferralCode == memId);
                if (member != null)
                {
                    int IsActive = member.IsActive = 1;
                    _User.UpdateStatus(member);
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
        public async Task<IActionResult> ViewTopupReport()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ViewTopupReport(DateTime? fromDate, DateTime? toDate, string userName, string packageName)
        {
            try
            {
                var allTopups = _User.GetTopupReport();
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
                var DirectRefferal = _User.GetDirectRefferalReport();
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

            var tree = _User.GetMemberTree(userId);

            if (tree == null)
            {
                TempData["Error"] = "No user found with this referral code.";
                return View();
            }

            return View(tree);
        }
        public async Task<IActionResult> DirectIncome()
        {
           
            return View();
        }

        [HttpPost]
        public IActionResult DirectIncome(DateTime? fromDate, DateTime? toDate, string loginId)
        {
            try
            {
                var AllDirectIncome = _User.GetDirectIncomeLedgerReport();


                if (fromDate.HasValue && toDate.HasValue)
                {
                    AllDirectIncome = AllDirectIncome.Where(m =>
                        m.Date.HasValue &&
                        m.Date.Value.Date >= fromDate.Value.Date &&
                        m.Date.Value.Date <= toDate.Value.Date
                    ).ToList();
                }
                else if (fromDate.HasValue)
                {
                    AllDirectIncome = AllDirectIncome.Where(m =>
                        m.Date.HasValue &&
                        m.Date.Value.Date >= fromDate.Value.Date
                    ).ToList();
                }
                else if (toDate.HasValue)
                {
                    AllDirectIncome = AllDirectIncome.Where(m =>
                        m.Date.HasValue &&
                        m.Date.Value.Date <= toDate.Value.Date
                    ).ToList();
                }


                if (!string.IsNullOrEmpty(loginId))
                    AllDirectIncome = AllDirectIncome.Where(m => m.UserID == loginId).ToList();


                return View("DirectIncome", AllDirectIncome);
            }
            catch (Exception ex)
            {


                ViewBag.ErrorMessage = "An error occurred while processing your request.";
                return View("DirectIncome", new List<Paying_Hub.Models.DirectIncomeLedgerModel>());
            }
        }

        public async Task<IActionResult> LevelIncome()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LevelIncome(DateTime? fromDate, DateTime? toDate, string loginId)
        {
            try
            {
                var AllLevelIncome = _User.GetLevelIncomeLedgerReport();


                if (fromDate.HasValue && toDate.HasValue)
                {
                    AllLevelIncome = AllLevelIncome.Where(m =>
                        m.Date.HasValue &&
                        m.Date.Value.Date >= fromDate.Value.Date &&
                        m.Date.Value.Date <= toDate.Value.Date
                    ).ToList();
                }
                else if (fromDate.HasValue)
                {
                    AllLevelIncome = AllLevelIncome.Where(m =>
                        m.Date.HasValue &&
                        m.Date.Value.Date >= fromDate.Value.Date
                    ).ToList();
                }
                else if (toDate.HasValue)
                {
                    AllLevelIncome = AllLevelIncome.Where(m =>
                        m.Date.HasValue &&
                        m.Date.Value.Date <= toDate.Value.Date
                    ).ToList();
                }


                if (!string.IsNullOrEmpty(loginId))
                    AllLevelIncome = AllLevelIncome.Where(m => m.UserID == loginId).ToList();


                return View("LevelIncome", AllLevelIncome);
            }
            catch (Exception ex)
            {


                ViewBag.ErrorMessage = "An error occurred while processing your request.";
                return View("LevelIncome", new List<Paying_Hub.Models.LevelIncomeLedgerModel>());
            }
        }

        public async Task<IActionResult> CashBackIncome()
        {
           
            return View();
        }

        [HttpPost]
        public IActionResult CashBackIncome(DateTime? fromDate, DateTime? toDate, string loginId)
        {
            try
            {
                var AllledgerDetails = _User.GetMemberROIPackageLedger();


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

        public IActionResult edit_user()
        {
            return View();
        }
        public IActionResult bank_detail_list()
        {
            return View();
        }
        public IActionResult approve_kyc()
        {
            return View();
        }
        public IActionResult ViewUserProfile()
        {
            return View();
        }


    }
}
