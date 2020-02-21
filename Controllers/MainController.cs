using PO_PurchasingUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;


namespace PO_PurchasingUI.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    [SessionTimeout]
    public class MainController : Controller
    {
        DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
        public string url;
        // GET: Main
        public ActionResult Index()
        {
            return View();
        }



        //------------------------------------SMTP----------------------------------------------------------------------------------------------------------------------------------------------//

        public JsonResult SendTestingEmail(SMTPModel serverModel)
        {
            var UserName = Session["UserName"].ToString();
            var UserID = Session["UserID"].ToString();

            var res = "success";
            var To = serverModel.SMTP_To;
            var Subject = serverModel.SMTP_Subject;
            var body = serverModel.SMTP_Body;
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            serverModel = DatabaseAccessDB.PopulateSMTPInformation();
            try
            {
                MailMessage mail = new MailMessage();

                //email  output
                mail.From = new MailAddress(serverModel.SMTP_Email, "procurementsystem@ati.com");
                mail.To.Add("" + To + "");
                mail.Subject = "" + Subject + "";
                mail.Body = "" + body + "";

                //email connection
                SmtpClient SmtpServer = new SmtpClient("" + serverModel.SMTP_Server + "");
                SmtpServer.Port = Convert.ToInt32(serverModel.SMTP_Port);
                SmtpServer.Credentials = new NetworkCredential("" + serverModel.SMTP_Email + "", "" + serverModel.SMTP_Password + "");

                //email sender
                SmtpServer.Send(mail);

                DatabaseAccessDB.InsertActionAuditTrailDB(UserName, "sent test email.", "SMTP");


            }
            catch
            {
                DatabaseAccessDB.InsertActionAuditTrailDB(UserName, "Failed sending test email.", "SMTP");

            }
            //create emailmessage



            return Json(res);
        }

        [HttpPost]
        public JsonResult InsertSMTP_Information(SMTPModel ServerModel)
        {
            var UserName = Session["UserName"].ToString();
            var UserID = Session["UserID"].ToString();

            var res = DatabaseAccessDB.InsertSMTPInformationDB(ServerModel);
            if (res == 0)
            {
                DatabaseAccessDB.InsertActionAuditTrailDB(UserName, "Successfully inserted SMTP Information.", "SMTP");

            }
            else
            {
                DatabaseAccessDB.InsertActionAuditTrailDB(UserName, "Failed insert SMTP Information.", "SMTP");

            }

            return Json(res);
        }

        [HttpPost]
        public JsonResult UpdateSMTPInformation(SMTPModel ServerModel)
        {
            var UserName = Session["UserName"].ToString();
            var UserID = Session["UserID"].ToString();

            var res = DatabaseAccessDB.InsertSMTPInformationDB(ServerModel);

            if (res == 0)
            {
                DatabaseAccessDB.InsertActionAuditTrailDB(UserName, "Successfully Updated SMTP Information.", "SMTP");

            }
            else
            {
                DatabaseAccessDB.InsertActionAuditTrailDB(UserName, "Failed Update SMTP Information.", "SMTP");

            }
            return Json(res);
        }

        //------------------------------------USER MAINTENANCE---------------------------------------------------------------------------------------------------------------------------------//

        [HttpPost]
        public ActionResult InsertUserInformation(UserMaintenance UserModel, HttpPostedFileBase File)
        {
            var UserName = "";
            if (Session["UserName"] != null)
            {
                UserName = Session["UserName"].ToString();
            }

            if (File != null && File.ContentLength > 0)
            {
                UserModel.Img = new byte[File.ContentLength];
                File.InputStream.Read(UserModel.Img, 0, File.ContentLength);
            }

            DatabaseAccessDB.InsertUserInformationDB(UserModel);
            ViewBag.SuccessInsert = true;
            Session["DataInserted"] = "Success";
            return RedirectToAction("CreateUser", "Home");


        }

        //[HttpGet]
        //public ActionResult UpdateUserInformation(int? id)
        //{
        //	var UserName = "";
        //	if (Session["UserName"] != null)
        //	{
        //		UserName = Session["UserName"].ToString();
        //	}


        //	object Edit_ID = Session["id"] ?? 0;
        //	id = Convert.ToInt32(Edit_ID);

        //	if (id == 0)
        //	{
        //		return RedirectToAction("EditUser", "Home");
        //	}

        //	ViewBag.EditUserForm = true;
        //	//Call Database Connection in (DatabaseAccess.cs)
        //	DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
        //	//Call Model in (MainModel.cs / Public Class UserMaintenance)
        //	UserMaintenance UserModel = new UserMaintenance();




        //	//Get and Retreive User Information
        //	UserModel = DatabaseAccessDB.EditUserInformationDB(id);
        //	// Populate List of Deparment
        //	UserModel.ListAllDepartment = DatabaseAccessDB.PopulateListDepartmentDB();
        //	// Populate List of User Role
        //	UserModel.ListAllUserRole = DatabaseAccessDB.PopulateListUserRoleDB();
        //	// Populate List of Approver
        //	UserModel.ListAllApprover = DatabaseAccessDB.PopulateListApproverDB(UserName);

        //	if (UserModel.UserRoleID != 0)
        //	{
        //		// Get selected User role to filter UserGroup.
        //		UserModel.ListAllUserGroup = DatabaseAccessDB.PopulateListUserGroupDB(Convert.ToString(UserModel.UserRoleID));

        //		//if Root populate all list of UserGroup (DROPDOWNLIST)
        //		if (UserModel.UserRoleID == 4)
        //		{
        //			UserModel.ListAllUserGroup = DatabaseAccessDB.PopulateAllListUserGroupDB();

        //		}
        //	}
        //	else
        //	{

        //		UserModel.ListAllUserGroup = DatabaseAccessDB.PopulateAllListUserGroupDB();

        //	}


        //	return View("~/VIews/Home/Maintenance/EditUser.cshtml", UserModel);


        //}

        [HttpPost]
        public ActionResult UpdateUserInformation(UserMaintenance UserModel, HttpPostedFileBase File)
        {

            if (File != null)
            {
                UserModel.Img = new byte[File.ContentLength];
                File.InputStream.Read(UserModel.Img, 0, File.ContentLength);
            }

            DatabaseAccessDB.UpdateUserInformationDB(UserModel);

            ViewBag.HeaderSuccess = string.Format("{0}", UserModel.UserName);

            return View("~/Views/Home/Maintenance/Edituser.cshtml");
        }


        //------------------------------------PO CLIENT QUOTATION----------------------------------------------------------------------------------------------------------------------------//

        [HttpPost]
        public JsonResult InsertClientQuotationInformation(List<ClientModel> ClientQuotationArray, ClientModel clientmodel)
        {
            int insertedRecords = 0;
            int InsertInfo = 0;
            var Information = "Information";
            var Grid = "ItemGrid";
            var Responsible = Session["UserID"].ToString() ?? "";
            var UserName = Session["UserName"].ToString();


            insertedRecords = DatabaseAccessDB.Insert_ClientQuotation_GRIDDB(ClientQuotationArray, clientmodel);

            if (insertedRecords != 0)
            {
                InsertInfo = DatabaseAccessDB.Insert_ClientQuotationDB(clientmodel);

                if (InsertInfo == 0)
                {
                    return Json(Information);
                }
            }
            else
            {
                return Json(Grid);
            }

            DatabaseAccessDB.InsertActionAuditTrailDB(UserName, "Request with " + ClientQuotationArray.Count + " Item/s and Quotation No. " + DatabaseAccessDB.Quotation_ControlNumber + " has been submitted.", "CLIENT QUOTATION");

            Session["QuotationControlNo"] = DatabaseAccessDB.Quotation_ControlNumber;
            return Json(insertedRecords);
        }

        [HttpPost]
        public JsonResult UpdateClientQuotationInformation(List<ClientModel> ClientQuotationArray, ClientModel clientmodel)
        {
            int insertedRecords = 0;
            int InsertInfo = 0;
            var Information = "Information";
            var Grid = "ItemGrid";
            var Responsible = Session["UserID"].ToString() ?? "";
            var UserName = Session["UserName"].ToString();


            insertedRecords = DatabaseAccessDB.Insert_ClientQuotation_GRIDDB(ClientQuotationArray, clientmodel);

            if (insertedRecords != 0)
            {
                InsertInfo = DatabaseAccessDB.Insert_ClientQuotationDB(clientmodel);

                if (InsertInfo == 0)
                {
                    return Json(Information);
                }

                var RejectOldQuotation = DatabaseAccessDB.UpdateClientQuotationDB(clientmodel.Quotation_ReferenceNo);
            }
            else
            {
                return Json(Grid);
            }

            DatabaseAccessDB.InsertActionAuditTrailDB(UserName, "Updated Request with " + ClientQuotationArray.Count + " Item/s and Quotation No. " + clientmodel.Quotation_ControlNo + " has been submitted.", "CLIENT QUOTATION");

            return Json(insertedRecords);
        }

        //------------------------------------PO FROM CLIENT----------------------------------------------------------------------------------------------------------------------------//

        [HttpPost]
        public JsonResult InsertUserClientInformation(List<ClientModel> ClientDetails, ClientModel clientmodel, List<string> SupplierPONumberList)
        {
            int insertedRecords = 0;
            int InsertInfo = 0;
            var Information = "Information";
            var Grid = "ItemGrid";
            var Responsible = Session["UserID"].ToString() ?? "";
            var UserName = Session["UserName"].ToString();


            insertedRecords = DatabaseAccessDB.InsertUserClientInformationGridDB(ClientDetails);

            if (insertedRecords != 0)
            {
                InsertInfo = DatabaseAccessDB.InsertUserClientInformationDB(clientmodel, Responsible, SupplierPONumberList, "");

                if (InsertInfo == 0)
                {
                    return Json(Information);
                }
            }
            else
            {
                return Json(Grid);
            }

            DatabaseAccessDB.InsertActionAuditTrailDB(UserName, "Request with " + ClientDetails.Count + " Item/s and POC" + DatabaseAccessDB.POC_ReferenceNo + " has been submitted.", "PO FROM CLIENT");

            return Json(insertedRecords);
        }

        [HttpPost]
        public JsonResult UpdatePOTagging(string POC_ReferenceNO, List<string> SupplierPONumberList, List<string> ListClientReferenceNo, string PONumber)
        {
            var data = 0;
            if (POC_ReferenceNO != null)
            {
                DatabaseAccessDB.Delete_POTaggingDB("", POC_ReferenceNO);
                if (SupplierPONumberList != null)
                {
                    for (int i = 0; i < SupplierPONumberList.Count; i++)
                    {
                        DatabaseAccessDB.InsertPOTaggingDB(POC_ReferenceNO, SupplierPONumberList[i]);
                        data++;
                    }
                    if (data == 0)
                    {
                        return Json(data);

                    }
                    return Json(data);

                }

            }
            else
            {
                DatabaseAccessDB.Delete_POTaggingDB(PONumber, "");
                if (ListClientReferenceNo != null)
                {
                    for (int i = 0; i < ListClientReferenceNo.Count; i++)
                    {
                        DatabaseAccessDB.InsertPOTaggingDB(ListClientReferenceNo[i], PONumber);
                        data++;
                    }

                    if (data == 0)
                    {
                        return Json(data);
                    }
                    return Json(data);

                }
            }

            return Json("empty");

        }

        [HttpPost]
        public JsonResult UpdateUserClientInformation(List<ClientModel> ClientDetails, ClientModel clientmodel, List<string> SupplierPONumberList, string Old_ReferenceNo)
        {
            int insertedRecords = 0;
            int InsertInfo = 0;
            int Reject_OldInfo = 0;
            var Information = "Information";
            var Grid = "ItemGrid";
            var Responsible = Session["UserID"].ToString() ?? "";
            var UserName = Session["UserName"].ToString();


            insertedRecords = DatabaseAccessDB.InsertUserClientInformationGridDB(ClientDetails);

            if (insertedRecords != 0)
            {
                Reject_OldInfo = DatabaseAccessDB.UpdateUserClientInformationDB(Old_ReferenceNo);
                if (Reject_OldInfo == 0)
                {
                    return Json(Information);
                }
                else
                {

                    InsertInfo = DatabaseAccessDB.InsertUserClientInformationDB(clientmodel, Responsible, SupplierPONumberList, Old_ReferenceNo);

                    if (InsertInfo == 0)
                    {
                        return Json(Information);
                    }
                }
            }
            else
            {
                return Json(Grid);

            }
            DatabaseAccessDB.InsertActionAuditTrailDB(UserName, "Updated Request with " + ClientDetails.Count + " Item/s and from " + Old_ReferenceNo + " to POC" + DatabaseAccessDB.POC_ReferenceNo + " has been submitted.", "PO FROM CLIENT");

            return Json(insertedRecords);
        }

        //------------------------------------PO TO SUPPLIER------------------------------------------------------------------------------------------------------------------------------------//

        [HttpPost]
        public JsonResult UpdateUserSupplierInformation(List<SupplierModel> SupplierDetails, SupplierModel supplier, List<string> ListClientReferenceNo, string Old_ReferenceNo, string HiddenPreview)
        {
            //connect SMTP Model (Model/MainModel.css)
            SMTPModel serverModel = new SMTPModel();

            int insertedRecords = 0;
            int InsertInfo = 0;
            int Reject_OldInfo = 0;

            var InsertPreview = "SuccessPreview";
            var Information = "Information";
            var Grid = "ItemGrid";

            if (Session["UserID"] == null)
            {
                var Login = "RedirectLogin";
                return Json(Login);
            }
            else
            {
                supplier.FullName = Session["UserID"].ToString();
            }

            if (HiddenPreview == "PREVIEW")
            {
                insertedRecords = DatabaseAccessDB.InsertPreviewUserSupplierInformationGridDB(SupplierDetails, supplier);

                if (insertedRecords != 0)
                {
                    InsertInfo = DatabaseAccessDB.InsertPreviewUserSupplierInformationDB(supplier);
                    if (InsertInfo == 0)
                    {
                        return Json(Information);
                    }
                    else
                    {
                        Session["Preview_No"] = DatabaseAccessDB.Preview_No;
                        Session["TypeOfVat"] = supplier.TypeOfVat;


                    }
                }

                else
                {
                    return Json(Grid);

                }

                return Json(InsertPreview);
            }
            else
            {
                insertedRecords = DatabaseAccessDB.InsertUserSupplierInformationGridDB(SupplierDetails, supplier);

                if (insertedRecords != 0)
                {
                    Reject_OldInfo = DatabaseAccessDB.UpdateUserSupplierInformationDB(Old_ReferenceNo);
                    if (Reject_OldInfo == 0)
                    {
                        return Json(Information);
                    }
                    else
                    {
                        InsertInfo = DatabaseAccessDB.InsertUserSupplierInformationDB(ListClientReferenceNo, supplier);
                        if (InsertInfo == 0)
                        {
                            return Json(Information);
                        }
                        else
                        {
                            Session["POSReferenceNo"] = DatabaseAccessDB.POS_ReferenceNo;
                            Session["PONUMBER"] = supplier.POSupplierNumber;
                            Session["OldPOSReferenceNo"] = Old_ReferenceNo;

                        }

                    }
                }
                else
                {
                    return Json(Grid);

                }
                DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Updated Request with " + SupplierDetails.Count + " Item/s and from " + Old_ReferenceNo + " to POS" + DatabaseAccessDB.POS_ReferenceNo + " has been submitted.", "PO TO SUPPLIER");

                return Json(insertedRecords);
            }

        }

        [HttpPost]
        public JsonResult InsertUserSupplierInformation(List<SupplierModel> SupplierDetails, SupplierModel supplier, List<string> ListClientReferenceNo, string HiddenPreview)
        {
            //connect SMTP Model (Model/MainModel.css)
            SMTPModel serverModel = new SMTPModel();

            int insertedRecords = 0;
            int InsertInfo = 0;
            var InsertPreview = "SuccessPreview";
            var Information = "Information";
            var Grid = "ItemGrid";
            if (Session["UserID"] == null)
            {
                var Login = "RedirectLogin";
                return Json(Login);
            }
            else
            {
                supplier.FullName = Session["UserID"].ToString();
            }

            if (HiddenPreview == "PREVIEW")
            {
                insertedRecords = DatabaseAccessDB.InsertPreviewUserSupplierInformationGridDB(SupplierDetails, supplier);

                if (insertedRecords != 0)
                {
                    InsertInfo = DatabaseAccessDB.InsertPreviewUserSupplierInformationDB(supplier);
                    if (InsertInfo == 0)
                    {
                        return Json(Information);
                    }
                    else
                    {
                        Session["Preview_No"] = DatabaseAccessDB.Preview_No;
                        Session["TypeOfVat"] = supplier.TypeOfVat;
                    }
                }

                else
                {
                    return Json(Grid);

                }

                return Json(InsertPreview);
            }

            else
            {
                insertedRecords = DatabaseAccessDB.InsertUserSupplierInformationGridDB(SupplierDetails, supplier);

                if (insertedRecords != 0)
                {
                    InsertInfo = DatabaseAccessDB.InsertUserSupplierInformationDB(ListClientReferenceNo, supplier);

                    if (InsertInfo == 0)
                    {
                        return Json(Information);
                    }

                    else
                    {

                        Session["POSReferenceNo"] = DatabaseAccessDB.POS_ReferenceNo;
                        Session["PONUMBER"] = DatabaseAccessDB.PONumber;
                        Session["OldPOSReferenceNo"] = "";

                    }

                    ////SELECT SMTP INFORMATION
                    //serverModel = DatabaseAccessDB.PopulateSMTPInformation();

                    ////GET ENDORSER EMAIL
                    //var GetReviewerEmail = DatabaseAccessDB.EmailNotificationDB("69");

                    //if (serverModel.SMTP_Status != "2")
                    //{
                    //	try
                    //	{
                    //		//Send Email to Approver
                    //		//create emailmessage
                    //		MailMessage mail = new MailMessage();
                    //		SmtpClient SmtpServer = new SmtpClient();
                    //		//email  output
                    //      mail.From = new MailAddress(serverModel.SMTP_Email, "procurementsystem@ati.com");

                    //		mail.To.Add("" + GetReviewerEmail + "");

                    //                       mail.Subject = "Purchase Order: FOR REVIEW (PO number: " + DatabaseAccessDB.PONumber +")";
                    //                       mail.Body = "Purchase Order requests your Review prepared by: " + Session["UserFullName"].ToString() + " with PO Number: " + DatabaseAccessDB.PONumber + "." + Environment.NewLine + "" + Environment.NewLine + " To endorse, click this http://10.0.0.124:8882/Home/GetForEndorsementDetails/" + "POS" + DatabaseAccessDB.POS_ReferenceNo + " and we will redirect you right away to the e-Procurement System.";

                    //		//email connection
                    //		SmtpServer = new SmtpClient("" + serverModel.SMTP_Server + "");
                    //		SmtpServer.Port = Convert.ToInt32(serverModel.SMTP_Port);
                    //		SmtpServer.Credentials = new NetworkCredential("" + serverModel.SMTP_Email + "", "" + serverModel.SMTP_Password + "");

                    //		//email sender
                    //		SmtpServer.Send(mail);
                    //	}
                    //	catch
                    //	{
                    //		RedirectToAction("SendingEmailError", "Home");
                    //	}
                    //}
                }

                else
                {
                    return Json(Grid);

                }
                DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Request with " + SupplierDetails.Count + " Item/s and  POS" + DatabaseAccessDB.POS_ReferenceNo + " has been submitted.", "PO TO SUPPLIER");

                return Json(insertedRecords);
            }


        }

        //------------------------------------PURCHASE REQUISITION------------------------------------------------------------------------------------------------------------------------------------//

        [HttpPost]
        public JsonResult InsertUsePRInformation(List<SupplierModel> PRMainList, SupplierModel supplier, string HiddenPreview)
        {
            //connect SMTP Model (Model/MainModel.css)
            SMTPModel serverModel = new SMTPModel();

            int insertedRecords = 0;
            int InsertInfo = 0;
            var InsertPreview = "SuccessPreview";
            var Information = "Information";
            var Grid = "ItemGrid";
            if (Session["UserID"] == null)
            {
                var Login = "RedirectLogin";
                return Json(Login);
            }
            else
            {
                supplier.FullName = Session["UserID"].ToString();
            }

            if (HiddenPreview == "PREVIEW")
            {
                //insertedRecords = DatabaseAccessDB.InsertPreviewUserSupplierInformationGridDB(SupplierDetails, supplier);
                insertedRecords = DatabaseAccessDB.Insert_PRPreviewInformation_GRIDDB(PRMainList, supplier);

                if (insertedRecords != 0)
                {
                    InsertInfo = DatabaseAccessDB.Insert_PRPreviewInformationDB(supplier);

                    if (InsertInfo == 0)
                    {
                        return Json(Information);
                    }
                    else
                    {
                        Session["PRPreview_No"] = DatabaseAccessDB.PR_PReviewNumber;
                    }
                }

                else
                {
                    return Json(Grid);

                }

                return Json(InsertPreview);
            }

            else
            {
                insertedRecords = DatabaseAccessDB.Insert_PRInformation_GRIDDB(PRMainList, supplier);

                if (insertedRecords != 0)
                {
                    InsertInfo = DatabaseAccessDB.Insert_PRInformationDB(supplier);

                    if (InsertInfo == 0)
                    {
                        return Json(Information);
                    }

                    else
                    {

                        Session["PRReferenceNo"] = DatabaseAccessDB.PR_ReferenceNo;
                        Session["PRNumber"] = DatabaseAccessDB.PR_ControlNumber;
                        Session["OldPRReferenceNo"] = "";

                    }

                }

                else
                {
                    return Json(Grid);

                }
                DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Request with " + PRMainList.Count + " Item/s and  PR" + DatabaseAccessDB.PR_ReferenceNo + " has been submitted.", "PURCHASE REQUISITION");

                return Json(insertedRecords);
            }


        }


        [HttpPost]
        public JsonResult UpdateUsePRInformation(List<SupplierModel> PRMainList, SupplierModel supplier, string HiddenPreview)
        {
            //connect SMTP Model (Model/MainModel.css)
            SMTPModel serverModel = new SMTPModel();

            int insertedRecords = 0;
            int InsertInfo = 0;
            int RejectOldRequest = 0;
            var InsertPreview = "SuccessPreview";
            var Information = "Information";
            var Grid = "ItemGrid";

            if (Session["UserID"] == null)
            {
                var Login = "RedirectLogin";
                return Json(Login);
            }
            else
            {
                supplier.FullName = Session["UserID"].ToString();
            }

            if (HiddenPreview == "PREVIEW")
            {
                //insertedRecords = DatabaseAccessDB.InsertPreviewUserSupplierInformationGridDB(SupplierDetails, supplier);
                insertedRecords = DatabaseAccessDB.Insert_PRPreviewInformation_GRIDDB(PRMainList, supplier);

                if (insertedRecords != 0)
                {

                    InsertInfo = DatabaseAccessDB.Insert_PRPreviewInformationDB(supplier);

                    if (InsertInfo == 0)
                    {
                        return Json(Information);
                    }
                    else
                    {
                        Session["PRPreview_No"] = DatabaseAccessDB.PR_PReviewNumber;
                    }


                }

                else
                {
                    return Json(Grid);

                }

                return Json(InsertPreview);
            }

            else
            {
                insertedRecords = DatabaseAccessDB.Insert_PRInformation_GRIDDB(PRMainList, supplier);

                if (insertedRecords != 0)
                {

                    InsertInfo = DatabaseAccessDB.Insert_PRInformationDB(supplier);

                    if (InsertInfo == 0)
                    {
                        return Json(Information);
                    }

                    else
                    {
                        RejectOldRequest = DatabaseAccessDB.RejectPRRequestDB(supplier.PRReferenceNo, DatabaseAccessDB.PR_ReferenceNo);
                        if (RejectOldRequest != 0)
                        {

                            Session["PRReferenceNo"] = DatabaseAccessDB.PR_ReferenceNo;
                            Session["PRNumber"] = supplier.PRNumber;
                            Session["OldPRReferenceNo"] = supplier.PRReferenceNo;

                        }
                        else
                        {

                            return Json(Information);

                        }
                    }

                }

                else
                {
                    return Json(Grid);

                }
                DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Updated Request with " + PRMainList.Count + " Item/s and from " + supplier.PRReferenceNo + " to PR" + DatabaseAccessDB.PR_ReferenceNo + " has been submitted.", "PURCHASE REQUISITION");
                return Json(insertedRecords);
            }


        }


        //------------------------------------CLIENT MASTER LIST----------------------------------------------------------------------------------------------------------------------//

        [HttpPost]
        public JsonResult InsertClientInformation(List<ClientMaster> value, ClientMaster ClientModel)
        {
            ViewBag.ClientForm = true;

            var InsertInfo2 = "ERROR1";
            var InsertAddress1 = "ERROR2";
            var count3 = "ERROR3";
            int count = 0;
            if (ClientModel.HiddenClient != ClientModel.Client)
            {
                count = DatabaseAccessDB.CheckClientIfExistingDB(ClientModel);

                if (count != 0)
                {
                    return Json(count3);
                }
            }
            if (ClientModel.HiddenClient != null)
            {

                DatabaseAccessDB.DeleteClientDB(ClientModel.HiddenClient);
                DatabaseAccessDB.DeleteClientAddressDB(ClientModel.HiddenClient);
            }
            else
            {
                count = DatabaseAccessDB.CheckClientIfExistingDB(ClientModel);

                if (count != 0)
                {
                    return Json(count3);
                }

            }



            var Insertinfo = DatabaseAccessDB.InsertClientInformationDB(ClientModel);

            if (Insertinfo != 0)
            {
                var InsertAddress = DatabaseAccessDB.InsertClientAddressinformationDB(value, ClientModel);

                if (InsertAddress == 0)
                {
                    DatabaseAccessDB.DeleteClientInformationGRIDB(ClientModel);
                    return Json(InsertAddress1);
                }
                DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "" + ClientModel.Client + " has been added with " + value.Count + " Client Location.", "CLIENT MASTER");

                return Json(InsertAddress);
            }
            else
            {
                return Json(InsertInfo2);
            }
        }

        [HttpPost]
        public JsonResult UpdateClientMasterInformation(List<ClientMaster> DeleteDetails, List<ClientMaster> value, ClientMaster ClientModel)
        {
            ViewBag.ClientForm = true;

            var InsertInfo2 = "ERROR1";
            var InsertAddress1 = "ERROR2";
            var count3 = "ERROR3";
            int count = 0;
            if (ClientModel.HiddenClient != ClientModel.Client)
            {
                count = DatabaseAccessDB.CheckClientIfExistingDB(ClientModel);

                if (count != 0)
                {
                    return Json(count3);
                }
            }

            var Insertinfo = DatabaseAccessDB.UpdateClientInformationDB(ClientModel);

            if (Insertinfo != 0)
            {
                DatabaseAccessDB.DeleteClientAddressInfoDB(DeleteDetails);
                var InsertAddress = DatabaseAccessDB.UpdateClientAddressinformationDB(value, ClientModel);

                if (InsertAddress == 0)
                {
                    return Json(InsertAddress1);
                }
                DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "" + ClientModel.Client + " has been Updated with " + value.Count + " Client Location.", "CLIENT MASTER");

                return Json(InsertAddress);
            }
            else
            {
                return Json(InsertInfo2);
            }
        }

        //------------------------------------ITEM MASTER LIST--------------------------------------------------------------------------------------------------------------------------//

        [HttpPost]
        public JsonResult InsertItemMasterInformation(List<ItemMaster> value)
        {
            int insertedRecords = 0;
            var InsertItem = "";
            int CheckItem = 0;
            string CheckItems = "SORRY";

            if (ModelState.IsValid)
            {
                CheckItem = DatabaseAccessDB.CheckItemIfExistingDB(value);
                if (CheckItem == 0)
                {
                    insertedRecords = DatabaseAccessDB.InsertItemInformationDB(value);
                    ModelState.Clear();
                    if (insertedRecords == 0)
                    {
                        InsertItem = "Failed";
                        return Json(InsertItem);

                    }
                }
                else
                {
                    return Json(CheckItems);
                }

            }
            DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "" + value.Count + " Item/s has been added", "ITEM MASTER");

            return Json(insertedRecords);
        }

        [HttpPost]
        public JsonResult UpdateItemInformation(ItemMaster ItemModel, string OldItem)
        {
            ViewBag.EditItemMaster = true;
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            ItemMaster itemMaster = new ItemMaster();
            var Messages = "";
            if (ItemModel.ItemName != OldItem)
            {
                string CheckItem = DatabaseAccessDB.CheckUpdateItemIfExistDB(ItemModel.ItemName);

                if (CheckItem != "")
                {
                    return Json(CheckItem = "FAILED");
                }

                Messages = "Item successfully Updated from " + OldItem + " to " + ItemModel.ItemName + ".";
            }
            else
            {
                Messages = "" + ItemModel.ItemName + " successfully Updated.";
            }

            var Data = DatabaseAccessDB.UpdateItemInformationDB(ItemModel);

            DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), Messages, "ITEM MASTER");
            return Json(Data);
        }

        //------------------------------------SUPPLIER MASTER LIST-----------------------------------------------------------------------------------------------------------------//

        [HttpPost]
        public JsonResult InsertSupplierInformation(SupplierMaster SupplierModel)
        {

            var Count = DatabaseAccessDB.InsertSupplierInformationDB(SupplierModel);
            if (Count == 0)
                return Json("ERROR1");

            DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "" + SupplierModel.Supplier + " Successfully Added.", "SUPPLIER MASTER");
            return Json(Count);
        }

        [HttpPost]
        public JsonResult UpdateSupplierInformation(SupplierMaster SupplierModel)
        {

            var Count = DatabaseAccessDB.UpdateSupplierInformationDB(SupplierModel);
            if (Count == 0)
                return Json("ERROR1");

            DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "" + SupplierModel.Supplier + " Successfully Updated.", "SUPPLIER MASTER");

            return Json(Count);

        }


        //------------------------------------FORMS MONITORING---------------------------------------------------------------------------------------------------------------------//

        [HttpPost]
        public JsonResult UpdateTransactionStatus(string StringClientStatus, string StringPOCReferenceNo, string StringSupplierStatus, string StringPOSReferenceNo, string PONumber, string PRStatus, string PRReferenceNo)
        {
            SMTPModel serverModel = new SMTPModel();
            serverModel = DatabaseAccessDB.PopulateSMTPInformation();

            if (StringClientStatus != null || StringPOCReferenceNo != null)
            {
                var Count = DatabaseAccessDB.UpdateClientTransactionStatusDB(StringClientStatus, StringPOCReferenceNo);
                DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Client with " + StringPOCReferenceNo + " Successfully Updated to " + StringClientStatus + ".", "FORMS MONITORING");
                return Json(Count);

            }
            else if (PRStatus != null || PRReferenceNo != null)
            {
                var Count = DatabaseAccessDB.UpdatePurchaseRequisitionTransactionStatusDB(PRStatus, PRReferenceNo);

                SupplierModel supplierModel = new SupplierModel();
                supplierModel = DatabaseAccessDB.GetPurchaseRequisitionInformationDB(PRReferenceNo);
                var GetRequesterEmail = DatabaseAccessDB.EmailNotificationDB(supplierModel.FullName);

                if (PRStatus == "CANCELLED")
                {
                    if (serverModel.SMTP_Status != "2")
                    {
                        try
                        {
                            var From = serverModel.SMTP_Email;
                            //create emailmessage
                            MailMessage mail = new MailMessage();

                            mail.From = new MailAddress(serverModel.SMTP_Email, "procurementsystem@ati.com");
                            mail.To.Add(GetRequesterEmail);
                            mail.Subject = "Purchase Requisition: CANCELLED (PR number " + supplierModel.PRNumber + ").";
                            mail.Body = "Your Purchase Requisition has been Cancelled.  <br /> <br /> <b> Don't recognize this activity ? </b> <br /> Please report it to support@archangeltech.com.ph to notify us and we will secure your account.";
                            mail.IsBodyHtml = true;

                            //email connection
                            SmtpClient SmtpServer = new SmtpClient("" + serverModel.SMTP_Server + "");
                            SmtpServer.Port = Convert.ToInt32(serverModel.SMTP_Port);
                            SmtpServer.Credentials = new NetworkCredential("" + serverModel.SMTP_Email + "", "" + serverModel.SMTP_Password + "");

                            //email sender
                            SmtpServer.Send(mail);
                        }
                        catch
                        {
                            DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Failed sending email to " + GetRequesterEmail + ".", "SMTP");
                        }

                    }
                }


                DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Purchase Requisition with PR number " + supplierModel.PRNumber + " Successfully Updated to " + PRStatus + ".", "FORMS MONITORING");
                return Json(Count);
            }
            else
            {
                var Count = DatabaseAccessDB.UpdateSupplierTransactionStatusDB(StringSupplierStatus, StringPOSReferenceNo, PONumber);
                SupplierModel supplierModel = new SupplierModel();
                supplierModel = DatabaseAccessDB.EditUserSupplierInformationDB(StringPOSReferenceNo);
                var GetRequesterEmail = DatabaseAccessDB.EmailNotificationDB(supplierModel.FullName);

                if (StringSupplierStatus == "CANCELLED")
                {
                    if (serverModel.SMTP_Status != "2")
                    {
                        try
                        {
                            var From = serverModel.SMTP_Email;
                            //create emailmessage
                            MailMessage mail = new MailMessage();

                            mail.From = new MailAddress(serverModel.SMTP_Email, "procurementsystem@ati.com");
                            mail.To.Add(GetRequesterEmail);
                            mail.Subject = "Purchase Order: CANCELLED (PO number " + PONumber + ").";
                            mail.Body = "Your Purchase Order has been Cancelled.  <br /> <br /> <b> Don't recognize this activity ? </b> <br /> Please report it to support@archangeltech.com.ph to notify us and we will secure your account.";
                            mail.IsBodyHtml = true;

                            //email connection
                            SmtpClient SmtpServer = new SmtpClient("" + serverModel.SMTP_Server + "");
                            SmtpServer.Port = Convert.ToInt32(serverModel.SMTP_Port);
                            SmtpServer.Credentials = new NetworkCredential("" + serverModel.SMTP_Email + "", "" + serverModel.SMTP_Password + "");

                            //email sender
                            SmtpServer.Send(mail);
                        }
                        catch
                        {
                            DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Failed sending email to " + GetRequesterEmail + ".", "SMTP");
                        }
                    }

                }


                DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Supplier with PO number" + PONumber + " Successfully Updated to " + StringSupplierStatus + ".", "FORMS MONITORING");
                return Json(Count);

            }

        }


        //------------------------------------ROUTING APPROVAL-----------------------------------------------------------------------------------------------------------------------//


        [HttpPost]
        public JsonResult UpdatePRForRoutingApproval(string PRReferenceNo, string FormStatus, string ApproverID, string EndorserID)
        {
            //Connect Database Access (Controller/DatabaseAccess.css)
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            //connect SMTP Model (Model/MainModel.css)
            SMTPModel serverModel = new SMTPModel();
            //connect Supplier Model (Model/MainModel.css)
            SupplierModel suppliermodel = new SupplierModel();
            var GetEndorserEmail = "";
            var GetApproverEmail = "";
            var GetRequesterEmail = "";
            var Count = 0;

            //SELECT SUPPLIER INFORAMTION
            suppliermodel = DatabaseAccessDB.GetPurchaseRequisitionInformationDB(PRReferenceNo);
            var CheckFormStatus = DatabaseAccessDB.CheckPurchaseRequisitionFormStatusDB(PRReferenceNo, suppliermodel.FullName);

            if (CheckFormStatus != FormStatus)
            {
                return Json(CheckFormStatus);
            }


            //SELECT SMTP INFORMATION
            serverModel = DatabaseAccessDB.PopulateSMTPInformation();

            //UPDATE ROUTING APPROVAL (FOR ENDORSEMENT AND FOR APPROVAL)
            Count = DatabaseAccessDB.UpdatePR_ForRoutingApprovalDB(PRReferenceNo, FormStatus);

            //GET EMAIL OF REQUESTER
            GetRequesterEmail = DatabaseAccessDB.EmailNotificationDB(suppliermodel.FullName);

            //GET ENDORSER EMAIL
            GetEndorserEmail = DatabaseAccessDB.EmailNotificationDB(EndorserID);

            //GET APPROVER EMAIL
            GetApproverEmail = DatabaseAccessDB.EmailNotificationDB(ApproverID);

            //ERROR
            if (Count == 0)
            {
                return Json(Count);
            }

            if (FormStatus == "FOR REVIEW")
            {

                Session["PRFORENDORSEMENT"] = "true";
                Session["PRNumber"] = suppliermodel.PRNumber;
                Session["PRReferenceNo"] = PRReferenceNo;
                Session["PREmailEndorser"] = GetEndorserEmail;
                Session["PRprepared"] = suppliermodel.FullNameResponsible;

            }

            else if (FormStatus == "FOR ENDORSEMENT")
            {


                Session["PRFORAPPROVAL"] = "true";
                Session["PRNumber"] = suppliermodel.PRNumber;
                Session["PRReferenceNo"] = PRReferenceNo;
                Session["PREmailApprover"] = GetApproverEmail;
                Session["PRprepared"] = suppliermodel.FullNameResponsible;

            }

            else
            {
                //GET APPROVER EMAIL
                GetApproverEmail = DatabaseAccessDB.EmailNotificationDB(ApproverID);
                if (serverModel.SMTP_Status != "2")
                {
                    try
                    {
                        //Send Email to Requester
                        //create emailmessage
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient();
                        //email  output
                        mail.From = new MailAddress(serverModel.SMTP_Email, "procurementsystem@ati.com");

                        mail.To.Add("" + GetRequesterEmail + "");
                        mail.Subject = "Purchase Requisition: APPROVED (PR number " + suppliermodel.PRNumber + ").";
                        mail.Body = "Your Purchase Requisition has been Approved.  <br /> <br /> <b> Don't recognize this activity ? </b> <br /> Please report it to support@archangeltech.com.ph to notify us and we will secure your account.";
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
                        DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Failed sending email to " + GetRequesterEmail + ".", "SMTP");
                    }


                    DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Request PO with " + suppliermodel.PRNumber + " Successfully Approved.", "PR APPROVED");

                }
            }
            return Json(Count);

        }

        [HttpPost]
        public JsonResult UpdatePRRejected(string PRReferenceNo, string FormStatus, string RejectRemarks)
        {
            //Connect Database Access (Controller/DatabaseAccess.css)
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            //connect SMTP Model (Model/MainModel.css)
            SMTPModel serverModel = new SMTPModel();
            //connect Supplier Model (Model/MainModel.css)
            SupplierModel suppliermodel = new SupplierModel();

            var GetRejectEmail = "";
            var GetRequesterEmail = "";
            var Count = 0;


            //SELECT SUPPLIER INFORAMTION
            suppliermodel = DatabaseAccessDB.GetPurchaseRequisitionInformationDB(PRReferenceNo);
            var CheckFormStatus = DatabaseAccessDB.CheckPurchaseRequisitionFormStatusDB(PRReferenceNo, suppliermodel.FullName);

            if (CheckFormStatus != FormStatus)
            {
                return Json(CheckFormStatus);
            }


            var RejectedBy = Session["UserID"].ToString() ?? "0";

            //SELECT SMTP INFORMATION
            serverModel = DatabaseAccessDB.PopulateSMTPInformation();

            Count = DatabaseAccessDB.UpdatePRRejectedDB(PRReferenceNo, RejectedBy, RejectRemarks);

            if (Count == 0)
            {
                return Json(Count);
            }


            //GET EMAIL OF REQUESTER
            GetRequesterEmail = DatabaseAccessDB.EmailNotificationDB(suppliermodel.FullName);

            //ERROR
            if (Count == 0)
            {
                return Json(Count);
            }

            //GET ENDORSER EMAIL
            GetRejectEmail = DatabaseAccessDB.EmailNotificationDB(RejectedBy);

            if (serverModel.SMTP_Status != "2")
            {
                try
                {
                    //create emailmessage
                    MailMessage mail = new MailMessage();
                    //create SmtpClient 
                    SmtpClient SmtpServer = new SmtpClient();

                    //Send Email to Requester
                    mail.From = new MailAddress(serverModel.SMTP_Email, "procurementsystem@ati.com");

                    mail.To.Add("" + GetRequesterEmail + "");
                    mail.Subject = "Purchase Requisition: REJECTED (PR number " + suppliermodel.PRNumber + ").";
                    mail.Body = "Your Purchase Requisition request has been Rejected. <br/> Reject Remarks: " + RejectRemarks + " <br /> <br /> <b> Don't recognize this activity ? </b> <br /> Please report it to support@archangeltech.com.ph to notify us and we will secure your account.";
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
                    DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Failed sending email to " + GetRequesterEmail + ".", "SMTP");
                }


                DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Request PO with " + suppliermodel.PRNumber + " Successfully Rejected.", "PR REJECTED");


            }

            return Json(Count);

        }
        [HttpPost]
        public JsonResult UpdateSupplierForRoutingApproval(string POSReferenceNO, string FormStatus, string ApproverID, string EndorserID)
        {
            //Connect Database Access (Controller/DatabaseAccess.css)
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            //connect SMTP Model (Model/MainModel.css)
            SMTPModel serverModel = new SMTPModel();
            //connect Supplier Model (Model/MainModel.css)
            SupplierModel suppliermodel = new SupplierModel();
            var GetEndorserEmail = "";
            var GetApproverEmail = "";
            var GetRequesterEmail = "";
            var Count = 0;

            //SELECT SUPPLIER INFORAMTION
            suppliermodel = DatabaseAccessDB.EditUserSupplierInformationDB(POSReferenceNO);


            var CheckFormStatus = DatabaseAccessDB.CheckSupplierFormStatusDB(POSReferenceNO, suppliermodel.FullName);

            if (CheckFormStatus != FormStatus)
            {
                return Json(CheckFormStatus);
            }


            //SELECT SMTP INFORMATION
            serverModel = DatabaseAccessDB.PopulateSMTPInformation();

            //UPDATE ROUTING APPROVAL (FOR ENDORSEMENT AND FOR APPROVAL)
            Count = DatabaseAccessDB.UpdateSupplierForRoutingApprovalDB(POSReferenceNO, FormStatus);

            //GET EMAIL OF REQUESTER
            GetRequesterEmail = DatabaseAccessDB.EmailNotificationDB(suppliermodel.FullName);

            //GET ENDORSER EMAIL
            GetEndorserEmail = DatabaseAccessDB.EmailNotificationDB(EndorserID);

            //GET APPROVER EMAIL
            GetApproverEmail = DatabaseAccessDB.EmailNotificationDB(ApproverID);

            //ERROR
            if (Count == 0)
            {
                return Json(Count);
            }

            if (FormStatus == "FOR REVIEW")
            {

                Session["FORENDORSEMENT"] = "true";
                Session["PONUMBER"] = suppliermodel.POSupplierNumber;
                Session["POSReferenceNo"] = POSReferenceNO;
                Session["EmailEndorser"] = GetEndorserEmail;
                Session["prepared"] = suppliermodel.FullNameResponsible;

            }

            else if (FormStatus == "FOR ENDORSEMENT")
            {


                Session["FORAPPROVAL"] = "true";
                Session["PONUMBER"] = suppliermodel.POSupplierNumber;
                Session["POSReferenceNo"] = POSReferenceNO;
                Session["EmailApprover"] = GetApproverEmail;
                Session["prepared"] = suppliermodel.FullNameResponsible;

            }
            else
            {
                //GET APPROVER EMAIL
                GetApproverEmail = DatabaseAccessDB.EmailNotificationDB(ApproverID);
                if (serverModel.SMTP_Status != "2")
                {
                    try
                    {
                        //Send Email to Requester
                        //create emailmessage
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient();
                        //email  output
                        mail.From = new MailAddress(serverModel.SMTP_Email, "procurementsystem@ati.com");

                        mail.To.Add("" + GetRequesterEmail + "");
                        mail.Subject = "Purchase Order: APPROVED (PO number " + suppliermodel.POSupplierNumber + ").";
                        mail.Body = "Your Purchase Order has been Approved.  <br /> <br /> <b> Don't recognize this activity ? </b> <br /> Please report it to support@archangeltech.com.ph to notify us and we will secure your account.";
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
                        DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Failed sending email to " + GetRequesterEmail + ".", "SMTP");
                    }


                    DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Request PO with " + suppliermodel.POSupplierNumber + " Successfully Approved.", "PO APPROVED");


                }
            }
            return Json(Count);

        }

        [HttpPost]
        public JsonResult UpdateSupplierRejected(string POSReferenceNO, string FormStatus, string RejectRemarks)
        {
            //Connect Database Access (Controller/DatabaseAccess.css)
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
            //connect SMTP Model (Model/MainModel.css)
            SMTPModel serverModel = new SMTPModel();
            //connect Supplier Model (Model/MainModel.css)
            SupplierModel suppliermodel = new SupplierModel();

            var GetRejectEmail = "";
            var GetRequesterEmail = "";
            var Count = 0;
            //SELECT SUPPLIER INFORAMTION
            suppliermodel = DatabaseAccessDB.EditUserSupplierInformationDB(POSReferenceNO);

            var CheckFormStatus = DatabaseAccessDB.CheckSupplierFormStatusDB(POSReferenceNO, suppliermodel.FullName);

            if (CheckFormStatus != FormStatus)
            {
                return Json(CheckFormStatus);
            }


            var RejectedBy = Session["UserID"].ToString() ?? "0";

            //SELECT SMTP INFORMATION
            serverModel = DatabaseAccessDB.PopulateSMTPInformation();

            Count = DatabaseAccessDB.UpdateSupplierRejectedDB(POSReferenceNO, RejectedBy, RejectRemarks);

            if (Count == 0)
            {
                return Json(Count);
            }



            //GET EMAIL OF REQUESTER
            GetRequesterEmail = DatabaseAccessDB.EmailNotificationDB(suppliermodel.FullName);

            //ERROR
            if (Count == 0)
            {
                return Json(Count);
            }

            //GET ENDORSER EMAIL
            GetRejectEmail = DatabaseAccessDB.EmailNotificationDB(RejectedBy);

            if (serverModel.SMTP_Status != "2")
            {
                try
                {
                    //create emailmessage
                    MailMessage mail = new MailMessage();
                    //create SmtpClient 
                    SmtpClient SmtpServer = new SmtpClient();

                    //Send Email to Requester
                    mail.From = new MailAddress(serverModel.SMTP_Email, "procurementsystem@ati.com");

                    mail.To.Add("" + GetRequesterEmail + "");
                    mail.Subject = "Purchase Order: REJECTED (PO number " + suppliermodel.POSupplierNumber + ").";
                    mail.Body = "Your Purchase Order request has been Rejected. <br/> Reject Remarks: " + RejectRemarks + " <br /> <br /> <b> Don't recognize this activity ? </b> <br /> Please report it to support@archangeltech.com.ph to notify us and we will secure your account.";
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
                    DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Failed sending email to " + GetRequesterEmail + ".", "SMTP");
                }


                DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Request PO with " + suppliermodel.POSupplierNumber + " Successfully Rejected.", "PO REJECTED");


            }

            return Json(Count);

        }


        //------------------------------------DR MODULE---------------------------------------------------------------------------------------------------------------------------//

        [HttpPost]
        public JsonResult UpdateClientReleasing(List<ClientModel> ItemDetails, ClientModel clientModel)
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();

            if (ItemDetails != null)
            {
                var Insert = DatabaseAccessDB.InsertItemReleasingDB(ItemDetails, clientModel);

                if (Insert != 0)
                {
                    var Update = DatabaseAccessDB.UpdateItemReleasedInformationDB(ItemDetails, clientModel);

                    Session["DRNumber"] = DatabaseAccessDB.DR_Number;

                    DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "" + DatabaseAccessDB.DR_Number + " with " + ItemDetails.Count + " Item/s  successfully released to " + clientModel.POC_ReferenceNo + ".", "DR RELEASING");

                    return Json(Update);

                }
                else
                {
                    return Json("failed");
                }
            }
            else
            {
                return Json("failed");

            }

        }

        [HttpPost]
        public JsonResult UpdateSupplierReceiving(List<SupplierModel> ItemDetails, SupplierModel supplierModel)
        {
            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();


            if (ItemDetails != null)
            {

                var Insert = DatabaseAccessDB.InsertItemReceivingDB(ItemDetails, supplierModel);

                if (Insert != 0)
                {
                    var Update = DatabaseAccessDB.UpdateItemReceivedInformationDB(ItemDetails, supplierModel);

                    DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "" + supplierModel.POSupplierNumber + " with " + ItemDetails.Count + " Item/s  successfully Received from " + supplierModel.Supplier + ".", "DR RECEIVING");

                    return Json(Update);


                }
                else
                {
                    return Json("failed");
                }
            }
            else
            {
                return Json("failed");
            }


        }

        [HttpPost]
        public JsonResult UpdateEditItemReceived(DRModel drModel)
        {
            var result = DatabaseAccessDB.UpdateEditItemRecievedDB(drModel);
            if (result != "0")
            {
                DatabaseAccessDB.Insert_ReceivingItemReturnDB(drModel, Session["UserFullName"].ToString());
            }
            DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "" + drModel.DRReferenceNo + ": " + drModel.DRItemReturn + " has been returned.", "DR RECEIVING");
            return Json(result);

        }

        [HttpPost]
        public JsonResult UpdateEditItemReleased(DRModel drModel)
        {

            var result = DatabaseAccessDB.UpdateEditItemReleasedDB(drModel);
            if (result != "0")
            {
                DatabaseAccessDB.Insert_ReleasingItemReturnDB(drModel, Session["UserFullName"].ToString());
            }
            DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "" + drModel.DRReferenceNo + ": " + drModel.DRItemReturn + " has been returned.", "DR RELEASING");
            return Json(result);

        }

        [HttpPost]
        public JsonResult UpdateDRManualReleasing(ClientModel drModel)
        {
            var check = DatabaseAccessDB.Check_UpdateManualReleasing(drModel.DeliveryNo);
            if (check != "")
            {
                return Json("existing");
            }
            var result = DatabaseAccessDB.UpdateDRManualReleasingDB(drModel);
            DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "" + drModel.DeliveryNo + " successfully added.", "DR RELEASING");
            return Json(result);
        }

        //------------------------------------ACCOUNTING MODULE-----------------------------------------------------------------------------------------------------//

        [HttpPost]
		public JsonResult insertAccountingReceivables(ClientModel clientmodel)
		{
			var Count = DatabaseAccessDB.InsertAccountingReceivablesDB(clientmodel);

			if (clientmodel.ACC_BillAmount != clientmodel.ACC_BillAmountHidden)
			{

				DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "First Process: " + clientmodel.POC_ReferenceNo + " billing has been updated with Bill type " + clientmodel.ACC_BillType == "SI" ? "SALE INVOICE" : "BILLING STATEMENT" + " and Bill Amount has been changed form " + clientmodel.ACC_BillAmountHidden + " to " + clientmodel.ACC_BillAmount + ".", "RECEIVABLES");

			}
			else
			{

				DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "First Process: " + clientmodel.POC_ReferenceNo + " billing has been updated with Bill type " + clientmodel.ACC_BillType == "SI" ? "SALE INVOICE" : "BILLING STATEMENT" + ".", "RECEIVABLES");

			}

			return Json(Count);
		}

		[HttpPost]
		public JsonResult insertAccountingPayables(SupplierModel suppliermodel)
		{
			var Count = DatabaseAccessDB.InsertAccountingPayablesDB(suppliermodel);

			if (suppliermodel.ACC_AmountPaid != suppliermodel.ACC_AmountPaidHidden && suppliermodel.ACC_LesswithTax != suppliermodel.ACC_LesswithTaxHidden)
			{
				DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "First Process: " + suppliermodel.POSupplierNumber + " payment has been updated with Bill type " + suppliermodel.BillingType == "SI" ? "SALE INVOICE" : "BILLING STATEMENT" + " and Amount Paid has been changed from " + suppliermodel.ACC_AmountPaidHidden + " to " + suppliermodel.ACC_AmountPaid + " and Withholding Tax has been changed from " + suppliermodel.ACC_LesswithTaxHidden + " to " + suppliermodel.ACC_LesswithTax + ".", "PAYABLES");
			}
			else if (suppliermodel.ACC_AmountPaid != suppliermodel.ACC_AmountPaidHidden)
			{
				DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "First Process: " + suppliermodel.POSupplierNumber + " payment has been updated with Bill type " + suppliermodel.BillingType == "SI" ? "SALE INVOICE" : "BILLING STATEMENT" + " and Amount Paid has been changed from " + suppliermodel.ACC_AmountPaidHidden + " to " + suppliermodel.ACC_AmountPaid + ".", "PAYABLES");

			}
			else if (suppliermodel.ACC_LesswithTax != suppliermodel.ACC_LesswithTaxHidden)
			{
				DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "First Process: " + suppliermodel.POSupplierNumber + " payment has been updated with Bill type " + suppliermodel.BillingType == "SI" ? "SALE INVOICE" : "BILLING STATEMENT" + " and Withholding Tax has been changed from " + suppliermodel.ACC_LesswithTaxHidden + " to " + suppliermodel.ACC_LesswithTax + ".", "PAYABLES");
			}
			else
			{
				DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "First Process: " + suppliermodel.POSupplierNumber + " payment has been updated with Bill type " + suppliermodel.BillingType == "SI" ? "SALE INVOICE" : "BILLING STATEMENT" + ".", "PAYABLES");
			}

			return Json(Count);
		}

		[HttpPost]
		public JsonResult UpdateAccountingReceivables(AccountingModel accountingmodel)
		{
			var Result = DatabaseAccessDB.UpdateEditReceivablesDB(accountingmodel);

			return Json(Result);
		}

		[HttpPost]
		public JsonResult UpdateAccountingPayables(AccountingModel accountingmodel)
		{
			var Result = DatabaseAccessDB.UpdateEditPayablesDB(accountingmodel);

			return Json(Result);
		}

		[HttpPost]
		public JsonResult UpdateSupplierPayment(SupplierModel accountingmodel)
		{
			var Result = DatabaseAccessDB.UpdateSupplierPaymentDB(accountingmodel);

			DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Second Process: " + accountingmodel.POSupplierNumber + " payment has been Updated with Bill type " + accountingmodel.BillingType == "SI" ? "SALE INVOICE" : "BILLING STATEMENT" + ".", "UPDATE SUPPLIER PAYMENT");
			return Json(Result);
		}

		[HttpPost]
		public JsonResult UpdateClientBilling(ClientModel accountingmodel)
		{
			var Result = DatabaseAccessDB.UpdateClientBillingDB(accountingmodel);
			DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Second Process: " + accountingmodel.POC_ReferenceNo + " Billing has been Updated with Bill type " + accountingmodel.ACC_BillType == "SI" ? "SALE INVOICE" : "BILLING STATEMENT" + ".", "UPDATE CLI");

			return Json(Result);
		}



		//------------------------------------CERITIFICATE OF COMPLETION MODULE--------------------------------------------------------------------------------------//

		[HttpPost]
		public JsonResult InsertCoCInformation(ClientModel clientmodel, string HiddenPreview)
		{
			var ResultPreview = "SuccessPreview";

			if (HiddenPreview == "PREVIEW")
			{
				var PreviewInsertCount = DatabaseAccessDB.InsertPreviewCertifcateOfCompletionDB(clientmodel);

				if (PreviewInsertCount == 0)
				{
					ResultPreview = "FailedPreview";
					return Json(ResultPreview);

				}

				Session["PreviewCOC"] = DatabaseAccessDB.Preview_No;
				return Json(ResultPreview);

			}
			else
			{
				var InsertCount = DatabaseAccessDB.InsertCertifcateOfCompletionDB(clientmodel, "0");

				Session["COC_ControlNo"] = DatabaseAccessDB.CoC_ControlNumber;

				if (InsertCount != 0)
				{
					SMTPModel serverModel = new SMTPModel();

					//SELECT SMTP INFORMATION
					serverModel = DatabaseAccessDB.PopulateSMTPInformation();

					var GetRequesterEmail = DatabaseAccessDB.EmailNotificationDB(clientmodel.FullNameResponsible);
					Session["RequesterEmail"] = GetRequesterEmail;

					//GET APPROVER EMAIL
					var GetApproverEmail = DatabaseAccessDB.EmailNotificationDB(clientmodel.ApproverID);


					if (serverModel.SMTP_Status != "2")
					{

						try
						{
							//Send Email to Approver
							//create emailmessage
							MailMessage mail = new MailMessage();
							SmtpClient SmtpServer = new SmtpClient();
							//email  output
							mail.From = new MailAddress(serverModel.SMTP_Email, "procurementsystem@ati.com");

							mail.To.Add("" + GetApproverEmail + "");

							mail.Subject = "Certificate of Completion: FOR APPROVAL(Control number: " + DatabaseAccessDB.CoC_ControlNumber + " Date: " + DateTime.Now.ToShortDateString() + ")";
							mail.Body = "You have Certificate of Completion requests your Approval prepared by: " + Session["UserFullName"].ToString() + " with Control Number: " + DatabaseAccessDB.CoC_ControlNumber + "." + Environment.NewLine + "" + Environment.NewLine + " To Approve,<br /> click this https://procurementsystem-beta.archangeltech.com.ph/Home/GetCertificateOfCompletion/" + DatabaseAccessDB.CoC_ControlNumber + " and we will redirect you right away to the e-Procurement System.<br /> <br /> <b> Don't recognize this activity ? </b> <br /> Please report it to support@archangeltech.com.ph to notify us and we will secure your account.";
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
							DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "COC: Failed sending email to " + GetApproverEmail + ".", "SMTP");
						}


						DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Request COC with " + DatabaseAccessDB.CoC_ControlNumber + " Successfully Sent to " + GetApproverEmail + " FOR APPROVAL.", "COC ROUTING APPROVAL");


					}
				}


				return Json(InsertCount);
			}


		}

		[HttpPost]
		public JsonResult UpdateCOCForRoutingApproval(string COC_ControlNo, string COC_Status, string COC_Resposible)
		{
			var Status = DatabaseAccessDB.CheckCetificateOfCompletionFormStatusDB(COC_ControlNo, COC_Resposible);

			if (Status != COC_Status)
			{
				return Json(Status);

			}
			var Result = DatabaseAccessDB.UpdateCertificateOfCompletionDB(COC_ControlNo);

			if (Result != 0)
			{
				SMTPModel serverModel = new SMTPModel();

				//SELECT SMTP INFORMATION
				serverModel = DatabaseAccessDB.PopulateSMTPInformation();

				var ApproverID = Session["UserID"].ToString();
				var GetApproverEmail = DatabaseAccessDB.EmailNotificationDB(ApproverID);
				var GetRequesterEmail = DatabaseAccessDB.EmailNotificationDB(COC_Resposible);

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
						mail.From = new MailAddress(serverModel.SMTP_Email, "procurementsystem@ati.com");

						mail.To.Add("" + GetRequesterEmail + "");
						mail.Subject = "Certificate of Completion: APPROVED (Control number " + COC_ControlNo + ").";
						mail.Body = "Your Certificate of Completion has been Approved.<br /> <br /> <b> Don't recognize this activity ? </b> <br /> Please report it to support@archangeltech.com.ph to notify us and we will secure your account.";
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
						DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "COC: Failed sending email to " + GetRequesterEmail + ".", "SMTP");
					}


					DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Request COC with " + DatabaseAccessDB.CoC_ControlNumber + "Successfully Approved.", "COC ROUTING APPROVAL");


				}
			}

			return Json(Result);
		}

		[HttpPost]
		public JsonResult UpdateRejectCOCForRoutingApproval(string COC_ControlNo, string COC_Status, string COC_Resposible, string COC_RejectRemarks)
		{
			var Status = DatabaseAccessDB.CheckCetificateOfCompletionFormStatusDB(COC_ControlNo, COC_Resposible);

			if (Status != COC_Status)
			{
				return Json(Status);

			}
			var UserID = Session["UserID"].ToString();

			var Result = DatabaseAccessDB.UpdateRejectCertificateOfCompletionDB(COC_ControlNo, COC_RejectRemarks, UserID);

			if (Result != 0)
			{
				SMTPModel serverModel = new SMTPModel();

				//SELECT SMTP INFORMATION
				serverModel = DatabaseAccessDB.PopulateSMTPInformation();

				var GetApproverEmail = DatabaseAccessDB.EmailNotificationDB(UserID);
				var GetRequesterEmail = DatabaseAccessDB.EmailNotificationDB(COC_Resposible);

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
						mail.From = new MailAddress(serverModel.SMTP_Email, "procurementsystem@ati.com");

						mail.To.Add("" + GetRequesterEmail + "");
						mail.Subject = "Certificate of Completion: REJECTED (Control number " + COC_ControlNo + ").";
						mail.Body = "Your Certificate of Completion request was Rejected. <br/> Reject Remarks: " + COC_RejectRemarks + " <br /> <br /> <b> Don't recognize this activity ? </b> <br /> Please report it to support@archangeltech.com.ph to notify us and we will secure your account.";
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
						DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "COC: Failed sending email to " + GetRequesterEmail + ".", "SMTP");
					}


					DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Request COC with " + DatabaseAccessDB.CoC_ControlNumber + "has been Rejected.", "COC ROUTING APPROVAL");


				}
			}

			return Json(Result);
		}

		[HttpPost]
		public JsonResult UpdateCoCInformation(ClientModel clientmodel, string HiddenPreview)
		{
			clientmodel.FullNameResponsible = Session["UserID"].ToString();

			var ResultPreview = "SuccessPreview";

			if (HiddenPreview == "PREVIEW")
			{
				var PreviewInsertCount = DatabaseAccessDB.InsertPreviewCertifcateOfCompletionDB(clientmodel);

				if (PreviewInsertCount == 0)
				{
					ResultPreview = "FailedPreview";
					return Json(ResultPreview);

				}

				Session["PreviewCOC"] = DatabaseAccessDB.Preview_No;
				return Json(ResultPreview);

			}
			else
			{
				var count = DatabaseAccessDB.InsertCertifcateOfCompletionDB(clientmodel, "1");
				if (count != 0)
				{
					var UpdateCount = DatabaseAccessDB.UpdateEditCertificateOfCompletion(clientmodel);
					Session["COC_ControlNo"] = clientmodel.COC_ControlNo;

					if (UpdateCount != 0)
					{
						SMTPModel serverModel = new SMTPModel();

						//SELECT SMTP INFORMATION
						serverModel = DatabaseAccessDB.PopulateSMTPInformation();

						var GetRequesterEmail = DatabaseAccessDB.EmailNotificationDB(clientmodel.FullNameResponsible);
						Session["RequesterEmail"] = GetRequesterEmail;

						//GET APPROVER EMAIL
						var GetApproverEmail = DatabaseAccessDB.EmailNotificationDB(clientmodel.ApproverID);


						if (serverModel.SMTP_Status != "2")
						{
							try
							{
								//Send Email to Approver
								//create emailmessage
								MailMessage mail = new MailMessage();
								SmtpClient SmtpServer = new SmtpClient();
								//email  output
								mail.From = new MailAddress(serverModel.SMTP_Email, "procurementsystem@ati.com");

								mail.To.Add("" + GetApproverEmail + "");

								mail.Subject = "Certificate of Completion: FOR APPROVAL (Control number: " + clientmodel.COC_ControlNo + ")";
								mail.Body = "You have Certificate of Completion requests your Approval  prepared by: " + Session["UserFullName"].ToString() + "." + Environment.NewLine + "" + Environment.NewLine + " To Approve,<br /> click this https://procurementsystem-beta.archangeltech.com.ph/Home/GetCertificateOfCompletion/" + clientmodel.COC_ControlNo + " and we will redirect you right away to the e-Procurement System. <br /> <br /> <b> Don't recognize this activity ? </b> <br /> Please report it to support@archangeltech.com.ph to notify us and we will secure your account.";

								//email connection
								SmtpServer = new SmtpClient("" + serverModel.SMTP_Server + "");
								SmtpServer.Port = Convert.ToInt32(serverModel.SMTP_Port);
								SmtpServer.Credentials = new NetworkCredential("" + serverModel.SMTP_Email + "", "" + serverModel.SMTP_Password + "");
								mail.IsBodyHtml = true;
								//email sender
								SmtpServer.Send(mail);
							}
							catch
							{
								DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "COC: Failed sending email to " + GetRequesterEmail + ".", "SMTP");
							}


							DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Updated Request COC with " + DatabaseAccessDB.CoC_ControlNumber + "Successfully sent to " + GetApproverEmail + " FOR REVISION.", "COC ROUTING APPROVAL");


						}
						return Json(UpdateCount);

					}
					else
					{
						return Json("Error");
					}
				}
				else
				{
					return Json("Error");

				}
			}
		}

	}
}