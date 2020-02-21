using Microsoft.AspNet.SignalR;
using PO_PurchasingUI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PO_PurchasingUI
{
	public class NotificationComponent
	{
		//here we will add function for register notification(will add sql dependency)

		public string name { get; set; }
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
                string ConnectionString = HttpContext.Current.Server.MapPath("~/DBServer/DatabaseConnection.txt");
                string Username = HttpContext.Current.Server.MapPath("~/DBServer/DatabaseUser.txt");
                string Password = HttpContext.Current.Server.MapPath("~/DBServer/DatabasePassword.txt");

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

        public void SupplierNotificaton(string id)
		{
            DatabaseConnectionString();

            string sqlCommand = @"SELECT [SUPPLIERID],[SUPPLIER_REFERENCENO] FROM [dbo].[POTOSUPPLIER] where (([SUPPLIER_REVIEWERID] = '" + id + "' AND [REVIEWED_DATE] is null and [FORMSTATUS] = 'FOR REVIEW') or ([SUPPLIER_APPROVERID] = '" + id + "' AND [ENDORSED_DATE] is not null and [FORMSTATUS] = 'FOR APPROVAL') or ([SUPPLIER_ENDORSERID] = '" + id + "' and [FORMSTATUS] = 'FOR ENDORSEMENT') or [RESPONSIBLE] = '" + id + "') and [SUPPLIER_STATUS] = 'ON-GOING' and [NEW_REFERENCENO] is null and [SUPPLIER_NOTIFICATION] = 'VIEW'";

			using (SqlConnection connection = new SqlConnection(PotoSupplier))
			{

				SqlCommand cmd = new SqlCommand(sqlCommand, connection);
				SqlDataAdapter sda = new SqlDataAdapter(cmd);
				DataTable dt = new DataTable();
				sda.Fill(dt);
				if (connection.State != ConnectionState.Open)
				{
					connection.Open();
				}
				cmd.Notification = null;
				SqlDependency sqlDep = new SqlDependency(cmd);
				sqlDep.OnChange += new OnChangeEventHandler(sqlDep_OnChange);

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					// nothing need to add here
				}
			}

		}


        public void PurchaseRequisitionNotificaton(string id)
        {
            DatabaseConnectionString();

            string sqlCommand = @"SELECT [PR_NUMBER],[PR_REFERENCENO] FROM [dbo].[PURCHASE_REQUISITION] where ((([PR_REVIEWER] = '" + id + "' AND [PR_REVIEWEDDATE] is null and [PR_FORMSTATUS] = 'FOR REVIEW') or ([PR_APPROVER] = '" + id + "' AND [PR_ENDORSEDDATE] is not null and [PR_FORMSTATUS] = 'FOR APPROVAL') or ([PR_ENDORSER] = '" + id + "' and [PR_FORMSTATUS] = 'FOR ENDORSEMENT')) or [PR_RESPONSIBLE] = '" + id + "') and [PR_STATUS] = 'ON-GOING' and [PR_NEWREFERENCENO] is null and [PR_NOTIFICATION] = 'VIEW'";

            using (SqlConnection connection = new SqlConnection(PotoSupplier))
            {

                SqlCommand cmd = new SqlCommand(sqlCommand, connection);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                cmd.Notification = null;
                SqlDependency sqlDep = new SqlDependency(cmd);
                sqlDep.OnChange += new OnChangeEventHandler(sqlDep_OnChange);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // nothing need to add here
                }
            }

        }



        public void SupplierStatus()
		{
            DatabaseConnectionString();

            string sqlCommand = @"SELECT [SUPPLIERID],[SUPPLIER_REFERENCENO] FROM [dbo].[POTOSUPPLIER] where [SUPPLIER_STATUS] = 'ON-GOING' and [NEW_REFERENCENO] is null and [SUPPLIER_NOTIFICATION] = 'VIEW'";

			using (SqlConnection connection = new SqlConnection(PotoSupplier))
			{

				SqlCommand cmd = new SqlCommand(sqlCommand, connection);
				SqlDataAdapter sda = new SqlDataAdapter(cmd);
				DataTable dt = new DataTable();
				sda.Fill(dt);
				if (connection.State != ConnectionState.Open)
				{
					connection.Open();
				}
				cmd.Notification = null;
				SqlDependency sqlDep = new SqlDependency(cmd);
				sqlDep.OnChange += new OnChangeEventHandler(sqlDep_OnChange);

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					// nothing need to add here
				}
			}

		}

		public void CoCNotificaton(string id)
		{
            DatabaseConnectionString();

            string sqlCommand = @"SELECT [COC_NUMBER],[COC_REFERENCENO] FROM [dbo].[CERTIFICATEOFCOMPLETION] where ([COC_APPROVEDBY] = '" + id + "' and [COC_FORMSTATUS] = 'FOR APPROVAL') or ([COC_PREPAREDBY] = '" + id + "') and [COC_UPDATECONTROLNO] is null and [COC_NOTIFICATION] = 'VIEW'";

			using (SqlConnection connection = new SqlConnection(PobyClient))
			{

				SqlCommand cmd = new SqlCommand(sqlCommand, connection);
				SqlDataAdapter sda = new SqlDataAdapter(cmd);
				DataTable dt = new DataTable();
				sda.Fill(dt);
				if (connection.State != ConnectionState.Open)
				{
					connection.Open();
				}
				cmd.Notification = null;
				SqlDependency sqlDep = new SqlDependency(cmd);
				sqlDep.OnChange += new OnChangeEventHandler(sqlDep_OnChange);

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					// nothing need to add here
				}
			}

		}

		void sqlDep_OnChange(Object sender, SqlNotificationEventArgs e)
		{
			if (e.Type == SqlNotificationType.Change)
			{
                SqlDependency sqlDep = sender as SqlDependency;
                sqlDep.OnChange -= sqlDep_OnChange;

                //from here we will send notification message to client
                var notificationHub = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                notificationHub.Clients.All.notify("added");

                //NotificationHub.SendMessages();



				SupplierNotificaton(name);
				CoCNotificaton(name);
                PurchaseRequisitionNotificaton(name);
                SupplierStatus();

			}

		}

	}
}