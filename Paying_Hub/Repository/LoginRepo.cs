using Dapper;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Data.SqlClient;
using Paying_Hub.Interface;
using Paying_Hub.Models;
using System.Data;
using System.Diagnostics.Metrics;
using System.Net.NetworkInformation;
using System.Text;

namespace Paying_Hub.Repository
{
    public class LoginRepo : ILogin
    {
        private readonly string _connectionString;

        public LoginRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        private IDbConnection Connection => new SqlConnection(_connectionString);

        public AdminLoginResponse AdminLogin(AdminLoginModel adminLoginModel)
        {
            try
            {
                using (var connection = Connection)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Email", adminLoginModel.UserName);

                    var admin = connection.QueryFirstOrDefault<AdminLoginResponse>(
                        "sp_Admin_Login",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    if (admin == null)
                        return null;


                    if (string.IsNullOrEmpty(adminLoginModel.Password) || string.IsNullOrEmpty(admin.PasswordHash))
                    {
                        return null; // or handle the error appropriately
                    }
                    // Verify hashed password
                    bool isValid = BCrypt.Net.BCrypt.Verify(adminLoginModel.Password, admin.PasswordHash);

                    if (!isValid)
                        return null;

                    return admin;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during admin login: " + ex.Message);
                throw;
            }
        }

        public static string EncodePasswordToBase64(string plainPassword)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(plainPassword);
            string base64Encoded = Convert.ToBase64String(passwordBytes);
            return base64Encoded;
        }

        public async Task<MemberMaster> ValidateUserLogin(string loginId, string password)
		{
            try
            {
                string encryptedPassword = EncodePasswordToBase64(password);

                using (var connection = Connection)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@LoginId", loginId);
                    parameters.Add("@Password", encryptedPassword);

                    var result = await connection.QueryFirstOrDefaultAsync<MemberMaster>(
                        "sp_User_Login", parameters, commandType: CommandType.StoredProcedure);

                    return result; 
                }
            }
            catch(Exception ex) {

                return null;
            }
		}

	}

}
