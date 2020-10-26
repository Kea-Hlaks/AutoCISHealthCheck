using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Collections;
using OpenQA.Selenium.Support.UI;
using System.Net.Http;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using OpenQA.Selenium.Interactions;
using System.Reflection.Emit;
using System.Globalization;
using System.Runtime.InteropServices;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.MachineLearning;
//using OpenCvSharp.ML;


namespace AutoCISHealthCheck
{
    class Simulation
    {
        #region Declarations
        public Status _status;
        IWebDriver Driver;
        public string LogSimulation;
        public int LogSimulationId;
        TestStatus logstatus = TestStatus.Passed;
        int logImageId = Convert.ToInt32(null);
        Retrieve_Save_Log_Info Rtrv = new Retrieve_Save_Log_Info();
        private ExtentReports _extent;
        private ExtentTest _testAlt;
        private string ImagePath;
        private readonly string localDocPath = ConfigurationManager.AppSettings["LocalDocPath"].ToString();
        private readonly int delays = Int32.Parse(ConfigurationManager.AppSettings["Thread"].ToString());
        private int Htmlattachment = 0;
        string fileExtension;
        private int documentID;
        private string fileName = "";
        private string Docpath;
        private Screenshot screenshot;
        private Document DCM;
        bool TestFail;
        private ExtentTest _test;
        private int sizeInBytes;
        private ArrayList Failures = new ArrayList();
        private int DocumentTypeId;
        private string ReportId = "";
        ArrayList ReportIdsAlt = new ArrayList();
        private string TestName;
        private string errorMessage = " ";
        private ExtentReports _extentAlt;
        private string newIndexpath;
        private DateTime logDate;
        private string newDashboardpath;
        private string UniversalImgPath;
        private string RequestNumber;
        private static readonly HttpClient client = new HttpClient();
        string GlobalProvince=null;
        string GlobalParcelType = null;
        string GlobalAministrativeDistr = null;
        int GlobalFarmNumber = 0;
        private bool FirstRequestMade = false;
        private string UniversalPassword;
        private string UniversalUsername;
        #endregion

        #region Properties
        protected ExtentTest _Test
        {
            get { return _test; }
            set { _test = value; }
        }
        protected Status Status
        {
            get { return _status; }
            set { _status = value; }
        }
        protected IWebDriver driver
        {
            get { return Driver; }
            set { Driver = value; }
        }
        protected int LogImageId
        {
            get { return logImageId; }
            set { logImageId = value; }
        }

        protected string LocalDocPath
        {
            get { return localDocPath; }
        }
        public int Delays
        {
            get { return delays; }
        }
        protected ExtentReports _Extent
        {
            get { return _extent; }
            set { _extent = value; }
        }
        protected ExtentReports _ExtentAlt
        {
            get { return _extentAlt; }
            set { _extentAlt = value; }
        }
        protected DateTime LogDate
        {
            get { return logDate; }
            set { logDate = value; }
        }
        protected ExtentTest _TestAlt
        {
            get { return _testAlt; }
            set { _testAlt = value; }
        }
        #endregion

