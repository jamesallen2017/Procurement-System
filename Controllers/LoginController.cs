using Google.Authenticator;
using PO_PurchasingUI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;

namespace PO_PurchasingUI.Controllers
{
	
	public class LoginController : Controller
	{
        string Security;
        string PobyClient;
        string MasterList;
        string PotoSupplier;
        string DrModule;
        string Accounting;

        private void DatabaseConnectionString()
        {
            try
            {
                string ConnectionString = Server.MapPath("~/DBServer/DatabaseConnection.txt");
                string Username = Server.MapPath("~/DBServer/DatabaseUser.txt");
                string Password = Server.MapPath("~/DBServer/DatabasePassword.txt");

                string ServerConnection = System.IO.File.ReadAllText(ConnectionString);
                string ServerName = System.IO.File.ReadAllText(Username);
                string ServerPassword = System.IO.File.ReadAllText(Password);

                string Server_Connection = ServerConnection.TrimEnd();
                string Server_Name = EncryptionAndDecryption.Decrypt(ServerName.TrimEnd());
                string Server_Password = EncryptionAndDecryption.Decrypt(ServerPassword.TrimEnd());


                Security = "Data Source=" + Server_Connection.ToString() + ";Initial Catalog=PO_SECURITY;Network Library=DBMSSOCN;User ID=" + Server_Name.ToString() + "; Password=" + Server_Password.ToString() + "";
                PobyClient = "Data Source=" + Server_Connection.ToString() + ";Initial Catalog=PO_CLIENT;Network Library=DBMSSOCN;User ID=" + Server_Name.ToString() + "; Password=" + Server_Password.ToString() + "";
                MasterList = "Data Source=" + Server_Connection.ToString() + ";Initial Catalog=PO_MASTERLIST;Network Library=DBMSSOCN;User ID=" + Server_Name.ToString() + "; Password=" + Server_Password.ToString() + "";
                PotoSupplier = "Data Source=" + Server_Connection.ToString() + ";Initial Catalog=PO_SUPPLIER;Network Library=DBMSSOCN;User ID=" + Server_Name.ToString() + "; Password=" + Server_Password.ToString() + "";
                DrModule = "Data Source=" + Server_Connection.ToString() + ";Initial Catalog=PO_DRMODULE;Network Library=DBMSSOCN;User ID=" + Server_Name.ToString() + "; Password=" + Server_Password.ToString() + "";
                Accounting = "Data Source=" + Server_Connection.ToString() + ";Initial Catalog=PO_ACCOUNTING;Network Library=DBMSSOCN;User ID=" + Server_Name.ToString() + "; Password=" + Server_Password.ToString() + "";


            }
            catch
            {
                throw;
            }
           
        }

		private void EnsureLoggedOut()
		{
			if (Request.IsAuthenticated)  // If the request is (still) marked as authenticated we send the user to the logout action
				LogOut();
		}

		public ActionResult LogOut()
		{
			try
			{
                //string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
                //									 // Get the IP  
                //string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

                //string[] computer_name = System.Net.Dns.GetHostEntry(Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' });
                //String ecn = System.Environment.MachineName;
                //var ComputerName = computer_name[0].ToString();

                DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
                var Remarks = "You have successfully log out";
                DatabaseAccessDB.InsertAuditTrailDB(Session["UserName"].ToString(), Remarks, "", "");

                DatabaseConnectionString();
                SqlDependency.Stop(PobyClient);
                SqlDependency.Stop(PotoSupplier);

                // First we clean the authentication ticket like always
                FormsAuthentication.SignOut();

				// Second we clear the principal to ensure the user does not retain any authentication
				HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
				Session.Abandon();
				Session.Clear();
				System.Web.HttpContext.Current.Session.RemoveAll();

                return RedirectToAction("UserLogin", "Login");


			}
			catch
			{
				FormsAuthentication.SignOut();
				// Second we clear the principal to ensure the user does not retain any authentication
				HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
				Session.Abandon();
				Session.Clear();
				System.Web.HttpContext.Current.Session.RemoveAll();


                return RedirectToAction("UserLogin", "Login");

			}
		}

