using Microsoft.Reporting.WebForms;
using PO_PurchasingUI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

namespace PO_PurchasingUI.Controllers
{
    public class ReportController : Controller
    {

        //private string Security = ConfigurationManager.ConnectionStrings["Security"].ConnectionString;
        //private string MasterList = ConfigurationManager.ConnectionStrings["PO_Masterlist"].ConnectionString;
        //private string PotoSupplier = ConfigurationManager.ConnectionStrings["PO_Supplier"].ConnectionString;
        //private string PobyClient = ConfigurationManager.ConnectionStrings["PO_Client"].ConnectionString;
        //private string DrModule = ConfigurationManager.ConnectionStrings["PO_DRModule"].ConnectionString;
        //private string Accounting = ConfigurationManager.ConnectionStrings["PO_Accounting"].ConnectionString;

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
        string Parameters = "";
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ProjectCostReport(string id)
        {
            DatabaseConnectionString();
            if (id != null)
            {

                using (SqlConnection con = new SqlConnection(Accounting))
                {
                    Warning[] warnings;
                    string[] streams;
                    string MIMETYPE = string.Empty;
                    string encoding = string.Empty;
                    string extension = string.Empty;
                    //process report
                    ReportViewer rptviewer = new ReportViewer();
                    rptviewer.ProcessingMode = ProcessingMode.Local;
                    //call report path
                    rptviewer.LocalReport.ReportPath = "Views/Report/ProjectCostReport.rdlc";
                    using (SqlCommand cmd2 = new SqlCommand("Generate_SupplierProjectCostReport", con))
                    {

                        cmd2.Parameters.AddWithValue("@CLIENT_REFERENCENO", id);

                        cmd2.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        //fill dataset
                        SqlDataAdapter adp = new SqlDataAdapter(cmd2);
                        DataSet ds = new DataSet();
                        //call stored procedure
                        adp.Fill(ds, "Generate_SupplierProjectCostReport");
                        ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                        rptviewer.LocalReport.DataSources.Add(datasource);

                        con.Close();
                    }


                    using (SqlCommand cmd = new SqlCommand("Generate_ClientProjectCostReport", con))
                    {

                        cmd.Parameters.AddWithValue("@CLIENTREFERENCENO", id);
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        //fill dataset
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        //call stored procedure
                        adp.Fill(ds, "Generate_ClientProjectCostReport");
                        ReportDataSource datasource = new ReportDataSource("DataSet2", ds.Tables[0]);
                        rptviewer.LocalReport.DataSources.Add(datasource);

                        con.Close();
                    }



                    //add datasoure to report
                    //convert report data to bytes
                    byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);

                    string Username = Session["UserName"].ToString();
                    Parameters = "PROJECT SUMMARY REPORT";

                    using (FileStream fs = new FileStream(Server.MapPath("~/Downloads/" + Username) + "\\" + Parameters + ".pdf", FileMode.Create))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                    }
                    //process report to pdf format
                    Session["PreviewTest"] = "Download";
                    Response.ClearHeaders();
                    Response.ContentType = "application/pdf";
                    Response.BinaryWrite(bytes);
                    Response.Flush();
                    Response.Close();
                    con.Close();
                }

            }
            return View("~/Views/Report/VIEW_SupplierReport.cshtml");

