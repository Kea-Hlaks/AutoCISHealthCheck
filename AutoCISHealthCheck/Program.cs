using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using NUnit.Framework.Interfaces;
using System.Collections;
using System.Configuration;
using System.Globalization;
using System.Drawing;
using OpenCvSharp;

namespace AutoCISHealthCheck
{
    class Program
    {
        #region Declarations
        Creds crd = new Creds();
        ArrayList credList = new ArrayList();
        Retrieve_Save_Log_Info Rtrv = new Retrieve_Save_Log_Info();
        IWebDriver driver = new FirefoxDriver();
        string Admin = ConfigurationManager.AppSettings["Recepient1"].ToString();
        int SimulationId = 0;
        Simulation sim = new Simulation();
        TestStatus finalStatus;
        string testname = "Login";
        string testDescription = "Checking if the database server is up";

        #endregion

        #region Methods 
        static void Main(string[] args)
        {
            //var path1 = "C:\\Users\\sello\\source\\repos\\AutoCISHealthCheck\\AutoCISHealthCheck\\Images\\CapturedOutput.png";
            //var path2 = "C:\\Users\\sello\\source\\repos\\AutoCISHealthCheck\\AutoCISHealthCheck\\Images\\5579_CIS.bmp";
            //double Tolerance = 0.90;
            //Mat Captured = Cv2.ImRead(path1);
            //Mat Expected = Cv2.ImRead(path2);
            //SSIMResult result = SSIMResult.getMSSIM(Captured, Expected);
            //double finalResult = double.Parse(result.score.ToString("0.00"));
            //if (finalResult > Tolerance)
            //{

            //}

            Program prm = new Program();
            
            prm.initiate();
           
        }
        public void initiate()
        {
            
            try
            {   
                SimulationId = 1;

                // serparate Clause for CSG
                try
                {
                    credList = Rtrv.getCredentials(2, 1);
                    for (int i = 0; i < credList.Count; i++)
                    {
                        crd = (Creds)credList[i];
                    }
                    sim.Start(null);
                    sim.BeforeTest(testname, testDescription);
                    sim.Login(driver, crd.Username, crd.Password, SimulationId);
                    sim.CheckTest();
                    finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                    if (finalStatus == TestStatus.Passed)
                    {
                        testname = "Capture single Request";
                        testDescription = "checking if the request goes through";
                        SimulationId++;//2
                        Retrieve_Save_Log_Info SRI = new Retrieve_Save_Log_Info();
                        ArrayList SearchData = SRI.GetSearchData();
                        Search_Data srch = new Search_Data();



                        for (int i = 0; i < SearchData.Count; i++)
                        {
                            srch = (Search_Data)SearchData[i];

                            sim.BeforeTest(testname + " - " + srch.Province, testDescription);
                            sim.MakeSingleRequest(driver, SimulationId, srch.ProvinceValue, srch.ParcelType, srch.AdministrativeDistrict, srch.FarmNumber);
                            sim.CheckTest();
                            finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);
                        }


                        if (finalStatus == TestStatus.Passed)
                        {
                            testname = "Cancel a Request";
                            testDescription = "Checking if the request can be successfully cancelled";
                            SimulationId++;//3

                            sim.BeforeTest(testname, testDescription);
                            sim.CancelRequest(driver, SimulationId);
                            sim.CheckTest();
                            finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);
                        }

                        //Bulk Req and sub processes
                        if (finalStatus == TestStatus.Passed)
                        {
                            testname = "Capture Bulk Request";
                            testDescription = "checking if the request goes through";
                            SimulationId++;//4

                            sim.BeforeTest(testname, testDescription);
                            sim.MakeBulkRequest(driver, SimulationId);
                            sim.CheckTest();
                            finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                            if (finalStatus == TestStatus.Passed)
                            {
                                testname = "Allocate Task";
                                testDescription = "Checking if the Information Manager can Allocate the Task to Officer";
                                SimulationId++;//5

                                credList = Rtrv.getCredentials(1, 2);
                                for (int i = 0; i < credList.Count; i++)
                                {
                                    crd = (Creds)credList[i];
                                }

                                sim.BeforeTest(testname, testDescription);
                                sim.Allocate_Task(driver, crd.Username, crd.Password, SimulationId);
                                sim.CheckTest();
                                finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                                if (finalStatus == TestStatus.Passed)
                                {
                                    testname = "Reassign Task";
                                    testDescription = "Checking if the Information Manager can Reallocate the Task to a different Officer";
                                    SimulationId++;//6

                                    sim.BeforeTest(testname, testDescription);
                                    sim.ReAssign_Task(driver);
                                    sim.CheckTest();
                                    finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);
                                    if (finalStatus == TestStatus.Passed)
                                    {


                                        testname = "Effect Task";
                                        testDescription = "Checking if the Information Officer can Effect the task";
                                        SimulationId++;//6

                                        credList = Rtrv.getCredentials(1, 3);
                                        for (int i = 0; i < credList.Count; i++)
                                        {
                                            crd = (Creds)credList[i];
                                        }

                                        sim.BeforeTest(testname, testDescription);
                                        sim.EffectTask(driver, crd.Username, crd.Password, SimulationId);
                                        sim.CheckTest();
                                        finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                                        if (finalStatus == TestStatus.Passed)
                                        {

                                            credList = Rtrv.getCredentials(2, 1);
                                            for (int i = 0; i < credList.Count; i++)
                                            {
                                                crd = (Creds)credList[i];
                                            }

                                            testname = "Upload Proof Of Payment";
                                            testDescription = "checking if the POP can be successfully uploaded";
                                            SimulationId++;//7

                                            sim.BeforeTest(testname, testDescription);
                                            sim.Upload_pop(driver, crd.Username, crd.Password, SimulationId);
                                            sim.CheckTest();
                                            finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);
                                        }


                                        if (finalStatus == TestStatus.Passed)
                                        {
                                            testname = "Scrutinize";
                                            testDescription = "Checking if the Task can be sucessfully Scrutinized";
                                            SimulationId++;//8

                                            credList = Rtrv.getCredentials(1, 4);
                                            for (int i = 0; i < credList.Count; i++)
                                            {
                                                crd = (Creds)credList[i];
                                            }

                                            sim.BeforeTest(testname, testDescription);
                                            sim.Scrutinize(driver, crd.Username, crd.Password, SimulationId);
                                            sim.CheckTest();
                                            finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                                            if (finalStatus == TestStatus.Passed)
                                            {
                                                testname = "Dispatch";
                                                testDescription = "Checking if the Task can be sucessfully Dispatched";
                                                SimulationId++;//9

                                                sim.BeforeTest(testname, testDescription);
                                                sim.Dispatch(driver, SimulationId);
                                                sim.CheckTest();
                                                finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                                                if (finalStatus == TestStatus.Passed)
                                                {
                                                    testname = "Log a Query";
                                                    testDescription = "Checking if the Query can be successfully Logged";
                                                    SimulationId++;//10

                                                    credList = Rtrv.getCredentials(2, 1);
                                                    for (int i = 0; i < credList.Count; i++)
                                                    {
                                                        crd = (Creds)credList[i];
                                                    }

                                                    sim.BeforeTest(testname, testDescription);
                                                    sim.LogQuery(driver, crd.Username, crd.Password, SimulationId);
                                                    sim.CheckTest();
                                                    finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                                                    if (finalStatus == TestStatus.Passed)
                                                    {
                                                        testname = "Close Query";
                                                        testDescription = "Checking if the Query can be successfully be closed";
                                                        SimulationId++;//11

                                                        credList = Rtrv.getCredentials(1, 4);
                                                        for (int i = 0; i < credList.Count; i++)
                                                        {
                                                            crd = (Creds)credList[i];
                                                        }

                                                        sim.BeforeTest(testname, testDescription);
                                                        sim.Close_Query(driver, crd.Username, crd.Password, SimulationId);
                                                        sim.CheckTest();
                                                        finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                                                        if (finalStatus == TestStatus.Passed)
                                                        {
                                                            testname = "Fee Calculator Config";
                                                            testDescription = "Checking if the Fixed Rate can be edited";
                                                            SimulationId++;//12

                                                            for (int i = 0; i < credList.Count; i++)
                                                            {
                                                                crd = (Creds)credList[i];
                                                            }
                                                            sim.BeforeTest(testname, testDescription);
                                                            sim.FeeCalculatorConfiguration(driver, crd.Username, crd.Password, SimulationId);
                                                            sim.CheckTest();
                                                            finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                                                            testname = "Delivery Method Config";
                                                            testDescription = "Checking New delivery Method can be added";
                                                            SimulationId++;//13

                                                            sim.BeforeTest(testname, testDescription);
                                                            sim.Configure_Delivery_Method(driver, SimulationId);
                                                            sim.CheckTest();
                                                            finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                                                            testname = "Fomat Types Config";
                                                            testDescription = "Checking New Fomat types can be added";
                                                            SimulationId++;//14

                                                            sim.BeforeTest(testname, testDescription);
                                                            sim.Configure_Fomat_Types(driver, SimulationId);
                                                            sim.CheckTest();
                                                            finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                                                            testname = "Access Registration";
                                                            testDescription = "Checking Registration landing page can be accessed successfully";
                                                            SimulationId++;//15

                                                            sim.BeforeTest(testname, testDescription);
                                                            sim.AccessRegistration(driver, SimulationId);
                                                            sim.CheckTest();
                                                            finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                                                            testname = "Register External User";
                                                            testDescription = "Checking if External user account can successfully be registered";
                                                            SimulationId++;//16

                                                            sim.BeforeTest(testname, testDescription);
                                                            sim.Register_External_User(driver, SimulationId);
                                                            sim.CheckTest();
                                                            finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                                                            testname = "Forgot Password";
                                                            testDescription = "Checking if the user can sucessfully reset password";
                                                            SimulationId++;//17

                                                            sim.BeforeTest(testname, testDescription);
                                                            credList = Rtrv.getCredentials(2, 5);
                                                            for (int i = 0; i < credList.Count; i++)
                                                            {
                                                                crd = (Creds)credList[i];
                                                            }
                                                            sim.PasswordRecovery(crd.Username, driver, SimulationId);
                                                            sim.CheckTest();
                                                            finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                                                            testname = "Validate security questions";
                                                            testDescription = "Checking if the system accepts security questions";
                                                            SimulationId++;//18

                                                            sim.BeforeTest(testname, testDescription);
                                                            sim.ValidateSecurityQues(driver, SimulationId);
                                                            sim.CheckTest();
                                                            finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);


                                                            testname = "Create New Password";
                                                            testDescription = "Checking if the User can successfully create a new password";
                                                            SimulationId++;//18

                                                            sim.BeforeTest(testname, testDescription);
                                                            sim.CreateNewPassword(driver, SimulationId);
                                                            sim.CheckTest();
                                                            finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);
                                                            
                                                            testname = "Check otp";
                                                            testDescription = "Checking if the user can successfully receive the otp after proceeding with forgot password";
                                                            SimulationId++;//19

                                                            sim.BeforeTest(testname, testDescription);
                                                            sim.ReceiveOtp(driver, SimulationId);
                                                            sim.CheckTest();
                                                            finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                                                            

                                                            testname = "Change password";
                                                            testDescription = "Checking if the user can successfully receive the otp after proceeding with forgot password";
                                                            SimulationId++;//19

                                                            sim.BeforeTest(testname, testDescription);
                                                            sim.ChangePassword(driver, SimulationId);
                                                            sim.CheckTest();
                                                            finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

















                                                            testname = "Email Adress validation";
                                                            testDescription = "Checking if the email adress can be successfully validated";
                                                            SimulationId++;//20

                                                            sim.BeforeTest(testname, testDescription);
                                                            sim.ValidateEmailAdress(driver, SimulationId);
                                                            sim.CheckTest();
                                                            finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                                                            testname = "Change email Adress";
                                                            testDescription = "Checking if the email adress can be successfully Changed";
                                                            SimulationId++;//21

                                                            sim.BeforeTest(testname, testDescription);
                                                            sim.ChangeEmailAdress(driver, SimulationId);
                                                            sim.CheckTest();
                                                            finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                                                            testname = "Deactivate Account";
                                                            testDescription = "Checking if the user Account can be successfully Deactivated";
                                                            SimulationId++;//22

                                                            sim.BeforeTest(testname, testDescription);
                                                            sim.DeactivateAccount(driver, SimulationId);
                                                            sim.CheckTest();
                                                            finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                                                            testname = "Configure Workflow";
                                                            testDescription = "Checking if the Workflow can be successfully Configured";
                                                            SimulationId++;//23

                                                            sim.BeforeTest(testname, testDescription);
                                                            sim.ConfigureWorkflow(driver, SimulationId);
                                                            sim.CheckTest();
                                                            finalStatus = sim.AfterTest(Admin, driver, crd.Username, crd.RoleType, crd.UserType);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        GIS();
                        sim.AfterConclusionReport();
                    }
                    else
                    {
                        GIS();
                        sim.AfterConclusionReport();
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                driver.Quit();
                driver.Dispose();
            }
            
        }
        public void GIS()
        {
            //Serparate Clause for GIS Automation 
            try
            {
                testname = "Check Map Boundary";
                testDescription = "checking if the map Boundary has loaded";
                SimulationId++;//12
                //var pathDoc = ConfigurationManager.AppSettings["GisRootFolder"].ToString();
                //sim.Start(pathDoc);
                sim.BeforeTest(testname, testDescription);
                sim.CheckBoundary(driver, SimulationId);
                sim.CheckTest();
                finalStatus = sim.AfterTest_Gis(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                testname = "Zoom in";
                testDescription = "Checking if zoom in functionality works";
                SimulationId++;//13

                sim.BeforeTest(testname, testDescription);
                sim.CheckZoomInLevel(driver, SimulationId);
                sim.CheckTest();
                finalStatus = sim.AfterTest_Gis(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                testname = "Zoom out";
                testDescription = "Checking if zoom out functionality works";
                SimulationId++;//13

                sim.BeforeTest(testname, testDescription);
                sim.CheckZoomOutLevel(driver, SimulationId);
                sim.CheckTest();
                finalStatus = sim.AfterTest_Gis(Admin, driver, crd.Username, crd.RoleType, crd.UserType);



                testname = "Query Gis Data";
                testDescription = "Checking if the Gis Data can be received";
                SimulationId++;//12
                sim.BeforeTest(testname, testDescription);
                sim.Query_Gis_Data(driver, SimulationId);
                sim.CheckTest();
                finalStatus = sim.AfterTest_Gis(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                if (finalStatus == TestStatus.Passed)
                {
                    testname = "Drag Search Modal";
                    testDescription = "Checking if the the Search Modal can be dragged/panned to anypoint on the map";
                    SimulationId++;//13

                    sim.BeforeTest(testname, testDescription);
                    sim.DragElement(driver, SimulationId);
                    sim.CheckTest();
                    finalStatus = sim.AfterTest_Gis(Admin, driver, crd.Username, crd.RoleType, crd.UserType);
                    if (finalStatus == TestStatus.Passed)
                    {
                        testname = "Minimise Search Modal";
                        testDescription = "Checking if the the Search Modal can be Minimised";
                        SimulationId++;//14

                        sim.BeforeTest(testname, testDescription);
                        sim.MinimiseSearch(driver, SimulationId);
                        sim.CheckTest();
                        finalStatus = sim.AfterTest_Gis(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                        if (finalStatus == TestStatus.Passed)
                        {
                            testname = "Close Search Modal";
                            testDescription = "Checking if the the Search Modal can be Closed";
                            SimulationId++;//15

                            sim.BeforeTest(testname, testDescription);
                            sim.CloseSearch(driver, SimulationId);
                            sim.CheckTest();
                            finalStatus = sim.AfterTest_Gis(Admin, driver, crd.Username, crd.RoleType, crd.UserType);
                        }

                    }
                }
                //test 14
                testname = "Search by Draw";
                testDescription = "Checking if we can successfullty draw a Polygon";
                SimulationId++;//14

                sim.BeforeTest(testname, testDescription);
                sim.DrawAPolygon(driver, SimulationId);
                sim.CheckTest();
                finalStatus = sim.AfterTest_Gis(Admin, driver, crd.Username, crd.RoleType, crd.UserType);

                //sim.AfterConclusionReport();
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public void NewTestCases()
        {
            try
            {
                SimulationId++;
                sim.Register_External_User(driver, SimulationId);

                SimulationId++;
                credList = Rtrv.getCredentials(2, 5);
                for (int i = 0; i < credList.Count; i++)
                {
                    crd = (Creds)credList[i];
                }
                sim.PasswordRecovery(crd.Username,driver, SimulationId);
            }
            catch (Exception)
            {
                
            }
        }
        #endregion
    }
}
