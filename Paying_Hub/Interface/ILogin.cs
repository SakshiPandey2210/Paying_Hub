
using Paying_Hub.Models;

namespace Paying_Hub.Interface
{
    public interface ILogin
    {
        AdminLoginResponse AdminLogin(AdminLoginModel adminLoginModel);


        Task<MemberMaster> ValidateUserLogin(string loginId, string password);

	}
}
