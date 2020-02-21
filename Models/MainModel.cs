using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Web.Mvc;

namespace PO_PurchasingUI.Models
{
    public class MainModel
    {
		
		public string FORREVIEW { get; set; }
		public string FORENDORSEMENT { get; set; }
		public string FORAPPROVAL { get; set; }
        public string APPROVED { get; set; }
        public string REJECTED { get; set; }
        public string CountClient { get; set; }
        public string CountSupplier { get; set; }
        public string CountOngoing { get; set; }
        public string CountClosed { get; set; }
        public string CountOrders { get; set; }
        public string CountSales { get; set; }
        public string CountSalesToday { get; set; }
        public string CountTotalEarnings { get; set; }
        public string CountAverageSales { get; set; }
        
        public string CountReviewed { get; set; }
        public string CountEndorsed { get; set; }
        public string CountCOC { get; set; }
        public string CountPOForApproval { get; set; }
        public string CountPOForEndorsement { get; set; }
        public string CountPOForReview { get; set; }

        public string CountPRForApproval { get; set; }
        public string CountPRForEndorsement { get; set; }
        public string CountPRForReview { get; set; }

        public string CountPR_TotalRequest { get; set; }
        public string CountPR_TotalRequested { get; set; }
        public string CountPO_TotalRequested { get; set; }
        public string CountCOC_TotalRequested { get; set; }


        public string VerificationUsed { get; set; }
        public string VerificationCode { get; set; }
        


        //NOTIFICATION DATA
        public string NotificationNo { get; set; }
        public string NotificationSupplier { get; set; }
        public string NotificationSupplierDate { get; set; }
        public string NotificationStatus { get; set; }
        public string NotificationPONumber { get; set; }
        public string NotificationProjectTitle { get; set; }
        public string NotificationControlNo { get; set; }
        public string NotificationClient { get; set; }
        public string NotificationCoCDate { get; set; }
        public string NotificationCoCStatus { get; set; }
        public string NotificationCOC { get; set; }
        public string NotificationPOS { get; set; }
        public string NotificationDate { get; set; }
        public string NotificationNumber { get; set; }
        public string NotificationFormStatus { get; set; }
        public string NotificationReferenceNo { get; set; }
        public string NotificationType { get; set; }
        public string NotificationRequestedType { get; set; }
        

        public string Months { get; set; }
        public string Records { get; set; }
    }

    public class SMTPModel
    {

        public int SMTPID { get; set; }
        public string SMTP_Status { get; set; }
        public string SMTP_Port { get; set; }
        public string SMTP_Server { get; set; }
        public string SMTP_Email { get; set; }
        public string SMTP_Password { get; set; }
        public string SMTP_ConfirmPassword { get; set; }
        public string SMTP_To { get; set; }
        public string SMTP_Subject { get; set; }
        public string SMTP_Body { get; set; }

        public List<SMTPModel> PopulateSMTP_information { get; set; }
    }

    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ReturnURL { get; set; }
        public string ConfirmationPassword { get; set; }
        public string NewPassword { get; set; }
        public string Email { get; set; }
        public int UserID { get; set; }
        public string Status { get; set; }
        public string EmployeeID { get; set; }
        public string UserGroup { get; set; }
        public string UserRole { get; set; }
        public string Department { get; set; }
        public string FullName { get; set; }
        public string UserGroupID { get; set; }
        public string UserRoleID { get; set; }
		public string DepartmentID { get; set; }
		public string VerificationDate { get; set; }
		public string VerificationCode { get; set; }
		public string VerificationCodeHidden { get; set; }
		public string Auth_SecretKey { get; set; }
		public string Auth_Status { get; set; }
		public string Auth_ManualCode { get; set; }
		public string Auth_QRCode { get; set; }

