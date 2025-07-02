using Dapper;
using Microsoft.Data.SqlClient;
using Paying_Hub.Interface;
using Paying_Hub.Models;
using System.Data;

namespace Paying_Hub.Repository
{
    public class MemberRepo: IMember
    {

        private readonly string _connectionString;

        public MemberRepo(IConfiguration configuration)
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
                    parameters.Add("@Role", "member"); // Hardcoded as per your logic
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
								Password = reader["Password"].ToString(),
								CreatedOn = Convert.ToDateTime(reader["CreatedOn"])
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				// Log exception here if needed
				// You may throw it or handle as per your pattern
				throw;
			}

			return memberList;
		}

		public List<TopUPReportModel> GetTopupReport()
		{
            try
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
            catch(Exception ex)
            {
                return null;
            }
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





	}
}