            //create connection

        }

        [HttpGet]
        public ActionResult ReportSupplier(string id)
        {
            DatabaseConnectionString();
            if (Session["POSReferenceNo"] != null || id != null)
            {


                using (SqlConnection con = new SqlConnection(PotoSupplier))
                {

                    //call stored procedure
                    using (SqlCommand cmd = new SqlCommand("Generate_SupplierReport", con))
                    {
                        //Parameters
                        if (id != null)
                        {
                            cmd.Parameters.AddWithValue("@POS_ReferenceNo", id);

                        }
                        else if (Session["FORENDORSEMENT"] != null || Session["FORAPPROVAL"] != null)
                        {
                            cmd.Parameters.AddWithValue("@POS_ReferenceNo", Session["POSReferenceNo"].ToString());
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@POS_ReferenceNo", "POS" + Session["POSReferenceNo"].ToString());
                        }
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        //fill dataset
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adp.Fill(ds, "Generate_SupplierReport");
                        //create datasource for report
                        ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                        Warning[] warnings;
                        string[] streams;
                        string MIMETYPE = string.Empty;
                        string encoding = string.Empty;
                        string extension = string.Empty;
                        //process report
                        ReportViewer rptviewer = new ReportViewer();
                        rptviewer.ProcessingMode = ProcessingMode.Local;
                        //call report path
                        rptviewer.LocalReport.ReportPath = "Views/Report/SupplierReport.rdlc";
                        //add datasoure to report
                        rptviewer.LocalReport.DataSources.Add(datasource);
                        //convert report data to bytes
                        byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                        //process report to pdf format
                        DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
                        SupplierModel supplierModel = new SupplierModel();
                        if (id == null)
                        {
                            SMTPModel serverModel = new SMTPModel();
                            serverModel = DatabaseAccessDB.PopulateSMTPInformation();
                            MemoryStream memoryStream = new MemoryStream(bytes);
                            memoryStream.Seek(0, SeekOrigin.Begin);

                            if (Session["FORENDORSEMENT"] != null)
                            {

                                if (serverModel.SMTP_Status != "2")
                                {
                                    var GetEndorserEmail = Session["EmailEndorser"].ToString();
                                    var FullNameResponsible = Session["prepared"].ToString();
                                    var POSupplierNumber = Session["PONUMBER"].ToString();
                                    var POSReferenceNO = Session["POSReferenceNo"].ToString();
                                    Attachment attachment = new Attachment(memoryStream, POSupplierNumber + ".pdf");

                                    try
                                    {
                                        //Send Email to Approver
                                        //create emailmessage
                                        MailMessage mail = new MailMessage();
                                        SmtpClient SmtpServer = new SmtpClient();
                                        //email  output
                                        mail.From = new MailAddress("procurementsystem@ati.com", "procurementsystem@ati.com");

                                        mail.To.Add("" + GetEndorserEmail + "");
                                        mail.Subject = "Purchase Order: FOR ENDORSEMENT (PO number: " + POSupplierNumber + ")";
                                        mail.Body = "Purchase Order requests your Endorsement prepared by: " + FullNameResponsible + " with PO Number: " + POSupplierNumber + "." + Environment.NewLine + "" + Environment.NewLine + " To Endorse,<br /> click this https://procurementsystem-beta.archangeltech.com.ph/Home/GetForEndorsementDetails/" + POSReferenceNO + " and we will redirect you right away to the e-Procurement System.<br /> <br /> <b> Don't recognize this activity ? </b> <br /> Please report it to support@archangeltech.com.ph to notify us and we will secure your account.";
                                        mail.IsBodyHtml = true;
                                        //email connection
                                        SmtpServer = new SmtpClient("" + serverModel.SMTP_Server + "");
                                        SmtpServer.Port = Convert.ToInt32(serverModel.SMTP_Port);
                                        SmtpServer.Credentials = new NetworkCredential("" + serverModel.SMTP_Email + "", "" + serverModel.SMTP_Password + "");
                                        mail.Attachments.Add(attachment);
                                        //email sender
                                        SmtpServer.Send(mail);
                                    }
                                    catch
                                    {
                                        DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Failed sending email to " + GetEndorserEmail + ".", "SMTP");
                                    }


                                    DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Request PO with " + POSupplierNumber + " Successfully Reviewed.", "PO FOR REVIEW");
                                    return RedirectToAction("FORREVIEW", "Home");


                                }
                            }

                            else if (Session["FORAPPROVAL"] != null)
                            {
                                if (serverModel.SMTP_Status != "2")
                                {
                                    var GetApproverEmail = Session["EmailApprover"].ToString();
                                    var FullNameResponsible = Session["prepared"].ToString();
                                    var POSupplierNumber = Session["PONUMBER"].ToString();
                                    var POSReferenceNO = Session["POSReferenceNo"].ToString();
                                    Attachment attachment = new Attachment(memoryStream, POSupplierNumber + ".pdf");

                                    try
                                    {
                                        //Send Email to Approver
                                        //create emailmessage
                                        MailMessage mail = new MailMessage();
                                        SmtpClient SmtpServer = new SmtpClient();
                                        //email  output
                                        mail.From = new MailAddress("procurementsystem@ati.com", "procurementsystem@ati.com");

                                        mail.To.Add("" + GetApproverEmail + "");
                                        mail.Subject = "Purchase Order: FOR APPROVAL (PO number: " + POSupplierNumber + ")";
                                        mail.Body = "Purchase Order requests your Approval prepared by: " + FullNameResponsible + " with PO Number: " + POSupplierNumber + "." + Environment.NewLine + "" + Environment.NewLine + " To Approve,<br /> click this https://procurementsystem-beta.archangeltech.com.ph/Home/GetForApprovalDetails/" + POSReferenceNO + " and we will redirect you right away to the e-Procurement System.<br /> <br /> <b> Don't recognize this activity ? </b> <br /> Please report it to support@archangeltech.com.ph to notify us and we will secure your account.";
                                        mail.IsBodyHtml = true;
                                        //email connection
                                        SmtpServer = new SmtpClient("" + serverModel.SMTP_Server + "");
                                        SmtpServer.Port = Convert.ToInt32(serverModel.SMTP_Port);
                                        SmtpServer.Credentials = new NetworkCredential("" + serverModel.SMTP_Email + "", "" + serverModel.SMTP_Password + "");
                                        mail.Attachments.Add(attachment);
                                        //email sender
                                        SmtpServer.Send(mail);
                                    }
                                    catch
                                    {
                                        DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Failed sending email to " + GetApproverEmail + ".", "SMTP");
                                    }


                                    DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Request PO with " + POSupplierNumber + " Successfully Endored.", "PO FOR ENDORSEMENT");
                                    return RedirectToAction("FORENDORSEMENT", "Home");

                                }
                            }
                            else
                            {

                                var PONumber = Session["PONUMBER"].ToString();
                                var ReferenceNo = Session["POSReferenceNo"].ToString();
                                var OldPOSReferenceNo = Session["OldPOSReferenceNo"].ToString();

                                supplierModel = DatabaseAccessDB.EditUserSupplierInformationDB(id ?? "POS" + Session["POSReferenceNo"].ToString());

                                var GetReviewerEmail = DatabaseAccessDB.EmailNotificationDB(supplierModel.ReviewerID);

                                Attachment attachment = new Attachment(memoryStream, PONumber + ".pdf");

                                if (serverModel.SMTP_Status != "2")
                                {
                                    try
                                    {
                                        var From = serverModel.SMTP_Email;
                                        var Subject = "Purchase Order: FOR REVIEW (P.O Number: " + PONumber + " Date: " + DateTime.Now.ToShortDateString() + ").";
                                        //create emailmessage
                                        MailMessage mail = new MailMessage();

                                        mail.From = new MailAddress(serverModel.SMTP_Email, "procurementsystem@ati.com");
                                        mail.To.Add(GetReviewerEmail);
                                        mail.Subject = "" + Subject + "";
                                        mail.Body = "Purchase Order requests your approval prepared by: " + Session["UserFullName"].ToString() + " with PO Number: " + PONumber + "." + Environment.NewLine + "" + Environment.NewLine + " To approve,<br /> click this https://procurementsystem-beta.archangeltech.com.ph/Home/GetForReviewDetails/" + "POS" + ReferenceNo + " and we will redirect you right away to the e-Procurement System.<br /> <br /> <b> Don't recognize this activity ? </b> <br /> Please report it to support@archangeltech.com.ph to notify us and we will secure your account.";
                                        mail.Attachments.Add(attachment);
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
                                        DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Failed sending email to " + GetReviewerEmail + ".", "SMTP");
                                    }


                                    DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Request PO with " + PONumber + " Successfully sent to Reviewer.", "PO FOR REVIEW");

                                    if (OldPOSReferenceNo != "")
                                    {
                                        return RedirectToAction("EditSupplier", "Home");
                                    }
                                    return RedirectToAction("POTOSUPPLIER", "Home");


                                }
                            }

                        }
                        supplierModel = DatabaseAccessDB.EditUserSupplierInformationDB(id ?? "POS" + Session["POSReferenceNo"].ToString());
                        id = null;
                        Parameters = "PO-" + supplierModel.POSupplierNumber ??Session["PONUMBER"].ToString();

                        string Username = Session["UserName"].ToString();

                        using (FileStream fs = new FileStream(Server.MapPath("~/Downloads/" + Username) + "\\" + Parameters + ".pdf", FileMode.Create))
                        {
                            fs.Write(bytes, 0, bytes.Length);
                        }
                        Session["PreviewTest"] = "Download";
                        Response.ClearHeaders();
                        Response.ContentType = "application/pdf";
                        Response.BinaryWrite(bytes);
                        Response.Flush();
                        Response.Close();
                        con.Close();
                    }
                }

            }
            return View("~/Views/Report/VIEW_SupplierReport.cshtml");

            //create connection

        }

        [HttpGet]
        public ActionResult ReportPreviewPurchaseRequisition()
        {
            DatabaseConnectionString();
            if (Session["PRPreview_No"] != null)
            {
                using (SqlConnection con = new SqlConnection(PotoSupplier))
                {
                    //call stored procedure
                    using (SqlCommand cmd = new SqlCommand("Generate_PreviewPurchaseRequisitionReport", con))
                    {
                        //Parameters

                        cmd.Parameters.AddWithValue("@PreviewNo", Session["PRPreview_No"].ToString());

                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        //fill dataset
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adp.Fill(ds, "Generate_PreviewPurchaseRequisitionReport");
                        //create datasource for report
                        ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                        Warning[] warnings;
                        string[] streams;
                        string MIMETYPE = string.Empty;
                        string encoding = string.Empty;
                        string extension = string.Empty;
                        //process report
                        ReportViewer rptviewer = new ReportViewer();
                        rptviewer.ProcessingMode = ProcessingMode.Local;
                        //call report path
                        rptviewer.LocalReport.ReportPath = "Views/Report/PreviewPurchaseRequisition.rdlc";
                        //add datasoure to report
                        rptviewer.LocalReport.DataSources.Add(datasource);
                        //convert report data to bytes
                        byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                        //process report to pdf format

                        Session["PreviewTest"] = "Download";
                        Response.ClearHeaders();
                        Response.ContentType = "application/pdf";
                        Response.BinaryWrite(bytes);
                        Response.Flush();
                        Response.Close();
                        con.Close();
                    }
                }
            }
            return View("~/Views/Report/VIEW_SupplierReport.cshtml");

        }

        [HttpGet]
        public ActionResult ReportBackup_VerificationCode()
        {
            DatabaseConnectionString();
            if (Session["UserID"] != null)
            {
                using (SqlConnection con = new SqlConnection(Security))
                {
                    //call stored procedure
                    using (SqlCommand cmd = new SqlCommand("Select_Backup_VerificationCode", con))
                    {
                        //Parameters

                        cmd.Parameters.AddWithValue("@UserID", Session["UserID"].ToString());

                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        //fill dataset
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adp.Fill(ds, "Select_Backup_VerificationCode");
                        //create datasource for report
                        ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                        Warning[] warnings;
                        string[] streams;
                        string MIMETYPE = string.Empty;
                        string encoding = string.Empty;
                        string extension = string.Empty;
                        //process report
                        ReportViewer rptviewer = new ReportViewer();
                        rptviewer.ProcessingMode = ProcessingMode.Local;
                        //call report path
                        rptviewer.LocalReport.ReportPath = "Views/Report/Backup_VerificationCode.rdlc";
                        //add datasoure to report
                        rptviewer.LocalReport.DataSources.Add(datasource);
                        //convert report data to bytes
                        byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                        //process report to pdf format

                        Session["PreviewTest"] = "Download";
                        Response.ClearHeaders();
                        Response.ContentType = "application/pdf";
                        Response.BinaryWrite(bytes);
                        Response.Flush();
                        Response.Close();
                        con.Close();
                    }
                }
            }
            return View("~/Views/Report/VIEW_SupplierReport.cshtml");

        }

        [HttpGet]
        public ActionResult ReportPreviewSupplier()
        {
            DatabaseConnectionString();
            if (Session["Preview_No"] != null)
            {
                using (SqlConnection con = new SqlConnection(PotoSupplier))
                {
                    //call stored procedure
                    using (SqlCommand cmd = new SqlCommand("Generate_PreviewSupplierReport", con))
                    {
                        //Parameters

                        cmd.Parameters.AddWithValue("@Preview_No", Session["Preview_No"].ToString());
                        cmd.Parameters.AddWithValue("@Typeof", Session["TypeOfVat"].ToString());
                        

                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        //fill dataset
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adp.Fill(ds, "Generate_PreviewSupplierReport");
                        //create datasource for report
                        ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                        Warning[] warnings;
                        string[] streams;
                        string MIMETYPE = string.Empty;
                        string encoding = string.Empty;
                        string extension = string.Empty;
                        //process report
                        ReportViewer rptviewer = new ReportViewer();
                        rptviewer.ProcessingMode = ProcessingMode.Local;
                        //call report path
                        rptviewer.LocalReport.ReportPath = "Views/Report/PreviewSupplierReport.rdlc";
                        //add datasoure to report
                        rptviewer.LocalReport.DataSources.Add(datasource);
                        //convert report data to bytes
                        byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                        //process report to pdf format

                        Session["PreviewTest"] = "Download";
                        Response.ClearHeaders();
                        Response.ContentType = "application/pdf";
                        Response.BinaryWrite(bytes);
                        Response.Flush();
                        Response.Close();
                        con.Close();
                    }
                }
            }
            return View("~/Views/Report/VIEW_SupplierReport.cshtml");

        }

        [HttpGet]
        public ActionResult ReportPreviewCertificateOfCompletion()
        {
            DatabaseConnectionString();
            if (Session["PreviewCOC"] != null)
            {
                using (SqlConnection con = new SqlConnection(PobyClient))
                {
                    //call stored procedure
                    using (SqlCommand cmd = new SqlCommand("Generate_PReviewCertificateOfCompletionReport", con))
                    {
                        //Parameters

                        cmd.Parameters.AddWithValue("@PREVIEWNO", Session["PreviewCOC"].ToString());

                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        //fill dataset
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adp.Fill(ds, "Generate_PReviewCertificateOfCompletionReport");
                        //create datasource for report
                        ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                        Warning[] warnings;
                        string[] streams;
                        string MIMETYPE = string.Empty;
                        string encoding = string.Empty;
                        string extension = string.Empty;
                        //process report
                        ReportViewer rptviewer = new ReportViewer();
                        rptviewer.ProcessingMode = ProcessingMode.Local;
                        //call report path
                        rptviewer.LocalReport.ReportPath = "Views/Report/COC_PreviewReport.rdlc";
                        //add datasoure to report
                        rptviewer.LocalReport.DataSources.Add(datasource);
                        //convert report data to bytes
                        byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                        //process report to pdf format

                        Session["PreviewTest"] = "Download";
                        Response.ClearHeaders();
                        Response.ContentType = "application/pdf";
                        Response.BinaryWrite(bytes);
                        Response.Flush();
                        Response.Close();
                        con.Close();
                    }
                }
            }
            return View("~/Views/Report/VIEW_SupplierReport.cshtml");

        }

        [HttpGet]
        public ActionResult CustomerDR_Report(string id)
        {
            DatabaseConnectionString();
            if (Session["DRNumber"] != null || id != null)
            {
                using (SqlConnection con = new SqlConnection(DrModule))
                {
                    //call stored procedure
                    using (SqlCommand cmd = new SqlCommand("Generate_DRReport", con))
                    {
                        //Parameters
                        if (id != null)
                        {
                            cmd.Parameters.AddWithValue("@DR_Number", id);
                            Parameters = id;
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@DR_Number", Session["DRNumber"].ToString());
                            Parameters = Session["DRNumber"].ToString();
                        }

                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        //fill dataset
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adp.Fill(ds, "Generate_DRReport");
                        //create datasource for report
                        ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                        Warning[] warnings;
                        string[] streams;
                        string MIMETYPE = string.Empty;
                        string encoding = string.Empty;
                        string extension = string.Empty;
                        //process report
                        ReportViewer rptviewer = new ReportViewer();
                        rptviewer.ProcessingMode = ProcessingMode.Local;
                        //call report path
                        rptviewer.LocalReport.ReportPath = "Views/Report/CustomerReport.rdlc";
                        //add datasoure to report
                        rptviewer.LocalReport.DataSources.Add(datasource);
                        //convert report data to bytes
                        byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                        //process report to pdf format
                        string Username = Session["UserName"].ToString();

                        using (FileStream fs = new FileStream(Server.MapPath("~/Downloads/" + Username) + "\\" + Parameters + ".pdf", FileMode.Create))
                        {
                            fs.Write(bytes, 0, bytes.Length);
                        }
                        Session["PreviewTest"] = "Download";
                        Response.ClearHeaders();
                        Response.ContentType = "application/pdf";
                        Response.BinaryWrite(bytes);
                        Response.Flush();
                        Response.Close();
                        con.Close();
                    }
                }
            }
            return View("~/Views/Report/VIEW_SupplierReport.cshtml");

        }

        [HttpGet]
        public ActionResult DR_Report(string id)
        {
            DatabaseConnectionString();
            if (Session["DRNumber"] != null || id != null)
            {
                using (SqlConnection con = new SqlConnection(DrModule))
                {
                    //call stored procedure
                    using (SqlCommand cmd = new SqlCommand("Generate_DRReport", con))
                    {
                        //Parameters
                        if (id != null)
                        {
                            cmd.Parameters.AddWithValue("@DR_Number", id);
                            Parameters = id;
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@DR_Number", Session["DRNumber"].ToString());
                            Parameters = Session["DRNumber"].ToString();
                        }
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        //fill dataset
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adp.Fill(ds, "Generate_DRReport");
                        //create datasource for report
                        ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                        Warning[] warnings;
                        string[] streams;
                        string MIMETYPE = string.Empty;
                        string encoding = string.Empty;
                        string extension = string.Empty;
                        //process report
                        ReportViewer rptviewer = new ReportViewer();
                        rptviewer.ProcessingMode = ProcessingMode.Local;
                        //call report path
                        rptviewer.LocalReport.ReportPath = "Views/Report/DRReport.rdlc";
                        //add datasoure to report
                        rptviewer.LocalReport.DataSources.Add(datasource);
                        //convert report data to bytes
                        byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                        //process report to pdf format
                        string Username = Session["UserName"].ToString();

                        using (FileStream fs = new FileStream(Server.MapPath("~/Downloads/" + Username) + "\\" + Parameters + ".pdf", FileMode.Create))
                        {
                            fs.Write(bytes, 0, bytes.Length);
                        }
                        Session["PreviewTest"] = "Download";
                        Response.ClearHeaders();
                        Response.ContentType = "application/pdf";
                        Response.BinaryWrite(bytes);
                        Response.Flush();
                        Response.Close();
                        con.Close();
                    }
                }
            }
            return View("~/Views/Report/VIEW_SupplierReport.cshtml");

        }

        [HttpGet]
        public ActionResult COC_Report(string id)
        {
            DatabaseConnectionString();
            if (Session["COC_ControlNo"] != null || id != null)
            {
                using (SqlConnection con = new SqlConnection(PobyClient))
                {
                    //call stored procedure
                    using (SqlCommand cmd = new SqlCommand("Generate_CertificateOfCompletionReport", con))
                    {
                        //Parameters

                        if (id != null)
                        {
                            cmd.Parameters.AddWithValue("@COC_ControlNo", id);
                            Parameters = id;

                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@COC_ControlNo", Session["COC_ControlNo"].ToString());
                            Parameters = Session["COC_ControlNo"].ToString();



                        }
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        //fill dataset
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adp.Fill(ds, "Generate_CertificateOfCompletionReport");
                        //create datasource for report
                        ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                        Warning[] warnings;
                        string[] streams;
                        string MIMETYPE = string.Empty;
                        string encoding = string.Empty;
                        string extension = string.Empty;
                        //process report
                        ReportViewer rptviewer = new ReportViewer();
                        rptviewer.ProcessingMode = ProcessingMode.Local;
                        //call report path
                        rptviewer.LocalReport.ReportPath = "Views/Report/COC_Report.rdlc";
                        //add datasoure to report
                        rptviewer.LocalReport.DataSources.Add(datasource);
                        //convert report data to bytes
                        byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                        //process report to pdf format

                        if (id != null)
                        {
                            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
                            var Status = DatabaseAccessDB.CheckCetificateOfCompletionFormStatusDB(id, Session["UserID"].ToString());
                            if (Status == "APPROVED" || Status == "REJECTED")
                            {
                                var Result = DatabaseAccessDB.UpdateViewedFormCOC(id);
                            }
                        }
                        string Username = Session["UserName"].ToString();

                        using (FileStream fs = new FileStream(Server.MapPath("~/Downloads/" + Username) + "\\" + Parameters + ".pdf", FileMode.Create))
                        {
                            fs.Write(bytes, 0, bytes.Length);
                        }
                        Session["PreviewTest"] = "Download";
                        Response.ClearHeaders();
                        Response.ContentType = "application/pdf";
                        Response.BinaryWrite(bytes);
                        Response.Flush();
                        Response.Close();
                        con.Close();
                    }
                }
            }
            return View("~/Views/Report/VIEW_SupplierReport.cshtml");

        }

        [HttpGet]
        public ActionResult ApprovedReportSupplier(string id)
        {
            DatabaseConnectionString();
            if (Session["POSReferenceNo"] != null || id != null)
            {



                using (SqlConnection con = new SqlConnection(PotoSupplier))
                {
                    //call stored procedure
                    using (SqlCommand cmd = new SqlCommand("Generate_ApprovedSupplierReport", con))
                    {
                        //Parameters
                        if (id != null)
                        {
                            cmd.Parameters.AddWithValue("@POS_ReferenceNo", id);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@POS_ReferenceNo", "POS" + Session["POSReferenceNo"].ToString());

                        }
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        //fill dataset
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adp.Fill(ds, "Generate_ApprovedSupplierReport");
                        //create datasource for report
                        ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                        Warning[] warnings;
                        string[] streams;
                        string MIMETYPE = string.Empty;
                        string encoding = string.Empty;
                        string extension = string.Empty;
                        //process report
                        ReportViewer rptviewer = new ReportViewer();
                        rptviewer.ProcessingMode = ProcessingMode.Local;
                        //call report path
                        rptviewer.LocalReport.ReportPath = "Views/Report/SupplierReport.rdlc";
                        //add datasoure to report
                        rptviewer.LocalReport.DataSources.Add(datasource);
                        //convert report data to bytes
                        byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                        //process report to pdf format
                        DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
                        SupplierModel supplierModel = new SupplierModel();

                        supplierModel = DatabaseAccessDB.EditUserSupplierInformationDB(id ?? "POS" + Session["UserID"].ToString());
                        Parameters = "PO-" + supplierModel.POSupplierNumber;
                        if (id != null)
                        {
                            var Status = DatabaseAccessDB.CheckSupplierFormStatusDB(id, Session["UserID"].ToString());
                            if (Status == "APPROVED" || Status == "REJECTED")
                            {
                                var Result = DatabaseAccessDB.UpdateViewedFormSupplier(id);
                            }
                        }

                        string Username = Session["UserName"].ToString();

                        using (FileStream fs = new FileStream(Server.MapPath("~/Downloads/" + Username) + "\\" + Parameters + ".pdf", FileMode.Create))
                        {
                            fs.Write(bytes, 0, bytes.Length);
                        }

                        id = null;
                        Session["PreviewTest"] = "Download";
                        Response.ClearHeaders();
                        Response.ContentType = "application/pdf";
                        Response.BinaryWrite(bytes);
                        Response.Flush();
                        Response.Close();
                        con.Close();
                    }
                }

            }
            return View("~/Views/Report/VIEW_SupplierReport.cshtml");

            //create connection

        }

        [HttpGet]
        public ActionResult PurchaseRequisitionReport(string id)
        {
            DatabaseConnectionString();
            if (Session["PRReferenceNo"] != null || id != null)
            {



                using (SqlConnection con = new SqlConnection(PotoSupplier))
                {
                    //call stored procedure
                    using (SqlCommand cmd = new SqlCommand("Generate_PurchaseRequisitionReport", con))
                    {
                        //Parameters
                        if (id != null)
                        {
                            cmd.Parameters.AddWithValue("@PR_REFERENCENO", id);
                        }
                        else if (Session["PRFORENDORSEMENT"] != null || Session["PRFORAPPROVAL"] != null)
                        {
                            cmd.Parameters.AddWithValue("@PR_REFERENCENO", Session["PRReferenceNo"].ToString());
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@PR_REFERENCENO", Session["PRReferenceNo"].ToString());

                        }
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        //fill dataset
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adp.Fill(ds, "Generate_PurchaseRequisitionReport");
                        //create datasource for report
                        ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                        Warning[] warnings;
                        string[] streams;
                        string MIMETYPE = string.Empty;
                        string encoding = string.Empty;
                        string extension = string.Empty;
                        //process report
                        ReportViewer rptviewer = new ReportViewer();
                        rptviewer.ProcessingMode = ProcessingMode.Local;
                        //call report path
                        rptviewer.LocalReport.ReportPath = "Views/Report/PurchaseRequisition.rdlc";
                        //add datasoure to report
                        rptviewer.LocalReport.DataSources.Add(datasource);
                        //convert report data to bytes
                        byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                        //process report to pdf format
                        if (id == null)
                        {
                            SMTPModel serverModel = new SMTPModel();
                            DatabaseAccess DatabaseAccessDB = new DatabaseAccess();
                            SupplierModel supplierModel = new SupplierModel();
                            serverModel = DatabaseAccessDB.PopulateSMTPInformation();
                            MemoryStream memoryStream = new MemoryStream(bytes);
                            memoryStream.Seek(0, SeekOrigin.Begin);

                            if (Session["PRFORENDORSEMENT"] != null)
                            {

                                if (serverModel.SMTP_Status != "2")
                                {
                                    var GetEndorserEmail = Session["PREmailEndorser"].ToString();
                                    var FullNameResponsible = Session["PRprepared"].ToString();
                                    var PRNumber = Session["PRNumber"].ToString();
                                    var PRReferenceNo = Session["PRReferenceNo"].ToString();

                                    Attachment attachment = new Attachment(memoryStream, PRNumber + ".pdf");

                                    try
                                    {
                                        //Send Email to Approver
                                        //create emailmessage
                                        MailMessage mail = new MailMessage();
                                        SmtpClient SmtpServer = new SmtpClient();
                                        //email  output
                                        mail.From = new MailAddress("procurementsystem@ati.com", "procurementsystem@ati.com");

                                        mail.To.Add("" + GetEndorserEmail + "");
                                        mail.Subject = "Purchase Requisition: FOR ENDORSEMENT (PR number: " + PRNumber + ")";
                                        mail.Body = "Purchase Requisition requests your Endorsement prepared by: " + FullNameResponsible + " with PR Number: " + PRNumber + "." + Environment.NewLine + "" + Environment.NewLine + " To Endorse,<br /> click this https://procurementsystem-beta.archangeltech.com.ph/Home/GetPR_ForEndorsementDetails/" + PRReferenceNo + " and we will redirect you right away to the e-Procurement System.<br /> <br /> <b> Don't recognize this activity ? </b> <br /> Please report it to support@archangeltech.com.ph to notify us and we will secure your account.";
                                        mail.IsBodyHtml = true;
                                        //email connection
                                        SmtpServer = new SmtpClient("" + serverModel.SMTP_Server + "");
                                        SmtpServer.Port = Convert.ToInt32(serverModel.SMTP_Port);
                                        SmtpServer.Credentials = new NetworkCredential("" + serverModel.SMTP_Email + "", "" + serverModel.SMTP_Password + "");
                                        mail.Attachments.Add(attachment);
                                        //email sender
                                        SmtpServer.Send(mail);
                                    }
                                    catch
                                    {
                                        DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Failed sending email to " + GetEndorserEmail + ".", "SMTP");
                                    }


                                    DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Request PR with " + PRNumber + " Successfully Reviewed.", "PR FOR REVIEW");
                                    return RedirectToAction("PR_FORREVIEW", "Home");


                                }
                            }

                            else if (Session["PRFORAPPROVAL"] != null)
                            {
                                if (serverModel.SMTP_Status != "2")
                                {
                                    var GetApproverEmail = Session["PREmailApprover"].ToString();
                                    var FullNameResponsible = Session["PRprepared"].ToString();
                                    var PRNumber = Session["PRNumber"].ToString();
                                    var PRReferenceNo = Session["PRReferenceNo"].ToString();
                                    Attachment attachment = new Attachment(memoryStream, PRNumber + ".pdf");

                                    try
                                    {
                                        //Send Email to Approver
                                        //create emailmessage
                                        MailMessage mail = new MailMessage();
                                        SmtpClient SmtpServer = new SmtpClient();
                                        //email  output
                                        mail.From = new MailAddress("procurementsystem@ati.com", "procurementsystem@ati.com");

                                        mail.To.Add("" + GetApproverEmail + "");
                                        mail.Subject = "Purchase Requisition: FOR APPROVAL (PR number: " + PRNumber + ")";
                                        mail.Body = "Purchase Requisition requests your Approval prepared by: " + FullNameResponsible + " with PR Number: " + PRNumber + "." + Environment.NewLine + "" + Environment.NewLine + " To Approve,<br /> click this https://procurementsystem-beta.archangeltech.com.ph/Home/GetPR_ForApprovalDetails/" + PRReferenceNo + " and we will redirect you right away to the e-Procurement System.<br /> <br /> <b> Don't recognize this activity ? </b> <br /> Please report it to support@archangeltech.com.ph to notify us and we will secure your account.";
                                        mail.IsBodyHtml = true;
                                        //email connection
                                        SmtpServer = new SmtpClient("" + serverModel.SMTP_Server + "");
                                        SmtpServer.Port = Convert.ToInt32(serverModel.SMTP_Port);
                                        SmtpServer.Credentials = new NetworkCredential("" + serverModel.SMTP_Email + "", "" + serverModel.SMTP_Password + "");
                                        mail.Attachments.Add(attachment);
                                        //email sender
                                        SmtpServer.Send(mail);
                                    }
                                    catch
                                    {
                                        DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Failed sending email to " + GetApproverEmail + ".", "SMTP");
                                    }


                                    DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Request PR with " + PRNumber + " Successfully Endored.", "PR FOR ENDORSEMENT");
                                    return RedirectToAction("PR_FORENDORSEMENT", "Home");

                                }
                            }
                            else
                            {

                                var PRNumber = Session["PRNumber"].ToString();
                                var ReferenceNo = Session["PRReferenceNo"].ToString();
                                var OldPRReferenceNo = Session["OldPRReferenceNo"].ToString();

                                supplierModel = DatabaseAccessDB.GetPurchaseRequisitionInformationDB(id ?? ReferenceNo);

                                var GetReviewerEmail = DatabaseAccessDB.EmailNotificationDB(supplierModel.ReviewerID);

                                Attachment attachment = new Attachment(memoryStream, PRNumber + ".pdf");

                                if (serverModel.SMTP_Status != "2")
                                {
                                    try
                                    {
                                        var From = serverModel.SMTP_Email;
                                        var Subject = "Purchase Requisition: FOR REVIEW (PR Number: " + PRNumber + " Date: " + DateTime.Now.ToShortDateString() + ").";
                                        //create emailmessage
                                        MailMessage mail = new MailMessage();

                                        mail.From = new MailAddress(serverModel.SMTP_Email, "procurementsystem@ati.com");
                                        mail.To.Add(GetReviewerEmail);
                                        mail.Subject = "" + Subject + "";
                                        mail.Body = "Purchase Requisition requests your approval prepared by: " + Session["UserFullName"].ToString() + " with PO Number: " + PRNumber + "." + Environment.NewLine + "" + Environment.NewLine + " To approve,<br /> click this https://procurementsystem-beta.archangeltech.com.ph/Home/GetPR_ForReviewDetails/" + ReferenceNo + " and we will redirect you right away to the e-Procurement System.<br /> <br /> <b> Don't recognize this activity ? </b> <br /> Please report it to support@archangeltech.com.ph to notify us and we will secure your account.";
                                        mail.Attachments.Add(attachment);
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
                                        DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Failed sending email to " + GetReviewerEmail + ".", "SMTP");
                                    }


                                    DatabaseAccessDB.InsertActionAuditTrailDB(Session["UserName"].ToString(), "Request PO with " + PRNumber + " Successfully sent to Reviewer.", "PO FOR REVIEW");

                                    if (OldPRReferenceNo != "")
                                    {
                                        return RedirectToAction("EditPurchaseRequisition", "Home");
                                    }

                                    return RedirectToAction("PurchaseRequisition", "Home");


                                }
                            }

                        }
                        SupplierModel supplierModel2 = new SupplierModel();
                        DatabaseAccess DatabaseAccessDB2 = new DatabaseAccess();

                        if (id != null)
                        {

                            var Status = DatabaseAccessDB2.CheckPurchaseRequisitionFormStatusDB(id, Session["UserID"].ToString());
                            supplierModel2 = DatabaseAccessDB2.GetPurchaseRequisitionInformationDB(id);
                            if (Status == "APPROVED" || Status == "REJECTED")
                            {
                                var Result = DatabaseAccessDB2.UpdateViewedFormPR(id);
                            }

                        }

                        id = null;
                        string Username = Session["UserName"].ToString();
                        Parameters = "PR-" + supplierModel2.PRNumber;

                        using (FileStream fs = new FileStream(Server.MapPath("~/Downloads/" + Username) + "\\" + Parameters + ".pdf", FileMode.Create))
                        {
                            fs.Write(bytes, 0, bytes.Length);
                        }
                        Session["PreviewTest"] = "Download";
                        Response.ClearHeaders();
                        Response.ContentType = "application/pdf";
                        Response.BinaryWrite(bytes);
                        Response.Flush();
                        Response.Close();
                        con.Close();
                    }
                }

            }
            return View("~/Views/Report/VIEW_SupplierReport.cshtml");

            //create connection

        }

        private AlternateView Mail_Body()
        {
            DatabaseConnectionString();
            string path = Server.MapPath(@"/assets/img/Test.jpg");
            LinkedResource Img = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
            Img.ContentId = "MyImage";
            string str = @"  
            <table>  
                <tr>  
                    <td> You have PO for Review prepared by: " + Session["UserFullName"].ToString() + @". Please see attached report.
					</td>  
                </tr>  
                <tr>  
                    <td>  

                    </td>  
                </tr></table>  
            ";

            //< img src = cid:MyImage id = 'img' alt = '' width = '70%' />
            AlternateView AV =
      AlternateView.CreateAlternateViewFromString(str, null, MediaTypeNames.Text.Html);
            AV.LinkedResources.Add(Img);
            return AV;
        }

        [HttpGet]
        public ActionResult PayablesReport()
        {
            DatabaseConnectionString();

            using (SqlConnection con = new SqlConnection(Accounting))
            {
                Warning[] warnings;
                string[] streams;
                string MIMETYPE = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;
                //process report
                ReportViewer rptviewer = new ReportViewer();
                rptviewer.ProcessingMode = ProcessingMode.Local;
                //call report path
                rptviewer.LocalReport.ReportPath = "Views/Report/PayablesReport.rdlc";
                using (SqlCommand cmd2 = new SqlCommand("Generate_PayablesReport", con))
                {

                    con.Open();
                    //fill dataset
                    SqlDataAdapter adp = new SqlDataAdapter(cmd2);
                    DataSet ds = new DataSet();
                    //call stored procedure
                    adp.Fill(ds, "Generate_PayablesReport");
                    ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                    rptviewer.LocalReport.DataSources.Add(datasource);

                    con.Close();
                }


                using (SqlCommand cmd = new SqlCommand("Select_OutStandingSupplier", con))
                {

                    con.Open();
                    //fill dataset
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    //call stored procedure
                    adp.Fill(ds, "Select_OutStandingSupplier");
                    ReportDataSource datasource = new ReportDataSource("DataSet2", ds.Tables[0]);
                    rptviewer.LocalReport.DataSources.Add(datasource);

                    con.Close();
                }



                //add datasoure to report
                //convert report data to bytes
                byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                //process report to pdf format
                Session["PreviewTest"] = "Download";
                Response.ClearHeaders();
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(bytes);
                Response.Flush();
                Response.Close();
                con.Close();
                return View("~/Views/Report/VIEW_SupplierReport.cshtml");

                //create connection

            }
        }

        [HttpGet]
        public ActionResult ReceivablesReport()
        {
            DatabaseConnectionString();


            using (SqlConnection con = new SqlConnection(Accounting))
            {
                Warning[] warnings;
                string[] streams;
                string MIMETYPE = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;
                //process report
                ReportViewer rptviewer = new ReportViewer();
                rptviewer.ProcessingMode = ProcessingMode.Local;
                //call report path
                rptviewer.LocalReport.ReportPath = "Views/Report/ReceivablesReport.rdlc";
                using (SqlCommand cmd2 = new SqlCommand("Generate_ReceivablesReport", con))
                {

                    con.Open();
                    //fill dataset
                    SqlDataAdapter adp = new SqlDataAdapter(cmd2);
                    DataSet ds = new DataSet();
                    //call stored procedure
                    adp.Fill(ds, "Generate_ReceivablesReport");
                    ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                    rptviewer.LocalReport.DataSources.Add(datasource);

                    con.Close();
                }


                using (SqlCommand cmd = new SqlCommand("Select_OutstandingClient", con))
                {

                    con.Open();
                    //fill dataset
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    //call stored procedure
                    adp.Fill(ds, "Select_OutstandingClient");
                    ReportDataSource datasource = new ReportDataSource("DataSet2", ds.Tables[0]);
                    rptviewer.LocalReport.DataSources.Add(datasource);

                    con.Close();
                }



                //add datasoure to report
                //convert report data to bytes
                byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                //process report to pdf format
                Session["PreviewTest"] = "Download";
                Response.ClearHeaders();
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(bytes);
                Response.Flush();
                Response.Close();
                con.Close();
                return View("~/Views/Report/VIEW_SupplierReport.cshtml");

                //create connection

            }

        }
        
        [HttpGet]
        public ActionResult ClientQuotationReport(string id)
        {
            DatabaseConnectionString();


            if (Session["QuotationControlNo"] != null || id != null)
            {
                using (SqlConnection con = new SqlConnection(PobyClient))
                {
                    //call stored procedure
                    using (SqlCommand cmd = new SqlCommand("Generate_ClientQuotationReport", con))
                    {
                        //Parameters

                        if (id != null)
                        {
                            cmd.Parameters.AddWithValue("@ControlNo", id);
                            Parameters = "QUOTATION-" + id;
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ControlNo", Session["QuotationControlNo"].ToString());
                            Parameters = "QUOTATION-" + Session["QuotationControlNo"].ToString();

                        }
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        //fill dataset
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adp.Fill(ds, "Generate_ClientQuotationReport");
                        //create datasource for report
                        ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                        Warning[] warnings;
                        string[] streams;
                        string MIMETYPE = string.Empty;
                        string encoding = string.Empty;
                        string extension = string.Empty;
                        //process report
                        ReportViewer rptviewer = new ReportViewer();
                        rptviewer.ProcessingMode = ProcessingMode.Local;
                        //call report path
                        rptviewer.LocalReport.ReportPath = "Views/Report/ClientQuotation.rdlc";
                        //add datasoure to report
                        rptviewer.LocalReport.DataSources.Add(datasource);
                        //convert report data to bytes
                        byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                        //process report to pdf format
                        string Username = Session["UserName"].ToString();

                        using (FileStream fs = new FileStream(Server.MapPath("~/Downloads/" + Username) + "\\" + Parameters + ".pdf", FileMode.Create))
                        {
                            fs.Write(bytes, 0, bytes.Length);
                        }
                        Session["PreviewTest"] = "Download";
                        Response.ClearHeaders();
                        Response.ContentType = "application/pdf";
                        //Response.AddHeader("content-disposition", "attachment; filename=QuotationReport(" + Parameters + ")." + extension);
                        Response.BinaryWrite(bytes);
                        Response.Flush();
                        Response.Close();
                        con.Close();
                    }
                }
            }
            return View("~/Views/Report/VIEW_SupplierReport.cshtml");

        }
    }
}