        public string ServerName { get; set; }
        public string ServerUsername { get; set; }
        public string ServerPassword { get; set; }
        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }


    }
    public class UserMaintenance
    {
        [Key]
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmployeeID { get; set; }
        public string EmailAddress { get; set; }
        public string Location { get; set; }
        public int DepartmentID { get; set; }
        public int UserGroupID { get; set; }
        public int UserRoleID { get; set; }
        public int ApproverID { get; set; }
        public string Position { get; set; }
        public string FullName { get; set; }
        public string UserStatus { get; set; }
        public byte[] Img { get; set; }
        public string stringtest { get; set; }
        public string UserRole { get; set; }
        public string Department { get; set; }
        public string UserGroup { get; set; }
        public string Audit_Message { get; set; }
        public string Audit_Module { get; set; }
        public string EntryDate { get; set; }
        public string VerificationDate { get; set; }
        public byte[] ImageSignature { get; set; }
        

        public List<SelectListItem> ListAllDepartment { get; set; }
        public List<SelectListItem> ListAllUserGroup { get; set; }
        public List<SelectListItem> ListAllUserRole { get; set; }
        public List<SelectListItem> ListAllApprover { get; set; }

        public List<UserMaintenance> PopulateSystemLogs { get; set; }
        public List<UserMaintenance> PopulateAllUserInformation { get; set; }
	}
    public class ClientMaster
    {
        [Key]
        public int ClientID { get; set; }
        public int AddID { get; set; }

        public string Client { get; set; }
        public string ClientAddress { get; set; }
        public string ContactNo { get; set; }
        public string ContactPerson { get; set; }
        public string EmailAddress { get; set; }
        public string Location { get; set; }
        public string ClientStatus { get; set; }
        public string HiddenClient { get; set; }
        public int CountActive { get; set; }

		

		public List<ClientMaster> PopulateAllClients { get; set; }
        public List<ClientMaster> PopulateAllClientsAddress { get; set; }
    }
    public class SupplierMaster
    {
        [Key]
        public int SupplierID { get; set; }
        public string Supplier { get; set; }
        public string SupplierAddress { get; set; }
        public string ContactNo { get; set; }
        public string ContactPerson { get; set; }
        public string EmailAddress { get; set; }
        public string Location { get; set; }
        public string SupplierStatus { get; set; }
		public int CountActive { get; set; }

		public List<SupplierMaster> PopulateAllSupplier { get; set; }
    }
    public class ItemMaster
    {
        [Key]
        public int ItemID { get; set; }
        public int CountActive { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string ItemMeasure { get; set; }
        public string ItemCategory { get; set; }
        public string ItemStatus { get; set; }
        public string ItemCode { get; set; }
        public string OldItemCode { get; set; }
        public string RowNumber { get; set; }
        
        public List<SelectListItem> ItemListCategory { get; set; }
        public List<SelectListItem> ItemListMeasure { get; set; }
        public List<SelectListItem> ItemCodeList { get; set; }
        public List<ItemMaster> PopulateAll_Item { get; set; }
    }
    public class ClientModel
    {
        public int ID { get; set; }
        public int NumberID { get; set; }
        public string POC_ReferenceNo { get; set; }
        public string Old_ReferenceNo { get; set; }
        public string PONumber { get; set; }
        public string PODate { get; set; }
        public string TermsofPayment { get; set; }
        public string ClientStatus { get; set; }
        public string Qty { get; set; }
        public string TotalAmount { get; set; }
        public string Amount { get; set; }
        public string ItemID { get; set; }
        public string Quotation_ItemID { get; set; }
		public string ItemName { get; set; }
        public string itemDescription { get; set; }
        public string ClientID { get; set; }
        public string ClientAddress { get; set; }
        public string ItemMeasure { get; set; }
        public string GrandTotal { get; set; }
        public string ProjectName { get; set; }
        public string EntryDate { get; set; }
        public string SignProposal { get; set; }
        public string SalesPersonnel { get; set; }
        public string ProjectDetails { get; set; }
        public string StartDate { get; set; }
        public string CompletionDate { get; set; }
        public string ApproverID { get; set; }
        public string POReferenceNo { get; set; }
        public string Location { get; set; }
        public string Supplier { get; set; }
		public string Discount { get; set; }
        public string ItemStatus { get; set; }
        public string Released { get; set; }
        public string ClientLocation { get; set; }
        public string SupplierPONumber { get; set; }
		public string ClientName { get; set; }
		public string QuotationDiscount { get; set; }
		public string ddlDelivery { get; set; }
		public string ddlPayment { get; set; }
		public string ddlWarranty { get; set; }
        public string ddlPriceValidity { get; set; }
		public string ddlOthersPayment { get; set; }
		public string Submittedby_ID { get; set; }
		public string AmountVAT { get; set; }


        //ITEM MASTER
        public int CountActive { get; set; }
		public string ItemCategory { get; set; }
		public string ItemCode { get; set; }
        

        //QUOTATION

        public string Quotation_ReferenceNo { get; set; }
		public string Quotation_ControlNo { get; set; }
		public string Quotation_ClientName { get; set; }
		public string Quotation_Company { get; set; }
		public string Quotation_Location { get; set; }
		public string Quotation_EntryDate { get; set; }
        public string Quotation_ProjectName { get; set; }
		


		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime POClientDate { get; set; }
		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal ACC_Discount { get; set; }
		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal DisplayAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal DisplayTotalAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal DisplayGrandTotal { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal Markup { get; set; }

        public List<SelectListItem> LocationList { get; set; }
		public List<SelectListItem> TypeOfNumber { get; set; }
		public List<SelectListItem> Approver { get; set; }
		public List<SelectListItem> Submittedby { get; set; }
        public List<SelectListItem> ItemNameList { get; set; }
        public List<SelectListItem> ClientNameList { get; set; }
        public List<SelectListItem> ClientAddressList { get; set; }
        public List<SelectListItem> ItemMeasureList { get; set; }
        public List<SelectListItem> SupplierPONumberList { get; set; }

        public List<SelectListItem> ddlListDelivery { get; set; }
        public List<SelectListItem> ddlListPayment { get; set; }
        public List<SelectListItem> ddlListWarranty { get; set; }
        public List<SelectListItem> ddlListPriceValidity { get; set; }

        public List<ClientModel> PopulateCertificateOfCompletion { get; set; }
        public List<ClientModel> PopulateAllEditClient { get; set; }
        public List<ClientModel> PopulateAllItemList { get; set; }
        public List<ClientModel> PopulateAllItem { get; set; }
		public List<ClientModel> PopulateAllReleasingItem { get; set; }
        public List<ClientModel> PopulateCOCInformation { get; set; }
        public List<ClientModel> PopulateAllApprovedProject { get; set; }




        //DELIVERY RECEIPT
        public string DeliveryNo { get; set; }
        public string DeliveryRemarks { get; set; }
        public string DeliveryDate { get; set; }
        public string ItemCount { get; set; }
        public string Collected { get; set; }
        public string Delivered { get; set; }
        public string FullNameResponsible { get; set; }



        //CERTIFICATE OF COMPLETION

        public string COC_ControlNo { get; set; }
        public string COC_ProjectTitle { get; set; }
        public string COC_Client { get; set; }
        public string COC_Resposible { get; set; }
        public string COC_RejectRemarks { get; set; }
        public string COC_Status { get; set; }
        public string COC_POReference { get; set; }
        public string COC_Assessedby { get; set; }
        public string NotificationView { get; set; }
        

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime COC_StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime COC_CompletionDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime COC_EntryDate { get; set; }




        //ACCOUNTING RECEIVABLES
        public string ACC_ReferenceNo { get; set; }
        public string ACC_BillNo { get; set; }
        public string ORCRNO { get; set; }
        public string ACC_BillType { get; set; }
        public string ACC_DatePaid { get; set; }
        public string ACC_BillDate { get; set; }
        public string ACC_BillAmountHidden { get; set; }
        public string ACC_BillAmount { get; set; }
        public string ACC_Particular { get; set; }
        public string ACC_Responsible { get; set; }
        public string ACC_PoNumber { get; set; }
        public string ACC_Quotation { get; set; }
        public string ACC_TypeOfTerms { get; set; }
        public string ACC_Status { get; set; }
        public string ClientBilling { get; set; }
        public string Typeofbilling { get; set; }
        public string ProjectStatus { get; set; }
		public string ACC_COCNO { get; set; }
		public string ApprovedDate { get; set; }
		public string RejectedDate { get; set; }


        public List<SelectListItem> ListofClientBilling { get; set; }
		public List<SelectListItem> ListBillingType { get; set; }

		public List<SelectListItem> ListofPaymentTerms { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal ACC_BALANCE { get; set; }

		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal DisplayACC_BillAmount { get; set; }

		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime DisplayACC_BillDate { get; set; }

		


		public List<ClientModel> PopulateClientdetails { get; set; }

    }
    public class SupplierModel
    {
        //SUPPLIER INFORMATION
        [Key]
        public int SupplierID { get; set; }
        public string POSReferenceNo { get; set; }
        public string POSupplierNumber { get; set; }
        public string POSupplierDate { get; set; }
        public string Discount { get; set; }
        public string Supplier { get; set; }
        public string FullName { get; set; }
        public string FullNameResponsible { get; set; }
        public string TermsofPayment { get; set; }
        public string Shippinginstruction { get; set; }
        public string SupplierAddresss { get; set; }
        public string DeliveryAddress { get; set; }
        public string SupplierStatus { get; set; }
        public string SignProposal { get; set; }
        public string ProjectDescription { get; set; }
        public string PRNumber { get; set; }
		public string ApproverID { get; set; }
        public string EndorserID { get; set; }
        public string CanvessedBy { get; set; }
		public string LocationAddressID { get; set; }
		public string DateNeeded { get; set; }
		public string PreviewHidden { get; set; }
        public string FormStatus { get; set; }
        public string RejectRemarks { get; set; }
        public string TypeOfVat { get; set; }
        public string TypeOfTerms { get; set; }
        public string ProjectName { get; set; }
        public string Percentage { get; set; }
        public string BillingType { get; set; }
        public string SupplierPayment { get; set; }
        public string ItemStatus { get; set; }
        public string ReviewedDate { get; set; }
        public string EndorsedDate { get; set; }
        public string ApprovedDate { get; set; }
        public string RejectedDate { get; set; }
        public string CancelledDate { get; set; }
        public string ReviewerID { get; set; }
        public string PreparedID { get; set; }
        public string PRReferenceNo { get; set; }
        public string NotificationView { get; set; }
        public string PRStatus { get; set; }

        

        //END SUPPLIER INFORMATION


        // PURCHASE REQUISITION
        public string SupplierID_One { get; set; }
		public string SupplierID_Two { get; set; }
		public string SupplierID_Three { get; set; }

        public string Amount_One { get; set; }
        public string Amount_Two { get; set; }
        public string Amount_Three { get; set; }


        public string TotalAmount_One { get; set; }
        public string TotalAmount_Two { get; set; }
        public string TotalAmount_Three { get; set; }


        public string GrandTotalAmount_One { get; set; }
        public string GrandTotalAmount_Two { get; set; }
        public string GrandTotalAmount_Three { get; set; }


        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal DisplayAmount_One { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal DisplayAmount_Two { get; set; }
		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal DisplayAmount_Three { get; set; }


		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal DisplayTotalAmount_One { get; set; }
		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal DisplayTotalAmount_Two { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal DisplayTotalAmount_Three { get; set; }


		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal DisplayGrandTotal_One { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal DisplayGrandTotal_Two { get; set; }

		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal DisplayGrandTotal_Three { get; set; }


		//GRID VIEW
		public string ParticularName { get; set; }
        public string itemDescription { get; set; }
        public string ItemMeasure { get; set; }
        public string Qty { get; set; }
        public string Amount { get; set; }
        public string TotalAmount { get; set; }
        public string AmountVAT { get; set; }
        public string AmountWithoutVAT { get; set; }
        public string GrandTotal { get; set; }
        public string GrandTotalVat { get; set; }
        public string GrandTotalWithoutVat { get; set; }

		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal ACC_Discount { get; set; }

		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PODate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PRDateNeeded { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal DisplayAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal DisplayTotalAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal DisplayAmountVAT { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal DisplayAmountWithoutVAT { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal DisplayGrandTotal { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal DisplayGrandTotalVat { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal DisplayGrandTotalWithoutVat { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal ACC_BALANCE { get; set; }

        // END GRID VIEW



        public List<SupplierModel> PopulateRoutingApproval { get; set; }
		public List<SupplierModel> PopulateAllUserClient { get; set; }
        public List<SupplierModel> PopulateAllEditSupplier { get; set; }
        public List<SupplierModel> PopulateAllItemList { get; set; }
        public List<SupplierModel> PopulateSupplierNotification { get; set; }
        public List<SupplierModel> PopulateAllReceivingItem { get; set; }
        public List<SupplierModel> PopulateAllEditPurchaseRequisition { get; set; }
        public List<SupplierModel> PopulateAllPurchaseRequisition { get; set; }
		public List<SupplierModel> PopulateSupplierDetails { get; set; }
		public List<SupplierModel> Populate_PRtransaction { get; set; }
        

        public List<string> SelectedListClientReferenceNo { get; set; }
		public List<SelectListItem> ListofPaymentTerms { get; set; }
		public List<SelectListItem> ListofSupplierPayment { get; set; }
        public List<SelectListItem> ListBillingType { get; set; }
		public List<SelectListItem> Approver { get; set; }
		public List<SelectListItem> Endorser { get; set; }
        public List<SelectListItem> SupplierName { get; set; }
        public List<SelectListItem> Particular { get; set; }
        public List<SelectListItem> ItemMeasureList { get; set; }
        public List<SelectListItem> ListClientName { get; set; }
        public List<SelectListItem> ListLocationAddress { get; set; }
        public List<SelectListItem> ListClientReferenceNo { get; set; }
        public List<SelectListItem> ListPRNumber { get; set; }
        public List<SelectListItem> ListAllUser { get; set; }
        


        //CLIENT DATA RETREIVED INTO TABLE
        public string POCReferenceNo { get; set; }
        public string SalesPersonnel { get; set; }
        public string PONumber { get; set; }
        public string Client { get; set; }
        public string ClientStatus { get; set; }
        public string EntryDate { get; set; }


        //DELIVERY RECEIPT

        public string ItemID { get; set; }
        public string DeliveryNo { get; set; }
        public string DeliveryRemarks { get; set; }
        public string DeliveryDate { get; set; }
        public string ItemCount { get; set; }
        public string Collected { get; set; }
        public string Delivered { get; set; }


        //ACCOUNTING PAYABLES
        public string ACC_Ponumber { get; set; }
        public string ACC_POCReference { get; set; }
        public string ACC_POSReference { get; set; }
        public string ACC_Particular { get; set; }
        public string ACC_CheckDate { get; set; }
        public string ACC_DateCollected { get; set; }
        public string ACC_ATICVNO { get; set; }
        public string ACC_LesswithTax { get; set; }
        public string ACC_AmountPaid { get; set; }
        public string ACC_InvoiceNo { get; set; }
        public string ACC_Responsible { get; set; }
        public string ACC_Status { get; set; }
        public string ACC_DRNO { get; set; }
        public string ACC_COCNO { get; set; }
        public string ACC_MRRNO { get; set; }
        public string ACC_LesswithTaxHidden { get; set; }
		public string ACC_AmountPaidHidden { get; set; }



		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal DisplayACC_LesswithTax { get; set; }
		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal DisplayACC_AmountPaid { get; set; }
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime DisplayACC_CheckDate { get; set; }


    }
    public class FormControlModel
    {

        public string POCReferenceNo { get; set; }
        public string POSReferenceNo { get; set; }
        public string Old_ReferenceNo { get; set; }
        public string ProjectName { get; set; }
        public string ClientName { get; set; }
        public string SupplierName { get; set; }
        public string PODate { get; set; }
        public string PONumber { get; set; }
        public string Client_termsPayment { get; set; }
        public string Supplier_termsPayment { get; set; }
        public string ClientStatus { get; set; }
        public string SupplierStatus { get; set; }
        public string POC_TYPE { get; set; }
        public string POS_TYPE { get; set; }
        public string Responsible { get; set; }
        public string SalesPersonnel { get; set; }
        public string EntryDate { get; set; }
        public string PONumberSupplier { get; set; }
        public string FormStatus { get; set; }
        public string RejectRemarks { get; set; }

        public string PaymentStatus { get; set; }
        public string BillingStatus { get; set; }
        public string SupStatus { get; set; }
        public string CoCStatus { get; set; }
        public string ItemStatus { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal GrandTotal { get; set; }

        public List<FormControlModel> Populate_POInformation { get; set; }
        public List<FormControlModel> Populate_Clienttransaction { get; set; }

    }




    public class DRModel
    {
        public int DRID { get; set; }
        public string DRNumber { get; set; }
        public string DRRemarks { get; set; }
        public string DRPONumber { get; set; }
        public string DRResponsible { get; set; }
        public string DRReceived { get; set; }
        public string DRReleased { get; set; }
        public string DRCollected { get; set; }
        public string DRQty { get; set; }
        public string DRParticular { get; set; }
        public string DRClient { get; set; }
        public string DRSupplier { get; set; }
        public string DRItemID { get; set; }
        public string DRReferenceNo { get; set; }
        public string DRItemReturn { get; set; }
        public string DRReasonRemarks { get; set; }
        public string Company { get; set; }
        public string DRDate { get; set; }
        public string ModuleType { get; set; }

		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DRDateReceived { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DRDateReleased { get; set; }

        public List<DRModel> PopulateReceivedLogs { get; set; }
        public List<DRModel> PopulateReleasedLogs { get; set; }
        public List<DRModel> PopulateItemReturnedLogs { get; set; }
		

	}




    public class AccountingModel
    {
        public string ACC_BillNo { get; set; }
        public string ACC_Particular { get; set; }
        public string ACC_InvoiceNo { get; set; }
        public string ACC_ATICVNO { get; set; }
        public string ACC_ID { get; set; }
        public string ACC_ReferenceNo { get; set; }
        public string REC_Qoutation { get; set; }
        public string REC_Amount { get; set; }
        public string ACC_Responsible { get; set; }
        public string REC_EntryDate { get; set; }
        public string ACC_Remarks { get; set; }
        public string REC_Client { get; set; }
        public string REL_Amount { get; set; }
        public string REL_Tax { get; set; }
        public string REL_EntryDate { get; set; }
        public string REL_Supplier { get; set; }
        public string ACC_PONumber { get; set; }
        public string ACC_Client { get; set; }
        public string ACC_Supplier { get; set; }
        public string ACC_ProjectName { get; set; }
        public string ACC_Status { get; set; }
        public string ORCRNO { get; set; }
        public string ACC_BillType { get; set; }
        public string ACC_DRNO { get; set; }
        public string ACC_COCNO { get; set; }
        public string ACC_MRRNO { get; set; }
        public string EntryDate { get; set; }
        public string Audit_Message { get; set; }
		public string UserName { get; set; }

		

		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal ACC_GrandTotalAmount { get; set; }

		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal ACC_Discount { get; set; }

		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal ACC_GrandTotalAmountPaid { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal ACC_BillAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal ACC_LesswithTax { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal ACC_OutStandingBalance { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal ACC_AmountPaid { get; set; }



        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ACC_CheckDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ACC_DateCollected { get; set; }

        public string ACC_DatePaid { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ACC_LastPaymentDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ACC_BillDate { get; set; }

		
		public List<AccountingModel> PopulateAccountingLogs { get; set; }
		public List<AccountingModel> PopulateEditReceivables { get; set; }
		public List<AccountingModel> PopulateEditPayables { get; set; }
        public List<AccountingModel> PopulateAccountingMonitoring { get; set; }




    }













}