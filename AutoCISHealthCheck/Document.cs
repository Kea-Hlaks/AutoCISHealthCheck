using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCISHealthCheck
{
    class Document
    {
        #region Declarations
        protected readonly string connStr = ConfigurationManager.ConnectionStrings["MyConnectionString"].ToString();
        private SqlConnection con;
        #endregion

        #region Properties
        protected string ConnStr
        {
            get { return connStr; }
        }
        protected SqlConnection Con
        {
            get { return con; }
            set { con = value; }
        }
        #endregion

        #region Methods
        public string  CaptureDoc(string localDocPath, string FileExtension, string fileName,long sizeInBytes, int DocumentTypeId)
        {
            int documentID = 0;
            con = new SqlConnection(connStr);
            try
            {
                SqlCommand cmd = new SqlCommand("Upload_Document", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlParameter pSQL = cmd.Parameters.Add("@DocumentID", SqlDbType.Int);
                pSQL.Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@DocPath", localDocPath);
                cmd.Parameters.AddWithValue("@Extension", FileExtension);
                cmd.Parameters.AddWithValue("@FileName", fileName);
                cmd.Parameters.AddWithValue("@Size", sizeInBytes);
                cmd.Parameters.AddWithValue("@DocumentTypeID", DocumentTypeId);
                con.Open();
                cmd.ExecuteNonQuery();
                documentID = Convert.ToInt32(cmd.Parameters["@DocumentID"].Value);
                
            }
            catch (Exception Ex)
            {

                    throw Ex;
            }
            finally
            {
                con.Close();
            }

            return  documentID.ToString();
            

        }

        protected internal void ChabgeFileName(string fileName, int documentID)
        {
            try
            {

                SqlConnection cons = new SqlConnection(connStr);
                cons.Open();
                SqlCommand cmds = new SqlCommand("ChangeFileName", cons)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmds.Parameters.AddWithValue("@NewFileName", fileName);
                cmds.Parameters.AddWithValue("@DocId", documentID);
                int rows = cmds.ExecuteNonQuery();
                cons.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected internal void UpdateDoc(string ReportId,DateTime Date)
        {
            try
            {
                con = new SqlConnection(connStr);
                SqlCommand cmd = new SqlCommand("update Logs set HtmlAttachmentId = @ReportId where Date=@Date", con);
                cmd.Parameters.AddWithValue("@ReportId", ReportId);
                cmd.Parameters.AddWithValue("@Date", Date);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion
    }
}