        #region Methods
        public void BeforeTest(string SimulationName, string SimulationDescription)
        {
            TestFail = false;
            logstatus = TestStatus.Passed;
            _status = Status.Pass;
            try
            {
                _Test = _Extent.CreateTest(SimulationName, SimulationDescription);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        protected string Capture(IWebDriver driver, string ScreenShotName)
        {

            ImagePath = LocalDocPath;
            fileExtension = ".jpg";
            documentID = 0;
            fileName = ScreenShotName + fileExtension;
            Docpath = LocalDocPath + fileName;
            try
            {
                Thread.Sleep(Delays);
                ITakesScreenshot ts = (ITakesScreenshot)driver;
                screenshot = ts.GetScreenshot();
                //string pth = System.Reflection.Assembly.GetCallingAssembly().CodeBase;
                //var dir = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", "");
                //DirectoryInfo di = Directory.CreateDirectory(dir + "\\Defect_screenshots\\");
                screenshot.SaveAsFile(Docpath);


                FileInfo imageInfo = new FileInfo(Docpath);
                sizeInBytes = Convert.ToInt32(imageInfo.Length);
                CaptureDocument();
                EditFile();
            }
            catch (Exception e)
            {

                throw e;
            }

            return Docpath;
        }
        public void Start(string DocsPath)
        {
            try
            {
                string storage = null;
                if (DocsPath != null)
                {
                    storage = DocsPath;
                }
                else
                {
                    storage = ConfigurationManager.AppSettings["RootFolder"].ToString();
                }

                _Extent = new ExtentReports();
                var dir = storage.Replace(storage, storage);
                DirectoryInfo div = Directory.CreateDirectory(dir);
                foreach (FileInfo file in div.GetFiles())
                {
                    file.Delete();
                }

                var htmlReporter = new ExtentHtmlReporter(dir + ".html");
                _Extent.AddSystemInfo("Project Name :", "Auto CIS Health Check");
                _Extent.AddSystemInfo("Tester :", "Sello");
                _Extent.AddSystemInfo("Manager :", "Phila");
                htmlReporter.Config.EnableTimeline = false;
                _Extent.AttachReporter(htmlReporter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public TestStatus AfterTest(string Email, IWebDriver driver, string Username, string RoleType, string Usertype)
        {
            string ReportImagepath = "";
            try
            {

                var stacktrace = "" + TestContext.CurrentContext.Result.StackTrace + "";


                switch (logstatus)
                {
                    case TestStatus.Failed:
                        logstatus = TestStatus.Failed;
                        _status = Status.Fail;
                        string screenShotPath = Capture(driver, LogSimulation);
                        //_test.Log(logstatus, "Test ended with " + Logstatus + "-" + errorMessage);
                        _test.Log(_status, "Name Of Test : " + TestName);
                        _test.Log(_status, "Type of Simulation: " + LogSimulation);
                        _test.Log(_status, "Snapshot below: " + _test.AddScreenCaptureFromPath(fileName));
                        _test.Log(_status, "Username : " + Username);
                        _test.Log(_status, "User Type : " + Usertype);
                        _test.Log(_status, "Role Type : " + RoleType);
                        ReportImagepath = Docpath;
                        if (LogSimulationId == 2)
                        {
                            _test.Log(_status, "Category :  Aerial Photography - and Imagery Related Products ");
                            _test.Log(_status, "Sub Category :  Film Transparency copy ");
                            _test.Log(_status, "Province : " + GlobalProvince);
                            _test.Log(_status, "Parcel Description [Parcel Type] : " + GlobalParcelType);
                            _test.Log(_status, "Administrative District/Registration Division : " + GlobalAministrativeDistr);
                            _test.Log(_status, "Farm Number : " + GlobalFarmNumber);
                        }

                        if (LogSimulationId == 3)
                        {
                            _test.Log(_status, "Category :  Aerial Photography - and Imagery Related Products ");
                            _test.Log(_status, "Sub Category :  Film Transparency copy ");
                            _test.Log(_status, "Province : Gauteng ");
                            _test.Log(_status, "Parcel Description [Parcel Type] : Farm");
                            _test.Log(_status, "Administrative District/Registration Division : JR");
                            _test.Log(_status, "Farm Number : 324");
                        }
                        if (TestFail == true)
                        {
                            _testAlt.Log(_status, "Name Of Test : " + TestName);
                            _testAlt.Log(_status, "Type of Simulation: " + LogSimulation);
                            _testAlt.Log(_status, "Snapshot below :" + _testAlt.AddScreenCaptureFromPath(Docpath));
                            _testAlt.Log(_status, "Snapshot below: " + _testAlt.AddScreenCaptureFromPath(@"Downloads\" + fileName));
                            _test.Log(_status, "Username : " + Username);
                            _test.Log(_status, "User Type : " + Usertype);
                            _test.Log(_status, "Role Type : " + RoleType);
                            ReportImagepath = Docpath;
                        }
                        break;
                    case TestStatus.Skipped:
                        logstatus = TestStatus.Skipped;
                        _status = Status.Skip;
                        _test.Log(_status, "Test ended with " + logstatus + "-" + " " + errorMessage);
                        _test.Log(_status, "Name Of Test : " + TestName);
                        _test.Log(_status, "Type of Simulation: " + LogSimulation);
                        _test.Log(_status, "Username : " + Username);
                        _test.Log(_status, "User Type : " + Usertype);
                        _test.Log(_status, "Role Type : " + RoleType);
                        break;
                    default:
                        logstatus = TestStatus.Passed;
                        _status = Status.Pass;
                        _test.Log(_status, "Test ended with: " + logstatus);
                        _test.Log(_status, "Name Of Test : " + TestName);
                        _test.Log(_status, "Type of Simulation : " + LogSimulation);
                        _test.Log(_status, "Username : " + Username);
                        _test.Log(_status, "User Type : " + Usertype);
                        _test.Log(_status, "Role Type : " + RoleType);
                        //_test.Log(_status, "Output Below : " + _test.AddScreenCaptureFromPath(UniversalImgPath));
                        _test.Log(_status, "Output Below : " + _test.AddScreenCaptureFromPath(fileName));

                        if (LogSimulationId == 2)
                        {
                            _test.Log(_status, "Category :  Aerial Photography - and Imagery Related Products ");
                            _test.Log(_status, "Sub Category :  Film Transparency copy ");
                            _test.Log(_status, "Province : " + GlobalProvince);
                            _test.Log(_status, "Parcel Description [Parcel Type] : " + GlobalParcelType);
                            _test.Log(_status, "Administrative District/Registration Division : " + GlobalAministrativeDistr);
                            _test.Log(_status, "Farm Number : " + GlobalFarmNumber);
                        }

                        if (LogSimulationId == 4)
                        {
                            _test.Log(_status, "Category :  Aerial Photography - and Imagery Related Products ");
                            _test.Log(_status, "Sub Category :  Film Transparency copy ");
                            _test.Log(_status, "Province : Gauteng ");
                            _test.Log(_status, "Parcel Description [Parcel Type] : Farm");
                            _test.Log(_status, "Administrative District/Registration Division : JR");
                            _test.Log(_status, "Farm Number : 324");
                        }
                        break;
                }

                LogResultsToDb();
                if (TestFail == true)
                {
                    Email_Templates mails = new Email_Templates();
                    _ExtentAlt.Flush();
                    var RootFolder = ConfigurationManager.AppSettings["RootFolder"].ToString();
                    try
                    {
                        string Body = "Please note that the following test have failed : " + "<b>" + TestName + "</b>" + "<br />" + "</b>" + "<br />"
                            + "See attachments Above";
                        string Subject = "Test error  " + TestName;
                        Email_Templates Mails = new Email_Templates();

                        string currentIndexPath = RootFolder + "index.html";
                        string currentDashboardPath = RootFolder + "dashboard.html";


                        DocumentTypeId = 5;
                        fileExtension = ".html";
                        DCM = new Document();

                        fileName = "index.html";
                        FileInfo DocInfo = new FileInfo(Docpath);
                        sizeInBytes = Convert.ToInt32(DocInfo.Length);

                        ReportId = DCM.CaptureDoc(localDocPath, fileExtension, fileName, sizeInBytes, DocumentTypeId);
                        ReportIdsAlt.Add(ReportId);
                        string IdentityPrefix = ReportId + "_" + TestName + "_";
                        newIndexpath = localDocPath + IdentityPrefix + fileName;
                        File.Copy(currentIndexPath, newIndexpath);
                        fileName = ReportId + "_" + fileName;
                        EditReport();
                        string NewReportId = ReportId.ToString();
                        DCM.UpdateDoc(NewReportId, LogDate);

                        //For Document 2 (dashboard.html)
                        fileName = "dashboard.html";
                        FileInfo DocInfo2 = new FileInfo(Docpath);
                        sizeInBytes = Convert.ToInt32(DocInfo2.Length);
                        ReportId = DCM.CaptureDoc(localDocPath, fileExtension, fileName, sizeInBytes, DocumentTypeId);
                        ReportIdsAlt.Add(ReportId);
                        string IdentityPrefix2 = ReportId + "_" + TestName + "_";
                        newDashboardpath = localDocPath + IdentityPrefix2 + "_" + "dashboard.html";
                        File.Copy(currentDashboardPath, newDashboardpath);
                        fileName = ReportId + "_" + fileName;
                        EditReport();

                        string ReportsIDS = ReportIdsAlt[0] + "," + ReportIdsAlt[1];
                        DCM.UpdateDoc(ReportsIDS, LogDate);

                        Mails.SendReportMail(Email, newIndexpath, newDashboardpath, ReportImagepath, Subject, Body);//Sending error mail since the test failed
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return logstatus;
        }

        public TestStatus AfterTest_Gis(string Email, IWebDriver driver, string Username, string RoleType, string Usertype)
        {
            string ReportImagepath = "";
            try
            {

                var stacktrace = "" + TestContext.CurrentContext.Result.StackTrace + "";


                switch (logstatus)
                {
                    case TestStatus.Failed:
                        logstatus = TestStatus.Failed;
                        _status = Status.Fail;
                        string screenShotPath = Capture(driver, LogSimulation);
                        //_test.Log(logstatus, "Test ended with " + Logstatus + "-" + errorMessage);
                        _test.Log(_status, "Name Of Test : " + TestName);
                        _test.Log(_status, "Type of Simulation: " + LogSimulation);
                        _test.Log(_status, "Snapshot below: " + _test.AddScreenCaptureFromPath(fileName));
                        ReportImagepath = Docpath;
                        
                        if (TestFail == true)
                        {
                            _testAlt.Log(_status, "Name Of Test : " + TestName);
                            _testAlt.Log(_status, "Type of Simulation: " + LogSimulation);
                            _testAlt.Log(_status, "Snapshot below :" + _testAlt.AddScreenCaptureFromPath(Docpath));
                            _testAlt.Log(_status, "Snapshot below: " + _testAlt.AddScreenCaptureFromPath(@"Downloads\" + fileName));
                            ReportImagepath = Docpath;
                        }
                        break;
                    case TestStatus.Skipped:
                        logstatus = TestStatus.Skipped;
                        _status = Status.Skip;
                        _test.Log(_status, "Test ended with " + logstatus + "-" + " " + errorMessage);
                        _test.Log(_status, "Name Of Test : " + TestName);
                        _test.Log(_status, "Type of Simulation: " + LogSimulation);
                        break;
                    default:
                        logstatus = TestStatus.Passed;
                        _status = Status.Pass;
                        _test.Log(_status, "Test ended with: " + logstatus);
                        _test.Log(_status, "Name Of Test : " + TestName);
                        _test.Log(_status, "Type of Simulation : " + LogSimulation);
                        //_test.Log(_status, "Output Below : " + _test.AddScreenCaptureFromPath(UniversalImgPath));
                        _test.Log(_status, "Output Below : " + _test.AddScreenCaptureFromPath(fileName));
                        break;
                }

                LogResultsToDb();
                if (TestFail == true)
                {
                    Email_Templates mails = new Email_Templates();
                    _ExtentAlt.Flush();
                    var RootFolder = ConfigurationManager.AppSettings["RootFolder"].ToString();
                    try
                    {
                        string Body = "Please note that the following test have failed : " + "<b>" + TestName + "</b>" + "<br />" + "</b>" + "<br />"
                            + "See attachments Above";
                        string Subject = "Test error  " + TestName;
                        Email_Templates Mails = new Email_Templates();

                        string currentIndexPath = RootFolder + "index.html";
                        string currentDashboardPath = RootFolder + "dashboard.html";


                        DocumentTypeId = 5;
                        fileExtension = ".html";
                        DCM = new Document();

                        fileName = "index.html";
                        FileInfo DocInfo = new FileInfo(Docpath);
                        sizeInBytes = Convert.ToInt32(DocInfo.Length);

                        ReportId = DCM.CaptureDoc(localDocPath, fileExtension, fileName, sizeInBytes, DocumentTypeId);
                        ReportIdsAlt.Add(ReportId);
                        string IdentityPrefix = ReportId + "_" + TestName + "_";
                        newIndexpath = localDocPath + IdentityPrefix + fileName;
                        File.Copy(currentIndexPath, newIndexpath);
                        fileName = ReportId + "_" + fileName;
                        EditReport();
                        string NewReportId = ReportId.ToString();
                        DCM.UpdateDoc(NewReportId, LogDate);

                        //For Document 2 (dashboard.html)
                        fileName = "dashboard.html";
                        FileInfo DocInfo2 = new FileInfo(Docpath);
                        sizeInBytes = Convert.ToInt32(DocInfo2.Length);
                        ReportId = DCM.CaptureDoc(localDocPath, fileExtension, fileName, sizeInBytes, DocumentTypeId);
                        ReportIdsAlt.Add(ReportId);
                        string IdentityPrefix2 = ReportId + "_" + TestName + "_";
                        newDashboardpath = localDocPath + IdentityPrefix2 + "_" + "dashboard.html";
                        File.Copy(currentDashboardPath, newDashboardpath);
                        fileName = ReportId + "_" + fileName;
                        EditReport();

                        string ReportsIDS = ReportIdsAlt[0] + "," + ReportIdsAlt[1];
                        DCM.UpdateDoc(ReportsIDS, LogDate);

                        Mails.SendReportMail(Email, newIndexpath, newDashboardpath, ReportImagepath, Subject, Body);//Sending error mail since the test failed
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return logstatus;
        }

        protected void LogResultsToDb()
        {
            Rtrv = new Retrieve_Save_Log_Info();
            Rtrv.LogResults(LogSimulationId, logstatus.ToString(), LogImageId, errorMessage, Htmlattachment,TestName);
            errorMessage = "";
        }
        public void Output(IWebDriver driver)
        {
            string imgPath = Capture(driver, LogSimulation);
            UniversalImgPath = imgPath;
        }
        protected void CaptureDocument()
        {
            string NewDocId;
            DCM = new Document();
            NewDocId = DCM.CaptureDoc(LocalDocPath, fileExtension, fileName, sizeInBytes, DocumentTypeId);
            documentID = Convert.ToInt32(NewDocId);
            LogImageId = documentID;

            fileName = documentID + "_" + fileName;
            //Docpath = LocalDocPath + fileName;
            //if (DocumentTypeId == 0)
            //{
            //    screenshot.SaveAsFile(Docpath);
            //}
            AddDateWaterMark();

        }
        public void AddDateWaterMark()
        {
            //string imgPath = Docpath;
            string imgPath = Docpath;

            Bitmap ProcesImg = ConvertToBitmap(imgPath);
            string DateText = DateTime.Now.ToString();

            PointF firstLocation = new PointF(10f, 10f);
            PointF secondLocation = new PointF(10f, 50f);
            //Bitmap bitmap = (Bitmap)Image.FromFile(ProcesImg);//load the image file

            using (Graphics graphics = Graphics.FromImage(ProcesImg))
            {
                using (Font arialFont = new Font("Arial", 15))
                {
                    graphics.DrawString(DateText, arialFont, Brushes.White, firstLocation);
                }
            }
            fileName = localDocPath + documentID + "_CIS.bmp";

            ProcesImg.Save(fileName);//save the image file
            ProcesImg.Dispose();


        }
        public Bitmap ConvertToBitmap(string fileName)
        {
            Bitmap bitmap;
            using (Stream bmpStream = System.IO.File.Open(fileName, System.IO.FileMode.Open))
            {
                Image image = Image.FromStream(bmpStream);

                bitmap = new Bitmap(image);

            }
            return bitmap;
        }

        protected void EditFile()
        {
            DCM = new Document();
            DCM.ChabgeFileName(fileName, documentID);
        } //editing the screenshotname
        protected void EditReport()
        {
            DCM = new Document();
            DCM.ChabgeFileName(fileName, Convert.ToInt32(ReportId));
        } //editing the .html reportname
        public void StartAlt()
        {
            Failures.Clear();
            errorMessage = "";
            fileName = "";
            if (ReportIdsAlt.Count > 0)
            {
                ReportIdsAlt.Clear();
            }

            try
            {
                _ExtentAlt = new ExtentReports();
                var storage = ConfigurationManager.AppSettings["RootFolder"].ToString();
                var dir = storage.Replace(storage, storage);

                //var PermanentDir = localDocPath; // for permanent report
                DirectoryInfo div = Directory.CreateDirectory(dir);
                foreach (FileInfo file in div.GetFiles())
                {
                    file.Delete();
                }

                var htmlReporterAlt = new ExtentHtmlReporter(dir + ".html");
                _ExtentAlt.AddSystemInfo("Project Name :", "CIS Health Check");
                _ExtentAlt.AddSystemInfo("Tester :", "Sello");
                _ExtentAlt.AddSystemInfo("Manager :", "Phila");
                htmlReporterAlt.Config.EnableTimeline = false;
                _ExtentAlt.AttachReporter(htmlReporterAlt);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void CheckTest()
        {

            if (logstatus == TestStatus.Failed)
            {
                StartAlt();
                BeforeTestAlt();
                TestFail = true;
            }
        }
        public void BeforeTestAlt()
        {
            try
            {
                _TestAlt = _ExtentAlt.CreateTest(TestContext.CurrentContext.Test.Name);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void AfterConclusionReport() //killing and releasing the one time report that is being sent to admin once a day
        {
            try
            {
                _Extent.Flush();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region TestCases
        //Test Cases
        [Test]
        public void Login(IWebDriver driver, string Username, string Password, int SimulationId)
        {
            try
            {
                //1 is Id for role
                //2 is Id for User
                LogSimulation = "Login - Banker";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;
                _status = Status.Pass;
                driver.Url = "http://10.1.15.226/#/login";
                IWebElement UserType = ExplicitWait(driver,"/html/body/az-root/az-login/div/div/div/div/form/div[1]/div[2]/label");
                UserType.Click();
                driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[2]/input")).SendKeys(Username);
                driver.FindElement(By.XPath("//*[@id='show_hide_password']")).SendKeys(Password);
                driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[5]/button")).Click();
                Output(driver);

            }
            catch (Exception ex)
            {
                logstatus = TestStatus.Failed;
                errorMessage = ex.ToString();
            }
        }
        [Test]
        public void MakeSingleRequest(IWebDriver driver, int SimulationId, string ProvinceValue, string ParcelTypee, string AdministrativeDistrict, int FarmNumber)
        {
            try
            {
                LogSimulation = "Capture Single Request";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;
                _status = Status.Pass;

                
                if (FirstRequestMade == false)
                {
                    driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/az-menu/div/ul/li[3]/a")).Click();
                    driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/az-menu/div/ul/li[3]/ul/li[1]/a")).Click();
                    FirstRequestMade = true;
                }

                var SelectCategory = driver.FindElement(By.XPath("//*[@id='maindatadiv']/div[1]/div/div/div[2]/form/div[1]/select"));
                var selectElement = new SelectElement(SelectCategory);
                Thread.Sleep(delays);
                selectElement.SelectByValue("COST007=Aerial Photography - and Imagery Related Products");

                //Thread.Sleep(Delays);
                //var SelectSubCategory = driver.FindElement(By.XPath("//*[@id='maindatadiv']/div[1]/div/div/div[2]/form/div[2]/select"));
                var SelectSubCategory = ExplicitWait(driver, "//*[@id='maindatadiv']/div[1]/div/div/div[2]/form/div[2]/select");
                selectElement = new SelectElement(SelectSubCategory);
                selectElement.SelectByValue("SUBCOST021=Film Transparency copy");

                driver.FindElement(By.XPath("//*[@id='maindatadiv']/div[2]/div/div/div[2]/div/div/div[1]/ul/li[2]/a")).Click();
                
                //Thread.Sleep(Delays);
                //var province = driver.FindElement(By.XPath("//*[@id='pd']/form/div/div/select"));
                var province = ExplicitWait(driver, "//*[@id='pd']/form/div/div/select");
                var SelectProvince = new SelectElement(province);
                SelectProvince.SelectByValue(ProvinceValue);
                GlobalProvince = LogSimulation +" - "+ SelectProvince.SelectedOption.Text;
                

                //Thread.Sleep(delays);
                //var ParcelType = driver.FindElement(By.XPath("//*[@id='pd']/form/div[2]/div/select"));
                var ParcelType = ExplicitWait(driver, "//*[@id='pd']/form/div[2]/div/select");
                var SelectParcelType = new SelectElement(ParcelType);
                SelectParcelType.SelectByValue(ParcelTypee);
                GlobalParcelType = SelectParcelType.SelectedOption.Text;

                //Thread.Sleep(Delays);
                //var AdministrativeDistrictRegistration = driver.FindElement(By.XPath("//*[@id='pd']/form/div[3]/div/select"));
                
                var AdministrativeDistrictRegistration = ExplicitWait(driver, "//*[@id='pd']/form/div[3]/div/select");
                var SelectAdministrativeDistr = new SelectElement(AdministrativeDistrictRegistration);
                Thread.Sleep(2000);
                SelectAdministrativeDistr.SelectByValue(AdministrativeDistrict);
                GlobalAministrativeDistr = SelectAdministrativeDistr.SelectedOption.Text;


                //Thread.Sleep(Delays);
                //var FarmNum = driver.FindElement(By.XPath("//*[@id='pd']/form/div[4]/div/input"));
                var FarmNum = ExplicitWait(driver, "//*[@id='pd']/form/div[4]/div/input");
                FarmNum.SendKeys(FarmNumber.ToString());
                GlobalFarmNumber = Convert.ToInt32(FarmNumber.ToString());

                driver.FindElement(By.XPath("//*[@id='pd']/form/div[5]/div/button")).Click();
                //Thread.Sleep(Delays);
                //driver.FindElement(By.XPath("//*[@id='checkbox2']")).Click();
                var CheckboxTick = ExplicitWait(driver, "//*[@id='checkbox2']");
                CheckboxTick.Click();

                var submit = driver.FindElement(By.CssSelector("#maindatadiv > div:nth-child(5) > div > div > div:nth-child(3) > div:nth-child(3) > button"));
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("arguments[0].scrollIntoView();", submit);
                submit.Click();


                Thread.Sleep(2000);
                var FormatType = driver.FindElement(By.XPath("//*[@id='createRequest']/div/div/div[2]/form/div[1]/select"));
                selectElement = new SelectElement(FormatType);
                selectElement.SelectByValue("Electronic(DVD)");


                //Thread.Sleep(delays*2);
                //var DeliveryMethods = driver.FindElement(By.XPath("//*[@id='createRequest']/div/div/div[2]/form/div[2]/select"));
                var DeliveryMethods = ExplicitWait(driver, "//*[@id='createRequest']/div/div/div[2]/form/div[2]/select");
                WebDriverWait Wait4Delivery = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                Wait4Delivery.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(DeliveryMethods));
                Thread.Sleep(2000);
                selectElement = new SelectElement(DeliveryMethods);
                selectElement.SelectByValue("Collection");

                var submitRequest = driver.FindElement(By.CssSelector("#createRequest > div > div > div.modal-footer > button:nth-child(1)"));

                WebDriverWait wait4Submitbtn = new WebDriverWait(driver,TimeSpan.FromSeconds(5));
                wait4Submitbtn.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(submitRequest));
                js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("arguments[0].scrollIntoView();", submitRequest);
                submitRequest.Click();
                //Thread.Sleep(delays);
                Output(driver);

            }
            catch (Exception ex)
            {
                logstatus = TestStatus.Failed;
                errorMessage = ex.StackTrace + " " + ex.ToString();
            }
        }
        [Test]
        public void MakeBulkRequest(IWebDriver driver, int SimulationId)
        {
            try
            {
                

                LogSimulation = "Capture Bulk Request";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;
                _status = Status.Pass;

                //driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/az-menu/div/ul/li[3]/a")).Click();
                Thread.Sleep(delays * 3);
                var Menubtn = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/az-menu/div/ul/li[3]/ul/li[1]/a");
                Menubtn.Click();
                var SearchRequestsPanel = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/div/div[3]/az-search-requests/div[1]/div[2]/div/div/div[2]/div/div/div[1]/ul/li[3]/a");
                Thread.Sleep(2000);
                SearchRequestsPanel.Click();

                var Menu2 = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/az-menu/div/ul/li[3]/ul/li[1]/a");
                Menu2.Click();
                var SelectCategory = ExplicitWait(driver,"//*[@id='maindatadiv']/div[1]/div/div/div[2]/form/div[1]/select");
                var selectElement = new SelectElement(SelectCategory);
                Thread.Sleep(1000);
                selectElement.SelectByValue("COST007=Aerial Photography - and Imagery Related Products");
                
                var SelectSubCategory = ExplicitWait(driver,"//*[@id='maindatadiv']/div[1]/div/div/div[2]/form/div[2]/select");
                selectElement = new SelectElement(SelectSubCategory);
                Thread.Sleep(Delays);
                selectElement.SelectByValue("SUBCOST021=Film Transparency copy");
                
                var province = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/div/div[3]/az-search-requests/div[1]/div[2]/div/div/div[2]/div/div/div[2]/div[3]/form/div[1]/div/select");
                selectElement = new SelectElement(province);
                Thread.Sleep(delays);
                selectElement.SelectByValue("PRV003");
                
                var ParcelType = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/div/div[3]/az-search-requests/div[1]/div[2]/div/div/div[2]/div/div/div[2]/div[3]/form/div[2]/div/select");
                selectElement = new SelectElement(ParcelType);
                selectElement.SelectByValue("ParcelFarm");
                
                var AdministrativeDistrictRegistration = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/div/div[3]/az-search-requests/div[1]/div[2]/div/div/div[2]/div/div/div[2]/div[3]/form/div[3]/div/select");
                selectElement = new SelectElement(AdministrativeDistrictRegistration);
                selectElement.SelectByValue("JR");

                var FarmNum = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/div/div[3]/az-search-requests/div[1]/div[2]/div/div/div[2]/div/div/div[2]/div[3]/form/div[4]/div/input");
                FarmNum.SendKeys("324");
                Thread.Sleep(delays);
                var SearchBtn = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/div/div[3]/az-search-requests/div[1]/div[2]/div/div/div[2]/div/div/div[2]/div[3]/form/div[5]/div/button");
                SearchBtn.Click();
                try
                {
                    var CheckboxT = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/div/div[3]/az-search-requests/div[1]/div[3]/div/div/div[3]/data-table/div/div/table/tbody[3]/tr/td[4]/input");
                    CheckboxT.Click();
                }
                catch (Exception)
                {
                    driver.FindElement(By.XPath("//*[@id='checkbox2']")).Click();
                }
                
                var submit = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-search-requests/div[1]/div[4]/div/div/div[3]/div[3]/button");
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("arguments[0].scrollIntoView();", submit);
                submit.Click();
                var WarningBtn = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-search-requests/div[2]/div/div/div[3]/button[1]");
                WarningBtn.Click();
                var FormatType = ExplicitWait(driver,"//*[@id='createRequest']/div/div/div[2]/form/div[1]/select");
                selectElement = new SelectElement(FormatType);
                selectElement.SelectByValue("Electronic(DVD)");
                
                var DeliveryMethods = ExplicitWait(driver,"//*[@id='createRequest']/div/div/div[2]/form/div[2]/select");
                selectElement = new SelectElement(DeliveryMethods);
                selectElement.SelectByValue("Collection");

                var submitRequest = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/div/div[3]/az-search-requests/div[3]/div/div/div[3]/button[1]");
                js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("arguments[0].scrollIntoView();", submitRequest);
                submitRequest.Click();
                Thread.Sleep(2000);
                driver.Url = "http://10.1.15.226/#/cis/im/my-requests";
                Func<IWebDriver, IWebElement> waitForSearchElement = new Func<IWebDriver, IWebElement>((IWebDriver Web) =>
                {
                    Thread.Sleep(delays);
                    IWebElement ReqNum = driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-my-requests/div[1]/div/div/div/div/div/div/div[2]/div/data-table/div/div/table/tbody[1]/tr/th/div"));
                    RequestNumber = ReqNum.Text.ToString();
                    return ReqNum;
                });
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMinutes(2));
                IWebElement targetElement = wait.Until(waitForSearchElement);
                Output(driver);
                var LogOutBtn = ExplicitWait(driver,"/html/body/az-root/az-cis/az-navbar/nav/ul/li/div/button");
                
                if (LogOutBtn.Displayed)
                {
                    Thread.Sleep(delays);
                    LogOutBtn.Click();
                }
                else
                {
                    Thread.Sleep(delays * 3);
                    LogOutBtn.Click();
                }
                
            }
            catch (Exception ex)
            {
                logstatus = TestStatus.Failed;
                _status = Status.Fail;
                errorMessage = ex.StackTrace + " " + ex.ToString();
            }
        }
        [Test]
        public void Upload_pop(IWebDriver driver, string Username, string Password, int SimulationId)
        {
            try
            {
                driver.Url = "http://10.1.15.226/#/login";
                IWebElement UserType = ExplicitWait(driver,"/html/body/az-root/az-login/div/div/div/div/form/div[1]/div[2]/label");
                UserType.Click();
                driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[2]/input")).SendKeys(Username);
                driver.FindElement(By.XPath("//*[@id='show_hide_password']")).SendKeys(Password);
                driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[5]/button")).Click();
                Thread.Sleep(delays);
                IWebElement InboxBtn = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/az-menu/div/ul/li[3]/a");
                InboxBtn.Click();
                Thread.Sleep(delays);
                driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/az-menu/div/ul/li[3]/ul/li[2]/a")).Click();
                Thread.Sleep(delays);
                driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-my-requests/div[1]/div/div/div/div/div/div/div[2]/div/div/div[1]/div/input")).SendKeys(RequestNumber);
                
                Func<IWebDriver, IWebElement> waitForRequestElement = new Func<IWebDriver, IWebElement>((IWebDriver Web) =>
                {

                    IWebElement Req = driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-my-requests/div[1]/div/div/div/div/div/div/div[2]/div/data-table/div/div/table/tbody/tr/td[8]/button"));
                    Req.Click();
                    return Req;
                });
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMinutes(2));
                IWebElement targetElement = wait.Until(waitForRequestElement);


                
                Thread.Sleep(delays);
                driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-my-requests/div[1]/div/div/div/div/div/div/div[2]/div/data-table/div/div/table/tbody[1]/tr/td[8]/div[2]/ul/li[2]/a")).Click();
                Thread.Sleep(delays);
                driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-my-requests/div[8]/div/div/div[2]/div[1]/div/input[1]")).SendKeys(@"C:\Users\sello\Desktop\Desktop_copy\Uploading Documents\POP.pdf");
                Thread.Sleep(delays);
                driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-my-requests/div[8]/div/div/div[3]/button[1]")).Click();

                Thread.Sleep(2000);
                Output(driver);
                Thread.Sleep(delays);
                var LogOutBtn = ExplicitWait(driver, "/html/body/az-root/az-cis/az-navbar/nav/ul/li/div/button");
                LogOutBtn.Click();
            }
            catch (Exception ex)
            {
                logstatus = TestStatus.Failed;
                _status = Status.Fail;
                errorMessage = ex.StackTrace + " " + ex.ToString();
            }
        }
        [Test]
        public void Allocate_Task(IWebDriver driver, string Username, string Password, int SimulationId)
        {
            try
            {
                LogSimulation = "Allocate Task";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;
                _status = Status.Pass;

                driver.Url = "http://10.1.15.226/#/login";
                //driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[1]/div[1]/label")).Click();

                IWebElement RoleTypeRadiobtn = ExplicitWait(driver, "/html/body/az-root/az-login/div/div/div/div/form/div[1]/div[1]/label");
                RoleTypeRadiobtn.Click();
                driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[2]/input")).SendKeys(Username);
                driver.FindElement(By.XPath("//*[@id='show_hide_password']")).SendKeys(Password);
                driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[5]/button")).Click();
                IWebElement Inboxbtn = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/az-menu/div/ul/li[2]/a");
                Inboxbtn.Click();
                IWebElement SearchTaskInput = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[1]/div/div/div/div/div/div/div[2]/div[1]/div[1]/div[1]/div/input");
                SearchTaskInput.SendKeys(RequestNumber);
                Thread.Sleep(delays);
                IWebElement OpenTask = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[1]/div/div/div/div/div/div/div[2]/div[1]/data-table/div/div/table/tbody/tr/td[8]/button");
                OpenTask.Click();
                var submit_ = driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[6]/div/div/div[3]/button[1]"));
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("arguments[0].scrollIntoView();", submit_);
                Thread.Sleep(delays);
                IWebElement AssignToOfficerBtn = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[6]/div/div/div[2]/form/div[2]/div/ul/li[2]/a");
                AssignToOfficerBtn.Click();
                Thread.Sleep(Delays);
                var OfficerList = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[6]/div/div/div[2]/form/div[2]/div/div/div[2]/div/select");
                SelectElement selectElement = new SelectElement(OfficerList);
                selectElement.SelectByValue("USR00011=wonder@drdlr.gov.za");
                Thread.Sleep(delays);
                submit_.Click();
                Output(driver);
                //IWebElement logoutbtn = ExplicitWait(driver, "/html/body/az-root/az-cis/az-navbar/nav/ul/li[2]/div/button");
                //logoutbtn.Click();
            }
            catch (Exception ex)
            {
                logstatus = TestStatus.Failed;
                _status = Status.Fail;
                errorMessage = ex.StackTrace + " " + ex.ToString();
            }

        }
        [Test]
        public void ReAssign_Task(IWebDriver driver)
        {
            try
            {
                driver.Navigate().Refresh();

                IWebElement SearchTaskInput = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[1]/div/div/div/div/div/div/div[2]/div[1]/div[1]/div[1]/div/input");
                SearchTaskInput.SendKeys(RequestNumber);
                IWebElement OpenTask = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[1]/div/div/div/div/div/div/div[2]/div[1]/data-table/div/div/table/tbody/tr/td[8]/button");
                OpenTask.Click();
                Thread.Sleep(delays);
                IWebElement AssignToOfficerBtn = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[8]/div/div/div[2]/form/div[2]/div/ul/li[1]/a");
                AssignToOfficerBtn.Click();
                Thread.Sleep(delays);
                var submit_ = driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[8]/div/div/div[3]/button[1]"));
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("arguments[0].scrollIntoView();", submit_);
                IWebElement OfficerList = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[8]/div/div/div[2]/form/div[2]/div/div/div[1]/div/select");
                SelectElement selectElement = new SelectElement(OfficerList);
                selectElement.SelectByText("Stuart Mbokane");
                Thread.Sleep(delays);
                submit_.Click();
                Output(driver);
                IWebElement Logoutbtn = ExplicitWait(driver, "/html/body/az-root/az-cis/az-navbar/nav/ul/li[2]/div/button");
                Logoutbtn.Click();
            }
            catch (Exception e)
            {

                throw e;
            }                                                                                                                                                  
        }
        [Test]
        public void EffectTask(IWebDriver driver, string Username, string Password, int SimulationId)
        {
            try
            {
                LogSimulation = "Allocate Task";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;
                _status = Status.Pass;

                driver.Url = "http://10.1.15.226/#/login";
                Thread.Sleep(Delays);
                driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[1]/div[1]/label")).Click();
                driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[2]/input")).SendKeys(Username);
                driver.FindElement(By.XPath("//*[@id='show_hide_password']")).SendKeys(Password);
                driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[5]/button")).Click();
                Thread.Sleep(delays);
                IWebElement Rolepanel = ExplicitWait(driver,"/html/body/az-root/az-cis/az-navbar/nav/form/select");
                SelectElement selectElement = new SelectElement(Rolepanel);
                selectElement.SelectByValue("INROLE0076");
                Thread.Sleep(delays);
                driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/az-menu/div/ul/li[2]/a")).Click();
                Thread.Sleep(delays);
                IWebElement searchText = driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[1]/div/div/div/div/div/div/div[2]/div[1]/div[1]/div[1]/div/input"));
                searchText.SendKeys(RequestNumber);
                driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[1]/div/div/div/div/div/div/div[2]/div[1]/data-table/div/div/table/tbody[1]/tr/td[8]/button")).Click();
                Thread.Sleep(delays);
                driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[8]/div/div/div[2]/form/div[2]/div/ul/li[4]/a")).Click();
                Thread.Sleep(delays);
                driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[8]/div/div/div[2]/form/div[2]/div/div/div[4]/form/div/div/label")).Click();
                Thread.Sleep(delays);
                driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[8]/div/div/div[3]/button[1]")).Click(); //Payment Verivication
                Thread.Sleep(delays);
                searchText.Clear();
                searchText.SendKeys(RequestNumber);
                Thread.Sleep(Delays);
                driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[1]/div/div/div/div/div/div/div[2]/div[1]/data-table/div/div/table/tbody/tr/td[8]/button")).Click();
                Thread.Sleep(delays);
                driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[8]/div/div/div[2]/form/div[2]/div/ul/li[5]/a")).Click();
                Thread.Sleep(delays);
                IWebElement FileUpload = driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[8]/div/div/div[2]/form/div[2]/div/div/div[5]/div[1]/div/input[1]"));
                FileUpload.SendKeys(@"C:\Users\sello\Desktop\Desktop_copy\Uploading Documents\dispatch document.pdf");
                Thread.Sleep(delays);
                var submit = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[8]/div/div/div[3]/button[1]");
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("arguments[0].scrollIntoView();", submit);
                submit.Click();
                Output(driver);
                IWebElement Logoutbtn = ExplicitWait(driver, "/html/body/az-root/az-cis/az-navbar/nav/ul/li[2]/div/button");
                Logoutbtn.Click();

            }
            catch (Exception ex)
            {
                logstatus = TestStatus.Failed;
                _status = Status.Fail;
                errorMessage = ex.StackTrace + " " + ex.ToString();
            }

        }
        [Test]
        public void Scrutinize(IWebDriver driver, string Username, string Password, int SimulationId)
        {
            try
            {
                LogSimulation = "Scrutinize";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;
                _status = Status.Pass;
                driver.Url = "http://10.1.15.226/#/login";
                IWebElement Internaluserbtn = ExplicitWait(driver, "/html/body/az-root/az-login/div/div/div/div/form/div[1]/div[1]/label");
                Internaluserbtn.Click();
                driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[2]/input")).SendKeys(Username);
                driver.FindElement(By.XPath("//*[@id='show_hide_password']")).SendKeys(Password);
                driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[5]/button")).Click();

                try
                {
                    IWebElement InboxBtn = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/az-menu/div/ul/li[2]/a");
                    InboxBtn.Click();
                }
                catch (Exception)
                {
                    driver.Url = "http://10.1.15.226/#/cis/uam/inbox";
                }
                Thread.Sleep(delays);
                driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[1]/div/div/div/div/div/div/div[2]/div[1]/div[1]/div[2]/div/label")).Click();
                Thread.Sleep(delays);
                IWebElement searchbox = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[1]/div/div/div/div/div/div/div[2]/div[1]/div[1]/div[1]/div/input");
                searchbox.SendKeys(RequestNumber);
                Thread.Sleep(delays);
                driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[1]/div/div/div/div/div/div/div[2]/div[1]/data-table/div/div/table/tbody/tr/td[8]/button")).Click();
                Thread.Sleep(delays);
                //driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[8]/div/div/div[3]/button[1]")).Click();
                var Accept_Request = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[9]/div/div/div[3]/button[1]");
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("arguments[0].scrollIntoView();", Accept_Request);
                Accept_Request.Click();
                Thread.Sleep(delays);
                IWebElement Open_Taskbtn = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[1]/div/div/div/div/div/div/div[2]/div[1]/data-table/div/div/table/tbody[1]/tr/td[8]/button");
                Open_Taskbtn.Click();
                Thread.Sleep(2000);
                Output(driver);
                
                ////Dispatch
                //var Dispatch = driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[10]/div/div/div[3]/button[1]"));
                //IJavaScriptExecutor js_ = (IJavaScriptExecutor)driver;
                //js_.ExecuteScript("arguments[0].scrollIntoView();", Dispatch);
                //Dispatch.Click();
                //Output(driver);
                //Thread.Sleep(delays * 2);
                //driver.FindElement(By.XPath("/html/body/az-root/az-cis/az-navbar/nav/ul/li[2]/div/button")).Click();
            }
            catch (Exception ex)
            {
                logstatus = TestStatus.Failed;
                _status = Status.Fail;
                errorMessage = ex.StackTrace + " " + ex.ToString();
            }
        }
        [Test]
        public void Dispatch(IWebDriver driver, int SimulationId)
        {
            try
            {
                var Dispatch = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/div/div[3]/az-tasks/div[10]/div/div/div[3]/button[1]");
                IJavaScriptExecutor js_ = (IJavaScriptExecutor)driver;
                js_.ExecuteScript("arguments[0].scrollIntoView();", Dispatch);
                Dispatch.Click();
                Thread.Sleep(2000);
                Output(driver);
                IWebElement Logoutbtn = ExplicitWait(driver, "/html/body/az-root/az-cis/az-navbar/nav/ul/li[2]/div/button");
                Logoutbtn.Click();
            }
            catch (Exception ex)
            {
                logstatus = TestStatus.Failed;
                _status = Status.Fail;
                errorMessage = ex.StackTrace + " " + ex.ToString();
            }
        }
        [Test]
        public void LogQuery(IWebDriver driver, string Username, string Password, int SimulationId)
        {
            try
            {
                LogSimulation = "Log Query";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;
                _status = Status.Pass;

                driver.Url = "http://10.1.15.226/#/login";
                IWebElement UserType = ExplicitWait(driver,"/html/body/az-root/az-login/div/div/div/div/form/div[1]/div[2]/label");
                UserType.Click();
                driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[2]/input")).SendKeys(Username);
                driver.FindElement(By.XPath("//*[@id='show_hide_password']")).SendKeys(Password);
                driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[5]/button")).Click();
                Thread.Sleep(Delays);
                IWebElement QueriesBtn = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/az-menu/div/ul/li[5]/a");
                QueriesBtn.Click();
                Thread.Sleep(delays);
                driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/az-menu/div/ul/li[5]/ul/li[2]/a")).Click();

                IWebElement IssueType = driver.FindElement(By.XPath("//*[@id='ul']/form/div[1]/select"));
                var selectElement = new SelectElement(IssueType);
                selectElement.SelectByValue("Registration");
                driver.FindElement(By.XPath("//*[@id='ul']/form/div[5]/textarea")).SendKeys("Automation Testing");
                driver.FindElement(By.XPath("//*[@id='ul']/form/div[7]/button")).Click();
                Thread.Sleep(2000);
                Output(driver);
                IWebElement logoutbtn = ExplicitWait(driver,"/html/body/az-root/az-cis/az-navbar/nav/ul/li/div/button");
                if (logoutbtn.Displayed)
                {
                    Thread.Sleep(delays);
                    logoutbtn.Click();
                }
                else
                {
                    Thread.Sleep(delays * 2);
                    logoutbtn.Click();
                }
            }
            catch (Exception ex)
            {
                logstatus = TestStatus.Failed;
                _status = Status.Fail;
                errorMessage = ex.StackTrace + " " + ex.ToString();
            }
        }
        [Test]
        public void CancelRequest(IWebDriver driver, int SimulationId)
        {
            try
            {
                LogSimulation = "Cancel Request";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;
                _status = Status.Pass;
                //Cancel Request
                driver.Url = "http://10.1.15.226/#/cis/im/my-requests";
                Thread.Sleep(2000);
                var ActinonsBtn = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-my-requests/div[1]/div/div/div/div/div/div/div[2]/div/data-table/div/div/table/tbody[1]/tr/td[8]/button");
                if (ActinonsBtn.Displayed)
                {
                    ActinonsBtn.Click();
                }
                else
                {
                    Thread.Sleep(Delays * 2);
                    ActinonsBtn.Click();
                }

                var CancelBtn = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-my-requests/div[1]/div/div/div/div/div/div/div[2]/div/data-table/div/div/table/tbody[1]/tr/td[8]/div[2]/ul/li[3]/a");
                CancelBtn.Click();
                var CancelReason = ExplicitWait(driver, "//*[@id='cancelRequestModal']/div/div/div[2]/form/div/textarea");
                CancelReason.SendKeys("Testing");
                var CancelRequestBtn = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-my-requests/div[2]/div/div/div[3]/button[1]");
                CancelRequestBtn.Click();
                Thread.Sleep(2000);
                Output(driver);

            }
            catch (Exception ex)
            {
                logstatus = TestStatus.Failed;
                _status = Status.Fail;
                errorMessage = ex.StackTrace + " " + ex.ToString();
            }
        }
        [Test]
        public void Close_Query(IWebDriver driver, string Username, string Password, int SimulationId)
        {
            try
            {
                LogSimulation = "Close Query";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;
                _status = Status.Pass;
                driver.Url = "http://10.1.15.226/#/login";
                IWebElement InternalUser = ExplicitWait(driver, "/html/body/az-root/az-login/div/div/div/div/form/div[1]/div[1]/label");
                InternalUser.Click();
                driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[2]/input")).SendKeys(Username);
                driver.FindElement(By.XPath("//*[@id='show_hide_password']")).SendKeys(Password);
                driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[5]/button")).Click();
                
                IWebElement QueriesBtn = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/az-menu/div/ul/li[7]/a");
                QueriesBtn.Click();
                IWebElement ActionsBtn = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-issue-logs/div[1]/div/div/div/div/div/div[2]/div[2]/div/data-table/div/div/table/tbody[1]/tr/td[8]/button");
                try
                {
                    ActionsBtn.Click();
                }
                catch (Exception ex)
                {
                    try
                    {
                        Thread.Sleep(delays);
                        IWebElement ActBtn = driver.FindElement(By.CssSelector("tbody.data-table-row-wrapper:nth-child(3) > tr:nth-child(1) > td:nth-child(9) > button:nth-child(2)"));
                        ActBtn.Click();
                    }
                    catch (Exception exp)
                    {
                        throw exp;
                    }
                    
                }
                
                Thread.Sleep(delays);
                IWebElement MoreInfoBtn = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/div/div[3]/az-issue-logs/div[1]/div/div/div/div/div/div[2]/div[2]/div/data-table/div/div/table/tbody[1]/tr/td[8]/div[2]/ul/li[2]/a");
                MoreInfoBtn.Click();
                IWebElement Comment = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/div/div[3]/az-issue-logs/div[3]/div/div/div[2]/form/div/textarea");
                Comment.SendKeys("Test");
                IWebElement SubmitBtn = ExplicitWait(driver,"/html/body/az-root/az-cis/div/div/div/div[3]/az-issue-logs/div[3]/div/div/div[3]/button[1]");
                SubmitBtn.Click();
                Thread.Sleep(2000);
                Output(driver);
            }
            catch (Exception ex)
            {

                logstatus = TestStatus.Failed;
                errorMessage = ex.StackTrace + " " + ex.ToString();
            }
        }
        [Test]
        public void FeeCalculatorConfiguration(IWebDriver driver, string Username, string Password, int SimulationId)
        {
            try
            {
                CultureInfo cult = new CultureInfo("en-US");
                //driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[1]/div[1]/label")).Click();
                //driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[2]/input")).SendKeys(Username);
                //driver.FindElement(By.XPath("//*[@id='show_hide_password']")).SendKeys(Password);
                //driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[5]/button")).Click();
                //Remember to remove the code above on production since the session from log query will be used

                driver.Url = "http://10.1.15.226/#/cis/im/im-config";

                //CultureInfo cult = new CultureInfo("en-US");
                //string value = "1200.00";
                //var convertDecimal = decimal.Parse(value, cult);
                //convertDecimal++;

                IWebElement FeeCalculatorTab = ExplicitWait(driver, "//*[@id='maindatadiv']/div[1]/div/div/div/div/div/div[1]/ul/li[2]/a");
                FeeCalculatorTab.Click();

                IWebElement AddCategoryBtn = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-im-config/div/div[1]/div/div/div/div/div/div[2]/div[2]/div/div[1]/button");
                AddCategoryBtn.Click();
                
                //IWebElement CategoryList = ExplicitWait(driver,"//*[@id='fc']/div/div[3]/select");
                Random random = new Random();
                int RandomNum = random.Next(0, 1000);
                IWebElement NameInput = driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-im-config/div/div[2]/div/div/div[2]/div/form/div[1]/input"));
                NameInput.SendKeys("Test Name : " + RandomNum);

                IWebElement DescriptionInput = driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-im-config/div/div[2]/div/div/div[2]/div/form/div[2]/input"));

                DescriptionInput.SendKeys("Test Description : " + RandomNum);

                IWebElement Savebtn = driver.FindElement(By.XPath("/html/body/az-root/az-cis/div/div/div/div[3]/az-im-config/div/div[2]/div/div/div[3]/button[1]"));

                Savebtn.Click();
                Thread.Sleep(delays);
                IWebElement CategoryList = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-im-config/div/div[1]/div/div/div/div/div/div[2]/div[2]/div/div[3]/select");
                SelectElement selectElement = new SelectElement(CategoryList);

                //if (selectElement.Options.Contains)
                //{
                //Noteeeeeeeeeee continue
                //}

                //SelectElement SelectCategory = new SelectElement(CategoryList);
                //SelectCategory.SelectByValue("COST007=Aerial Photography - and Imagery Related Products");

                //IWebElement EditBtn = ExplicitWait(driver, "//*[@id='fc']/div/div[6]/table/tbody/tr[1]/td[6]/button");
                //EditBtn.Click();

                //IWebElement FixedRateAmt = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-im-config/div/div[4]/div/div/div[2]/div/form/div[2]/input");

                //string CalculatedRate = FixedRateAmt.GetAttribute("value");

                //var ConvertedAmount = decimal.Parse(CalculatedRate, cult);
                //ConvertedAmount++;

                //FixedRateAmt.Clear();
                //FixedRateAmt.SendKeys(ConvertedAmount.ToString());
                Thread.Sleep(2000);
                Output(driver);
            }
            catch (Exception ex)
            {
                _status = Status.Fail;
                logstatus = TestStatus.Failed;
                errorMessage = ex.ToString();
            }
        }
        [Test]
        public void Configure_Delivery_Method(IWebDriver driver , int SimulationId)
        {
            try
            {
                LogSimulation = "Adds new Delivery Method ";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;

                driver.Url = "http://10.1.15.226/#/cis/im/update-types";

                IWebElement Addbtn = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-update-types/div/div[1]/div/div/div/div/div/div[2]/div[1]/div/div[1]/button");
                Addbtn.Click();

                Random random = new Random();
                int RandomNum = random.Next(0, 1000);
                IWebElement NameInput = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-update-types/div/div[3]/div/div/div[2]/div/form/div[1]/input");
                NameInput.SendKeys("Test : " + RandomNum);
                IWebElement DescriptionInput = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-update-types/div/div[3]/div/div/div[2]/div/form/div[2]/input");
                DescriptionInput.SendKeys("Automation Test : " + RandomNum);

                IWebElement Savebtn = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-update-types/div/div[3]/div/div/div[3]/button[1]");
                Savebtn.Click();
                Thread.Sleep(2000);
                Output(driver);
            }
            catch (Exception ex)
            {
                _status = Status.Fail;
                logstatus = TestStatus.Failed;
                errorMessage = ex.ToString();
            }
        }
        //User Acccess management
        public void ValidateEmailAdress(IWebDriver driver, int SimulationId)
        {
            try
            {
                LogSimulation = "Checks if the Email fomat is correct";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;

                logstatus = TestStatus.Skipped;
                _status = Status.Skip;
            }
            catch (Exception ex)
            {
                _status = Status.Fail;
                logstatus = TestStatus.Failed;
                errorMessage = ex.ToString();
            }
        }

        public void ChangeEmailAdress(IWebDriver driver, int SimulationId)
        {
            try
            {
                LogSimulation = "Checks if the Email Adress can be successfully changed";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;

                logstatus = TestStatus.Skipped;
                _status = Status.Skip;
            }
            catch (Exception ex)
            {
                _status = Status.Fail;
                logstatus = TestStatus.Failed;
                errorMessage = ex.ToString();
            }
        }
        public void DeactivateAccount(IWebDriver driver, int SimulationId)
        {
            try
            {
                LogSimulation = "Checks if user Account can be successfully deactivated";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;

                logstatus = TestStatus.Skipped;
                _status = Status.Skip;
            }
            catch (Exception ex)
            {
                _status = Status.Fail;
                logstatus = TestStatus.Failed;
                errorMessage = ex.ToString();
            }
        }

        public void ConfigureWorkflow(IWebDriver driver, int SimulationId)
        {
            try
            {
                LogSimulation = "Checks the workflow can be successfully configured";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;

                logstatus = TestStatus.Skipped;
                _status = Status.Skip;

            }
            catch (Exception ex)
            {
                _status = Status.Fail;
                logstatus = TestStatus.Failed;
                errorMessage = ex.ToString();
            }
        }

        public void Configure_Fomat_Types(IWebDriver driver , int SimulationId)
        {
            try
            {
                LogSimulation = "Adds new Delivery Method (Module Management)";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;

                driver.Url = "http://10.1.15.226/#/cis/im/update-types";

                IWebElement FormatType = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-update-types/div/div[1]/div/div/div/div/div/div[1]/ul/li[2]/a"); //Format type
                FormatType.Click();

                IWebElement Addbtn = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-update-types/div/div[1]/div/div/div/div/div/div[2]/div[2]/div/div[1]/button");
                Addbtn.Click();

                Random random = new Random();
                int RandomNum = random.Next(0, 1000);
                IWebElement NameInput = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-update-types/div/div[4]/div/div/div[2]/div/form/div[1]/input");
                NameInput.SendKeys("Test : " + RandomNum);
                IWebElement DescriptionInput = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-update-types/div/div[4]/div/div/div[2]/div/form/div[2]/input");
                DescriptionInput.SendKeys("Automation Test : " + RandomNum);

                IWebElement Savebtn = ExplicitWait(driver, "/html/body/az-root/az-cis/div/div/div/div[3]/az-update-types/div/div[4]/div/div/div[3]/button[1]");
                Savebtn.Click();
                Thread.Sleep(2000);
                Output(driver);
            }
            catch (Exception ex)
            {
                _status = Status.Fail;
                logstatus = TestStatus.Failed;
                errorMessage = ex.ToString();
            }    
        }
        [Test]
        public void Query_Gis_Data(IWebDriver driver, int SimulationId)
        {
            try
            {
                LogSimulation = "Fetch Search Result Data";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;


                driver.Navigate().Refresh();
                IWebElement SearchBtn = ExplicitWait(driver,"/html/body/nav/div/div/ul/li/div[7]/button");
                if (SearchBtn != null)
                {
                    SearchBtn.Click();

                    IWebElement Province = driver.FindElement(By.XPath("//*[@id='ddlProvince']"));
                    SelectElement SelectProvince = new SelectElement(Province);
                    SelectProvince.SelectByValue("KZN");

                    IWebElement SearchCriteria = driver.FindElement(By.Id("ddlSCriteria"));
                    SelectElement SelectSearchCriteria = new SelectElement(SearchCriteria);
                    SelectSearchCriteria.SelectByValue("1");
                    IWebElement FarmName = driver.FindElement(By.Id("txtFarmSearch"));
                    FarmName.Click();
                    FarmName.SendKeys("westlands");
                    FarmName.SendKeys(Keys.Enter);

                    IWebElement ResultTable = ExplicitWait(driver, "/html/body/div[3]/div/div/div[2]/div/table");

                    if (ResultTable != null)
                    {
                        _status = Status.Pass;
                        logstatus = TestStatus.Passed;

                        driver.FindElement(By.XPath("/html/body/div[3]/div/div/div[2]/div/table/tbody/tr[1]/td[1]")).Click();
                    }
                    else
                    {
                        _status = Status.Fail;
                        logstatus = TestStatus.Failed;
                    }
                    Thread.Sleep(2000);
                    Output(driver);
                }
                else
                {
                    _status = Status.Fail;
                    logstatus = TestStatus.Failed;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [Test]
        public void DragElement(IWebDriver driver, int SimulationId)
        {
            try
            {
                LogSimulation = "Drag Results Box";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;

                var myElement = driver.FindElement(By.XPath("//*[@id='ctrlSearch']"));
                var ElementLocation = myElement.Location; //the default location of the 

                Actions actions = new Actions(driver);
                actions.DragAndDropToOffset(myElement, -500, 200).Build().Perform();

                var ElementLocationAfter = myElement.Location;

                if (ElementLocation != ElementLocationAfter)

                {
                    _status = Status.Pass;
                    logstatus = TestStatus.Passed;

                }
                else
                {
                    _status = Status.Fail;
                    logstatus = TestStatus.Failed;
                }
                Thread.Sleep(2000);
                Output(driver);
            }
            catch (Exception e)
            {

                throw e;
            }

            //driver.Url = "http://dwapps.co.za/DRDLRRM/CSG/infomap.html";

            //IWebElement SearchBtn = ExplicitWait(driver, "/html/body/nav/div/div/ul/li/div[7]/button");
            //if (SearchBtn != null)
            //{
            //    SearchBtn.Click();
            //}

            
        }
        [Test]
        public void MinimiseSearch(IWebDriver driver, int SimulationId)
        {
            try
            {
                LogSimulation = "Minimise Results Data";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;

                var SearchModal = driver.FindElement(By.XPath("//*[@id='ctrlSearch']"));
                var initialHeight = SearchModal.Size;
                IWebElement Minimusebtn = driver.FindElement(By.XPath("/html/body/div[3]/table/tbody/tr/td[2]"));
                Minimusebtn.Click();
                var HeightAfter = SearchModal.Size;

                if (initialHeight!=HeightAfter)
                {
                    _status = Status.Pass;
                    logstatus = TestStatus.Passed;
                    
                }
                else
                {
                    _status = Status.Fail;
                    logstatus = TestStatus.Failed;
                }
                Output(driver);
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        [Test]
        public void CloseSearch(IWebDriver driver, int SimulationId)
        {
            try
            {
                LogSimulation = "Close Result Data";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;

                var SearchModal = driver.FindElement(By.XPath("//*[@id='ctrlSearch']"));
                bool ModalVisibility = SearchModal.Displayed;

                IWebElement CloseBtn = driver.FindElement(By.XPath("/html/body/div[3]/table/tbody/tr/td[3]/img"));
                CloseBtn.Click();
                ModalVisibility = SearchModal.Displayed;
                if (!ModalVisibility)
                {
                    _status = Status.Pass;
                    logstatus = TestStatus.Passed;
                    
                }
                else
                {
                    _status = Status.Fail;
                    logstatus = TestStatus.Failed;
                }
                Output(driver);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [Test]
        public void CheckZoomInLevel(IWebDriver driver, int SimulationId)
        {
            try
            {
                LogSimulation = "Zoom in";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;

                driver.Url = "http://dwapps.co.za/DRDLRRM/CSG/infomap.html";
                Thread.Sleep(Delays * 2);
                IWebElement scale = ExplicitWait(driver,"//*[@id='dZoomLevel']");
                var InitialScaleValues = scale.Text;

                IWebElement zoomInBtn = driver.FindElement(By.XPath("/html/body/div[1]/div/div[3]/a[1]"));
                
                for (int i = 0; i < 4; i++)
                {
                    zoomInBtn.Click();
                }
                
                var ScaleSizeAfter = scale.Text;

                if (ScaleSizeAfter != InitialScaleValues)
                {
                    _status = Status.Pass;
                    logstatus = TestStatus.Passed;
                }
                else
                {
                    _status = Status.Fail;
                    logstatus = TestStatus.Failed;
                    errorMessage = "Failed to zoom out";
                }
                Output(driver);
            }
            catch (Exception e)
            {

                throw e;
            }

            
        }
        public void CheckZoomOutLevel(IWebDriver driver, int SimulationId)
        {
            try
            {
                LogSimulation = "Zoom Out";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;

                IWebElement scale = driver.FindElement(By.XPath("//*[@id='dZoomLevel']"));
                var InitialScaleValues = scale.Text;
                
                IWebElement zoomOutBtn = driver.FindElement(By.XPath("/html/body/div[1]/div/div[3]/a[2]"));
                
                for (int i = 0; i < 3; i++)
                {
                    zoomOutBtn.Click();
                }
                IWebElement Scaleaftr = driver.FindElement(By.XPath("//*[@id='dZoomLevel']"));

                var ScaleSizeAfter = Scaleaftr.Text;

                if (ScaleSizeAfter != InitialScaleValues)
                {
                    _status = Status.Pass;
                    logstatus = TestStatus.Passed;
                }
                else
                {
                    _status = Status.Fail;
                    logstatus = TestStatus.Failed;
                    errorMessage = "Failed to zoom out";
                }
                Output(driver);
            }
            catch (Exception e)
            {
                _status = Status.Fail;
                logstatus = TestStatus.Failed;
                errorMessage = e.ToString();
            }
        }
        [Test]
        public void DrawAPolygon(IWebDriver driver, int SimulationId)
        {
            
            try
            {
                LogSimulation = "Draw a Polygon";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;

                //IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                //js.ExecuteScript("var locationRio = {lat: -22.915, lng: -43.197};var map = new google.maps.Map(document.getElementById('map'), {zoom: 13,center: locationRio,gestureHandling: 'cooperative'");

                driver.FindElement(By.XPath("/html/body/nav/div/div/ul/li/div[8]/button")).Click();
                Thread.Sleep(2000);
                driver.FindElement(By.XPath("/html/body/div[2]/div/div[1]/button[2]")).Click();


                IWebElement map = driver.FindElement(By.XPath("//*[@id='OpenLayers_Layer_Vector_74_svgRoot']"));



                //Actions Zoom = new Actions(driver);
                //Zoom.MoveToElement(map).MoveByOffset(100, 10).DoubleClick();
                //for (int i = 0; i < 6; i++)
                //{
                //    Zoom.Click().DoubleClick().Build().Perform();
                //}


                //for (int i = 0; i < 10; i++)
                //{
                //    Actions pan = new Actions(driver);
                //    pan.ClickAndHold(map).MoveByOffset(200+i, 10);
                //    pan.Release(map).MoveByOffset(400, 20);
                //    IAction panTest = pan.Build();
                //    panTest.Perform();

                //}
                //Actions Builder = new Actions(driver);
                //IAction Draw = Builder.MoveToElement(map, 135, 15).Click().MoveByOffset(200, 60).Click().MoveByOffset(100, 70).DoubleClick().Build();
                //Draw.Perform();


                Actions vertex1 = new Actions(driver);
                vertex1.MoveToElement(map).MoveByOffset(100, 100).Click();
                IAction clickNextPoint = vertex1.Build();
                clickNextPoint.Perform();

                Actions vertex2 = new Actions(driver);
                vertex2.MoveToElement(map).MoveByOffset(10, 100).Click();
                IAction clickNextPoint2 = vertex2.Build();
                clickNextPoint2.Perform();

                Actions vertex3 = new Actions(driver);
                vertex3.MoveToElement(map).MoveByOffset(10, 10).Click();
                IAction clickNextPoint3 = vertex3.Build();
                clickNextPoint3.Perform();



                Actions vertex4 = new Actions(driver);
                //IWebElement Elem = driver.FindElement(By.Id("OpenLayers_Map_42_OpenLayers_ViewPort"));
                //IWebElement hope = driver.FindElement(By.Id("ctrlMAP"));
                //vertex4.DoubleClick(hope).Perform();

                //IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                //js.ExecuteScript("OpenLayers-2.13.1/OpenLayers.js");


                try
                {
                    Actions act = new Actions(driver);
                    IWebElement unknown_Element = driver.FindElement(By.XPath("//*[contains(@id,'OpenLayers_Geometry_Point')]"));
                    act.DoubleClick(unknown_Element).Build().Perform();

                    act.MoveToElement(unknown_Element).DoubleClick().Build().Perform();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error line 1553" + e);
                }



                vertex4.MoveToElement(map).MoveByOffset(100, 10).DoubleClick().Build().Perform();
                vertex4.Click().DoubleClick().Click().Build().Perform();
                Thread.Sleep(3000);
                vertex4.DoubleClick(map).Build().Perform();
                //driver.FindElement(By.XPath("/html/body/div[2]/div/div[3]/div[1]/label")).Click();


                vertex1.MoveToElement(map).MoveByOffset(100, 100).ClickAndHold();
                IAction clickNextPointt = vertex1.Build();
                clickNextPointt.Perform();

                vertex4.Click().DoubleClick().Build().Perform();


                Output(driver);

                logstatus = TestStatus.Skipped;
                _status = Status.Skip;
            }
            catch (Exception e)
            {
                logstatus = TestStatus.Failed;
                _status = Status.Fail;
                errorMessage = e.ToString();
            }
        }
        [Test]
        public void CheckInternetSpeed()
        {
            try
            {
                const string tempfile = "tempfile.tmp";
                System.Net.WebClient webClient = new System.Net.WebClient();

                Console.WriteLine("Downloading file....");

                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                webClient.DownloadFile("http://dl.google.com/googletalk/googletalk-setup.exe", tempfile);
                sw.Stop();

                FileInfo fileInfo = new FileInfo(tempfile);
                long speed = fileInfo.Length / sw.Elapsed.Seconds;

                Console.WriteLine("Download duration: {0}", sw.Elapsed);
                Console.WriteLine("File size: {0}", fileInfo.Length.ToString("N0"));
                Console.WriteLine("Speed: {0} bps ", speed.ToString("N0"));

                Console.WriteLine("Press any key to continue...");
            }
            catch (Exception)
            {

                throw;
            }
        }
        [Test]
        public void CheckBoundary(IWebDriver driver, int SimulationId)
        {
            try
            {
                LogSimulation = "checking if the map Boundary has loaded";
                TestName = LogSimulation;
                LogDate = DateTime.Now;
                LogSimulationId = SimulationId;

                _status = Status.Skip;
                logstatus = TestStatus.Skipped;

                Output(driver);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        [Test]
        public void AcesssSystem(IWebDriver driver, int SimulationId)
        {
            try
            {
                driver.Url = "http://10.1.15.226/#/login";

                IWebElement loginform = ExplicitWait(driver, "/html/body/az-root/az-login/div/div/div");
                if (!loginform.Displayed)
                {
                    logstatus = TestStatus.Failed;
                    _status = Status.Fail;
                    errorMessage = "Failed to access Home Page";
                }
            }
            catch (Exception e)
            {
                logstatus = TestStatus.Failed;
                _status = Status.Fail;
                errorMessage = e.ToString();
            }
        }
        [Test]
        public void AccessRegistration(IWebDriver driver, int SimulationId)
        {
            try
            {
                try
                {
                    IWebElement LogOutBtn = driver.FindElement(By.XPath("/html/body/az-root/az-cis/az-navbar/nav/ul/li/div/button"));
                    LogOutBtn.Click();
                }
                catch (Exception)
                {

                }

                driver.Url = "http://10.1.15.226/#/external-register";

                IWebElement NxtBtn = ExplicitWait(driver, "/html/body/az-root/az-external-register/div/div/div/div/div[5]/div/button");
                if (!NxtBtn.Displayed)
                {
                    logstatus = TestStatus.Failed;
                    _status = Status.Fail;
                    errorMessage = "Failed to access registration page";
                }
                Output(driver);
            }
            catch (Exception e)
            {

                logstatus = TestStatus.Failed;
                _status = Status.Fail;
                errorMessage = e.ToString();
            }
        }
        [Test]
        public void Register_External_User(IWebDriver driver , int SimulationId)
        {
            try
            {
                //IWebDriver _driver = new ChromeDriver();
                //driver = _driver;
                try
                {
                    IWebElement LogOutBtn = driver.FindElement(By.XPath("/html/body/az-root/az-cis/az-navbar/nav/ul/li/div/button"));
                    LogOutBtn.Click();
                }
                catch (Exception)
                {
                        
                }
                
                driver.Url = "http://10.1.15.226/#/external-register";
                IWebElement SelectProvince = ExplicitWait(driver,"/html/body/az-root/az-external-register/div/div/div/div/div[2]/form/div[3]/select");

                SelectElement Province = new SelectElement(SelectProvince);
                Province.SelectByValue("PRV003=Gauteng");

                IWebElement SelectRole = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[2]/form/div[5]/div/select"));
                SelectElement Role = new SelectElement(SelectRole);
                Role.SelectByValue("EX003=Banker=N");

                IWebElement ContctEmail = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[2]/form/div[6]/div/input"));

                Random R = new Random();
                int RandomNum = R.Next(50000);
                char randomChar = (char)R.Next('a', 'z');
                string _Email = "testingapps" + randomChar + RandomNum +"@gmail.com";

                ContctEmail.SendKeys(_Email);
                Thread.Sleep(5000);

                //object expct;
                //IJavaScriptExecutor jsw = (IJavaScriptExecutor)driver;
                //expct = jsw.ExecuteScript("document.querySelector('body > az - root > az - external - register > div > div > div > div > div:nth - child(6) > div > button').click()");

                IWebElement nxtStpBtn = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[5]/div/button"));

                nxtStpBtn.Click();

                //Next step form

                IWebElement title = ExplicitWait(driver,"/html/body/az-root/az-external-register/div/div/div/div/div[3]/form/div[1]/select");

                SelectElement SelectTitle = new SelectElement(title);
                SelectTitle.SelectByText("Mr");

                IWebElement FName = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[3]/form/div[2]/input"));
                FName.SendKeys("Sello");

                IWebElement LName = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[3]/form/div[3]/input"));
                LName.SendKeys("Seloane");

                IWebElement OrgType = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[3]/form/div[4]/select"));
                SelectElement SelectOrgType = new SelectElement(OrgType);
                SelectOrgType.SelectByValue("ORGTY6=General");

                IWebElement Sector = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[3]/form/div[5]/select"));
                SelectElement SelectSector = new SelectElement(Sector);
                SelectSector.SelectByValue("SEC44=Eletronics");

                IWebElement MobileNum = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[3]/form/div[6]/div/input"));
                MobileNum.SendKeys("0740778077");

                IWebElement TelNum = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[3]/form/div[7]/div/input"));
                TelNum.SendKeys("0119203521");

                IWebElement Adress_1 = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[3]/form/div[8]/input"));
                Adress_1.SendKeys("Test Adress1");

                IWebElement Adress_2 = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[3]/form/div[9]/input"));
                Adress_2.SendKeys("Test Adress2");

                IWebElement Adress_3 = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[3]/form/div[10]/input"));
                Adress_3.SendKeys("Test Adress3");

                IWebElement PostCode = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[3]/form/div[11]/input"));
                PostCode.SendKeys("1619");

                IWebElement NxtStpBtn = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[5]/div/button[2]"));
                NxtStpBtn.Click();
                
                //Step 3 More Details

                IWebElement CommType = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[4]/form/div[1]/select")); //Preferred Mode of Communication
                SelectElement SelectCommType = new SelectElement(CommType);
                SelectCommType.SelectByValue("COMM002=Mobile");

                IWebElement SecQue_1 = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[4]/form/div[3]/select"));
                SelectElement SelectSecQue_1 = new SelectElement(SecQue_1);
                SelectSecQue_1.SelectByValue("SECU001=What was your favorite place to visit as a child?");

                IWebElement Answer_1 = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[4]/form/div[4]/input"));
                Answer_1.SendKeys("Test");

                IWebElement SecQue_2 = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[4]/form/div[6]/select"));
                SelectElement SelectSecQue_2 = new SelectElement(SecQue_2);
                SelectSecQue_2.SelectByValue("SECU002=In what city were you born?");
                
                IWebElement Answer_2 = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[4]/form/div[7]/input"));
                Answer_2.SendKeys("Test");

                IWebElement SecQue_3 = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[4]/form/div[9]/select"));
                SelectElement SelectSecQue_3 = new SelectElement(SecQue_3);
                SelectSecQue_3.SelectByValue("SECU004=What is your mothers maiden name?");
                
                IWebElement Answer_3 = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[4]/form/div[10]/input"));
                Answer_3.SendKeys("Test");

                object returnedjs;
                IJavaScriptExecutor jsb = (IJavaScriptExecutor)driver;
                returnedjs = jsb.ExecuteScript("document.querySelector('#checkbox4').click()");
                Thread.Sleep(10000);
                IWebElement SubmitBtn = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[5]/div/button[2]"));
                SubmitBtn.Click();
                Thread.Sleep(Delays * 2);

                //Check if the Account is registered

                driver.Url = "http://10.1.15.226/#/external-register";
                //driver.Url = "http://10.1.15.226/#/external-register";
                IWebElement _SelectProvince = ExplicitWait(driver, "/html/body/az-root/az-external-register/div/div/div/div/div[2]/form/div[3]/select");

                SelectElement _Province = new SelectElement(_SelectProvince);
                _Province.SelectByValue("PRV003=Gauteng");

                IWebElement _SelectRole = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[2]/form/div[5]/div/select"));
                SelectElement _Role = new SelectElement(_SelectRole);
                _Role.SelectByValue("EX003=Banker=N");

                IWebElement _ContctEmail = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[2]/form/div[6]/div/input"));

                //Random R = new Random();
                //int RandomNum = R.Next(50000);
                //char randomChar = (char)R.Next('a', 'z');
                //string _Email = "testingapps" + randomChar + RandomNum + "@gmail.com";

                _ContctEmail.SendKeys(_Email);

                IWebElement _nxtStpBtn = driver.FindElement(By.XPath("/html/body/az-root/az-external-register/div/div/div/div/div[5]/div/button"));
                Thread.Sleep(delays);
                _nxtStpBtn.Click();

                bool displayed = CheckErrorMessage(driver, "/html/body/az-root/az-external-register/div/div/div/div/div[2]/form/div[6]/span");

                if (displayed)
                {
                    _status = Status.Pass;
                    logstatus = TestStatus.Passed;
                }
                else
                {
                    _status = Status.Fail;
                    logstatus = TestStatus.Failed;
                    errorMessage = "Failed to Register Account";
                }
                Output(driver);
            }
            catch (Exception ex)
            {
                _status = Status.Fail;
                logstatus = TestStatus.Failed;
                errorMessage = ex.ToString();
            }
        }
        [Test]
        public bool CheckErrorMessage(IWebDriver driver, string ExpectedElement)
        {
            bool displayed = false;
            IWebElement targetElement = null;
            try
            {
                
                try
                {
                    Func<IWebDriver, IWebElement> waitForSearchElement = new Func<IWebDriver, IWebElement>((IWebDriver Web) =>
                    {
                        IWebElement FoundElement = driver.FindElement(By.XPath(ExpectedElement));
                        return FoundElement;
                    });
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                    targetElement = wait.Until(waitForSearchElement);
                    Thread.Sleep(2000);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                if (targetElement != null)
                {
                    bool visible = targetElement.Displayed;
                    if (visible)
                    {
                        displayed = visible;
                    }
                }
                return displayed;
            }
            catch (Exception)
            {
                throw;
            }
        }
        [Test]
        public void PasswordRecovery(string Username,IWebDriver driver, int SimulationId)
        {
            try
            {
                string EmailAdr = Username;
                UniversalUsername = EmailAdr;
                
                driver.Url = "http://10.1.15.226/#/forgot-password";
                IWebElement Email = ExplicitWait(driver, "/html/body/az-root/az-forgot-password/div[1]/div/div/div/form/div[1]/input");
                Email.SendKeys(EmailAdr);
                IWebElement Submitbtn = driver.FindElement(By.XPath("/html/body/az-root/az-forgot-password/div[1]/div/div/div/form/div[2]/button[1]"));
                Submitbtn.Click();
                Thread.Sleep(delays);
                IWebElement Answer1 = driver.FindElement(By.XPath("/html/body/az-root/az-forgot-password/div[2]/div/div/div[2]/form/div[1]/div[2]/input"));
                IWebElement Answer2 = driver.FindElement(By.XPath("/html/body/az-root/az-forgot-password/div[2]/div/div/div[2]/form/div[2]/div[2]/input"));
                IWebElement Answer3 = driver.FindElement(By.XPath("/html/body/az-root/az-forgot-password/div[2]/div/div/div[2]/form/div[3]/div[2]/input"));

                Answer1.SendKeys("Test");
                Answer2.SendKeys("Test");
                Answer3.SendKeys("Test");
                Output(driver);
                //aftersubmitting

                //IWebElement SecSubmitbtn = driver.FindElement(By.XPath("/html/body/az-root/az-forgot-password/div[2]/div/div/div[2]/form/div[5]/button[1]"));
                //SecSubmitbtn.Click();

                //Email_Templates mails = new Email_Templates();
                //Thread.Sleep(Delays * 3);
                //Password = mails.FetchEmailPassword();

                //driver.Url = "http://10.1.15.226/#/login";
                //IWebElement UserType = ExplicitWait(driver, "/html/body/az-root/az-login/div/div/div/div/form/div[1]/div[2]/label");
                //UserType.Click();
                //driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[2]/input")).SendKeys(EmailAdr);
                //driver.FindElement(By.XPath("//*[@id='show_hide_password']")).SendKeys(Password);
                //driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[5]/button")).Click();

                
            }
            catch (Exception e)
            {
                logstatus = TestStatus.Failed;
                _status = Status.Fail;
                errorMessage = e.ToString();
            }
        }
        [Test]
        public void ValidateSecurityQues(IWebDriver driver, int SimulationId)
        {
            try
            {
                IWebElement SecSubmitbtn = driver.FindElement(By.XPath("/html/body/az-root/az-forgot-password/div[2]/div/div/div[2]/form/div[5]/button[1]"));
                SecSubmitbtn.Click();
                Output(driver);
            }
            catch (Exception e)
            {
                logstatus = TestStatus.Failed;
                _status = Status.Fail;
                errorMessage = e.ToString();
            }
        }
        public void CreateNewPassword(IWebDriver driver, int SimulationId)
        {
            try
            {
                IWebElement NewPass = ExplicitWait(driver, "//*[@id='show_hide_password1']");
                IWebElement ConfirmPass = driver.FindElement(By.XPath("//*[@id='show_hide_password2']"));

                Random random = new Random();
                const string chars = "ABcDEfGHIJkLmNOpQrSTUVwXYz0123456789";
                string PassKey = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray()) + "@123";
                UniversalPassword = PassKey;
                NewPass.SendKeys(PassKey);
                ConfirmPass.SendKeys(PassKey);

                IWebElement ResetPassbtn = driver.FindElement(By.XPath("/html/body/az-root/az-reset-password/div/div/div/div/form/div[5]/button"));
                ResetPassbtn.Click();

                IWebElement UserType = ExplicitWait(driver, "/html/body/az-root/az-login/div/div/div/div/form/div[1]/div[2]/label");
                UserType.Click();
                driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[2]/input")).SendKeys(UniversalUsername);
                driver.FindElement(By.XPath("//*[@id='show_hide_password']")).SendKeys(PassKey);
                driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[5]/button")).Click();

                IWebElement Logoutbtn = ExplicitWait(driver, "/html/body/az-root/az-cis/az-navbar/nav/ul/li/div/button");//we use logout button to assert if the account was successfully created and logged in 
                if (!Logoutbtn.Displayed)
                {
                    logstatus = TestStatus.Failed;
                    _status = Status.Fail;
                    errorMessage = "Failed to reset the password";
                }
                Output(driver);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void ReceiveOtp(IWebDriver driver, int SimulationId)//Creating a new password after logging in to the system with the One Time password
        {
            try
            {
                //Default 
                logstatus = TestStatus.Failed;
                _status = Status.Fail;
                errorMessage = "No otp received";
            }
            catch (Exception e)
            {
                logstatus = TestStatus.Failed;
                _status = Status.Fail;
                errorMessage = e.ToString();
            }
        }

        [Test]
        public void ChangePassword(IWebDriver driver,int SimulationId)//The user should be able to capture all the attributes for the password and save the new password
        {
            try
            {
                try
                {
                    IWebElement LogOutBtn = driver.FindElement(By.XPath("/html/body/az-root/az-cis/az-navbar/nav/ul/li/div/button"));
                    LogOutBtn.Click();
                }
                catch (Exception)
                {

                }
                driver.Url = "http://10.1.15.226/#/login";
                IWebElement UserType = ExplicitWait(driver, "/html/body/az-root/az-login/div/div/div/div/form/div[1]/div[2]/label");
                UserType.Click();
                driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[2]/input")).SendKeys(UniversalUsername);
                driver.FindElement(By.XPath("//*[@id='show_hide_password']")).SendKeys(UniversalPassword);
                driver.FindElement(By.XPath("/html/body/az-root/az-login/div/div/div/div/form/div[5]/button")).Click();

                driver.Url = "http://10.1.15.226/#/cis/uam/profile";
                IWebElement UpdatePasswordbtn = ExplicitWait(driver, "//*[@id='maindatadiv']/div[1]/div/div/div/div/div/div[1]/ul/li[3]/a");
                UpdatePasswordbtn.Click();

                IWebElement Oldpassword = ExplicitWait(driver, "//*[@id='sections']/form/div[1]/input");
                IWebElement PassWord = driver.FindElement(By.XPath("//*[@id='sections']/form/div[3]/input"));
                IWebElement ConfirmPass = driver.FindElement(By.XPath("//*[@id='sections']/form/div[5]/input"));

                Random random = new Random();
                const string chars = "ABcDEfGHIJkLmNOpQrSTUVwXYz0123456789";
                string PassKey = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray()) + "@123";
                

                Oldpassword.SendKeys(UniversalPassword);
                PassWord.SendKeys(PassKey);
                ConfirmPass.SendKeys(PassKey);
                UniversalPassword = PassKey;

                IWebElement Savebtn = driver.FindElement(By.XPath("//*[@id='sections']/form/div[8]/button"));
                Savebtn.Click();
                IWebElement MessageTost = driver.FindElement(By.XPath("//*[@id='toast - container']"));
                if (MessageTost.Displayed)
                {

                }
            }
            catch (Exception e)
            {
                logstatus = TestStatus.Failed;
                _status = Status.Fail;
                errorMessage = e.ToString();
            }
        }
        [Test]
        public void ReRun(IWebDriver driver)
        {
            
            try
            {
                object returnedjs;
                IJavaScriptExecutor jsb = (IJavaScriptExecutor)driver;
                returnedjs = jsb.ExecuteScript("document.querySelector('#checkbox4').click()");

                IWebElement radbtn = driver.FindElement(By.CssSelector("#checkbox4"));
                radbtn.Click();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IWebElement ExplicitWait(IWebDriver driver,string ExpectedElement)
        { 
            IWebElement targetElement = null;
            try
            {
                Func<IWebDriver, IWebElement> waitForSearchElement = new Func<IWebDriver, IWebElement>((IWebDriver Web) =>
                {
                    IWebElement FoundElement = driver.FindElement(By.XPath(ExpectedElement));
                    return FoundElement;
                });
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMinutes(2));
                targetElement = wait.Until(waitForSearchElement);
                Thread.Sleep(2000);
            }
            catch (Exception e)
            {

                throw e;
            }
            return targetElement;
        } //Wait for the element specified by the xpath parameter

        public void ReadImg()
        {
            try
            {
                string imagePath = "C:\\Users\\sello\\source\repos\\AutoCISHealthCheck\\PythonApplication1\\CapturedScreenShots\\CapturedOutput.png";

            }
            catch (Exception)
            {

                throw;
            }
        }



        public bool CompareBitmapsFast(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1 == null || bmp2 == null)
                return false;
            if (Equals(bmp1, bmp2))
                return true;
            if (!bmp1.Size.Equals(bmp2.Size) || !bmp1.PixelFormat.Equals(bmp2.PixelFormat))
                return false;

            int bytes = bmp1.Width * bmp1.Height * (Image.GetPixelFormatSize(bmp1.PixelFormat) / 8);

            bool result = true;
            byte[] b1bytes = new byte[bytes];
            byte[] b2bytes = new byte[bytes];

            BitmapData bitmapData1 = bmp1.LockBits(new Rectangle(0, 0, bmp1.Width, bmp1.Height), ImageLockMode.ReadOnly, bmp1.PixelFormat);
            BitmapData bitmapData2 = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadOnly, bmp2.PixelFormat);

            Marshal.Copy(bitmapData1.Scan0, b1bytes, 0, bytes);
            Marshal.Copy(bitmapData2.Scan0, b2bytes, 0, bytes);

            for (int n = 0; n <= bytes - 1; n++)
            {
                if (b1bytes[n] != b2bytes[n])
                {
                    result = false;
                    break;
                }
            }

            bmp1.UnlockBits(bitmapData1);
            bmp2.UnlockBits(bitmapData2);

            return result;
        }



        #endregion
    }
}