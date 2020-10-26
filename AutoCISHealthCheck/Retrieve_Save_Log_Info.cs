using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Collections;

namespace AutoCISHealthCheck
{
    class Retrieve_Save_Log_Info
    {
        #region Declarations
        static string conStr = ConfigurationManager.ConnectionStrings["MyConnectionString"].ToString();
        SqlConnection con = new SqlConnection(conStr);
        #endregion

        #region Methods
        public ArrayList getCredentials(int UserId, int RoleId)
        {
            Creds LoginCreds = new Creds();
            ArrayList CredList = new ArrayList();

            SqlCommand cmd = new SqlCommand("RetrieveCredentials", con);
            cmd.Parameters.AddWithValue("@LogUserId", UserId);
            cmd.Parameters.AddWithValue("@LogRoleId", RoleId);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection)) //close con on reader.Close
            {
                while (reader.Read())
                {
                    LoginCreds = new Creds
                    {
                        Username = reader["Username"].ToString(),
                        Password = reader["Password"].ToString(),
                        UserId = Convert.ToInt32(reader["UserId"].ToString()),
                        RoleId = Convert.ToInt32(reader["RoleId"].ToString()),
                        RoleType = reader["RoleName"].ToString(),
                        UserType = reader["UserTypeName"].ToString()

                    };
                    CredList.Add(LoginCreds);
                }
            }    //The stored procedure should take 2 params and return creds
            con.Close();
            return CredList;
        }
        public ArrayList GetSearchData()
        {
            Search_Data srch;
            ArrayList SearchDataList = new ArrayList();

            try
            {
                SqlCommand cmd = new SqlCommand("GetSearchData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection)) //close con on reader.Close
                {
                    while (reader.Read())
                    {
                        srch = new Search_Data()
                        {
                            Province = reader["Province"].ToString(),
                            ParcelType = reader["ParcelType"].ToString(),
                            AdministrativeDistrict = reader["AdministrativeDistrict"].ToString(),
                            FarmNumber = Convert.ToInt32(reader["FarmNumber"].ToString()),
                            ProvinceIndex = Convert.ToInt32(reader["ProvinceIndex"].ToString()),
                            ProvinceValue = reader["ProvinceValue"].ToString()
                        };
                        SearchDataList.Add(srch);
                    }
                }
                con.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return SearchDataList;

        }

        public void LogResults(int SimulationId,string Status,int ImageId,string Errormessage,int Htmlattachment, string TestName)
        {
            SqlCommand cmd = new SqlCommand("LogResults", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@LogStatus", Status);
            cmd.Parameters.AddWithValue("@LogSimulationId", SimulationId);
            cmd.Parameters.AddWithValue("@LogErrorMessage", Errormessage);
            cmd.Parameters.AddWithValue("@LogImageId", ImageId);
            cmd.Parameters.AddWithValue("@LogHtmlAttachmentId", Htmlattachment);
            cmd.Parameters.AddWithValue("@TestName", TestName);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        #endregion
    }
}
