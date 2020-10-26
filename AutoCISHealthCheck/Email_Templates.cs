using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Collections;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using OpenPop.Pop3;
using OpenPop.Mime;

namespace AutoCISHealthCheck
{
    class Email_Templates
    {
        #region Declarations
        Pop3Client _client;
        protected readonly string connStr = ConfigurationManager.ConnectionStrings["MyConnectionString"].ToString();

        private string username = ConfigurationManager.AppSettings["Username"].ToString();
        private string password = ConfigurationManager.AppSettings["Password"].ToString();
        private int smtpport = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"].ToString());
        private string from = ConfigurationManager.AppSettings["FROM"].ToString();
        private string smtp_server = ConfigurationManager.AppSettings["smtp_server"].ToString();
        string admins = ConfigurationManager.AppSettings["Admin"].ToString();
        private MailMessage mail;
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
        private string Username
        {
            get { return username; }
            set { username = value; }
        }
        private string Password
        {
            get { return password; }
            set { password = value; }
        }
        private int SmtpPort
        {
            get { return smtpport; }
            set { smtpport = value; }
        }
        private string From
        {
            get { return from; }
            set { from = value; }
        }
        private string Smtp_server
        {
            get { return smtp_server; }
            set { smtp_server = value; }
        }
        private MailMessage Mail
        {
            get { return mail; }
            set { mail = value; }
        }
        private string Admins
        {
            get { return admins; }
            set { admins = value; }
        }
        #endregion

