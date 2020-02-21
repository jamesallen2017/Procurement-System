using PO_PurchasingUI.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using System.Security.Principal;
using System.Web.Security;
using System.Net.Mail;
using System.Net;
using System.Web.Script.Serialization;
using System.Web;
using System.Linq;
using Newtonsoft.Json;
using System.Collections;

namespace PO_PurchasingUI.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    [SessionTimeout]
    public class HomeController : Controller
    {
        public JsonRequestBehavior JsonRequestBehavior { get; private set; }
        public List<SelectListItem> Data { get; private set; }
        public List<string> ListStringData { get; private set; }
        [HttpPost]
        public JsonResult GenerateVerificationCodeList()
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

            var UserID = Session["UserID"].ToString();

            var ExistsCodes = DatabaseAccessDB.PopulateBackupVerificationCode(UserID);

            if (ExistsCodes.Count == 0)
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

                var z = DatabaseAccessDB.InsertBackupVerificationCode(ListCode,UserID);
            }
            else {

                ListCode = ExistsCodes;
            }
           

            return Json(ListCode, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GenerateNewVerificationCodeList()
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

            var UserID = Session["UserID"].ToString();

            var ExistsCodes = DatabaseAccessDB.DeleteBackupVerificationCode(UserID);

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

                var z = DatabaseAccessDB.InsertBackupVerificationCode(ListCode, UserID);


            return Json(ListCode, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            var myUserGroupID = Session["UserGroupID"].ToString();

            if (Session["UserName"] != null)
            {
                if (myUserGroupID == "5")
                {
                    return View("~/Views/home/index.cshtml");
                }
                else {
                    return View("~/Views/home/Defaultindex.cshtml");

                }
            }
            return View("~/Views/home/UserLogin.cshtml");
        }

        public ActionResult Authenticator()
        {
            LoginModel login = new LoginModel();
            DatabaseAccess database = new DatabaseAccess();
            string Responsible = Session["UserID"].ToString() ?? "0";

            login = database.GetUserSessionDB(Responsible);
            ViewBag.ImageUrl = login.Auth_QRCode;
            ViewBag.Username = login.UserName;
            ViewBag.SecretKey = login.Auth_SecretKey;
            ViewBag.ManualCode = login.Auth_ManualCode;
            ViewBag.Status = login.Auth_Status;
            return View("~/Views/home/GoogleAuthenticator.cshtml", login);
        }
        [HttpPost]
        public JsonResult UpdateAuthenticationNewCodes(string UserID, string Status)
        {
            DatabaseAccess db = new DatabaseAccess();
           var data =  db.update(UserID, Status);
            return Json(data);
        }

        [HttpPost]
        public JsonResult EnableAuthenticator(string UserID)
        {
            DatabaseAccess db = new DatabaseAccess();
            var data = db.EnableDisableAuthenticatorDB(UserID, "1");
            return Json(data);
        }

        [HttpPost]
        public JsonResult DisableAuthenticator(string UserID)
        {
            DatabaseAccess db = new DatabaseAccess();
            var data = db.EnableDisableAuthenticatorDB(UserID, "0");
            return Json(data);
        }



        public ActionResult SendingEmailError()
        {
            return View("~/Views/Shared/SendingEmailError.cshtml");

        }

        public ActionResult UserLogin()
        {
            return View("~/Views/Home/UserLogin.cshtml");
        }

        //-------------------------------------------------------------------*CLIENT QUOTATION*-------------------------------------------------------------------------------------------------------//
        public ActionResult CLIENT_QUOTATIONITEM()
        {

            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ClientModel clientModel = new ClientModel();
            ViewBag.ItemQuotation = true;
            clientModel.PopulateAllItem = DatabaseAccessDB.PopulateAllIQuotationDB();
            return View("~/Views/Home/Client/CreateClientQuotation.cshtml", clientModel);

        }

        [HttpPost]
        public ActionResult CLIENT_QUOTATION(string selectedItems)
        {

            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ClientModel clientModel = new ClientModel();

            clientModel.ClientNameList = DatabaseAccessDB.List_PopulateClientNameDB();
            clientModel.Submittedby = DatabaseAccessDB.Select_AllSalesUserDB();
            var result = JsonConvert.DeserializeObject<List<ClientModel>>(selectedItems);
            ViewBag.ItemQuotation = false;
            clientModel.ItemNameList = DatabaseAccessDB.List_PopulateItemNameDB(); // POPULATE ALL ITEM NAME
            clientModel.PopulateAllItemList = result; // POPULATE ALL ITEM NAME
            return View("~/Views/Home/Client/CreateClientQuotation.cshtml", clientModel);

        }

        public ActionResult CLIENT_QUOTATION()
        {

            return RedirectToAction("CLIENT_QUOTATIONITEM", "Home");

        }

        public ActionResult CLIENT_EDITQUOTATIONFORM(string id)
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ClientModel clientModel = new ClientModel();
            ViewBag.ItemQuotation = false;
            clientModel = DatabaseAccessDB.GetCLientQuotationDB(id);
            clientModel.PopulateAllItemList = DatabaseAccessDB.PopulateItemClientQoutationGRIDDB(id);
            clientModel.LocationList = DatabaseAccessDB.List_PopulateAllClientAddressDB(clientModel.ClientID);
            clientModel.ClientNameList = DatabaseAccessDB.List_PopulateClientNameDB();
            clientModel.Submittedby = DatabaseAccessDB.Select_AllSalesUserDB();
            clientModel.ItemNameList = DatabaseAccessDB.List_PopulateItemNameDB(); // POPULATE ALL ITEM NAME
            return View("~/Views/Home/Client/EditClientQuotation.cshtml", clientModel);

        }

        public ActionResult CLIENT_EDITQUOTATION()
        {

            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ClientModel clientModel = new ClientModel();

            ViewBag.ItemQuotation = true;
            string Responsible = Session["UserID"].ToString() ?? "0";
            clientModel.PopulateAllItem = DatabaseAccessDB.PopulateEditClientAllIQuotationDB(Responsible);
            return View("~/Views/Home/Client/EditClientQuotation.cshtml", clientModel);
        }

        public ActionResult InquiryClientQuotation()
        {

            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ClientModel clientModel = new ClientModel();
            string Responsible = Session["UserID"].ToString() ?? "0";

            clientModel.PopulateAllItem = DatabaseAccessDB.PopulateInquiry_ClientAllIQuotationDB(Responsible);
            return View("~/Views/Home/Inquiry/Inquiry_ClientQuotation.cshtml", clientModel);
        }

        //-------------------------------------------------------------------*CLIENT*-------------------------------------------------------------------------------------------------------//

        public ActionResult POBYCLIENT_QUOTATION()
        {

            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ClientModel clientModel = new ClientModel();
            ViewBag.ItemQuotation = true;
            string Responsible = Session["UserID"].ToString() ?? "0";
            clientModel.PopulateAllItem = DatabaseAccessDB.PopulateClientAllIQuotationDB(Responsible);
            return View("~/Views/Home/Client/CreateClient.cshtml", clientModel);

        }

        public ActionResult POBYCLIENT(string id)
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ClientModel clientModel = new ClientModel();
            clientModel = DatabaseAccessDB.GetCLientQuotationDB(id);
            clientModel.PopulateAllItemList = DatabaseAccessDB.PopulateItemClientQoutationGRIDDB(id);
            clientModel.LocationList = DatabaseAccessDB.List_PopulateAllClientAddressDB(clientModel.ClientID);
            clientModel.ClientNameList = DatabaseAccessDB.List_PopulateClientNameDB();
            clientModel.ListofPaymentTerms = DatabaseAccessDB.List_PaymentTermsDB();
            clientModel.ItemNameList = DatabaseAccessDB.List_PopulateItemNameDB();
            clientModel.SupplierPONumberList = DatabaseAccessDB.List_SupplierPONumberDB();
            clientModel.SignProposal = id;
            ViewBag.ItemQuotation = false;
            return View("~/Views/Home/Client/CreateClient.cshtml", clientModel);
        }

        public ActionResult EditClient()
        {
            ViewBag.ClientMonitoring = true;
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ClientModel clientModel = new ClientModel();


            var Responsible = Session["UserID"].ToString() ?? ""; // Get Userid

            clientModel.PopulateAllEditClient = DatabaseAccessDB.PopulateAllEditClientDB(Responsible); // Select All Client transaction under userid account


            return View("~/Views/Home/Client/EditClient.cshtml", clientModel); // Return edit page.
        }

        [HttpPost]
        public JsonResult GetClientLocation(string ClientID)
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            var data = DatabaseAccessDB.List_PopulateAllClientAddressDB(ClientID);
            return Json(Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditClientForm(string id) // EditClient.cshtml(ReferenceNo)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/home/index.cshtml");
            }
            ViewBag.ClientMonitoring = false;
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ClientModel clientModel = new ClientModel();

            clientModel = DatabaseAccessDB.EditUserClientInformationDB(id); // POPULATE CLIENT INFORMATION BASE ON REFERENCE NO
            clientModel.PopulateAllItemList = DatabaseAccessDB.PolulateClientItemListDB(id); // POPULATE ITEM LIST UNDER CLIENT NAME
            clientModel.LocationList = DatabaseAccessDB.List_PopulateAllClientAddressDB(clientModel.ClientID); // GET / POPULATE CLIENT ADDRESS BASE ON CLIENT REFERENCE NO
            clientModel.ClientNameList = DatabaseAccessDB.List_PopulateClientNameDB(); // POPULATE ALL CLIENT NAME
            clientModel.ItemNameList = DatabaseAccessDB.List_PopulateItemNameDB(); // POPULATE ALL ITEM NAME
            clientModel.ListofPaymentTerms = DatabaseAccessDB.List_PaymentTermsDB();
            clientModel.SupplierPONumberList = DatabaseAccessDB.List_SupplierPONumberDB();

            ViewBag.PONumber = id;


            return View("~/Views/Home/Client/EditClient.cshtml", clientModel); // RENDER VIEW (EditClient.cshtml)
        }


        //-------------------------------------------------------------------*SUPPLIER*-------------------------------------------------------------------------------------------------------//
        public ActionResult PurchaseRequisition()
        {
            var UserName = "";
            if (Session["UserName"] != null)
            {
                UserName = Session["UserName"].ToString();
            }
            SupplierModel supplierModel = new SupplierModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ViewBag.SupplierMonitoring = false;

            supplierModel.Approver = DatabaseAccessDB.PopulateListApproverDB(UserName);
            supplierModel.Endorser = DatabaseAccessDB.PopulateListApproverDB(UserName);
            supplierModel.ListAllUser = DatabaseAccessDB.PopulateListAllUserDB();
            supplierModel.SupplierName = DatabaseAccessDB.List_AllSupplierDB();
            supplierModel.Particular = DatabaseAccessDB.List_PopulateItemNameDB();
            return View("~/VIews/Home/Supplier/PurchaseRequisition.cshtml", supplierModel);
        }

        public ActionResult EditPurchaseRequisition()
        {
            SupplierModel supplierModel = new SupplierModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ViewBag.RequisitionMonitoring = true;
            string Responsible = Session["UserID"].ToString() ?? "0";
            supplierModel.PopulateAllEditPurchaseRequisition = DatabaseAccessDB.PopulateAllEditPurchaseRequisitionDB(Responsible);
            return View("~/VIews/Home/Supplier/EditPurchaseRequisition.cshtml", supplierModel);

        }

        [HttpGet]
        public ActionResult GetPurchaseRequisition(string id)
        {
            SupplierModel supplierModel = new SupplierModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ViewBag.RequisitionMonitoring = false;

            var UserName = "";
            if (Session["UserName"] != null)
            {
                UserName = Session["UserName"].ToString();
            }

            supplierModel = DatabaseAccessDB.GetPurchaseRequisitionInformationDB(id);
            supplierModel.PopulateAllItemList = DatabaseAccessDB.GetPurchaseRequisition_GRIDDB(id);
            supplierModel.Approver = DatabaseAccessDB.PopulateListApproverDB(UserName);
            supplierModel.Endorser = DatabaseAccessDB.PopulateListApproverDB(UserName);
            supplierModel.ListAllUser = DatabaseAccessDB.PopulateListAllUserDB();
            supplierModel.SupplierName = DatabaseAccessDB.List_AllSupplierDB();
            supplierModel.Particular = DatabaseAccessDB.List_PopulateItemNameDB();

            return View("~/VIews/Home/Supplier/EditPurchaseRequisition.cshtml", supplierModel);

        }

        public ActionResult POTOSUPPLIER()
        {
            var UserName = "";
            if (Session["UserName"] != null)
            {
                UserName = Session["UserName"].ToString();
            }
            SupplierModel supplierModel = new SupplierModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ViewBag.SupplierMonitoring = false;

            supplierModel.ListClientName = DatabaseAccessDB.List_PopulateClientNameDB();
            supplierModel.ListClientReferenceNo = DatabaseAccessDB.List_PopulateClientReferenceNoDB();
            supplierModel.Approver = DatabaseAccessDB.PopulateListApproverDB(UserName);
            supplierModel.Endorser = DatabaseAccessDB.PopulateListApproverDB(UserName);
            supplierModel.ListofPaymentTerms = DatabaseAccessDB.List_PaymentTermsDB();
            supplierModel.SupplierName = DatabaseAccessDB.List_AllSupplierDB();
            supplierModel.Particular = DatabaseAccessDB.List_PopulateItemNameDB();
            supplierModel.ListPRNumber = DatabaseAccessDB.List_AllPRNumberDB();
            return View("~/VIews/Home/Supplier/CreateSupplier.cshtml", supplierModel);



        }

        public ActionResult EditSupplier()
        {
            ViewBag.SupplierMonitoring = true;
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            SupplierModel supplierModel = new SupplierModel();

            var Responsible = Session["UserID"].ToString() ?? ""; // Get userid

            supplierModel.PopulateAllEditSupplier = DatabaseAccessDB.PopulateAllEditSupplierDB(Responsible); // select All edit transanction under userid account
            return View("~/Views/Home/Supplier/EditSupplier.cshtml", supplierModel);
        }

        [HttpPost]
        public JsonResult PopulateListClientReferenceNo(string PONumber)
        {
            SupplierModel supplierModel = new SupplierModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            var data = DatabaseAccessDB.ListClientReferenceNoDB(PONumber);
            return Json(ListStringData = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet);// RENDER VALUE IN AJAX (LINE 333) CreateClient.cshtml

        }

        [HttpPost]
        public JsonResult PopulateListSupplierPONumber(string POC_ReferenceNo)
        {
            SupplierModel supplierModel = new SupplierModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            var data = DatabaseAccessDB.ListSupplierPONumberDB(POC_ReferenceNo);
            return Json(ListStringData = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet);// RENDER VALUE IN AJAX (LINE 333) CreateClient.cshtml

        }

        [HttpGet]
        public ActionResult EditSupplierForm(string id)
        {
            var UserName = "";
            if (Session["UserName"] != null)
            {
                UserName = Session["UserName"].ToString();
            }
            else if (string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/home/index.cshtml");
            }
            SupplierModel supplierModel = new SupplierModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ViewBag.SupplierMonitoring = false;


            supplierModel = DatabaseAccessDB.EditUserSupplierInformationDB(id);
            supplierModel.Particular = DatabaseAccessDB.List_PopulateItemNameDB();
            supplierModel.ListClientReferenceNo = DatabaseAccessDB.List_PopulateClientReferenceNoDB();
            supplierModel.SupplierAddresss = DatabaseAccessDB.PopulateSupplierAddressDB(supplierModel.Supplier);
            supplierModel.PopulateAllItemList = DatabaseAccessDB.PopulateSupplierItemListDB(supplierModel.POSReferenceNo);
            supplierModel.Approver = DatabaseAccessDB.PopulateListApproverDB(UserName);
            supplierModel.Endorser = DatabaseAccessDB.PopulateListApproverDB(UserName);
            supplierModel.ListofPaymentTerms = DatabaseAccessDB.List_PaymentTermsDB();
            supplierModel.SupplierName = DatabaseAccessDB.List_AllSupplierDB();
            supplierModel.ListPRNumber = DatabaseAccessDB.List_GetAllPRNumberDB(supplierModel.PRNumber);
            ViewBag.PONumber = supplierModel.POSupplierNumber;
            return View("~/VIews/Home/Supplier/EditSupplier.cshtml", supplierModel);
        }

        [HttpGet]
        public ActionResult AddSupplier(string id)
        {
            var UserName = "";

            if (Session["UserName"] != null)
            {
                UserName = Session["UserName"].ToString();
            }
            else if (string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/home/index.cshtml");
            }

            SupplierModel supplierModel = new SupplierModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ViewBag.SupplierMonitoring = false;



            supplierModel.POCReferenceNo = id;  // GET CLIENT REFERENCENO

            supplierModel.Particular = DatabaseAccessDB.List_ClientItemListDB(id); // GET ITEM NAME BASE ON CLIENT REFERENCE  NO
            supplierModel.ListLocationAddress = DatabaseAccessDB.List_PopulateAllClientAddressDB(id); // GET CLIENT ADDRESS BASE ON CLIENT REFERENCE NO
            supplierModel.Approver = DatabaseAccessDB.PopulateListApproverDB(""); // POPULATE ALL LIST OF APPROVER
            supplierModel.Endorser = DatabaseAccessDB.PopulateListApproverDB(""); // POPULATE ALL LIST OF ENDORSER
            supplierModel.ListofPaymentTerms = DatabaseAccessDB.List_PaymentTermsDB(); // POPULATE ALL LIST OF ENDORSER
            supplierModel.SupplierName = DatabaseAccessDB.List_AllSupplierDB(); // POPULATE ALL SUPPLIER NAME

            return View("~/VIews/Home/Supplier/CreateSupplier.cshtml", supplierModel); //RENDER VIEW (CreateSupplier.cshtml)
        }

        //-------------------------------------------------------------------*CERTIFICATE OF COMPLETION*-------------------------------------------------------------------------------------------------------//

        public ActionResult AddCertificateOfCompletion(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/home/index.cshtml");
            }

            ViewBag.COCTable = false;
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ClientModel clientModel = new ClientModel();
            clientModel = DatabaseAccessDB.GetPOReferenceNo(id);
            clientModel.ClientNameList = DatabaseAccessDB.List_PopulateClientNameDB();
            clientModel.Approver = DatabaseAccessDB.PopulateListApproverDB(""); // POPULATE ALL LIST OF APPROVER
            clientModel.FullNameResponsible = Session["UserID"].ToString();
            return View("~/Views/Home/Client/CertificateOfCompletion.cshtml", clientModel);
        }

        public ActionResult CertificateOfCompletion()
        {
            ViewBag.COCTable = true;
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ClientModel clientModel = new ClientModel();
            clientModel.PopulateAllReleasingItem = DatabaseAccessDB.PopulateAllCOCClientRequestDB();
            return View("~/Views/Home/Client/CertificateOfCompletion.cshtml", clientModel);
        }

        public ActionResult EditCertificateOfCompletion()
        {
            ViewBag.edit = true;
            var UserID = "";
            if (Session["UserID"] != null)
            {
                UserID = Session["USERID"].ToString();
            }
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ClientModel cocModel = new ClientModel();

            cocModel.PopulateCOCInformation = DatabaseAccessDB.PopulateCOCinformationDB(UserID);
            return View("~/Views/Home/Client/EditCertificateOfCompletion.cshtml", cocModel);
        }

        public ActionResult EditCertificateOfCompletionForm(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/home/index.cshtml");
            }

            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ClientModel cocModel = new ClientModel();
            ViewBag.edit = false;
            cocModel = DatabaseAccessDB.EditCertificateOfCompletionDB(id);
            cocModel.ClientNameList = DatabaseAccessDB.List_PopulateClientNameDB();
            cocModel.Approver = DatabaseAccessDB.PopulateListApproverDB(""); // POPULATE ALL LIST OF APPROVER
            return View("~/Views/Home/Client/EditCertificateOfCompletion.cshtml", cocModel);
        }


        //-------------------------------------------------------------------I*NQUIRY*-------------------------------------------------------------------------------------------------------//
        

        public ActionResult InquiryPurchaseRequisition()
        {
            SupplierModel supplierModel = new SupplierModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            string Responsible = Session["UserID"].ToString() ?? "0";
            supplierModel.PopulateAllPurchaseRequisition = DatabaseAccessDB.PopulateInquiryPurchaseRequisitionDB(Responsible);
            return View("~/Views/Home/Inquiry/Inquiry_PurchaseRequisition.cshtml", supplierModel);
        }

        public ActionResult InquirySupplier()
        {
            SupplierModel supplierModel = new SupplierModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            string Responsible = Session["UserID"].ToString() ?? "0";
            supplierModel.PopulateSupplierDetails = DatabaseAccessDB.PopulateInquirySupplierDB(Responsible);
            return View("~/Views/Home/Inquiry/Inquiry_Supplier.cshtml", supplierModel);
        }

        public ActionResult InquiryClient()
        {
            ClientModel clientModel = new ClientModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            string Responsible = Session["UserID"].ToString() ?? "0";
            clientModel.PopulateClientdetails = DatabaseAccessDB.PopulateInquiryClientDB(Responsible);
            clientModel.SupplierPONumberList = DatabaseAccessDB.List_SupplierPONumberDB();

            return View("~/Views/Home/Inquiry/Inquiry_Client.cshtml", clientModel);
        }

        public ActionResult InquiryCertificateOfCompletion()
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ClientModel clientModel = new ClientModel();
            string Responsible = Session["UserID"].ToString() ?? "0";
            clientModel.PopulateCOCInformation = DatabaseAccessDB.PopulateInquiryCertificateOfCompletionDB(Responsible);
            return View("~/Views/Home/Inquiry/Inquiry_CertificateOfCompletion.cshtml", clientModel);
        }

        [HttpGet]
        public JsonResult PopulateListOfPoNumber()
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            var data = DatabaseAccessDB.List_SupplierPONumberDB();

            return Json(Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet);// RENDER VALUE IN AJAX (LINE 333) CreateClient.cshtml

        }

        [HttpGet]
        public JsonResult List_PopulateClientReferenceNo()
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            var data = DatabaseAccessDB.List_PopulateClientReferenceNoDB();

            return Json(Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet);// RENDER VALUE IN AJAX (LINE 333) CreateClient.cshtml

        }
        //-------------------------------------------------------------------*DELIVERY RECEIPT*-------------------------------------------------------------------------------------------------------//


        public ActionResult ReceivedLogs()
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            DRModel model = new DRModel();
            ViewBag.ReceivedLogs = true;

            model.PopulateReceivedLogs = DatabaseAccessDB.PopulateReceivedLogsDB();
            return View("~/Views/home/DeliveryReceipt/ReceivedLogs.cshtml", model);

        }

        public ActionResult ReleasedLogs()
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            DRModel model = new DRModel();
            ViewBag.ReleasedLogs = true;
            model.PopulateReleasedLogs = DatabaseAccessDB.PopulateRelasedLogsDB();
            return View("~/Views/home/DeliveryReceipt/ReleasedLogs.cshtml", model);


        }

        public ActionResult ItemReturnedLogs()
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            DRModel model = new DRModel();
            model.PopulateItemReturnedLogs = DatabaseAccessDB.PopulateItemReturnedLogsDB();
            return View("~/Views/home/DeliveryReceipt/ItemReturned_Logs.cshtml", model);


        }

        [HttpPost]
        public JsonResult PopulateItemReleasing(string StringPOCReferenceNo, string Location)
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();


            var Result = DatabaseAccessDB.Select_CheckIfAllItemReleasedDB(StringPOCReferenceNo);


            if (Result == "CLOSED")
            {
                return new JsonResult { Data = Result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {

                //if (TypeOf_DR == "RECEIVING")
                //{
                //	var list = DatabaseAccessDB.GetItemReceivedDB(StringPOCReferenceNo);
                //	return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                //}
                //else
                //{
                var list = DatabaseAccessDB.GetItemRelasedDB(StringPOCReferenceNo, Location);

                return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                //}
            }



        }

        [HttpPost]
        public JsonResult PopulateItemReceiving(string StringPOSReferenceNo)
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();


            var Result = DatabaseAccessDB.Select_CheckIfAllItemReceivedDB(StringPOSReferenceNo);


            if (Result == "CLOSED")
            {
                return new JsonResult { Data = Result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {

                var list = DatabaseAccessDB.GetItemReceivedDB(StringPOSReferenceNo);
                return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }



        }

        [HttpGet]
        public ActionResult DR_RELEASING()
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ClientModel clientmodel = new ClientModel();
            clientmodel.DeliveryNo = DatabaseAccessDB.AutoGenerateDRNumbersDB();
            clientmodel.PopulateAllReleasingItem = DatabaseAccessDB.PopulateAllClientRequestDB();
            return View("/VIews/Home/DeliveryReceipt/DR_Releasing.cshtml", clientmodel);
        }

        [HttpGet]
        public ActionResult DR_RECEIVING()
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            SupplierModel supplierModel = new SupplierModel();
            supplierModel.PopulateAllReceivingItem = DatabaseAccessDB.PopulateAllDRApprovedSupplierRequestDB();
            return View("/VIews/Home/DeliveryReceipt/DR_Receiving.cshtml", supplierModel);
        }

        [HttpGet]
        public ActionResult RelasedLogs_DR(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/home/index.cshtml");
            }
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            DRModel model = new DRModel();
            ViewBag.ReleasedLogs = false;
            model.PopulateReleasedLogs = DatabaseAccessDB.PopulateDRRelasedLogsDB(id);
            return View("~/Views/home/DeliveryReceipt/ReleasedLogs.cshtml", model);

        }

        [HttpGet]
        public ActionResult ReceivedLogs_DR(string id, string id3)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/home/index.cshtml");
            }

            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            DRModel model = new DRModel();
            ViewBag.ReceivedLogs = false;
            model.PopulateReceivedLogs = DatabaseAccessDB.PopulateDRReceivedLogsDB(id, id3);
            return View("~/Views/home/DeliveryReceipt/ReceivedLogs.cshtml", model);

        }


        //-------------------------------------------------------------------P*ROJECT MONITORING*-------------------------------------------------------------------------------------------------------//


        public ActionResult ProjectMonitoring()
        {
            ViewBag.FormsClientTransaction = false;
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            FormControlModel formControlModel = new FormControlModel();
            formControlModel.Populate_POInformation = DatabaseAccessDB.PopulateAllPOInformationDB();
            return View("/VIews/Home/FormControl/ClientProjectMonitoring.cshtml", formControlModel);
            //return View("/VIews/Home/MaintenancePage.cshtml");

        }

        public ActionResult PO_Monitoring()
        {
            ViewBag.FormsClientTransaction = false;
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            FormControlModel formControlModel = new FormControlModel();
            formControlModel.Populate_Clienttransaction = DatabaseAccessDB.PopulateClientSupplierTransactionDB();
            return View("/VIews/Home/FormControl/SupplierProjectMonitoring.cshtml", formControlModel);
            //return View("/VIews/Home/MaintenancePage.cshtml");

        }

        public ActionResult PR_Monitoring()
        {
            ViewBag.FormsClientTransaction = false;
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            SupplierModel formControlModel = new SupplierModel();
            formControlModel.Populate_PRtransaction = DatabaseAccessDB.PopulatePRTransactionDB();
            return View("/VIews/Home/FormControl/PRProjectMonitoring.cshtml", formControlModel);
            //return View("/VIews/Home/MaintenancePage.cshtml");

        }
        


        //-------------------------------------------------------------------*ACCOUNTING*-------------------------------------------------------------------------------------------------------//

        public ActionResult ProjectCostReport()
        {
            ViewBag.ClientMonitoring = true;
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ClientModel clientModel = new ClientModel();

            clientModel.PopulateAllEditClient = DatabaseAccessDB.PopulateProjectCostReportDB(); // Select All Client transaction under userid account

            return View("~/Views/Home/Accounting/ProjectCost.cshtml", clientModel); // Return edit page.
        }

        public ActionResult AccountingLogs()
        {
            AccountingModel main = new AccountingModel();

            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            main.PopulateAccountingLogs = DatabaseAccessDB.PopulateAccountingLogs();
            return View("~/Views/Home/Accounting/AccountingMovements.cshtml", main);
        }

        public ActionResult PopulatePayablesReport()
        {
            AccountingModel main = new AccountingModel();

            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();

            main.PopulateEditPayables = DatabaseAccessDB.PopulatefilterpayablesReportDB();
            return View("~/Views/Home/Accounting/PayablesReport.cshtml", main);

        }

        public ActionResult PopulateReceivablesReport()
        {
            AccountingModel main = new AccountingModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();

            main.PopulateEditReceivables = DatabaseAccessDB.PopulatefilterreceivablesReportDB();

            return View("~/Views/Home/Accounting/ReceivablesReport.cshtml", main);
        }

        public ActionResult RECEIVABLES()
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ClientModel clientmodel = new ClientModel();
            ViewBag.Receivables = true;
            clientmodel.PopulateClientdetails = DatabaseAccessDB.PopulateClientReceivablesDB();
            return View("~/Views/Home/Accounting/Receivables.cshtml", clientmodel);


        }

        public ActionResult PAYABLES()
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            SupplierModel suppliermodel = new SupplierModel();
            ViewBag.Payables = true;
            suppliermodel.PopulateSupplierDetails = DatabaseAccessDB.PopulateSupplierPayablesDB();
            return View("~/Views/Home/Accounting/Payables.cshtml", suppliermodel);

        }
        
        public ActionResult EDITRECEIVABLES()
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            AccountingModel Accountingmodel = new AccountingModel();
            ViewBag.Receivables = true;
            Accountingmodel.PopulateEditReceivables = DatabaseAccessDB.PopulateReceivablesInformationDB();
            return View("~/Views/Home/Accounting/EditReceivables.cshtml", Accountingmodel);

        }

        public ActionResult EDITPAYABLES()
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            AccountingModel Accountingmodel = new AccountingModel();
            ViewBag.Payables = true;
            Accountingmodel.PopulateEditPayables = DatabaseAccessDB.PopulatePayablesInformationDB();
            return View("~/Views/Home/Accounting/EditPayables.cshtml", Accountingmodel);
        }

        public ActionResult CLIENT_MONITORING()
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            AccountingModel Accountingmodel = new AccountingModel();
            ViewBag.AccountingMonitoring = true;
            Accountingmodel.PopulateAccountingMonitoring = DatabaseAccessDB.PopulateClientMonitoringDB();
            return View("~/Views/Home/Accounting/ClientMonitoring.cshtml", Accountingmodel);
        }

        public ActionResult SUPPLIER_MONITORING()
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            AccountingModel Accountingmodel = new AccountingModel();
            ViewBag.AccountingMonitoring = true;
            Accountingmodel.PopulateAccountingMonitoring = DatabaseAccessDB.PopulateSupplierMonitoringDB();
            return View("~/Views/Home/Accounting/SupplierMonitoring.cshtml", Accountingmodel);
        }

        [HttpGet]
        public ActionResult GETRECEIVABLES(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/home/index.cshtml");
            }

            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ClientModel clientmodel = new ClientModel();
            ViewBag.Receivables = false;

            string Types = "";
            string Particulars = "";

            clientmodel = DatabaseAccessDB.GetInformationReceivablesDB(id);
            clientmodel.ListofPaymentTerms = DatabaseAccessDB.PopulateClientPaymentTermsDB(id);
            clientmodel.ListofClientBilling = DatabaseAccessDB.List_UpdateClientBillingDB(id);



            foreach (var item in clientmodel.ListofPaymentTerms)
            {
                Particulars = item.Value;
            }
            clientmodel.ListBillingType = DatabaseAccessDB.PopulateClientBillingTypeDB(id, Particulars);

            foreach (var item in clientmodel.ListBillingType)
            {
                Types = item.Value;
            }

            clientmodel.ACC_Responsible = Session["UserFullName"].ToString();
            return View("~/Views/Home/Accounting/Receivables.cshtml", clientmodel);
        }

        [HttpPost]
        public JsonResult GetBillingTypeReceivables(string TypeOfNumber, string ACC_TypeOfTerms, string POC_ReferenceNo)
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            var list = DatabaseAccessDB.GetBillClientDB(TypeOfNumber, ACC_TypeOfTerms, POC_ReferenceNo);
            return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult GetBillingTypePayables(string BillingType, string TypeOfTerms, string POSReferenceNo)
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            var list = DatabaseAccessDB.GetBillSupplierDB(BillingType, TypeOfTerms, POSReferenceNo);
            return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public ActionResult GetSupplierPaymentDetails(string id)
        {

            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            SupplierModel model = new SupplierModel();
            model = DatabaseAccessDB.GetPayablesPaymentDetailsDB(id);
            model.ACC_Responsible = Session["UserFullName"].ToString();
            model.ListofSupplierPayment = DatabaseAccessDB.List_UpdateSupplierPaymentDB(model.ACC_POSReference);

            return View("~/Views/Home/Accounting/UpdatePayment.cshtml", model);

        }

        [HttpGet]
        public ActionResult GetClientBillingDetails(string id)
        {

            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ClientModel clientmodel = new ClientModel();
            clientmodel = DatabaseAccessDB.GetReceivablesBillingDetailsDB(id);
            clientmodel.ACC_Responsible = Session["UserFullName"].ToString();
            string ReferenceNo = clientmodel.ACC_ReferenceNo;
            clientmodel.ListofClientBilling = DatabaseAccessDB.List_UpdateClientBillingDB(ReferenceNo);

            return View("~/Views/Home/Accounting/UpdateBilling.cshtml", clientmodel);

        }

        [HttpGet]
        public ActionResult GETPAYABLES(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/home/index.cshtml");
            }

            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            SupplierModel suppliermodel = new SupplierModel();
            ViewBag.Payables = false;


            suppliermodel = DatabaseAccessDB.GetInformationPayablesDB(id);
            suppliermodel.ACC_Responsible = Session["UserFullName"].ToString();
            suppliermodel.ListofPaymentTerms = DatabaseAccessDB.PopulateSupplierPaymentTermsDB(id);
            suppliermodel.ListofSupplierPayment = DatabaseAccessDB.List_UpdateSupplierPaymentDB(id);
            string Types = "";
            string Particulars = "";



            foreach (var item in suppliermodel.ListofPaymentTerms)
            {
                Particulars = item.Value;
            }
            suppliermodel.ListBillingType = DatabaseAccessDB.PopulateSupplierBillingTypeDB(id, Particulars);

            foreach (var item in suppliermodel.ListBillingType)
            {
                Types = item.Value;
            }





            return View("~/Views/Home/Accounting/Payables.cshtml", suppliermodel);

        }

        [HttpGet]
        public ActionResult GETEDITRECEIVABLES(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/home/index.cshtml");
            }

            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            AccountingModel Accountingmodel = new AccountingModel();
            ViewBag.Receivables = false;
            Accountingmodel = DatabaseAccessDB.EditReceivablesInformationDB(id);
            return View("~/Views/Home/Accounting/EditReceivables.cshtml", Accountingmodel);

        }

        [HttpGet]
        public ActionResult GETEDITPAYBLES(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/home/index.cshtml");
            }

            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            AccountingModel Accountingmodel = new AccountingModel();
            ViewBag.Payables = false;
            Accountingmodel = DatabaseAccessDB.EditPayablesInformationDB(id);
            return View("~/Views/Home/Accounting/EditPayables.cshtml", Accountingmodel);

        }

        [HttpGet]
        public ActionResult GETSUPPLIER_MONITORING(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/home/index.cshtml");
            }
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            AccountingModel Accountingmodel = new AccountingModel();
            ViewBag.AccountingMonitoring = false;
            Accountingmodel.PopulateEditPayables = DatabaseAccessDB.GetSupplierMonitoringDB(id);
            return View("~/Views/Home/Accounting/SupplierMonitoring.cshtml", Accountingmodel);

        }

        [HttpGet]
        public ActionResult GETCLIENT_MONITORING(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/home/index.cshtml");
            }
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            AccountingModel Accountingmodel = new AccountingModel();
            ViewBag.AccountingMonitoring = false;
            Accountingmodel.PopulateEditReceivables = DatabaseAccessDB.GetClientMonitoringDB(id);
            return View("~/Views/Home/Accounting/ClientMonitoring.cshtml", Accountingmodel);

        }

        [HttpPost]
        public JsonResult GetPopulatePONumberSupplier(string POREFERENCENO)
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            var list = DatabaseAccessDB.GetPopulatePoNumberSupplier(POREFERENCENO);
            return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult GetPopulatePONumberClient(string POREFERENCENO)
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            var list = DatabaseAccessDB.GetPopulatePoNumberClient(POREFERENCENO);
            return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //-------------------------------------------------------------------*ROUNTING APPROVAL*-------------------------------------------------------------------------------------------------------//

        [CheckAuthorization]
        public ActionResult COCFORAPPROVAL()
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ClientModel clientModel = new ClientModel();
            var UserID = Session["UserID"].ToString();
            ViewBag.CoCMonitoring = true;
            clientModel.PopulateCertificateOfCompletion = DatabaseAccessDB.PopulateCertificateOfCompletionDB(UserID);
            return View("~/Views/RoutingApproval/CoCForRoutingApproval.cshtml", clientModel);


        }

        [CheckAuthorization]
        public ActionResult FORENDORSEMENT()
        {
            SupplierModel supplierModel = new SupplierModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            int USERID = Convert.ToInt32(Session["UserID"].ToString() ?? "0");
            if (USERID == 0)
            {
                return View("~/Views/home/UserLogin.cshtml");
            }
            supplierModel.PopulateRoutingApproval = DatabaseAccessDB.PopulateForRoutingApprovalDB(USERID, "FOR ENDORSEMENT");
            ViewBag.SupplierMonitoring = true;
            return View("~/VIews/RoutingApproval/ForEndorsement.cshtml", supplierModel);

        }

        [CheckAuthorization]
        public ActionResult FORREVIEW()
        {
            SupplierModel supplierModel = new SupplierModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            int USERID = Convert.ToInt32(Session["UserID"].ToString() ?? "0");
            if (USERID == 0)
            {
                return View("~/Views/home/UserLogin.cshtml");
            }
            supplierModel.PopulateRoutingApproval = DatabaseAccessDB.PopulateForRoutingApprovalDB(USERID, "FOR REVIEW");
            ViewBag.SupplierMonitoring = true;
            return View("~/VIews/RoutingApproval/ForReview.cshtml", supplierModel);

        }

        [CheckAuthorization]
        public ActionResult FORAPPROVAL()
        {
            ViewBag.SupplierMonitoring = true;
            SupplierModel supplierModel = new SupplierModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            int USERID = Convert.ToInt32(Session["UserID"].ToString() ?? "0");
            if (USERID == 0)
            {
                return View("~/Views/home/UserLogin.cshtml");
            }
            supplierModel.PopulateRoutingApproval = DatabaseAccessDB.PopulateForRoutingApprovalDB(USERID, "FOR APPROVAL");
            return View("~/VIews/RoutingApproval/ForApproval.cshtml", supplierModel);
        }

        [CheckAuthorization]
        public ActionResult PR_FORREVIEW()
        {
            SupplierModel supplierModel = new SupplierModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            int USERID = Convert.ToInt32(Session["UserID"].ToString() ?? "0");
            if (USERID == 0)
            {
                return View("~/Views/home/UserLogin.cshtml");
            }
            supplierModel.PopulateRoutingApproval = DatabaseAccessDB.PopulateForPRRoutingApprovalDB(USERID, "FOR REVIEW");
            ViewBag.RequisitionMonitoring = true;
            return View("~/VIews/RoutingApproval/PR_ForReview.cshtml", supplierModel);

        }
        [CheckAuthorization]
        public ActionResult PR_FORENDORSEMENT()
        {
            SupplierModel supplierModel = new SupplierModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            int USERID = Convert.ToInt32(Session["UserID"].ToString() ?? "0");
            if (USERID == 0)
            {
                return View("~/Views/home/UserLogin.cshtml");
            }
            supplierModel.PopulateRoutingApproval = DatabaseAccessDB.PopulateForPRRoutingApprovalDB(USERID, "FOR ENDORSEMENT");
            ViewBag.RequisitionMonitoring = true;
            return View("~/VIews/RoutingApproval/PR_ForEndorsement.cshtml", supplierModel);

        }

        [CheckAuthorization]
        public ActionResult PR_FORAPPROVAL()
        {
            SupplierModel supplierModel = new SupplierModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            int USERID = Convert.ToInt32(Session["UserID"].ToString() ?? "0");
            if (USERID == 0)
            {
                return View("~/Views/home/UserLogin.cshtml");
            }
            supplierModel.PopulateRoutingApproval = DatabaseAccessDB.PopulateForPRRoutingApprovalDB(USERID, "FOR APPROVAL");
            ViewBag.RequisitionMonitoring = true;
            return View("~/VIews/RoutingApproval/PR_ForApproval.cshtml", supplierModel);

        }

        [CheckAuthorization]
        [HttpGet]
        public ActionResult GetPR_ForReviewDetails(string id)
        {
            var UserName = "";
            var UserID = "";

            if (Session["UserName"] != null)
            {
                UserName = Session["UserName"].ToString();
                UserID = Session["UserID"].ToString();
            }
            else if (string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/home/index.cshtml");
            }


            SupplierModel supplierModel = new SupplierModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ViewBag.RequisitionMonitoring = false;




            supplierModel = DatabaseAccessDB.GetPurchaseRequisitionInformationDB(id);


            if (supplierModel.FormStatus != "FOR REVIEW" || UserID != supplierModel.ReviewerID)
            {
                return View("~/Views/home/index.cshtml");

            }

            supplierModel.PopulateAllItemList = DatabaseAccessDB.GetPurchaseRequisition_GRIDDB(id);
            supplierModel.Approver = DatabaseAccessDB.PopulateListApproverDB("");
            supplierModel.Endorser = DatabaseAccessDB.PopulateListApproverDB("");
            supplierModel.ListAllUser = DatabaseAccessDB.PopulateListAllUserDB();

            supplierModel.SupplierName = DatabaseAccessDB.List_AllSupplierDB();
            supplierModel.Particular = DatabaseAccessDB.List_PopulateItemNameDB();


            return View("~/VIews/RoutingApproval/PR_ForReview.cshtml", supplierModel);
        }

        [CheckAuthorization]
        [HttpGet]
        public ActionResult GetPR_ForEndorsementDetails(string id)
        {
            var UserName = "";
            var UserID = "";

            if (Session["UserName"] != null)
            {
                UserName = Session["UserName"].ToString();
                UserID = Session["UserID"].ToString();
            }
            else if (string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/home/index.cshtml");
            }


            SupplierModel supplierModel = new SupplierModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ViewBag.RequisitionMonitoring = false;




            supplierModel = DatabaseAccessDB.GetPurchaseRequisitionInformationDB(id);


            if (supplierModel.FormStatus != "FOR ENDORSEMENT" || UserID != supplierModel.EndorserID)
            {
                return View("~/Views/home/index.cshtml");

            }

            supplierModel.PopulateAllItemList = DatabaseAccessDB.GetPurchaseRequisition_GRIDDB(id);
            supplierModel.Approver = DatabaseAccessDB.PopulateListApproverDB("");
            supplierModel.Endorser = DatabaseAccessDB.PopulateListApproverDB("");
            supplierModel.ListAllUser = DatabaseAccessDB.PopulateListAllUserDB();

            supplierModel.SupplierName = DatabaseAccessDB.List_AllSupplierDB();
            supplierModel.Particular = DatabaseAccessDB.List_PopulateItemNameDB();


            return View("~/VIews/RoutingApproval/PR_ForEndorsement.cshtml", supplierModel);
        }

        [CheckAuthorization]
        [HttpGet]
        public ActionResult GetPR_ForApprovalDetails(string id)
        {
            var UserName = "";
            var UserID = "";

            if (Session["UserName"] != null)
            {
                UserName = Session["UserName"].ToString();
                UserID = Session["UserID"].ToString();
            }
            else if (string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/home/index.cshtml");
            }


            SupplierModel supplierModel = new SupplierModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ViewBag.RequisitionMonitoring = false;




            supplierModel = DatabaseAccessDB.GetPurchaseRequisitionInformationDB(id);


            if (supplierModel.FormStatus != "FOR APPROVAL" || UserID != supplierModel.ApproverID)
            {
                return View("~/Views/home/index.cshtml");

            }

            supplierModel.PopulateAllItemList = DatabaseAccessDB.GetPurchaseRequisition_GRIDDB(id);
            supplierModel.Approver = DatabaseAccessDB.PopulateListApproverDB("");
            supplierModel.Endorser = DatabaseAccessDB.PopulateListApproverDB("");
            supplierModel.ListAllUser = DatabaseAccessDB.PopulateListAllUserDB();
            supplierModel.SupplierName = DatabaseAccessDB.List_AllSupplierDB();
            supplierModel.Particular = DatabaseAccessDB.List_PopulateItemNameDB();


            return View("~/VIews/RoutingApproval/PR_ForApproval.cshtml", supplierModel);
        }

        [CheckAuthorization]
        [HttpGet]
        public ActionResult GetForReviewDetails(string id)
        {
            var UserName = "";
            var UserID = "";

            if (Session["UserName"] != null)
            {
                UserName = Session["UserName"].ToString();
                UserID = Session["UserID"].ToString();
            }
            else if (string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/home/index.cshtml");
            }


            SupplierModel supplierModel = new SupplierModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ViewBag.SupplierMonitoring = false;

            supplierModel = DatabaseAccessDB.EditUserSupplierInformationDB(id);
            var Status = DatabaseAccessDB.CheckSupplierFormStatusDB(id, supplierModel.FullName);

            if (Status != "FOR REVIEW" || UserID != supplierModel.ReviewerID)
            {
                return View("~/Views/home/index.cshtml");

            }

            supplierModel.Particular = DatabaseAccessDB.List_PopulateItemNameDB();
            supplierModel.ListClientReferenceNo = DatabaseAccessDB.List_PopulateClientReferenceNoDB();
            supplierModel.SupplierAddresss = DatabaseAccessDB.PopulateSupplierAddressDB(supplierModel.Supplier);
            supplierModel.PopulateAllItemList = DatabaseAccessDB.Select_RoutingApprovalDetails_GRIDDB(id);
            supplierModel.Approver = DatabaseAccessDB.PopulateListApproverDB("");
            supplierModel.Endorser = DatabaseAccessDB.PopulateListApproverDB("");
            supplierModel.ListofPaymentTerms = DatabaseAccessDB.List_PaymentTermsDB();
            supplierModel.SupplierName = DatabaseAccessDB.List_AllSupplierDB();
            ViewBag.PONumber = supplierModel.POSupplierNumber;


            return View("~/VIews/RoutingApproval/ForReview.cshtml", supplierModel);
        }

        [CheckAuthorization]
        [HttpGet]
        public ActionResult GetForEndorsementDetails(string id)
        {


            var UserName = "";
            var UserID = "";

            if (Session["UserName"] != null)
            {
                UserName = Session["UserName"].ToString();
                UserID = Session["UserID"].ToString();
            }
            else if (string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/home/index.cshtml");
            }



            SupplierModel supplierModel = new SupplierModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();



            ViewBag.SupplierMonitoring = false;
            supplierModel = DatabaseAccessDB.EditUserSupplierInformationDB(id);
            var Status = DatabaseAccessDB.CheckSupplierFormStatusDB(id, supplierModel.FullName);



            if (Status != "FOR ENDORSEMENT" || UserID != supplierModel.EndorserID)
            {
                return View("~/Views/home/index.cshtml");

            }

            supplierModel.Particular = DatabaseAccessDB.List_PopulateItemNameDB();
            supplierModel.ListClientReferenceNo = DatabaseAccessDB.List_PopulateClientReferenceNoDB();
            supplierModel.SupplierAddresss = DatabaseAccessDB.PopulateSupplierAddressDB(supplierModel.Supplier);
            supplierModel.PopulateAllItemList = DatabaseAccessDB.Select_RoutingApprovalDetails_GRIDDB(id);
            supplierModel.Approver = DatabaseAccessDB.PopulateListApproverDB("");
            supplierModel.Endorser = DatabaseAccessDB.PopulateListApproverDB("");
            supplierModel.ListofPaymentTerms = DatabaseAccessDB.List_PaymentTermsDB();
            supplierModel.SupplierName = DatabaseAccessDB.List_AllSupplierDB();
            ViewBag.PONumber = supplierModel.POSupplierNumber;
            return View("~/VIews/RoutingApproval/ForEndorsement.cshtml", supplierModel);
        }

        [CheckAuthorization]
        [HttpGet]
        public ActionResult GetForApprovalDetails(string id)
        {
            var UserName = "";
            var UserID = "";

            if (Session["UserName"] != null)
            {
                UserName = Session["UserName"].ToString();
                UserID = Session["UserID"].ToString();
            }
            else if (string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/home/index.cshtml");
            }


            SupplierModel supplierModel = new SupplierModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ViewBag.SupplierMonitoring = false;


            supplierModel = DatabaseAccessDB.EditUserSupplierInformationDB(id);
            var Status = DatabaseAccessDB.CheckSupplierFormStatusDB(id, supplierModel.FullName);


            if (Status != "FOR APPROVAL" || UserID != supplierModel.ApproverID)
            {
                return View("~/Views/home/index.cshtml");

            }


            supplierModel.Particular = DatabaseAccessDB.List_PopulateItemNameDB();
            supplierModel.ListClientReferenceNo = DatabaseAccessDB.List_PopulateClientReferenceNoDB();
            supplierModel.SupplierAddresss = DatabaseAccessDB.PopulateSupplierAddressDB(supplierModel.Supplier);
            supplierModel.PopulateAllItemList = DatabaseAccessDB.Select_RoutingApprovalDetails_GRIDDB(id);
            supplierModel.Approver = DatabaseAccessDB.PopulateListApproverDB("");
            supplierModel.Endorser = DatabaseAccessDB.PopulateListApproverDB("");
            supplierModel.ListofPaymentTerms = DatabaseAccessDB.List_PaymentTermsDB();
            supplierModel.SupplierName = DatabaseAccessDB.List_AllSupplierDB();
            ViewBag.PONumber = supplierModel.POSupplierNumber;
            return View("~/VIews/RoutingApproval/ForApproval.cshtml", supplierModel);
        }

        [CheckAuthorization]
        [HttpGet]
        public ActionResult GetCertificateOfCompletion(string id)
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ClientModel clientModel = new ClientModel();
            var FullName = "";

            if (Session["UserName"] != null)
            {
                FullName = Session["UserFullName"].ToString();
            }
            else if (string.IsNullOrWhiteSpace(id))
            {
                return View("~/Views/home/index.cshtml");
            }

            clientModel = DatabaseAccessDB.EditRoutingCertificateOfCompletionDB(id);
            var Status = DatabaseAccessDB.CheckCetificateOfCompletionFormStatusDB(id,clientModel.COC_Resposible);



            if (Status != "FOR APPROVAL" || FullName != clientModel.ApproverID)
            {
                return View("~/Views/home/index.cshtml");
            }

            ViewBag.CoCMonitoring = false;
            return View("~/Views/RoutingApproval/CoCForRoutingApproval.cshtml", clientModel);
        }

        //-------------------------------------------------------------------*MAINTENANCE*-------------------------------------------------------------------------------------------------------//

        [HttpPost]
        public JsonResult UserResetPassword(string UserID, string EmployeeID, string UserName, string Fullname)
        {
            var res = 0;
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            res = DatabaseAccessDB.ResetPasswordDB(EmployeeID, UserName); // Update password Base on Employee ID and User Name 

            if (res != 0)
            {
                SMTPModel serverModel = new SMTPModel();

                //SELECT SMTP INFORMATION
                serverModel = DatabaseAccessDB.PopulateSMTPInformation();

                var GetRequesterEmail = DatabaseAccessDB.EmailNotificationDB(UserID);

                //GET APPROVER EMAIL


                if (serverModel.SMTP_Status != "2")
                {
                    try
                    {
                        //Send Email to Requester
                        //create emailmessage
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient();
                        //email  output
                        mail.From = new MailAddress(serverModel.SMTP_Email, "E-Procurement");

                        mail.To.Add("" + GetRequesterEmail + "");
                        mail.Subject = "Password Reset: Successful (Date:" + DateTime.Now.ToShortDateString() + ")";
                        mail.Body = "Hi " + Fullname + ", <br /> Your e-Procurement password has been Updated.<br /> <br /> <b> Don't recognize this activity ? </b> <br /> Please report it to support@archangeltech.com.ph to notify us and we will secure your account.";
                        mail.IsBodyHtml = true;
                        //email connection
                        SmtpServer = new SmtpClient("" + serverModel.SMTP_Server + "");
                        SmtpServer.Port = Convert.ToInt32(serverModel.SMTP_Port);
                        SmtpServer.Credentials = new NetworkCredential("" + serverModel.SMTP_Email + "", "" + serverModel.SMTP_Password + "");

                        //email sender
                        SmtpServer.Send(mail);
                    }
                    catch
                    {
                        RedirectToAction("SendingEmailError", "Home");
                    }

                }
            }

            return Json(res); // Render Information in View Ajax (Linek 272)
        }
        
        [HttpGet]
        public ActionResult EditUserForm(int? id)
        {
            var UserName = "";
            if (Session["UserName"] != null)
            {
                UserName = Session["UserName"].ToString();
            }
            else if (string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                return View("~/Views/home/index.cshtml");
            }

            ViewBag.EditUserForm = true;
            //Call Database Connection in (DatabaseAccess.cs)
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            //Call Model in (MainModel.cs / Public Class UserMaintenance)
            UserMaintenance UserModel = new UserMaintenance();

            //Get and Retreive User Information
            UserModel = DatabaseAccessDB.EditUserInformationDB(id);
            Session["id"] = id;
            // Populate List of Deparment
            UserModel.ListAllDepartment = DatabaseAccessDB.PopulateListDepartmentDB();
            // Populate List of User Role
            UserModel.ListAllUserRole = DatabaseAccessDB.PopulateListUserRoleDB();
            // Populate List of Approver
            UserModel.ListAllApprover = DatabaseAccessDB.PopulateListApproverDB(UserName);

            UserModel.ListAllUserGroup = DatabaseAccessDB.PopulateListUserGroupDB(Convert.ToString(UserModel.UserRoleID));

            UserModel.ListAllUserGroup = DatabaseAccessDB.PopulateAllListUserGroupDB(); // populate all list user group
            return View("~/VIews/Home/Maintenance/EditUser.cshtml", UserModel);
        }

        [HttpGet]
        public ActionResult EditItemMaster(int? id)
        {
            if (string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                return View("~/Views/home/index.cshtml");
            }
            ViewBag.EditItemMaster = true; // Show Edit Item Master Form. 
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ItemMaster itemMaster = new ItemMaster();
            itemMaster = DatabaseAccessDB.EditItemInformationDB(id); // Get Item Information Base on Item ID       
            itemMaster.ItemCodeList = DatabaseAccessDB.List_ItemCode();
            itemMaster.ItemListMeasure = DatabaseAccessDB.PopulateListofMeasurementDB(); // Populate All List Measure Unit
            itemMaster.ItemListCategory = DatabaseAccessDB.List_PopulateAllItemCategoryDB();
            return View("~/VIews/Home/MasterList/ItemMaster.cshtml", itemMaster); // Render ItemMaster.cshtml

        }

        [HttpGet]
        public ActionResult EditClientMaster(int? id, ClientMaster clientMaster)
        {
            if (string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                return View("~/Views/home/index.cshtml");
            }

            ViewBag.ClientForm = true; // Show Edit Client Form 
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ClientMaster clientmodel = new ClientMaster();
            //Get and Retrieve Client Information 
            clientmodel = DatabaseAccessDB.EditClientInformationDB(id); // Get Client Information Base on Client ID
            clientmodel.HiddenClient = clientmodel.Client;
            clientmodel.PopulateAllClientsAddress = DatabaseAccessDB.PopulateClientAddressDB(id); //  Populate Client Address Under Client Name

            return View("~/VIews/Home/MasterList/EditClientMaster.cshtml", clientmodel);

        }

        [HttpGet]
        public ActionResult EditSupplierMaster(int? id)
        {
            if (string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                return View("~/Views/home/index.cshtml");
            }
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            var SupplierInformation = DatabaseAccessDB.EditSupplierInformationDB(id); // Populate Supplier information base on supplier id
            return View("~/VIews/Home/MasterList/EditSupplierMaster.cshtml", SupplierInformation); // render information in view (editSupplierMaster.cshtml)
        }

        public ActionResult SMTP()
        {
            SMTPModel serverModel = new SMTPModel();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();

            serverModel = DatabaseAccessDB.PopulateSMTPInformation();
            return View("~/Views/home/SMTP/SmtpServer.cshtml", serverModel);
        }

        public ActionResult TestEmailForm()
        {
            return View("~/Views/home/SMTP/SMTP_TestEmail.cshtml");
        }

        public ActionResult CreateUser()
        {



            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            UserMaintenance UserModel = new UserMaintenance();
            var UserName = "";
            if (Session["UserName"] != null)
            {
                UserName = Session["UserName"].ToString();
            }
            UserModel.ListAllDepartment = DatabaseAccessDB.PopulateListDepartmentDB();
            UserModel.ListAllUserRole = DatabaseAccessDB.PopulateListUserRoleDB();
            UserModel.ListAllUserGroup = DatabaseAccessDB.PopulateAllListUserGroupDB();
            UserModel.ListAllApprover = DatabaseAccessDB.PopulateListApproverDB(UserName);

            //Validation if  Successfully Inserted or Updated.
            var DataInserted = Session["DataInserted"] ?? "";
            if (DataInserted != "")
            {
                Session["DataInserted"] = null;
                ViewBag.HeaderSuccess = string.Format("{0}", UserModel.UserName);

            }
            return View("~/VIews/Home/Maintenance/CreateUser.cshtml", UserModel);
        }

        public ActionResult EditUser()
        {
            ViewBag.EditUserMonitoring = true;
            UserMaintenance UserModel = new UserMaintenance();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            UserModel.PopulateAllUserInformation = DatabaseAccessDB.POpulateAllUserInformationDB();
            return View("~/VIews/Home/Maintenance/EditUser.cshtml", UserModel);
        }

        public ActionResult ItemMaster()
        {


            ViewBag.ItemMonitoring = true;
            ItemMaster itemModel = new ItemMaster();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            itemModel.PopulateAll_Item = DatabaseAccessDB.PopulateAllItemMasterDB();
            itemModel.CountActive = DatabaseAccessDB.CountActiveItem;
            return View("~/VIews/Home/MasterList/ItemMaster.cshtml", itemModel);
        }

        public ActionResult CreateItemMaster()
        {
            ViewBag.ItemMonitoring = false;
            ItemMaster itemModel = new ItemMaster();
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();

            itemModel.ItemCodeList = DatabaseAccessDB.List_ItemCode();
            itemModel.ItemListMeasure = DatabaseAccessDB.PopulateListofMeasurementDB();
            itemModel.ItemListCategory = DatabaseAccessDB.List_PopulateAllItemCategoryDB();
            return View("~/VIews/Home/MasterList/ItemMaster.cshtml", itemModel);
        }

        public ActionResult ClientMaster()
        {


            //Call Database Connection in (DatabaseAccess.cs)
            DatabaseAccess databaseAccessDB = new DatabaseAccess();
            //Call Model in (MainModel.cs / Public Class Client Master)
            ClientMaster ClientModel = new ClientMaster();

            //Validation if Insert or Update.
            object DataInserted = Session["DataInserted"] ?? "";

            //Confirmation Data was Inserted
            if (DataInserted != "")
            {
                // find in Views/Home/Masterlist/ClientMaster.cshtml
                ViewBag.SuccessInserted = true;
                ViewBag.ClientForm = true;
                Session["DataInserted"] = null;

            }
            //Show Table Only
            else
            {
                // find in Views/Home/Masterlist/ClientMaster.cshtml
                ViewBag.ClientMonitoring = true;
            }
            //Retreive Data into table
            ClientModel.PopulateAllClients = databaseAccessDB.PopulateAllClientInformationDB();
            ClientModel.CountActive = databaseAccessDB.CountActives;

            //Return Page.
            return View("~/VIews/Home/MasterList/ClientMaster.cshtml", ClientModel);
        }

        public ActionResult CreateClientMaster()
        {

            ViewBag.ClientForm = true;
            return View("~/VIews/Home/MasterList/ClientMaster.cshtml");
        }

        public ActionResult SupplierMaster()
        {


            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            SupplierMaster SupplierModel = new SupplierMaster();
            //Validation if Insert or Update.
            object DataInserted = Session["DataInserted"] ?? "";

            //Confirmation Data was Inserted
            if (DataInserted != "")
            {
                // find in Views/Home/Masterlist/ClientMaster.cshtml
                ViewBag.SuccessInserted = true;
                ViewBag.ClientForm = true;
                Session["DataInserted"] = null;
                ViewBag.SupplierMonitoring = false;
            }
            //Show Table Only
            else
            {
                // find in Views/Home/Masterlist/ClientMaster.cshtml
                ViewBag.SupplierMonitoring = true;
            }

            SupplierModel.PopulateAllSupplier = DatabaseAccessDB.PopulateAllSupplierInformationDB();
            SupplierModel.CountActive = DatabaseAccessDB.CountActives;

            return View("~/Views/Home/MasterList/SupplierMaster.cshtml", SupplierModel);
        }

        public ActionResult CreateSupplierMaster()
        {


            ViewBag.SupplierMonitoring = false;
            return View("~/VIews/Home/MasterList/SupplierMaster.cshtml");
        }

        //-------------------------------------------------------------------*NOTIFICATION*-------------------------------------------------------------------------------------------------------//

        public JsonResult GetCountSupplierNotification()
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            int Count = 0;
            var SessionEnd = "";
            int USERID = Convert.ToInt32(Session["UserID"].ToString() ?? "0");
            if (USERID != 0)
            {
                Count = DatabaseAccessDB.PopulateCountSupplierNotificationDB(USERID);
                return new JsonResult { Data = Count, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                SessionEnd = "End";
                return new JsonResult { Data = SessionEnd, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

        }

        public JsonResult PopulateSystemNotifaction()
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            int USERID = Convert.ToInt32(Session["UserID"].ToString() ?? "0");
            List<MainModel> main = new List<MainModel>();
            main = DatabaseAccessDB.PopulateSystemNotifaction(USERID);

            return new JsonResult { Data = main, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult PopulateSystemRequestedNotifaction()
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            int USERID = Convert.ToInt32(Session["UserID"].ToString() ?? "0");
            List<MainModel> main = new List<MainModel>();
            main = DatabaseAccessDB.PopulateSystemRequestedNotifaction(USERID);

            return new JsonResult { Data = main, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public JsonResult GetNotificationSuppliers(string id)
        {

            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            int USERID = Convert.ToInt32(Session["UserID"].ToString() ?? "0");
            List<MainModel> main = new List<MainModel>();
            main = DatabaseAccessDB.PopulateSupplierNotifaction(USERID);


            return new JsonResult { Data = main, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
		public JsonResult GetRequestedNotificationSuppliers()
		{

			DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
			int USERID = Convert.ToInt32(Session["UserID"].ToString() ?? "0");
			List<MainModel> main = new List<MainModel>();
			main = DatabaseAccessDB.PopulateRequestedSupplierNotifaction(USERID);


			return new JsonResult { Data = main, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		[HttpGet]
		public JsonResult GetNotificationCertificateOfCompletion()
		{

			DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
			int USERID = Convert.ToInt32(Session["UserID"].ToString() ?? "0");

			var list = DatabaseAccessDB.PopulateCertificateOfCompletionNotificationDB(USERID);

			return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		[HttpGet]
		public JsonResult GetRequestedNotificationCertificateOfCompletion()
		{

			DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
			int USERID = Convert.ToInt32(Session["UserID"].ToString() ?? "0");

			var list = DatabaseAccessDB.PopulateRequestedCertificateOfCompletionNotificationDB(USERID);

			return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		public JsonResult PopulateNotification()
		{
            var User = Session["UserID"].ToString() ?? "";
			DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
			MainModel mainmodel = new MainModel();



			mainmodel = DatabaseAccessDB.NotificationDB(User);
			return new JsonResult { Data = mainmodel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

		}


		//-------------------------------------------------------------------*OTHERS*-------------------------------------------------------------------------------------------------------//

		public ActionResult SystemLogs()
		{
			UserMaintenance main = new UserMaintenance();

			DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
			main.PopulateSystemLogs = DatabaseAccessDB.PopulateSystemLogs();
			return View("~/Views/Home/Maintenance/SystemLogs.cshtml", main);
		}

		[HttpGet]
		public JsonResult PopulateitemList()
		{
			DatabaseAccess databaseAccessDB = new DatabaseAccess();
			var ItemCategory = databaseAccessDB.List_PopulateItemNameDB(); // POPULATE ALL LIST ITEM CATEGORY IN GRIDVIEW

			return Json(Data = ItemCategory, JsonRequestBehavior = JsonRequestBehavior.AllowGet);// RENDER VALUE IN AJAX (LINE 333) CreateClient.cshtml
		}

		[HttpGet]
		public JsonResult RefereshLocation(string ClientID)
		{
			DatabaseAccess databaseAccessDB = new DatabaseAccess();
			var LocationCategory = databaseAccessDB.List_PopulateAllClientAddressDB(ClientID); // POPULATE ALL LIST ITEM CATEGORY IN GRIDVIEW

			return Json(Data = LocationCategory, JsonRequestBehavior = JsonRequestBehavior.AllowGet);// RENDER VALUE IN AJAX (LINE 333) CreateClient.cshtml
		}

        [HttpGet]
        public JsonResult RefereshClient()
        {
            DatabaseAccess databaseAccessDB = new DatabaseAccess();
            var ClientCategory = databaseAccessDB.List_PopulateClientNameDB(); // POPULATE ALL LIST ITEM CATEGORY IN GRIDVIEW

            return Json(Data = ClientCategory, JsonRequestBehavior = JsonRequestBehavior.AllowGet);// RENDER VALUE IN AJAX (LINE 333) CreateClient.cshtml
        }

		[HttpGet]
		public JsonResult RefereshSupplier()
		{
			DatabaseAccess databaseAccessDB = new DatabaseAccess();
			var SupplierCategory = databaseAccessDB.List_AllSupplierDB();

			return Json(Data = SupplierCategory, JsonRequestBehavior = JsonRequestBehavior.AllowGet);// RENDER VALUE IN AJAX (LINE 333) CreateClient.cshtml
		}

		[HttpPost]
		public JsonResult GetItemDetails(string value, string POCReferenceNo)
		{
			DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
			SupplierModel supplierModel = new SupplierModel();
			supplierModel.ItemMeasure = DatabaseAccessDB.GetItemMeasurementDB(value); // GET ITEM MEASURE DEPEND ON ITEM NAME
			supplierModel.Qty = DatabaseAccessDB.GetItemQuantityDB(value, POCReferenceNo); // GET ITEM QUANTITY ON ITEM NAME
			supplierModel.itemDescription = DatabaseAccessDB.GetItemDescriptionDB(value); // GET ITEM DESCRIPTION ON ITEM NAME

			return Json(supplierModel);
		}

		//[HttpPost]
		//public JsonResult PopulateListUserGroup(string value)
		//{
		//	DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
		//	UserMaintenance userModel = new UserMaintenance();
		//	var res = DatabaseAccessDB.PopulateListUserGroupDB(value); // POPULATE LIST USER GROUP DEPEND ON USEROLE
		//	return Json(res);
		//}

		[HttpPost]
		public JsonResult PopulateListSupplierAddress(string value)
		{
			DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
			var res = 0;
			if (value != "")
			{
				SupplierModel supplierModel = new SupplierModel();
				supplierModel = DatabaseAccessDB.PopulateListSupplierAddressDB(value); // POPULATE LIST SUPPLIER ADDRESS DEPEND ON SUPPLIER NAME
				return Json(supplierModel.SupplierAddresss);
			}
			return Json(res);

		}

		[HttpPost]
		public JsonResult PopulateListClientAddress(string value)
		{
			DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
			var res = 0;
			if (value != "")
			{
				ClientModel clientModel = new ClientModel();
				var address = DatabaseAccessDB.PopulateListClientAddressDB(value); // // POPULATE LIST CLIENT ADDRESS DEPEND ON CLIENT NAME
                clientModel.ClientAddress = address;
				return Json(clientModel.ClientAddress ?? "");
			}
			return Json(res);

		}

        [HttpPost]
        public JsonResult ChartNumberofSales()
        {
            DatabaseAccess databaseAccess = new DatabaseAccess();
            List<object> iData = new List<object>();
            iData = databaseAccess.GetNumberofSalesDB();
            return Json(iData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChartNumberofOrder()
        {
            DatabaseAccess databaseAccess = new DatabaseAccess();
            List<object> iData = new List<object>();
            iData = databaseAccess.GetNumberofOrdersDB();
            return Json(iData, JsonRequestBehavior.AllowGet);
        }

		public ActionResult UserRole(UserMaintenance maintenance)
		{
			DatabaseAccess databaseAccess = new DatabaseAccess();

				ViewBag.Tree = "true";
			
				maintenance.PopulateAllUserInformation = databaseAccess.PopulateDept_UserRole();
				return View("~/Views/Home/Maintenance/UserRole.cshtml", maintenance);

		}

        public ActionResult UserRoleEdit(string id, string edit, UserMaintenance maintenance)
        {
            DatabaseAccess databaseAccess = new DatabaseAccess();

                ViewBag.Tree = "false";
                maintenance.ListAllUserGroup = databaseAccess.PopulateAllListUserGroupDB();
                maintenance.ListAllUserRole = databaseAccess.PopulateListUserRoleDB();
                ModuleAccess(maintenance, "1");
                ViewBag.edit = edit;
                return View("~/Views/Home/Maintenance/UserRole.cshtml", maintenance);

        }

        public ActionResult UserRoleCreate()
		{
			DatabaseAccess databaseAccess = new DatabaseAccess();
			UserMaintenance user = new UserMaintenance();

			user.ListAllUserGroup = databaseAccess.PopulateAllListUserGroupDB();
			user.ListAllUserRole = databaseAccess.PopulateListUserRoleDB();
			ViewBag.Tree = "false";
			ModuleAccess(user, "0");
			return View("~/Views/Home/Maintenance/UserRole.cshtml", user);
		}

		[HttpPost]
		public ActionResult InsertModuleAccess(string selectedItems, UserMaintenance user, string edit)
		{
            DatabaseAccess databaseAccess = new DatabaseAccess();
            ModuleAccess moduleAccess = new ModuleAccess();
            List<ModuleAccess> TreeView = new List<ModuleAccess>();
            user.ListAllUserGroup = databaseAccess.PopulateAllListUserGroupDB();
            user.ListAllUserRole = databaseAccess.PopulateListUserRoleDB();


            if ( user.DepartmentID == 0 || user.UserRoleID == 0)
            {
                ViewBag.HeadMessages = string.Format("Invalid.");
                ViewBag.Messages = string.Format("Please Select Department and User role");
       
                ViewBag.Tree = "false";
                ModuleAccess(user, "0");
                return View("~/Views/Home/Maintenance/UserRole.cshtml",user);
            }

            else if(selectedItems == "")
            {
                ViewBag.HeadMessages = string.Format("Invalid.");
                ViewBag.Messages = string.Format("Check atleast one module");
                ViewBag.Tree = "false";
                ModuleAccess(user, "0");
                return View("~/Views/Home/Maintenance/UserRole.cshtml", user);
            }


            List<ModuleAccess> items = (new JavaScriptSerializer()).Deserialize<List<ModuleAccess>>(selectedItems);
			foreach (var item in items)
			{
				if (item.id != "1" && item.id != "2" && item.id != "3" && item.id != "4" && item.id != "5" &&
					item.id != "6" && item.id != "7" && item.id != "8" && item.id != "9")
				{
					moduleAccess = new ModuleAccess()
					{
						selectedID = item.id
					};

					TreeView.Add(moduleAccess);
				}


			}
			moduleAccess.AllModuleAccess = TreeView;
			int data = 0;
			if (edit != "true")
			{
				data = databaseAccess.CheckAccessIfExistingDB(user);
			}
			if (data == 0)
			{
				databaseAccess.InsertModuleAccess(moduleAccess, user);
                if (edit != "true")
                {
                    databaseAccess.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Department: " + user.Department + " and Role: " + user.UserRole + "has been Added.", "USER ROLE");
                }
                else {
                    databaseAccess.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Department: " + user.Department + " and Role: " + user.UserRole + " Access has been Updated.", "USER ROLE");
                }
            }
			else {
				ViewBag.HeadMessages = string.Format("Invalid.");
				ViewBag.Messages = string.Format("Department and UserRole was already exist");
				ViewBag.Tree = "false";
                ModuleAccess(user, "0");
                return View("~/Views/Home/Maintenance/UserRole.cshtml", user);
			}

			return RedirectToAction("UserRole","Home");
		}

		public List<ModuleAccess> ModuleAccess(UserMaintenance maintenance,string editCount)
		{

			DatabaseAccess databaseAccess = new DatabaseAccess();
			ModuleAccess moduleAccess = new ModuleAccess();
			List<ModuleAccess> TreeView = new List<ModuleAccess>();

			moduleAccess.MainList = databaseAccess.PopulateMainPage();
			moduleAccess.SubList = databaseAccess.PopulateSubPage();

			if (maintenance.UserRoleID == 0 && maintenance.DepartmentID == 0 && editCount == "0" || editCount == "0")
			{

				foreach (ModuleAccess row in moduleAccess.MainList)
				{
					TreeView.Add(new ModuleAccess { id = row.MainID.ToString(), parent = "#", text = row.MainPage });
				}
				//Loop and add the Child Nodes.
				foreach (ModuleAccess row in moduleAccess.SubList)
				{
					TreeView.Add(new ModuleAccess { id = row.SubMainID.ToString() + "-" + row.SubPageID.ToString(), parent = row.SubMainID.ToString(), text = row.SubPage });
				}

				//Serialize to JSON string.

			}
			else {

				moduleAccess.EditMainList = databaseAccess.PopulateEditMainPage(maintenance);
				moduleAccess.EditSubList = databaseAccess.PopulateEditSubPage(maintenance);

				
				foreach (ModuleAccess row in moduleAccess.MainList)
				{
					var count = 0;

					foreach (var item in moduleAccess.EditMainList)
					{
						if(item.xMainID == row.MainID)
						{
							TreeView.Add(new ModuleAccess { id = row.MainID.ToString(), parent = "#", text = row.MainPage});
							count++;
							break;

						}

					}
					if (count == 0)
						TreeView.Add(new ModuleAccess { id = row.MainID.ToString(), parent = "#", text = row.MainPage});

				}
				//Loop and add the Child Nodes.
				foreach (ModuleAccess row in moduleAccess.SubList)
				{
					var count = 0;
					
						foreach (var item in moduleAccess.EditSubList)
						{
						if (item.xSubPageID == row.SubPageID)
						{
							TreeView.Add(new ModuleAccess { id = row.SubMainID.ToString() + "-" + row.SubPageID.ToString(), parent = row.SubMainID.ToString(), text = row.SubPage, state = new TreeViewAttribute { selected = true } });
							count++;
							break;

						}

					}

					if (count == 0)
						TreeView.Add(new ModuleAccess { id = row.SubMainID.ToString() + "-" + row.SubPageID.ToString(), parent = row.SubMainID.ToString(), text = row.SubPage, state = new TreeViewAttribute { selected = false } });

				}

			}


			ViewBag.Json = (new JavaScriptSerializer()).Serialize(TreeView);
			return TreeView;

		}

		[HttpGet]
		public JsonResult GetModuleAccess()
		{
            DatabaseAccess databaseAccess = new DatabaseAccess();
			var UserID = Session["UserID"].ToString();
			var UserRole = Session["UserRole"].ToString();
            var UserGroup = Session["UserGroupID"].ToString();
            LoginModel loginModel = new LoginModel();

            loginModel = databaseAccess.GetUserSessionDB(UserID);
            if (loginModel.UserRoleID != UserRole)
            {
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
            }

            List<string> iData = new List<string>();

			iData = databaseAccess.PopulateModuleAccess(UserRole, UserGroup);

            // Stringify your list


            return Json(iData, JsonRequestBehavior.AllowGet);
			


		}

	}
}