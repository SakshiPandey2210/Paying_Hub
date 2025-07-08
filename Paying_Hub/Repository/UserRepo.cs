using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Paying_Hub.Interface;
using Paying_Hub.Models;
using System.Data;
using System.Text;

namespace Paying_Hub.Repository
{
    public class UserRepo : IUser
    {

        private readonly string _connectionString;

        public UserRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);

        public bool DoesReferralCodeExist(string referralCode)
        {

            try
            {


                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("sp_CheckReferralCodeExistence", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ReferralCode", referralCode);

                    conn.Open();
                    var result = cmd.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int count))
                    {
                        return count > 0;
                    }
                }

                return false;

            }
            catch (Exception ex)
            {
                return false;
            }

        }


        public bool DoesSponserIdExist(string sponserId)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("sp_CheckSponserIdExistence", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SponserId", sponserId);

                    conn.Open();
                    var result = cmd.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int count))
                    {
                        return count > 0;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int GetTotalMemberCount()
        {
            using (var conn = Connection)
            {
                string procedureName = "sp_memberRefferalcount";
                int count = conn.QueryFirstOrDefault<int>(procedureName, commandType: CommandType.StoredProcedure);

                return count;
            }
        }

        public string AddMember(MemberMaster member)
        {
            try
            {
                using (var conn = Connection)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@ReferralCode", member.ReferralCode);
                    parameters.Add("@SponserId", member.SponserId);
                    parameters.Add("@SponserName", member.SponserName);
                    parameters.Add("@Name", member.Name);
                    parameters.Add("@MobileNumber", member.MobileNumber);
                    parameters.Add("@EmailId", member.EmailId);
                    parameters.Add("@State", member.State);
                    parameters.Add("@City", member.City);
                    parameters.Add("@PinCode", member.PinCode);
                    parameters.Add("@Address", member.Address);
                    parameters.Add("@Role", "member");
                    parameters.Add("@NomineeName", string.IsNullOrEmpty(member.NomineeName) ? null : member.NomineeName);
                    parameters.Add("@NomineeAge", member.NomineeAge.HasValue ? member.NomineeAge : null);
                    parameters.Add("@NomineeRelation", string.IsNullOrEmpty(member.NomineeRelation) ? null : member.NomineeRelation);
                    parameters.Add("@IsActive", true);
                    parameters.Add("@CreatedBy", 1);

                    conn.Execute("usp_InsertMember", parameters, commandType: CommandType.StoredProcedure);

                    return "Member added successfully.";
                }
            }
            catch (SqlException ex)
            {
                return "SQL Error: " + ex.Message;
            }
            catch (Exception ex)
            {
                return "Unexpected error: " + ex.Message;
            }
        }

        public string GetSponserNameById(string sponserId)
        {
            try
            {
                using (var conn = Connection)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@SponserId", sponserId);

                    var result = conn.QueryFirstOrDefault<SponserInfo>(
                        "usp_GetSponserNameById",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return result?.SponserName ?? string.Empty;
                }


            }
            catch (SqlException ex)
            {
                return "";
            }
        }


        public List<MemberMaster> GetAllMembers()
        {
            using (var conn = Connection)
            {
                string procedureName = "usp_GetAllMembers";
                var members = conn.Query<MemberMaster>(procedureName, commandType: CommandType.StoredProcedure).ToList();
                return members;
            }
        }


        public List<packageviewModel> GetPackageByUserId(string userid)
        {
            try
            {
                using (var conn = Connection)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@ReferralCode", userid);

                    var result = conn.Query<packageviewModel>("usp_GetPackageByUserId", parameters, commandType: CommandType.StoredProcedure);

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {

                return new List<packageviewModel>();
            }
        }


        public string PackageRegistration(packageviewModel Data)
        {
            try
            {
                using (var conn = Connection)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@UserId", Data.UserId);
                    parameters.Add("@PackId", Data.PackageId);
                    conn.Execute("Sp_PackageLedgerEntries", parameters, commandType: CommandType.StoredProcedure);

                    return "Package Registration successfully.";
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }


        public List<MemberMaster> GetAllMemberPasswords(DateTime? fromDate, DateTime? toDate, string loginId, string mobileNumber)
        {
            var memberList = new List<MemberMaster>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("sp_GetMemberPasswords", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@FromDate", (object)fromDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ToDate", (object)toDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LoginId", (object)loginId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MobileNumber", (object)mobileNumber ?? DBNull.Value);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            memberList.Add(new MemberMaster
                            {
                                MemberId = reader["MemberId"].ToString(),
                                Name = reader["Name"].ToString(),
                                ReferralCode = reader["ReferralCode"].ToString(),
                                MobileNumber = reader["MobileNumber"].ToString(),
                                Password = DecodeBase64(reader["Password"].ToString()),
                                CreatedOn = Convert.ToDateTime(reader["CreatedOn"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return memberList;
        }

        private string DecodeBase64(string encodedPassword)
        {
            if (string.IsNullOrEmpty(encodedPassword))
                return string.Empty;

            try
            {
                byte[] data = Convert.FromBase64String(encodedPassword);
                return Encoding.UTF8.GetString(data);
            }
            catch
            {

                return encodedPassword;
            }
        }


        public List<TopUPReportModel> GetTopupReport()
        {
            var list = new List<TopUPReportModel>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_GetTopupReport", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new TopUPReportModel
                        {
                            SrNo = Convert.ToInt32(reader["SrNo"]),
                            UserID = reader["UserID"].ToString(),
                            UserName = reader["UserName"].ToString(),
                            PackageName = reader["PackageName"].ToString(),
                            Amount = Convert.ToDecimal(reader["Amount"]),
                            PackageDate = Convert.ToDateTime(reader["PackageDate"])
                        });
                    }
                }
            }

            return list;
        }


        public string UpdateStatus(MemberMaster member)
        {
            try
            {
                using (var conn = Connection)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@ReferralCode", member.ReferralCode);
                    parameters.Add("@IsActive", member.IsActive);
                    conn.Execute("sp_UpdateStatus", parameters, commandType: CommandType.StoredProcedure);

                    if (member.IsActive == 0)
                    {
                        return "Member Deactive successfully.";
                    }
                    else
                    {
                        return "Member Active successfully.";
                    }
                }
            }
            catch (SqlException ex)
            {
                return "SQL Error: " + ex.Message;
            }
            catch (Exception ex)
            {
                return "Unexpected error: " + ex.Message;
            }
        }

        public List<LevelIncomeLedgerModel> GetLevelIncomeLedgerReport()
        {
            var list = new List<LevelIncomeLedgerModel>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_GetLevelIncomeLedgerReport", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LevelIncomeLedgerModel
                        {
                            SrNo = Convert.ToInt32(reader["SrNo"]),
                            UserID = reader["UserID"].ToString(),
                            UserName = reader["UserName"].ToString(),
                            TransactionID = reader["TransactionID"].ToString(),
                            Description = reader["Description"].ToString(),
                            Amount = Convert.ToDecimal(reader["Amount"]),
                            Date = Convert.ToDateTime(reader["Date"])
                        });
                    }
                }
            }

            return list;
        }

        public List<DirectIncomeLedgerModel> GetDirectIncomeLedgerReport()
        {
            var list = new List<DirectIncomeLedgerModel>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_GetDirectIncomeLedgerReport", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DirectIncomeLedgerModel
                        {
                            SrNo = Convert.ToInt32(reader["SrNo"]),
                            UserID = reader["UserID"].ToString(),
                            UserName = reader["UserName"].ToString(),
                            TransactionID = reader["TransactionID"].ToString(),
                            Description = reader["Description"].ToString(),
                            Amount = Convert.ToDecimal(reader["Amount"]),
                            Date = Convert.ToDateTime(reader["Date"])
                        });
                    }
                }
            }

            return list;
        }


        public List<DirectRefferalView> GetDirectRefferalReport()
        {
            var list = new List<DirectRefferalView>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_GetDirectRefferalReport", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DirectRefferalView
                        {
                            SrNo = Convert.ToInt32(reader["SrNo"]),
                            UserID = reader["UserID"].ToString(),
                            UserName = reader["UserName"].ToString(),
                            Package = reader["Packages"].ToString(),
                            MobileNo = reader["MobileNumber"].ToString(),
                            Date = Convert.ToDateTime(reader["Date"])
                        });
                    }
                }
            }

            return list;
        }
        public MemberTreeViewModel GetMemberTree(string referralCode)
        {
            try
            {
                using (var conn = Connection)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@ReferralCode", referralCode);

                    var flatList = conn.Query<MemberTreeViewModel>(
                        "sp_GetMemberDownlineTree",
                        parameters,
                        commandType: CommandType.StoredProcedure,
                        commandTimeout: 120
                    ).ToList();

                    if (!flatList.Any())
                        return null;

                    // Safely find root node (case-insensitive match just in case)
                    var root = flatList
                        .FirstOrDefault(x => x.ReferralCode?.Trim().Equals(referralCode.Trim(), StringComparison.OrdinalIgnoreCase) == true);

                    if (root == null)
                        return null;

                    BuildTree(root, flatList);

                    return root;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching tree: " + ex.Message);
            }
        }


        private void BuildTree(MemberTreeViewModel parent, List<MemberTreeViewModel> allMembers)
        {
            if (parent == null)
                return;

            parent.DirectReferrals = allMembers
                .Where(m => m.SponserId == parent.ReferralCode)
                .ToList();

            foreach (var child in parent.DirectReferrals)
            {
                BuildTree(child, allMembers);
            }
        }

        public List<MemberROIPackageLedger> GetMemberROIPackageLedger()
        {
            using (var conn = Connection)
            {
                string procedureName = "sp_GetMemberROIPackageLedgerDetails";
                var RoiList = conn.Query<MemberROIPackageLedger>(procedureName, commandType: CommandType.StoredProcedure).ToList();
                return RoiList;
            }
        }


    }

}