        #region Methods
        protected internal void SendEmailPrimary(string msg, string Recepient, string subject, bool attachment) // For only the IT Support
        {
            try
            {
                
                string support = ConfigurationManager.AppSettings["Admin"].ToString();
                string Test = ConfigurationManager.AppSettings["Recepient1"].ToString();
                var mainPath = ConfigurationManager.AppSettings["RootFolder"].ToString();
                
                Mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(Smtp_server);

                mail.Priority = MailPriority.High;

                Mail.From = new MailAddress(From);
                Mail.To.Add(Recepient);
                Mail.Subject = subject;
                Mail.IsBodyHtml = true;

                string Footer = "<br /> Regards : SPLUM";
                Mail.Body = msg + "<br />" + Footer;

                if (attachment)
                {
                    string path1 = mainPath + "index.html";
                    string path2 = mainPath + "dashboard.html";
                    Mail.Attachments.Add(new Attachment(path2));
                    Mail.Attachments.Add(new Attachment(path1));
                }
                
                

                SmtpServer.Port = SmtpPort;
                SmtpServer.Credentials = new System.Net.NetworkCredential(Username, Password);
                SmtpServer.EnableSsl = false;

                SmtpServer.Send(Mail);
                Mail.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        protected internal void SendReportMail(string Email, string newIndexpath, string newDashboardpath,string ReportImagepath ,string Subject, string Body)
        {
            
            Con = new SqlConnection(connStr);
            
            try
            {
                //Save_Retrieve_Info SRI = new Save_Retrieve_Info();
                string fileName = "Desktop.html";
                string FilePath = Path.Combine(Environment.CurrentDirectory, @"Data\", fileName);
                string Recepient1 = ConfigurationManager.AppSettings["Recepient1"].ToString();
                

                Mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(Smtp_server);
                Mail.Priority = MailPriority.High;
                Mail.From = new MailAddress(From);
                Mail.To.Add(Email);
                Mail.Subject = Subject;
                Mail.IsBodyHtml = true;
                string PrimaryBody = "Good Day Team<br /> <br />";
                Mail.Body = PrimaryBody + Body;
                string path1 = newIndexpath;
                string path2 = newDashboardpath;
                Mail.Attachments.Add(new Attachment(path2));
                Mail.Attachments.Add(new Attachment(path1));
                if (ReportImagepath != null)
                {
                    mail.Attachments.Add(new Attachment(ReportImagepath));
                }
                SmtpServer.Port = SmtpPort;
                SmtpServer.Credentials = new System.Net.NetworkCredential(Username, Password);
                SmtpServer.EnableSsl = false;
                SmtpServer.Send(Mail);
                Mail.Dispose();

            }
            catch (Exception ex)
            {
                Email_Templates Et = new Email_Templates();
                Et.SendEmailPrimary(ex.ToString(), ConfigurationManager.AppSettings["Admin"].ToString(), "Failed to Send Email", false);
                throw ex;
            }
        } //for only reports

        protected internal void DriverFailEmail(string DriverName, string ErrorMessage)
        {

            try
            {
                Mail = new MailMessage();
                Mail.Priority = MailPriority.High;
                Mail.From = new MailAddress(From);
                Mail.To.Add(Admins);
                Mail.Subject = "Web Driver Failure";
                Mail.IsBodyHtml = true;
                Mail.Body = "Good Day Admin <br />Please Note that the CRM Automation APP <b>WEB DRIVER(Browser failed)</b> !!.." + "<br /> *Browser Name : " + DriverName + "<br />  Error message below <br />" + ErrorMessage;
                SmtpClient SmtpServer = new SmtpClient(Smtp_server);

                SmtpServer.Port = SmtpPort;
                SmtpServer.Credentials = new System.Net.NetworkCredential(Username, Password);
                SmtpServer.EnableSsl = false;

                SmtpServer.Send(Mail);
                Mail.Dispose();

            }
            catch (Exception ex)
            {

                throw ex;
            }

            
        }

        public void Connect(string hostname, string username, string password, int port, bool isUseSsl)
        {
            try
            {
                _client = new Pop3Client();
                _client.Connect("pop.gmail.com", 995, true);
                _client.Authenticate("testingappsdw@gmail.com", "Dataworld@123");
                int Messages = _client.GetMessageCount();
                //_client.DeleteAllMessages();
                Message mes =  _client.GetMessage(78);
                var i = mes.MessagePart.MessageParts;
                
                _client.Disconnect();



                
            }
            catch (Exception e)
            {
                _client.Disconnect();
                throw e;
            }
            
        }
        public string FetchEmailPassword()
        {
            string Password = null;
            Pop3Client client = new Pop3Client();
            try
            {
                // The client disconnects from the server when being disposed
               
                    // Connect to the server
                    client.Connect("pop.gmail.com", 995, true);

                    // Authenticate ourselves towards the server
                    client.Authenticate("testingappsdw@gmail.com", "Dataworld@123");

                    // Get the number of messages in the inbox
                    int messageCount = client.GetMessageCount();
                    

                    // We want to download all messages
                    List<Message> allMessages = new List<Message>(messageCount);

                    // Messages are numbered in the interval: [1, messageCount]
                    // Ergo: message numbers are 1-based.
                    // Most servers give the latest message the highest number

                    allMessages.Add(client.GetMessage(messageCount));
                    Message Mymsg = allMessages[0];
                    
                    Password = FindPlainTextInMessage(allMessages[0]);
                    // Now return the fetched messages
                
                return Password;
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                //client.Disconnect();
                client.Dispose();
            }
        }
        public string FindPlainTextInMessage(Message message)
        {
            string Password = null;
            try
            {
                MessagePart plainText = message.FindFirstPlainTextVersion();
                if (plainText != null)
                {
                    string Test;
                    Test = plainText.GetBodyAsText();
                    // Save the plain text to a file, database or anything you like
                    //plainText.Save(new FileInfo("plainText.txt"));
                }
                else
                {
                    MessagePart Html = message.FindFirstHtmlVersion();
                    if (Html != null)
                    {
                        string text = Html.GetBodyAsText();

                        string toBeSearched = "Your password is : ";
                        //string code = text.Substring(text.IndexOf(toBeSearched) + toBeSearched.Length);

                        int ix = text.IndexOf(toBeSearched);
                        
                        string code = text.Substring(ix + toBeSearched.Length);
                        // do something here
                        Password = code.Substring(0, code.Length > 9 ? 9 : code.Length);
                        Password = String.Concat(Password.Where(c => !Char.IsWhiteSpace(c)));
                    }
                }
                return Password;
            }
            catch (Exception)
            {

                throw;
            }
            
        }
        public static string FindStringTakeX(string strValue, string findKey, int take, bool ignoreWhiteSpace = true)
        {
            int index = strValue.IndexOf(findKey) + findKey.Length;

            if (index >= 0)
            {
                if (ignoreWhiteSpace)
                {
                    while (strValue[index].ToString() == " ")
                    {
                        index++;
                    }
                }

                if (strValue.Length >= index + take)
                {
                    string result = strValue.Substring(index, take);

                    return result;
                }


            }

            return string.Empty;
        }

        #endregion
    }
}