        public ActionResult ErrorHandler()
        {
            return View("~/Views/Shared/Error.cshtml");

        }

        

        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, Location = OutputCacheLocation.None)]
        public ActionResult UserLogin()
		{

            //Uri uri = new Uri(System.Web.HttpContext.Current.Request.Url.AbsoluteUri);
            //string link = string.Format("{0}://{1}", uri.Scheme, uri.Authority);
            //if (link == "https://procurementsystem.archangeltech.com.ph"  || link == "http://10.0.0.124:8882" || link == "http://localhost:59455")
            //{

            //	if (Session["UserName"] != null)
            //		return View("~/Views/home/index.cshtml");

            //	ViewBag.Login = true;
            //	return View("~/Views/home/UserLogin.cshtml");
            //}
            //else
            //{
            //	Response.Redirect("https://procurementsystem.archangeltech.com.ph");
            //}
            //return View();
            //DatabaseAccess DatabaseAccessDB = new DatabaseAccess();

            //DatabaseAccessDB.GenerateTwoFactorAuthentication();

            if (Session["UserName"] != null)
                return RedirectToAction("index", "Home");


            ViewBag.Login = true;
			return View("~/Views/home/UserLogin.cshtml");
		}

        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, Location = OutputCacheLocation.None)]
        public ActionResult ServerConfiguration()
        {
            if (Session["UserName"] != null)
                return RedirectToAction("index", "Home");

            ViewBag.AdminPanel = "false";
            Session["AdminPanel"] = "false";
            return View("~/Views/home/ServerConfiguration.cshtml");
        }

        [HttpPost]
        public ActionResult ServerConfiguration(LoginModel model, string AdminPanel)
        {
            string ConnectionString = Server.MapPath("~/DBServer/DatabaseConnection.txt");
            string Username = Server.MapPath("~/DBServer/DatabaseUser.txt");
            string Password = Server.MapPath("~/DBServer/DatabasePassword.txt");

            string AdminUser = Server.MapPath("~/DBServer/AdminUser.txt");
            string AdminPass = Server.MapPath("~/DBServer/AdminPassword.txt");

            string ServerConnection = System.IO.File.ReadAllText(ConnectionString);
            string ServerName = EncryptionAndDecryption.Decrypt(System.IO.File.ReadAllText(Username).TrimEnd());
            string ServerPassword = EncryptionAndDecryption.Decrypt(System.IO.File.ReadAllText(Password).TrimEnd());

            string AdminUserName = System.IO.File.ReadAllText(AdminUser);
            string AdminPassword = System.IO.File.ReadAllText(AdminPass);

            string User2 = EncryptionAndDecryption.Decrypt(AdminUserName);
            string Pass2 = EncryptionAndDecryption.Decrypt(AdminPassword);
            string admin = Session["AdminPanel"].ToString();

            if (model.AdminUsername != User2.ToString().TrimEnd() && model.AdminPassword != Pass2.ToString().TrimEnd() && admin == "false")
            {
                ViewBag.HeaderError = string.Format("Unable  to connect to server.");
                ViewBag.MessageError = string.Format("Check your Admin Username or Admin Password.");
                return View("~/Views/home/ServerConfiguration.cshtml");
            }
            else
            {
                if (admin != "true")
                {
                    ViewBag.AdminPanel = "true";
                    Session["AdminPanel"] = "true";
                    model.ServerName = ServerConnection;
                    model.ServerUsername = ServerName;
                    model.ServerPassword = ServerPassword;
                    return View("~/Views/home/ServerConfiguration.cshtml", model);
                }

            }



            using (StreamWriter sw = System.IO.File.CreateText(ConnectionString))
            {
                sw.WriteLine(model.ServerName);
            }

            using (StreamWriter sw = System.IO.File.CreateText(Username))
            {
                sw.WriteLine(EncryptionAndDecryption.Encrypt(model.ServerUsername));
            }

            using (StreamWriter sw = System.IO.File.CreateText(Password))
            {
                sw.WriteLine(EncryptionAndDecryption.Encrypt(model.ServerPassword));
            }

            DatabaseConnectionString();

            using (SqlConnection connection = new SqlConnection(Security))
            {
                try
                {
                    connection.Open();
                }
                catch
                {
                    ViewBag.HeaderError = string.Format("Unable  to connect to server.");
                    ViewBag.MessageError = string.Format("Check your server connection.");
                    return View();
                }
            }

            ViewBag.HeaderSuccess = string.Format("Connection Success.");
            ViewBag.MessageSuccess = string.Format("Your Connection Server has connected.");
            Session.Abandon();
            Session.Clear();
            return View("~/Views/home/ServerConfiguration.cshtml");
        }

        public ActionResult Index()
		{

			if (Session["UserName"] != null)
				return RedirectToAction("index", "Home");

            ViewBag.Login = true;
			return View("~/Views/home/UserLogin.cshtml");

		}

		public ActionResult NewPasswordAuthentication()
		{
			try
			{
				EnsureLoggedOut(); // removed session and user identity 
                LoginModel loginModel = new LoginModel();
                ViewBag.Login = false;
                ViewBag.NewPassword = true;
                return View("~/Views/home/UserLogin.cshtml", loginModel);
			}
			catch
			{
                throw;
			}
		}

        public ActionResult LoginAuthentication(string ReturnUrl)
		{

			LoginModel Userinfo = new LoginModel();
			try
			{
				EnsureLoggedOut();
				if (Url.IsLocalUrl(Userinfo.ReturnURL))
				{
					return Redirect(Userinfo.ReturnURL);
				}

                return RedirectToAction("index", "Login"); 

			}
			catch
			{
                throw;

			}
		}

        public ActionResult VerificationAuthentication(string ReturnUrl)
        {
            LoginModel Userinfo = new LoginModel();
            try
            {
                EnsureLoggedOut();
                if (Url.IsLocalUrl(Userinfo.ReturnURL))
                {
                    return Redirect(Userinfo.ReturnURL);
                }

                return RedirectToAction("index", "Login");

            }
            catch
            {
                throw;

            }
        }

        public void GenerateBackUpCodes(string UserID)
        {

            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            MainModel model = new MainModel();

            List<MainModel> ListCode = new List<MainModel>();

            Random rand = new Random();

            string temp = "";

            string IDString = "";

            string RandomChars = "";

            int PasswordLength = 6;

            RandomChars = "1,2,3,4,5,6,7,8,9,0";

            Char[] seprateChar = { ',' };

            string[] arrRandomChar = RandomChars.Split(seprateChar);

            var ExistCodes = DatabaseAccessDB.PopulateBackupVerificationCode(UserID);

            if (ExistCodes.Count == 0)
            {
                int CodeLength = 6;

                for (int c = 0; c < CodeLength; c++)
                {

                    for (int i = 0; i < PasswordLength; i++)
                    {
                        temp = arrRandomChar[rand.Next(0, arrRandomChar.Length)];
                        IDString += temp;
                    }
                    model = new MainModel()
                    {
                        VerificationCode = IDString,

                    };

                    ListCode.Add(model);

                    temp = "";

                    IDString = "";

                    RandomChars = "";

                }

                DatabaseAccessDB.InsertBackupVerificationCode(ListCode, UserID);
            }
        }
        
  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerificationAuthentication(string UserID, string VerificationCode, string ReturnUrl)
        {
            String pin = VerificationCode.Trim();
            Boolean status = ValidateTwoFactorPIN(pin);

            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            LoginModel loginModel = new LoginModel();
            MainModel model = new MainModel();

            if (!status)
            {
                var ExistsCodes = DatabaseAccessDB.PopulateBackupVerificationCode(UserID);

                var item = ExistsCodes.FirstOrDefault(x => x.VerificationCode == VerificationCode && x.VerificationUsed == "1");

                if (item != null)
                {
                    DatabaseAccessDB.UpdateBackupVerificationCode(item.VerificationCode, UserID);

                }

                else {
                    ViewBag.VerificationError = "true";
                    ViewBag.VerificationForm = true;
                    return View("~/Views/home/UserLogin.cshtml");
                }

            }
           

                if (Session["ReturnUrl"] == null || Session["ReturnUrl"].ToString() == "")
                {
                    Session["ReturnUrl"] = ReturnUrl;
                }
                else
                {
                    ReturnUrl = Session["ReturnUrl"].ToString();
                }

                loginModel = DatabaseAccessDB.GetUserSessionDB(UserID);

                //string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                //if (string.IsNullOrEmpty(ipAddress))
                //{
                //    ipAddress = Request.ServerVariables["REMOTE_ADDR"];
                //}

                //string[] computer_name = System.Net.Dns.GetHostEntry(Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' });
                //String ecn = System.Environment.MachineName;
                //var ComputerName = computer_name[0].ToString();


                ViewBag.VerificationError = "false";
                Session["UserName"] = loginModel.UserName;
                Session["UserID"] = loginModel.UserID;
                Session["EmployeeID"] = loginModel.EmployeeID;
                Session["UserRole"] = loginModel.UserRoleID;
                Session["UserRoleName"] = loginModel.UserRole;
                Session["DepartmentID"] = loginModel.DepartmentID;
                Session["Department"] = loginModel.Department;
                Session["UserGroup"] = loginModel.UserGroup;
                Session["UserGroupID"] = loginModel.UserGroupID;
                Session["UserFullName"] = loginModel.FullName;
                Session["EmailAddress"] = loginModel.Email;
                Session["ModuleAccess"] = "true";


            DatabaseAccessDB.DeletePreviewsInformationDB(Session["UserID"].ToString());
            DatabaseAccessDB.DeleteCOCPreviewsInformationDB(Session["UserID"].ToString());
            DatabaseAccessDB.UpdateVerificationDate(Session["UserID"].ToString());
            FormsAuthentication.SetAuthCookie(Session["UserID"].ToString(), false);

            var Remarks = "You have successfully log in";
            DatabaseAccessDB.InsertAuditTrailDB(loginModel.UserName, Remarks, "", "");






            if (Url.IsLocalUrl(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                else
                {

                    NotificationComponent NC = new NotificationComponent();
                    string userid = Session["UserID"].ToString() ?? "";
                    NC.name = userid;

                    if (userid != "")
                        NC.SupplierNotificaton(userid);
                    NC.CoCNotificaton(userid);
                    NC.PurchaseRequisitionNotificaton(userid);
                    NC.SupplierStatus();
                    return RedirectToAction("index", "Home");
                }


            




        }

        [HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LoginAuthentication(string UserName, string Password, string ReturnUrl)
		{

	  //      string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

   //         if (string.IsNullOrEmpty(ipAddress))
   //         {
   //             ipAddress = Request.ServerVariables["REMOTE_ADDR"];
   //         }
   //         string[] computer_name = System.Net.Dns.GetHostEntry(Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' });
			//String ecn = System.Environment.MachineName;
			//var ComputerName = computer_name[0].ToString();


			DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
			LoginModel loginModel = new LoginModel();
			

			var UserID = DatabaseAccessDB.PopulateUserSessionDB(UserName, Password);

			if ( Session["ReturnUrl"] == null || Session["ReturnUrl"].ToString() == "")
			{
				Session["ReturnUrl"] = ReturnUrl;
			}
			else
			{
				ReturnUrl = Session["ReturnUrl"].ToString();
			}

			if (UserID != "")
			{
				loginModel = DatabaseAccessDB.GetUserSessionDB(UserID);
                //GenerateBackUpCodes(UserID);

                if (loginModel.Auth_Status == "1")
                {
                    ViewBag.VerificationForm = true;
                    Session["AuthenticationCode"] = loginModel.Auth_SecretKey;
                    return View("~/Views/home/UserLogin.cshtml", loginModel);
                }

				if (loginModel.Status != "Active")
				{
					ViewBag.Validator = true;
					ViewBag.Login = true;
					ViewBag.ValidateError = "Your account has been deactivated. Please Contact our administrator for assistance";
					HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
					Session.Abandon();
					Session.Clear();
					System.Web.HttpContext.Current.Session.RemoveAll();
					return View("~/Views/home/UserLogin.cshtml");
				}

				loginModel.UserName = UserName;
				loginModel.UserID = Convert.ToInt32(UserID);

				if (Password == "P@ssw0rd") 
                {
                    ViewBag.Login = false;
                    ViewBag.NewPassword = true;
                    return View("~/Views/home/UserLogin.cshtml",loginModel);

                }

                Session["UserName"] = UserName;
                Session["UserID"] = loginModel.UserID;
                Session["EmployeeID"] = loginModel.EmployeeID;
                Session["UserRole"] = loginModel.UserRoleID;
                Session["UserRoleName"] = loginModel.UserRole;
                Session["DepartmentID"] = loginModel.DepartmentID;
                Session["Department"] = loginModel.Department;
                Session["UserGroup"] = loginModel.UserGroup;
                Session["UserGroupID"] = loginModel.UserGroupID;
                Session["UserFullName"] = loginModel.FullName;
                Session["EmailAddress"] = loginModel.Email;
                Session["ModuleAccess"] = "true";

                //GenerateBackUpCodes(UserID);
                DatabaseAccessDB.DeletePreviewsInformationDB(Session["UserID"].ToString());
                DatabaseAccessDB.DeleteCOCPreviewsInformationDB(Session["UserID"].ToString());
                FormsAuthentication.SetAuthCookie(Session["UserID"].ToString(), false);

                var Remarks = "You have successfully log in";
                DatabaseAccessDB.InsertAuditTrailDB(UserName, Remarks, "", "");

                DatabaseConnectionString();
                SqlDependency.Start(PobyClient);
                SqlDependency.Start(PotoSupplier);

                NotificationComponent NC = new NotificationComponent();
                string userid = Session["UserID"].ToString() ?? "";
                NC.name = userid;

                if (userid != "")
                NC.SupplierNotificaton(userid);
                NC.CoCNotificaton(userid);
                NC.PurchaseRequisitionNotificaton(userid);
                NC.SupplierStatus();

                if (!Directory.Exists(Server.MapPath("~/Downloads\\" + UserName)))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Downloads\\" + UserName));
                }

                if (Url.IsLocalUrl(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                else
                {
                   
                    return RedirectToAction("index", "Home");
                }
            }
			else
			{
				ViewBag.Login = true;
				ViewBag.LoginError = "true";
				return View("~/Views/home/UserLogin.cshtml");
			}

		}

		[HttpGet]
		public ActionResult ExtendSession()
		{

			FormsAuthentication.SetAuthCookie(User.Identity.Name, false); // render session into Cookie to extend session.
			var data = new { IsSuccess = true }; 
			return Json(data, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult NewPasswordAuthentication(LoginModel loginModel)
		{
            string ReturnUrl = "";
            //string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            //if (string.IsNullOrEmpty(ipAddress))
            //{
            //    ipAddress = Request.ServerVariables["REMOTE_ADDR"];
            //}
            //string[] computer_name = System.Net.Dns.GetHostEntry(Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' });
            //String ecn = System.Environment.MachineName;
            //var ComputerName = computer_name[0].ToString();


            int count = 0;
		    Session["UserName"] = loginModel.UserName;
			if (!ModelState.IsValid)
			{
				ViewBag.NewPassword = true;
				return View("~/Views/home/UserLogin.cshtml");
			}

			DatabaseAccess DatabaseAccessDB = new DatabaseAccess();

				count = DatabaseAccessDB.UpdateNewPasswordDB(loginModel);

			if (count != 0)
			{

				loginModel = DatabaseAccessDB.GetUserSessionDB(Convert.ToString(loginModel.UserID));

                if (loginModel.Auth_Status == "1")
                {
                    ViewBag.VerificationForm = true;
                    Session["AuthenticationCode"] = loginModel.Auth_SecretKey;
                    return View("~/Views/home/UserLogin.cshtml", loginModel);
                }

                if (loginModel.Status != "Active")
				{
					ViewBag.Validator = true;
					ViewBag.NewPassword = true;
					ViewBag.ValidateError = "Oops, Your account has been deactivated. Please Contact our administrator for assistance";
					HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
					Session.Abandon();
					Session.Clear();
					System.Web.HttpContext.Current.Session.RemoveAll();
					return View("~/Views/home/UserLogin.cshtml");
				}

                Session["UserID"] = loginModel.UserID;
                Session["EmployeeID"] = loginModel.EmployeeID;
                Session["UserRole"] = loginModel.UserRoleID;
                Session["UserRoleName"] = loginModel.UserRole;
                Session["DepartmentID"] = loginModel.DepartmentID;
                Session["Department"] = loginModel.Department;
                Session["UserGroup"] = loginModel.UserGroup;
                Session["UserGroupID"] = loginModel.UserGroupID;
                Session["UserFullName"] = loginModel.FullName;
                Session["EmailAddress"] = loginModel.Email;
                Session["ModuleAccess"] = "true";


                //GenerateBackUpCodes(Convert.ToString(loginModel.UserID));
                var Remarks = "You have successfully Change your Password";
                DatabaseAccessDB.InsertAuditTrailDB(loginModel.UserName, Remarks, "", "");
                DatabaseAccessDB.DeletePreviewsInformationDB(Session["UserID"].ToString());
                DatabaseAccessDB.DeleteCOCPreviewsInformationDB(Session["UserID"].ToString());
                FormsAuthentication.SetAuthCookie(Session["UserID"].ToString(), false);

                DatabaseConnectionString();
                SqlDependency.Start(PobyClient);
                SqlDependency.Start(PotoSupplier);

                NotificationComponent NC = new NotificationComponent();
                string userid = Session["UserID"].ToString() ?? "";
                NC.name = userid;

                if (userid != "")
                    NC.SupplierNotificaton(userid);
                NC.CoCNotificaton(userid);
                NC.PurchaseRequisitionNotificaton(userid);
                NC.SupplierStatus();

                if (!Directory.Exists(Server.MapPath("~/Downloads\\" + loginModel.UserName)))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Downloads\\" + loginModel.UserName));
                }

                if (Session["ReturnUrl"] == null || Session["ReturnUrl"].ToString() == "")
                {
                    Session["ReturnUrl"] = ReturnUrl;
                }
                else
                {
                    ReturnUrl = Session["ReturnUrl"].ToString();
                }


                if (Url.IsLocalUrl(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                else
                {

                    return RedirectToAction("index", "Home");
                }

            }
			else
			{
				ViewBag.Validator = true;
				ViewBag.NewPassword = true;
				ViewBag.ValidateError = "Please check your username or password";
			}
			
			return View("~/Views/home/UserLogin.cshtml");
		}


        public Boolean ValidateTwoFactorPIN(String pin)
        {
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            return tfa.ValidateTwoFactorPIN(Session["AuthenticationCode"].ToString(), pin);
        }

    }
}