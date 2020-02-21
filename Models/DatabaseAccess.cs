using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Web.Mvc;
using Google.Authenticator;
using System.ComponentModel;
using System.Drawing;

namespace PO_PurchasingUI.Models
{
    public class DatabaseAccess
    {
        ////Connection String..
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


        public string Preview_No { get; set; }
        public string PONumber { get; set; }
        public string Quotation_ControlNumber { get; set; }
        public string Quotation_Number { get; set; }
        public string Quotation_ReferenceNo { get; set; }

        public string PR_ControlNumber { get; set; }
        public string PR_Number { get; set; }
        public string PR_ReferenceNo { get; set; }
        public string PR_PReviewNumber { get; set; }

        public string DR_Number { get; set; }
        public string CoC_ControlNumber { get; set; }
        public string CoC_Number { get; set; }
        public string Client_Number { get; set; }
        public int CountActives { get; set; }

        public string Item_Code { get; set; }


        public string POC_ReferenceNo { get; set; }
        public string POC_Year { get; set; }
        public string POC_Number { get; set; }

        public string POS_ReferenceNo { get; set; }
        public string POS_Year { get; set; }
        public string POS_Number { get; set; }

        //Data holder For User Session
        public string SessionUserName { get; set; }
        public string SessionPassword { get; set; }
        public string SessionUserID { get; set; }
        public string ViewForm { get; set; }
        public int CountActiveItem { get; set; }

        //Encrypt and Decrpyt PAssword(Not used)
        public string Encrypt(string str)
        {
            string EncrptKey = "2013;[pnuLIT)Procurement";
            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byKey = System.Text.Encoding.UTF8.GetBytes(EncrptKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(str);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }
        public string Decrypt(string str)
        {
            str = str.Replace(" ", "+");
            string DecryptKey = "2013;[pnuLIT)Procurement";
            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byte[] inputByteArray = new byte[str.Length];

            byKey = System.Text.Encoding.UTF8.GetBytes(DecryptKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByteArray = Convert.FromBase64String(str.Replace(" ", "+"));
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }

        //Auto Generate Reference NO of PO BY CLIENT AND PO TO SUPPLIER
        private void AutoGenerateNumber_PREVIEW()
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("AutoGenerate_PreviewSupplierNo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                SqlDataReader sdr = command.ExecuteReader();
                                while (sdr.Read())
                                {
                                    Preview_No = sdr["NUMBER"].ToString();
                                }
                                connection.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AutoGenerateNumber_PR_PREVIEW()
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("AutoGenerate_PreviewPRNo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                SqlDataReader sdr = command.ExecuteReader();
                                while (sdr.Read())
                                {
                                    PR_PReviewNumber = sdr["NUMBER"].ToString();
                                }
                                connection.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AutoGenerateNumber_COCPREVIEW()
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("AutoGenerate_PreviewCOCNo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                SqlDataReader sdr = command.ExecuteReader();
                                while (sdr.Read())
                                {
                                    Preview_No = sdr["NUMBER"].ToString();
                                }
                                connection.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AutoGenerateNumber_CLIENTPREVIEW()
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    using (command = new SqlCommand("AutoGenerate_ClientNUmber", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                SqlDataReader sdr = command.ExecuteReader();
                                while (sdr.Read())
                                {
                                    Client_Number = sdr["NUMBER"].ToString();
                                }
                                connection.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AutoGenerateReferenceNo_CLIENT()
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("AutoGenerate_ClientRefrenceNo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                SqlDataReader sdr = command.ExecuteReader();
                                while (sdr.Read())
                                {
                                    POC_ReferenceNo = sdr["CLIENTNO"].ToString();
                                    POC_Year = sdr["ENTRYYEAR"].ToString();
                                    POC_Number = sdr["NUMBER"].ToString();
                                }
                                connection.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AutoGenerateControlNumber_QUOTATION()
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("AutoGenerate_ClientQuotationControlNo", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        SqlDataReader sdr = command.ExecuteReader();
                        while (sdr.Read())
                        {
                            Quotation_ControlNumber = sdr["ControlNumber"].ToString();
                            Quotation_Number = sdr["Number"].ToString();
                            Quotation_ReferenceNo = sdr["REFERENCENO"].ToString();



                        }
                    }

                }
            }
            finally
            {
                connection.Close();

            }
        }

        private void AutoGenerateControlNumber_PRNUMBER()
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("AutoGenerate_PRNumber", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        SqlDataReader sdr = command.ExecuteReader();
                        while (sdr.Read())
                        {
                            PR_ControlNumber = sdr["ControlNumber"].ToString();
                            PR_Number = sdr["Number"].ToString();
                            PR_ReferenceNo = sdr["REFERENCENO"].ToString();



                        }
                    }

                }
            }
            finally
            {
                connection.Close();

            }
        }

        private void AutoGenerateReferenceNo_SUPPLIER()
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("AutoGenerate_SupplierReferenceNo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                SqlDataReader sdr = command.ExecuteReader();
                                while (sdr.Read())
                                {
                                    POS_ReferenceNo = sdr["SUPPLIERNO"].ToString();
                                    POS_Year = sdr["ENTRYYEAR"].ToString();
                                    POS_Number = sdr["NUMBER"].ToString();
                                }
                                connection.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AutoGeneratePONumber_SUPPLIER()
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("AutoGenerate_PONumber", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        SqlDataReader sdr = command.ExecuteReader();
                        while (sdr.Read())
                        {
                            PONumber = sdr["PONumber"].ToString();
                        }
                    }

                }
            }
            finally
            {
                connection.Close();

            }
        }

        private void AutoGenerateControlNumber_COC()
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("AutoGenerate_COCControlNumber", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        SqlDataReader sdr = command.ExecuteReader();
                        while (sdr.Read())
                        {
                            CoC_ControlNumber = sdr["COC_ControlNumber"].ToString();
                            CoC_Number = sdr["COC_Number"].ToString();
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                // Add useful information to the exception
                throw new ApplicationException("Something wrong happened in the calculation module :", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public void AutoGenerateDRNumberDB()
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(DrModule))
                {
                    connection.Open();
                    using (command = new SqlCommand("AutoGenerate_DRNumber", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        SqlDataReader sdr = command.ExecuteReader();
                        while (sdr.Read())
                        {
                            DR_Number = sdr["DRNumber"].ToString();
                        }
                    }

                }
            }
            finally
            {
                connection.Close();

            }
        }

        public string AutoGenerateDRNumbersDB()
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            string DR_Numbers = "";
            try
            {
                using (connection = new SqlConnection(DrModule))
                {
                    connection.Open();
                    using (command = new SqlCommand("Get_DRNumber", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        SqlDataReader sdr = command.ExecuteReader();
                        while (sdr.Read())
                        {
                            DR_Numbers = sdr["DRNumber"].ToString();
                        }
                        return DR_Numbers;
                    }

                }
            }
            finally
            {
                connection.Close();

            }
        }

        private void AutoGenerateItemCodeDB(string ItemCode)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    using (command = new SqlCommand("Generate_ItemCode", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ItemCode", ItemCode);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        SqlDataReader sdr = command.ExecuteReader();
                        while (sdr.Read())
                        {
                            Item_Code = sdr["ControlNumber"].ToString();
                        }
                    }

                }
            }
            finally
            {
                connection.Close();

            }
        }

        //public string PopulateClientAddressDB(string id)
        //{
        //	string ClientAddress = "";
        //	SqlConnection connection = null;
        //	SqlCommand command = null;
        //	SupplierModel supplierModel = null;

        //	try
        //	{
        //		using (connection = new SqlConnection(PobyClient))
        //		{
        //			connection.Open();
        //			using (command = new SqlCommand("Select_GetClientAddress", connection))
        //			{

        //				command.CommandType = CommandType.StoredProcedure;
        //				command.Parameters.AddWithValue("@Number", id);

        //				using (SqlDataAdapter sda = new SqlDataAdapter(command))
        //				{

        //					using (DataTable dt = new DataTable())
        //					{
        //						sda.Fill(dt);

        //						foreach (DataRow row in dt.Rows)
        //						{
        //							ClientAddress = row["CLIENT_ADDRESS"].ToString();

        //						}
        //						connection.Close();
        //						return ClientAddress;
        //					}

        //				}
        //			}
        //		}
        //	}
        //	catch (Exception ex)
        //	{
        //		throw ex;
        //	}

        //}

        public string PopulateSupplierAddressDB(string id)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            string SupplierAddress = "";
            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("List_AllSupplierAddressInformation", connection))
                    {

                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SupplierID", id);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {

                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {

                                    SupplierAddress = row["SupplierAddress"].ToString();

                                }
                                connection.Close();
                                return SupplierAddress;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public int CheckItemIfExistingDB(List<ItemMaster> value)
        {
            DatabaseConnectionString();
            int Result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            var checker = "";
            if (value == null)
            {
                value = new List<ItemMaster>();
            }
            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    //Loop and insert retcords.
                    foreach (ItemMaster item in value)
                    {
                        using (command = new SqlCommand("Select_CheckItemIfExisting", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@ITEM_NAME", item.ItemName);

                            SqlDataAdapter sda = new SqlDataAdapter(command);
                            DataTable dt = new DataTable();

                            sda.Fill(dt);
                            foreach (DataRow row in dt.Rows)
                            {
                                checker = row["ITEMNAME"].ToString();
                                Result++;
                            }
                        }

                    }
                    return Result;
                }
            }
            catch
            {
                return Result = 0;
            }
            finally
            {
                connection.Close();
            }
        }

        public string CheckUpdateItemIfExistDB(string Item)
        {
            DatabaseConnectionString();
            string Result = "";
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    //Loop and insert retcords.

                    using (command = new SqlCommand("Select_CheckItemIfExisting", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ITEM_NAME", Item);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();

                        sda.Fill(dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            Result = row["ITEMNAME"].ToString();
                        }
                    }
                }
                return Result;
            }
            catch
            {
                return Result;
            }
            finally
            {
                connection.Close();
            }
        }

        public int CheckClientIfExistingDB(ClientMaster ClientModel)
        {
            DatabaseConnectionString();
            int Result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            var checker = "";
            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();


                    using (command = new SqlCommand("Select_ClientifExisting", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@CLIENT", ClientModel.Client);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();

                        sda.Fill(dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            checker = row["CLIENT"].ToString();
                            Result++;
                        }
                    }

                    return Result;
                }
            }
            catch
            {
                return Result = 0;
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public string CheckSupplierFormStatusDB(string POS_ReferenceNo, string User)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            string Result = "";

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_SupplierFormStatus", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", POS_ReferenceNo);
                        command.Parameters.AddWithValue("@User", User);
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);
                                foreach (DataRow row in dt.Rows)
                                {
                                    Result = row["FORMSTATUS"].ToString();
                                }
                            }
                        }
                    }
                }

                return Result;
            }
            catch
            {
                return Result = "";
            }
            finally
            {
                connection.Close();
            }
        }

        public string CheckPurchaseRequisitionFormStatusDB(string ReferenceNo,string User)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            string Result = "";

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_PRFormStatus", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", ReferenceNo);
                        command.Parameters.AddWithValue("@User", User);
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);
                                foreach (DataRow row in dt.Rows)
                                {
                                    Result = row["PR_FORMSTATUS"].ToString();
                                }
                            }
                        }
                    }
                }

                return Result;
            }
            catch
            {
                return Result = "";
            }
            finally
            {
                connection.Close();
            }
        }

        public string CheckCetificateOfCompletionFormStatusDB(string ControlNo, string User)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            string Result = "";

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_CheckCertificateOfCompletionStatus", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ControlNo", ControlNo);
                        command.Parameters.AddWithValue("@User", User);
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);
                                foreach (DataRow row in dt.Rows)
                                {
                                    Result = row["COC_FORMSTATUS"].ToString();
                                }
                            }
                        }
                    }
                }

                return Result;
            }
            catch
            {
                return Result = "";
            }
            finally
            {
                connection.Close();
            }
        }

        public string Select_CheckIfAllItemReleasedDB(string POC_ReferenceNo)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            string Result = "";

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_CheckIfAllItemReleased", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@REFERENCENO", POC_ReferenceNo);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        SqlDataReader sdr = command.ExecuteReader();
                        while (sdr.Read())
                        {
                            Result = sdr["ITEM_STATUS"].ToString();
                        }

                        return Result;
                    }
                }
            }
            catch
            {
                return Result;
            }
            finally
            {
                connection.Close();
            }
        }

        public string Select_CheckIfAllItemReceivedDB(string POS_ReferenceNo)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            string Result = "";

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_CheckIfAllItemReceived", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@REFERENCENO", POS_ReferenceNo);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        SqlDataReader sdr = command.ExecuteReader();
                        while (sdr.Read())
                        {
                            Result = sdr["ITEM_STATUS"].ToString();
                        }

                        return Result;
                    }
                }
            }
            catch
            {
                return Result;
            }
            finally
            {
                connection.Close();
            }
        }

        private void DeletePreviewSupplierInformationGRIDDB()
        {
            DatabaseConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(PotoSupplier))
                {
                    using (SqlCommand command = new SqlCommand("Delete_PreviewUserSupplierInformation_GRID", connection))
                    {
                        connection.Open();

                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PREVIEW_NO", Preview_No);

                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch
            {
                throw;

            }

        }

        private void DeletePreviewPRInformationGRIDDB()
        {
            DatabaseConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(PotoSupplier))
                {
                    using (SqlCommand command = new SqlCommand("Delete_PreviewPRInformation_GRID", connection))
                    {
                        connection.Open();

                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PREVIEW_NO", PR_PReviewNumber);

                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch
            {
                throw;

            }

        }

        private void DeleteSupplierInformationGRIDDB()
        {
            DatabaseConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(PotoSupplier))
                {
                    using (SqlCommand command = new SqlCommand("Delete_UserSupplierInformation_GRID", connection))
                    {
                        connection.Open();

                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", "POS" + POS_ReferenceNo);

                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch
            {
                throw;

            }

        }

        private void DeletePRInformationGRIDDB()
        {
            DatabaseConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(PotoSupplier))
                {
                    using (SqlCommand command = new SqlCommand("Delete_PRInformation_GRID", connection))
                    {
                        connection.Open();

                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", PR_ReferenceNo);

                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch
            {
                throw;

            }

        }

        private void DeleteClientInformationGRIDDB()
        {
            DatabaseConnectionString();
            using (SqlConnection connection = new SqlConnection(PobyClient))
            {
                using (SqlCommand command = new SqlCommand("Delete_UserClientInformation_GRID", connection))
                {
                    connection.Open();

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ReferenceNo", "POC" + POC_ReferenceNo);

                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        private void DeleteClientQuotationGRIDDB(ClientModel client)
        {
            DatabaseConnectionString();
            using (SqlConnection connection = new SqlConnection(PobyClient))
            {
                using (SqlCommand command = new SqlCommand("Delete_ClientQuotation_GRID", connection))
                {
                    connection.Open();

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ControlNo", Quotation_ControlNumber ?? client.Quotation_ControlNo);

                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public void DeletePreviewsInformationDB(string UserID)
        {
            DatabaseConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(PotoSupplier))
                {
                    using (SqlCommand command = new SqlCommand("Delete_PreviewsInformation", connection))
                    {
                        connection.Open();

                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@USERID", UserID);

                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch
            {
                throw;

            }
        }

        public void DeleteCOCPreviewsInformationDB(string UserID)
        {
            DatabaseConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(PobyClient))
                {
                    using (SqlCommand command = new SqlCommand("Delete_PreviewInformation", connection))
                    {
                        connection.Open();

                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@USERID", UserID);

                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch
            {
                throw;

            }
        }

        public void DeleteCertificateOfCompletionDB()
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;

            using (connection = new SqlConnection(PobyClient))
            {
                using (command = new SqlCommand("Delete_CertificateOfCompletion", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ControlNo", CoC_ControlNumber);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteClientInformationGRIDB(ClientMaster ClientModel)
        {
            DatabaseConnectionString();
            using (SqlConnection connection = new SqlConnection(MasterList))
            {
                using (SqlCommand command = new SqlCommand("Delete_ClientInformation", connection))
                {
                    connection.Open();

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CLIENT", ClientModel.Client);

                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public void DeleteClientAddressDB(string id)
        {
            DatabaseConnectionString();
            using (SqlConnection connection = new SqlConnection(MasterList))
            {
                using (SqlCommand command = new SqlCommand("Delete_ClientAddressInformation", connection))
                {
                    connection.Open();

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CLIENTID", id);

                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public void DeleteClientAddressInfoDB(List<ClientMaster> DeleteDetails)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            int Result = 0;
            if (DeleteDetails == null)
            {
                DeleteDetails = new List<ClientMaster>();
            }

            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    foreach (ClientMaster item in DeleteDetails)
                    {
                        using (command = new SqlCommand("Delete_ClientAddressInfo", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@No", item.AddID);

                            Result++;
                            command.ExecuteNonQuery();
                        }
                    }
                    connection.Close();
                }
            }
            catch
            {
                throw;
            }
        }

        public void DeleteClientDB(string id)
        {
            DatabaseConnectionString();
            using (SqlConnection connection = new SqlConnection(MasterList))
            {
                using (SqlCommand command = new SqlCommand("Delete_ClientIDInformation", connection))
                {
                    connection.Open();

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CLIENTID", id);

                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public void Delete_DrRReleasingDB(string id)
        {
            DatabaseConnectionString();
            using (SqlConnection connection = new SqlConnection(DrModule))
            {
                using (SqlCommand command = new SqlCommand("Delete_DrRReleasing", connection))
                {
                    connection.Open();

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@DRNumber", id);

                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public void Delete_POTaggingDB(string PO, string client)
        {
            DatabaseConnectionString();
            using (SqlConnection connection = new SqlConnection(PotoSupplier))
            {
                using (SqlCommand command = new SqlCommand("Delete_POTagging", connection))
                {
                    connection.Open();

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PO_Number", PO);
                    command.Parameters.AddWithValue("@ClientReferenceNo", client);

                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

        }

        public void InsertPOTaggingDB(string POReferenceNO, string PONumber)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {

                    connection.Open();

                    using (command = new SqlCommand("Insert_PO_tagging", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ClientReferenceNo", POReferenceNO);
                        command.Parameters.AddWithValue("@PONumber", PONumber);
                        command.ExecuteNonQuery();
                    }

                }

            }
            catch
            {

            }
            finally
            {

            }

        }




        //LIST SELECT ITEM LIST
        public string GetItemMeasurementDB(string value)
        {
            DatabaseConnectionString();
            string StringMeasure = "";
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_GetItemMeasurement", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Item", value);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                using (SqlDataReader sdr = command.ExecuteReader())
                                {
                                    while (sdr.Read())
                                    {
                                        StringMeasure = sdr["Symbol"].ToString();
                                    }
                                }
                            }
                        }
                    }
                }
                return StringMeasure;
            }
            catch
            {
                return StringMeasure;
            }
            finally
            {
                connection.Close();
            }

        }

        public string GetItemDescriptionDB(string value)
        {
            DatabaseConnectionString();
            string StringDesc = "";
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_GetItemDescription", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Item", value);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                using (SqlDataReader sdr = command.ExecuteReader())
                                {
                                    while (sdr.Read())
                                    {
                                        StringDesc = sdr["ITEMDESCRIPTION"].ToString();
                                    }
                                }
                            }
                        }
                    }
                }
                return StringDesc;
            }
            catch
            {
                return StringDesc;
            }
            finally
            {
                connection.Close();
            }

        }

        public string GetItemQuantityDB(string value, string POC)
        {
            DatabaseConnectionString();
            string StringQuantity = "";
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_GetItemQuantity", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ITEMNAME", value);
                        command.Parameters.AddWithValue("@REFERENCE", POC);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                using (SqlDataReader sdr = command.ExecuteReader())
                                {
                                    while (sdr.Read())
                                    {
                                        StringQuantity = sdr["CLIENT_QTY"].ToString();
                                    }
                                }
                            }
                        }
                    }
                }
                return StringQuantity;
            }
            catch
            {
                return StringQuantity;
            }
            finally
            {
                connection.Close();
            }

        }

        public string GetBillClientDB(string Type, string Particulars, string Reference)
        {
            DatabaseConnectionString();
            string model = "";
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_ClientPaymentTerms", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Particulars", Particulars);
                        command.Parameters.AddWithValue("@TypeofBilling", Type);
                        command.Parameters.AddWithValue("@DropDownlist", "FALSE");
                        command.Parameters.AddWithValue("@ReferenceNo", Reference);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                using (SqlDataReader sdr = command.ExecuteReader())
                                {
                                    while (sdr.Read())
                                    {

                                        model = sdr["TOTALBALANCE"].ToString();
                                    }
                                }
                            }
                        }
                    }
                }
                return model;
            }
            catch
            {
                return model;
            }
            finally
            {
                connection.Close();
            }

        }

        public List<object> GetNumberofSalesDB()
        {
            DatabaseConnectionString();
            List<object> list = new List<object>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Chart_NumberofSales", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);


                        foreach (DataColumn dc in dt.Columns)
                        {
                            List<object> x = new List<object>();
                            x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                            list.Add(x);
                        }
                        return list;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }



        }

        public List<object> GetNumberofOrdersDB()
        {
            DatabaseConnectionString();
            List<object> list = new List<object>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Chart_NumberofOrders", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);


                        foreach (DataColumn dc in dt.Columns)
                        {
                            List<object> x = new List<object>();
                            x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                            list.Add(x);
                        }
                        return list;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }



        }

        public List<string> ListClientReferenceNoDB(string PONumber)
        {
            DatabaseConnectionString();
            List<string> listReferenceNo = new List<string>();
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_POTagging", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNO", "");
                        command.Parameters.AddWithValue("@PO_Number", PONumber);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {

                            var Value = row["TAG_CLIENT_REF"].ToString();


                            listReferenceNo.Add(Value);
                        }
                        return listReferenceNo;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<string> ListSupplierPONumberDB(string POCReferenceNo)
        {
            DatabaseConnectionString();
            List<string> listReferenceNo = new List<string>();
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_POTagging", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNO", POCReferenceNo);
                        command.Parameters.AddWithValue("@PO_Number", "");

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {

                            var Value = row["TAG_PO_NUMBER"].ToString();


                            listReferenceNo.Add(Value);
                        }
                        return listReferenceNo;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }


        //POPULATE DATA IN DROPDOWNLIST
        public List<SelectListItem> PopulateListUserGroupDB(string value)
        {
            DatabaseConnectionString();
            List<SelectListItem> UserGroupList = new List<SelectListItem>();

            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("List_GetUserGroup", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@USERROLEID", value);
                        SqlDataReader sdr = command.ExecuteReader();

                        while (sdr.Read())
                        {
                            UserGroupList.Add(new SelectListItem
                            {
                                Text = sdr["USERGROUPNAME"].ToString(),
                                Value = sdr["USERGROUPID"].ToString()
                            });
                        }
                    }
                    return UserGroupList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<SelectListItem> List_ClientItemListDB(string id)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            List<SelectListItem> ClientItemList = new List<SelectListItem>();

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_ClientItemList", connection))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@ReferenceNo", id);

                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    ClientItemList.Add(new SelectListItem
                                    {
                                        Text = row["CLIENT_ITEMNAME"].ToString(),
                                        Value = row["CLIENT_ITEMID"].ToString()

                                    });

                                }
                                connection.Close();
                                return ClientItemList;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<SelectListItem> PopulateListDepartmentDB()
        {
            DatabaseConnectionString();
            List<SelectListItem> DeptList = new List<SelectListItem>();
            SqlConnection connection = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("List_AllDepartment", connection))
                    {
                        SqlDataReader sdr = command.ExecuteReader();

                        while (sdr.Read())
                        {
                            DeptList.Add(new SelectListItem
                            {
                                Text = sdr["DEPTNAME"].ToString(),
                                Value = sdr["DEPTID"].ToString()

                            });

                        }

                        return DeptList;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }


        }

        public List<SelectListItem> PopulateAllListUserGroupDB()
        {
            DatabaseConnectionString();
            List<SelectListItem> UserGroupList = new List<SelectListItem>();

            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("List_AllUserGroup", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        SqlDataReader sdr = command.ExecuteReader();

                        while (sdr.Read())
                        {
                            UserGroupList.Add(new SelectListItem
                            {
                                Text = sdr["USERGROUPNAME"].ToString(),
                                Value = sdr["USERGROUPID"].ToString()
                            });
                        }
                    }
                    return UserGroupList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<SelectListItem> PopulateListUserRoleDB()
        {
            DatabaseConnectionString();
            List<SelectListItem> UserRoleList = new List<SelectListItem>();
            SqlConnection connection = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("List_AllUserRole", connection))
                    {
                        SqlDataReader sdr = command.ExecuteReader();
                        while (sdr.Read())
                        {
                            UserRoleList.Add(new SelectListItem
                            {
                                Text = sdr["USERROLENAME"].ToString(),
                                Value = sdr["USERROLEID"].ToString()
                            });
                        }
                        return UserRoleList;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }

        }

        public List<SelectListItem> PopulateListApproverDB(string UserName)
        {
            DatabaseConnectionString();
            List<SelectListItem> ApproverList = new List<SelectListItem>();
            SqlConnection connection = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("List_AllApproverEndorser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@USERNAME", UserName);
                        SqlDataReader sdr = command.ExecuteReader();
                        while (sdr.Read())
                        {
                            ApproverList.Add(new SelectListItem
                            {

                                Text = sdr["FULLNAME"].ToString(),
                                Value = sdr["USERID"].ToString()

                            });
                        }
                        return ApproverList;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<SelectListItem> PopulateListAllUserDB()
        {
            DatabaseConnectionString();
            List<SelectListItem> ApproverList = new List<SelectListItem>();
            SqlConnection connection = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Select_AllUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        SqlDataReader sdr = command.ExecuteReader();
                        while (sdr.Read())
                        {
                            ApproverList.Add(new SelectListItem
                            {

                                Text = sdr["FULLNAME"].ToString(),
                                Value = sdr["USERID"].ToString()

                            });
                        }
                        return ApproverList;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<SelectListItem> Select_AllSalesUserDB()
        {
            DatabaseConnectionString();
            List<SelectListItem> ApproverList = new List<SelectListItem>();
            SqlConnection connection = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Select_AllSalesUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        SqlDataReader sdr = command.ExecuteReader();
                        while (sdr.Read())
                        {
                            ApproverList.Add(new SelectListItem
                            {

                                Text = sdr["FULLNAME"].ToString(),
                                Value = sdr["USERID"].ToString()

                            });
                        }
                        return ApproverList;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }
        

        public List<SelectListItem> PopulateListofMeasurementDB()
        {
            DatabaseConnectionString();
            List<SelectListItem> ListofMeasure = new List<SelectListItem>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_PopulateListOfMeasurement", connection))
                    {
                        SqlDataReader sdr = command.ExecuteReader();
                        while (sdr.Read())
                        {
                            ListofMeasure.Add(new SelectListItem
                            {

                                Text = sdr["Symbol"].ToString(),
                                Value = sdr["UM_ID"].ToString()

                            });
                        }
                        return ListofMeasure;
                    }
                }
            }
            catch
            {
                return ListofMeasure;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<SelectListItem> List_AllSupplierDB()
        {
            DatabaseConnectionString();
            List<SelectListItem> ListSupplier = new List<SelectListItem>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    using (command = new SqlCommand("List_AllSupplierInformation", connection))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    ListSupplier.Add(new SelectListItem
                                    {
                                        Value = row["SUPPLIERID"].ToString(),
                                        Text = row["SUPPLIER"].ToString()
                                    });
                                }

                            }
                        }

                    }
                    connection.Close();
                    return ListSupplier;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListItem> List_AllPRNumberDB()
        {
            DatabaseConnectionString();
            List<SelectListItem> ListSupplier = new List<SelectListItem>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_PopulateListPRNumber", connection))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    ListSupplier.Add(new SelectListItem
                                    {
                                        Value = row["PR_NUMBER"].ToString(),
                                        Text = row["PR_NUMBER"].ToString()
                                    });
                                }

                            }
                        }

                    }
                    connection.Close();
                    return ListSupplier;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListItem> List_GetAllPRNumberDB(string Number)
        {
            DatabaseConnectionString();
            List<SelectListItem> ListSupplier = new List<SelectListItem>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_PopulateGetListPRNumber", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("Number", Number);
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    ListSupplier.Add(new SelectListItem
                                    {
                                        Value = row["PR_NUMBER"].ToString(),
                                        Text = row["PR_NUMBER"].ToString()
                                    });
                                }

                            }
                        }

                    }
                    connection.Close();
                    return ListSupplier;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListItem> List_PopulateItemNameDB()
        {
            DatabaseConnectionString();
            List<SelectListItem> ItemList = new List<SelectListItem>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    using (command = new SqlCommand("List_AllItemInformation", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ItemList.Add(new SelectListItem
                            {
                                Value = row["ITEMID"].ToString(),
                                Text = row["ITEMNAME"].ToString()
                            });
                        }
                    }
                    connection.Close();
                    return ItemList;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListItem> List_PopulateClientNameDB()
        {
            DatabaseConnectionString();
            List<SelectListItem> ClientList = new List<SelectListItem>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    using (command = new SqlCommand("List_AllClientInformation", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ClientList.Add(new SelectListItem
                            {
                                Text = row["CLIENT"].ToString(),
                                Value = row["CLIENTID"].ToString()
                            });
                        }
                    }
                    connection.Close();
                    return ClientList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }



        }

        public List<SelectListItem> List_PopulateAllClientAddressDB(string id)
        {
            DatabaseConnectionString();
            List<SelectListItem> ClientAddressList = new List<SelectListItem>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_PopulateAllClientAddress", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ClientID", id);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ClientAddressList.Add(new SelectListItem
                            {
                                Text = row["CLIENT_LOCATION"].ToString(),
                                Value = row["NUMBER"].ToString()
                            });
                        }
                    }
                    connection.Close();
                    return ClientAddressList;
                }
            }
            catch
            {
                return ClientAddressList;
            }
        }

        public List<SelectListItem> List_PopulateClientReferenceNoDB()
        {
            DatabaseConnectionString();
            List<SelectListItem> ClientAddressList = new List<SelectListItem>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Populate_ClientReferenceNo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ClientAddressList.Add(new SelectListItem
                            {
                                Text = row["PROJECTNAME"].ToString(),
                                Value = row["CLIENT_REFERENCENO"].ToString()
                            });
                        }
                    }
                    connection.Close();
                    return ClientAddressList;
                }
            }
            catch
            {
                return ClientAddressList;
            }
        }

        public List<SelectListItem> List_PopulateAllItemCategoryDB()
        {
            DatabaseConnectionString();
            List<SelectListItem> ListItemCategory = new List<SelectListItem>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_ItemCategory", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ListItemCategory.Add(new SelectListItem
                            {

                                Text = row["CATEGORYNAME"].ToString(),
                                Value = row["CATEGORYID"].ToString(),

                            });
                        }
                        return ListItemCategory;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<SelectListItem> List_PaymentTermsDB()
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            List<SelectListItem> ListPayment = new List<SelectListItem>();

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_PaymentTerms", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);


                        foreach (DataRow row in dt.Rows)
                        {
                            ListPayment.Add(new SelectListItem()
                            {
                                Text = row["PAYMENTTERMS"].ToString(),
                                Value = row["TERMSID"].ToString(),
                            });
                        }
                        return ListPayment;


                    }
                }
            }
            catch
            {
                return ListPayment;

            }
            finally
            {
                connection.Close();
            }
        }

        public List<SelectListItem> PopulateClientPaymentTermsDB(string id)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            List<SelectListItem> ListPayment = new List<SelectListItem>();

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_ClientParticulars", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", id);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);


                        foreach (DataRow row in dt.Rows)
                        {
                            ListPayment.Add(new SelectListItem()
                            {
                                Text = row["PARTICULARS"].ToString(),
                                Value = row["PARTICULARS"].ToString(),
                            });
                        }
                        return ListPayment;


                    }
                }
            }
            catch
            {
                return ListPayment;

            }
            finally
            {
                connection.Close();
            }
        }

        public List<SelectListItem> PopulateClientBillingTypeDB(string id, string Particular)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            List<SelectListItem> ListPayment = new List<SelectListItem>();

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_ClientPaymentTerms", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", id);
                        command.Parameters.AddWithValue("@TypeofBilling", DBNull.Value);
                        command.Parameters.AddWithValue("@Particulars", Particular);
                        command.Parameters.AddWithValue("@DropDownlist", "TRUE");
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);


                        foreach (DataRow row in dt.Rows)
                        {
                            ListPayment.Add(new SelectListItem()
                            {
                                Text = row["BILLINGNAME"].ToString(),
                                Value = row["BILLINGTYPE"].ToString(),
                            });
                        }
                        return ListPayment;


                    }
                }
            }
            catch
            {
                return ListPayment;

            }
            finally
            {
                connection.Close();
            }
        }

        public List<SelectListItem> PopulateSupplierPaymentTermsDB(string id)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            List<SelectListItem> ListPayment = new List<SelectListItem>();

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_SupplierParticulars", connection))
                    {

                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", id);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);


                        foreach (DataRow row in dt.Rows)
                        {
                            ListPayment.Add(new SelectListItem()
                            {
                                Text = row["PARTICULARS"].ToString(),
                                Value = row["PARTICULARS"].ToString(),
                            });
                        }
                        return ListPayment;


                    }
                }
            }
            catch
            {
                return ListPayment;

            }
            finally
            {
                connection.Close();
            }
        }

        public List<SelectListItem> PopulateSupplierBillingTypeDB(string id, string Particular)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            List<SelectListItem> ListPayment = new List<SelectListItem>();

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_SupplierPaymentTerms", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", id);
                        command.Parameters.AddWithValue("@TypeofBilling", DBNull.Value);
                        command.Parameters.AddWithValue("@Particulars", Particular);
                        command.Parameters.AddWithValue("@DropDownlist", "TRUE");
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);


                        foreach (DataRow row in dt.Rows)
                        {
                            ListPayment.Add(new SelectListItem()
                            {
                                Text = row["BILLINGNAME"].ToString(),
                                Value = row["BILLINGTYPE"].ToString(),
                            });
                        }
                        return ListPayment;


                    }
                }
            }
            catch
            {
                throw;
                return ListPayment;

            }
            finally
            {
                connection.Close();
            }
        }

        public List<SelectListItem> List_UpdateSupplierPaymentDB(string ReferenceNo)
        {
            DatabaseConnectionString();
            List<SelectListItem> listPaymentDB = new List<SelectListItem>();
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_GetSupplierPayment", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@REFERENCENO", ReferenceNo);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            listPaymentDB.Add(new SelectListItem
                            {
                                Text = row["ACC_INVOICENO"].ToString(),
                                Value = row["ACC_ID"].ToString(),
                            });
                        }
                        return listPaymentDB;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<SelectListItem> List_UpdateClientBillingDB(string ReferenceNo)
        {
            DatabaseConnectionString();
            List<SelectListItem> listPaymentDB = new List<SelectListItem>();
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_GetClientBilling", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@REFERENCENO", ReferenceNo);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            listPaymentDB.Add(new SelectListItem
                            {
                                Text = row["BILLNO"].ToString(),
                                Value = row["ACCID"].ToString(),
                            });
                        }
                        return listPaymentDB;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<SelectListItem> List_SupplierPONumberDB()
        {
            DatabaseConnectionString();
            List<SelectListItem> ListPONumber = new List<SelectListItem>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_ListSupplierPONumber", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ListPONumber.Add(new SelectListItem
                            {
                                Text = row["PROJECTNAME"].ToString(),
                                Value = row["SUPPLIER_PONUMBER"].ToString()
                            });
                        }
                    }
                    connection.Close();
                    return ListPONumber;
                }
            }
            catch
            {
                return ListPONumber;
            }
        }

        public List<SelectListItem> List_ItemCode()
        {
            DatabaseConnectionString();
            List<SelectListItem> ListPONumber = new List<SelectListItem>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    using (command = new SqlCommand("List_ItemCode", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ListPONumber.Add(new SelectListItem
                            {
                                Text = row["ItemDesc"].ToString(),
                                Value = row["ItemCode"].ToString()
                            });
                        }
                    }
                    connection.Close();
                    return ListPONumber;
                }
            }
            catch
            {
                return ListPONumber;
            }
        }




        //POPULATION OF DATA IN TABLE


        public List<ClientMaster> PopulateAllClientInformationDB()
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            List<ClientMaster> ClientList = new List<ClientMaster>();
            int CountActive = 0;
            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_AllClientInformation", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ClientMaster ClientModel = new ClientMaster
                            {
                                ClientID = Convert.ToInt32(row["CLIENTID"].ToString()),
                                Client = row["CLIENT"].ToString(),
                                EmailAddress = row["EMAILADDRESS"].ToString(),
                                ContactNo = row["CONTACTNO"].ToString(),
                                ContactPerson = row["CONTACTPERSON"].ToString(),
                                ClientStatus = row["CLIENTSTATUS"].ToString(),
                            };

                            if (ClientModel.ClientStatus == "Active")
                            {
                                CountActive++;
                            }
                            CountActives = CountActive;


                            ClientList.Add(ClientModel);
                        }

                        connection.Close();
                        return ClientList;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<UserMaintenance> POpulateAllUserInformationDB()
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            List<UserMaintenance> ListUserInformation = new List<UserMaintenance>();
            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_AllUserInformation", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            UserMaintenance UserModel = new UserMaintenance
                            {
                                UserID = Convert.ToInt32(row["USERID"].ToString()),
                                EmployeeID = row["EMPID"].ToString(),
                                UserName = row["USERNAME"].ToString(),
                                FullName = row["FULLNAME"].ToString(),
                                FirstName = row["FIRSTNAME"].ToString(),
                                LastName = row["LASTNAME"].ToString(),
                                EmailAddress = row["EMAIL"].ToString(),
                                UserStatus = row["USERSTATUS"].ToString(),
                                UserRole = row["USERROLENAME"].ToString()
                            };

                            ListUserInformation.Add(UserModel);
                        }

                    }
                    connection.Close();
                    return ListUserInformation;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<ItemMaster> PopulateAllItemMasterDB()
        {
            DatabaseConnectionString();
            int CountActive = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            List<ItemMaster> ItemLIst = new List<ItemMaster>();
            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_AllItemInformation", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ItemMaster ItemModel = new ItemMaster
                            {
                                RowNumber = row["ROWNUMBER"].ToString(),
                                ItemID = Convert.ToInt32(row["ITEMID"].ToString()),
                                ItemCategory = row["ITEMCATEGORYNAME"].ToString(),
                                ItemName = row["ITEMNAME"].ToString(),
                                ItemDescription = row["ITEMDESCRIPTION"].ToString(),
                                ItemMeasure = row["Symbol"].ToString(),
                                ItemCode = row["ITEMCODE"].ToString(),
                                ItemStatus = row["ITEMSTATUS"].ToString()
                            };
                            if (ItemModel.ItemStatus == "Active")
                            {
                                CountActive++;
                            }
                            CountActiveItem = CountActive;

                            ItemLIst.Add(ItemModel);



                        }
                        connection.Close();
                        return ItemLIst;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SupplierMaster> PopulateAllSupplierInformationDB()
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            List<SupplierMaster> SupplierList = new List<SupplierMaster>();
            int CountActive = 0;
            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_AllSupplierInformation", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            SupplierMaster SupplierModel = new SupplierMaster
                            {
                                SupplierID = Convert.ToInt32(row["SUPPLIERID"].ToString()),
                                Supplier = row["SUPPLIER"].ToString(),
                                SupplierAddress = row["SUPPLIERADDRESS"].ToString(),
                                EmailAddress = row["EMAILADDRESS"].ToString(),
                                ContactNo = row["CONTACTNO"].ToString(),
                                ContactPerson = row["CONTACTPERSON"].ToString(),
                                SupplierStatus = row["SUPPLIERSTATUS"].ToString(),
                                Location = row["LOCATION"].ToString()
                            };

                            if (SupplierModel.SupplierStatus == "Active")
                            {
                                CountActive++;
                            }
                            CountActives = CountActive;

                            SupplierList.Add(SupplierModel);
                        }
                        connection.Close();
                        return SupplierList;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ClientMaster> PopulateClientAddressDB(int? id)
        {
            DatabaseConnectionString();
            List<ClientMaster> Clientlist = new List<ClientMaster>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    using (command = new SqlCommand("Edit_ClientAddress", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@CLIENTID", id);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    ClientMaster clientModel = new ClientMaster
                                    {
                                        AddID = Convert.ToInt32(row["Number"].ToString()),
                                        ClientAddress = row["CLIENT_ADDRESS"].ToString(),
                                        Location = row["CLIENT_LOCATION"].ToString(),

                                    };
                                    Clientlist.Add(clientModel);

                                }
                                connection.Close();
                                return Clientlist;
                            }
                        }
                    }

                }
            }
            catch
            {
                return Clientlist;

            }
        }

        public List<FormControlModel> PopulateAllPOInformationDB()
        {
            DatabaseConnectionString();
            List<FormControlModel> ListFormControl = new List<FormControlModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_FormsControl", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            FormControlModel formcontrolModel = new FormControlModel
                            {
                                POCReferenceNo = row["CLIENT_REFERENCENO"].ToString(),
                                ClientName = row["CLIENT"].ToString(),
                                PONumber = row["CLIENT_PONUMBER"].ToString(),
                                ClientStatus = row["CLIENT_STATUS"].ToString(),
                                ProjectName = row["CLIENT_PROJECTNAME"].ToString(),
                                Responsible = row["RESPONSIBLE"].ToString(),
                                SalesPersonnel = row["CLIENT_SALES"].ToString(),
                                EntryDate = row["ENTRYDATE"].ToString(),
                                PaymentStatus = row["ACCOUNTING_STATUS"].ToString(),
                                CoCStatus = row["COC_CONTROLNO"].ToString(),
                                GrandTotal = Convert.ToDecimal(row["CLIENT_GRANDTOTAL"].ToString()),
                                ItemStatus = row["ITEMSTATUS"].ToString(),


                            };
                            ListFormControl.Add(formcontrolModel);
                        }
                        connection.Close();
                        return ListFormControl;
                    }
                }

            }
            catch
            {
                return ListFormControl;
            }
        }

        public List<FormControlModel> PopulateClientSupplierTransactionDB()
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            List<FormControlModel> ListFormControl = new List<FormControlModel>();

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_GetSupplierTransaction", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            FormControlModel formcontrolModel = new FormControlModel
                            {
                                POSReferenceNo = row["SUPPLIER_REFERENCENO"].ToString(),
                                SupplierName = row["SUPPLIER"].ToString(),
                                PONumberSupplier = row["SUPPLIER_PONUMBER"].ToString(),
                                SupplierStatus = row["SUPPLIER_STATUS"].ToString(),
                                GrandTotal = Convert.ToDecimal(row["SUPPLIER_GRANDTOTAL"].ToString()),
                                Responsible = row["RESPONSIBLE"].ToString(),
                                EntryDate = row["ENTRYDATE"].ToString(),
                                FormStatus = row["FORMSTATUS"].ToString(),
                                SupStatus = row["ACCOUNTING_STATUS"].ToString(),
                                RejectRemarks = row["REJECTED_REMARKS"].ToString(),
                                ItemStatus = row["ITEMSTATUS"].ToString(),
                            };
                            ListFormControl.Add(formcontrolModel);
                        }
                        connection.Close();
                        return ListFormControl;
                    }
                }

            }
            catch
            {
                return ListFormControl;
            }
        }

        public List<ClientModel> PopulateAllEditClientDB(string Responsible)
        {
            DatabaseConnectionString();
            List<ClientModel> ListClient = new List<ClientModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            using (connection = new SqlConnection(PobyClient))
            {
                connection.Open();
                using (command = new SqlCommand("Select_PopulateAllGoingUserClient", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@USER", Responsible);
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        ClientModel clientModel = new ClientModel
                        {
                            POC_ReferenceNo = row["CLIENT_REFERENCENO"].ToString(),
                            PONumber = row["CLIENT_PONUMBER"].ToString(),
                            DisplayGrandTotal = Convert.ToDecimal(row["CLIENT_GRANDTOTAL"].ToString()),
                            PODate = row["CLIENT_PODATE"].ToString(),
                            EntryDate = row["ENTRYDATE"].ToString(),
                            ClientID = row["CLIENTNAME"].ToString(),
                            ClientStatus = row["CLIENT_STATUS"].ToString(),
                            ProjectName = row["CLIENT_PROJECTNAME"].ToString(),
                            SignProposal = row["CLIENT_QUOTATIONNO"].ToString()
                        };
                        ListClient.Add(clientModel);
                    }
                    connection.Close();
                    return ListClient;
                }
            }
        }

        public List<ClientModel> PopulateProjectCostReportDB()
        {
            DatabaseConnectionString();
            List<ClientModel> ListClient = new List<ClientModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            using (connection = new SqlConnection(Accounting))
            {
                connection.Open();
                using (command = new SqlCommand("Select_POProjectSummary", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        ClientModel clientModel = new ClientModel
                        {
                            POC_ReferenceNo = row["CLIENT_REFERENCENO"].ToString(),
                            PONumber = row["CLIENT_PONUMBER"].ToString(),
                            DisplayGrandTotal = Convert.ToDecimal(row["CLIENT_GRANDTOTAL"].ToString()),
                            PODate = row["CLIENT_PODATE"].ToString(),
                            EntryDate = row["ENTRYDATE"].ToString(),
                            ClientID = row["CLIENTNAME"].ToString(),
                            ClientStatus = row["CLIENT_STATUS"].ToString(),
                            ProjectName = row["CLIENT_PROJECTNAME"].ToString(),
                            SignProposal = row["CLIENT_QUOTATIONNO"].ToString(),
                            ProjectStatus = row["PROJECTSTATUS"].ToString()
                        };
                        ListClient.Add(clientModel);
                    }
                    connection.Close();
                    return ListClient;
                }
            }
        }

        public List<ClientModel> PopulateItemClientQoutationGRIDDB(string id)
        {
            DatabaseConnectionString();
            List<ClientModel> LIstItemModel = new List<ClientModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            using (connection = new SqlConnection(PobyClient))
            {
                connection.Open();
                using (command = new SqlCommand("Select_EditClientQuotation_GRID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ControlNo", id);
                    using (SqlDataAdapter sda = new SqlDataAdapter(command))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);

                            foreach (DataRow row in dt.Rows)
                            {
                                ClientModel clientModel = new ClientModel
                                {

                                    ItemID = row["QUOTATION_ITEMNAME"].ToString(),
                                    itemDescription = row["QUOTATION_ITEMDESC"].ToString(),
                                    ItemMeasure = row["QUOTATION_ITEMMEASURE"].ToString(),
                                    Qty = row["QUOTATION_QTY"].ToString(),
                                    DisplayAmount = Convert.ToDecimal(row["QUOTATION_AMOUNT"].ToString()),
                                    Markup = Convert.ToDecimal(row["QUOTATION_MARKUP"].ToString()),
                                    DisplayTotalAmount = Convert.ToDecimal(row["QUOTATION_TOTALAMOUNT"].ToString()),

                                };
                                GetItemMeasurementDB(clientModel.ItemID);
                                LIstItemModel.Add(clientModel);
                            }
                        }
                    }
                }
                connection.Close();
                return LIstItemModel;

            }
        }

        public List<ClientModel> PolulateClientItemListDB(string id)
        {
            DatabaseConnectionString();
            List<ClientModel> LIstItemModel = new List<ClientModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            using (connection = new SqlConnection(PobyClient))
            {
                connection.Open();
                using (command = new SqlCommand("Select_EditClientInformation_GRID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ReferenceNo", id);
                    using (SqlDataAdapter sda = new SqlDataAdapter(command))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);

                            foreach (DataRow row in dt.Rows)
                            {
                                ClientModel clientModel = new ClientModel
                                {

                                    ItemID = row["CLIENT_ITEMNAME"].ToString(),
                                    itemDescription = row["CLIENT_ITEMDESC"].ToString(),
                                    ItemMeasure = row["CLIENT_ITEMMEASURE"].ToString(),
                                    Qty = row["CLIENT_QTY"].ToString(),
                                    DisplayAmount = Convert.ToDecimal(row["CLIENT_AMOUNT"].ToString()),
                                    Markup = Convert.ToDecimal(row["CLIENT_MARKUP"].ToString()),
                                    DisplayTotalAmount = Convert.ToDecimal(row["CLIENT_TOTALAMOUNT"].ToString()),
                                    Released = row["CLIENT_RELEASED"].ToString(),

                                };
                                GetItemMeasurementDB(clientModel.ItemID);
                                LIstItemModel.Add(clientModel);
                            }
                        }
                    }
                }
                connection.Close();
                return LIstItemModel;

            }
        }

        public List<ClientModel> PopulateClientReceivablesDB()
        {
            DatabaseConnectionString();
            List<ClientModel> ListClient = new List<ClientModel>();
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_PopulateClientInformation", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ClientModel clientModel = new ClientModel
                            {
                                POC_ReferenceNo = row["CLIENT_REFERENCENO"].ToString(),
                                PONumber = row["CLIENT_PONUMBER"].ToString(),
                                DisplayGrandTotal = Convert.ToDecimal(row["CLIENT_GRANDTOTAL"].ToString()),
                                PODate = row["CLIENT_PODATE"].ToString(),
                                EntryDate = row["ENTRYDATE"].ToString(),
                                ClientID = row["CLIENTNAME"].ToString(),
                                ClientStatus = row["CLIENT_STATUS"].ToString(),
                                ProjectName = row["CLIENT_PROJECTNAME"].ToString(),
                                SignProposal = row["CLIENT_QUOTATIONNO"].ToString(),
                                ACC_Status = row["ACCOUNTING_STATUS"].ToString(),
                                ACC_BALANCE = Convert.ToDecimal(row["BALANCE"].ToString()),
                                ACC_TypeOfTerms = row["PAYMENTTERMS"].ToString(),
                            };
                            ListClient.Add(clientModel);
                        }
                        return ListClient;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {

                connection.Close();
            }

        }

        public List<ClientModel> PopulateAllClientRequestDB()
        {
            DatabaseConnectionString();
            List<ClientModel> ListDeliveryReceipt = new List<ClientModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    using (command = new SqlCommand("Select_PopulateAllClientRequest", connection))
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);

                            foreach (DataRow row in dt.Rows)
                            {
                                ClientModel clientModel = new ClientModel
                                {
                                    POC_ReferenceNo = row["CLIENT_REFERENCENO"].ToString(),
                                    SignProposal = row["CLIENT_QUOTATIONNO"].ToString(),
                                    ClientID = row["CLIENT_IDNAME"].ToString(),
                                    PONumber = row["CLIENT_PONUMBER"].ToString(),
                                    ClientStatus = row["CLIENT_STATUS"].ToString(),
                                    DisplayGrandTotal = Convert.ToDecimal(row["CLIENT_GRANDTOTAL"].ToString()),
                                    FullNameResponsible = row["RESPONSIBLE"].ToString(),
                                    EntryDate = row["ENTRYDATE"].ToString(),
                                    Location = row["CLIENT_LOCATION"].ToString(),
                                    ClientAddress = row["CLIENT_ADDRESS"].ToString(),
                                    ProjectName = row["CLIENT_PROJECTNAME"].ToString(),
                                    ItemStatus = row["ITEMSTATUS"].ToString(),
                                };
                                ListDeliveryReceipt.Add(clientModel);
                            }
                            return ListDeliveryReceipt;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<ClientModel> PopulateAllCOCClientRequestDB()
        {
            DatabaseConnectionString();
            List<ClientModel> ListDeliveryReceipt = new List<ClientModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    using (command = new SqlCommand("Select_PopulateAllCOCClientRequest", connection))
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);

                            foreach (DataRow row in dt.Rows)
                            {
                                ClientModel clientModel = new ClientModel
                                {
                                    POC_ReferenceNo = row["CLIENT_REFERENCENO"].ToString(),
                                    SignProposal = row["CLIENT_QUOTATIONNO"].ToString(),
                                    ClientID = row["CLIENT_IDNAME"].ToString(),
                                    PONumber = row["CLIENT_PONUMBER"].ToString(),
                                    ClientStatus = row["CLIENT_STATUS"].ToString(),
                                    DisplayGrandTotal = Convert.ToDecimal(row["CLIENT_GRANDTOTAL"].ToString()),
                                    FullNameResponsible = row["RESPONSIBLE"].ToString(),
                                    EntryDate = row["ENTRYDATE"].ToString(),
                                    ClientAddress = row["CLIENT_ADDRESS"].ToString(),
                                    Location = row["CLIENT_LOCATION"].ToString(),
                                    ProjectName = row["CLIENT_PROJECTNAME"].ToString(),
                                    ItemStatus = row["ITEMSTATUS"].ToString(),
                                };
                                ListDeliveryReceipt.Add(clientModel);
                            }
                            return ListDeliveryReceipt;
                        }
                    }
                }
            }
            catch
            {
                return ListDeliveryReceipt;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<ClientModel> PopulateAllIQuotationDB()
        {
            DatabaseConnectionString();
            int CountActive = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            List<ClientModel> ItemLIst = new List<ClientModel>();
            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_AllItemInformation", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ClientModel ItemModel = new ClientModel
                            {
                                ItemID = row["ITEMID"].ToString(),
                                ItemCategory = row["ITEMCATEGORYNAME"].ToString(),
                                ItemName = row["ITEMNAME"].ToString(),
                                itemDescription = row["ITEMDESCRIPTION"].ToString(),
                                ItemMeasure = row["Symbol"].ToString(),
                                ItemStatus = row["ITEMSTATUS"].ToString(),
                                ItemCode = row["ITEMCODE"].ToString(),
                            };
                            if (ItemModel.ItemStatus == "Active")
                            {
                                CountActive++;
                                CountActiveItem = CountActive;
                                ItemLIst.Add(ItemModel);
                            }




                        }
                        connection.Close();
                        return ItemLIst;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ClientModel> PopulateClientAllIQuotationDB(string id)
        {
            DatabaseConnectionString();
            int CountActive = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            List<ClientModel> ItemLIst = new List<ClientModel>();
            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_PopulateAllClientQuotation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", id);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ClientModel ItemModel = new ClientModel
                            {
                                Quotation_ControlNo = row["QUOTATION_CONTROLNO"].ToString(),
                                Quotation_ReferenceNo = row["QUOTATION_REFERENCENO"].ToString(),
                                Quotation_ClientName = row["QUOTATION_CLIENTNAME"].ToString(),
                                Quotation_Company = row["CLIENT"].ToString(),
                                Quotation_Location = row["CLIENT_LOCATION"].ToString(),
                                Quotation_EntryDate = row["ENTRYDATE"].ToString(),
                            };

                            if (ItemModel.ItemStatus == "Active")
                            {
                                CountActive++;
                            }

                            CountActiveItem = CountActive;

                            ItemLIst.Add(ItemModel);

                        }
                        connection.Close();
                        return ItemLIst;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ClientModel> PopulateEditClientAllIQuotationDB(string id)
        {
            DatabaseConnectionString();
            int CountActive = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            List<ClientModel> ItemLIst = new List<ClientModel>();
            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_PopulateEditAllClientQuotation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@userID", id);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ClientModel ItemModel = new ClientModel
                            {
                                Quotation_ControlNo = row["QUOTATION_CONTROLNO"].ToString(),
                                Quotation_ReferenceNo = row["QUOTATION_REFERENCENO"].ToString(),
                                Quotation_ClientName = row["QUOTATION_CLIENTNAME"].ToString(),
                                Quotation_ProjectName = row["QUOTATION_PROJECTNAME"].ToString(),
                                Quotation_Company = row["CLIENT"].ToString(),
                                Quotation_Location = row["CLIENT_LOCATION"].ToString(),
                                Quotation_EntryDate = row["ENTRYDATE"].ToString(),
                            };

                            if (ItemModel.ItemStatus == "Active")
                            {
                                CountActive++;
                            }

                            CountActiveItem = CountActive;

                            ItemLIst.Add(ItemModel);

                        }
                        connection.Close();
                        return ItemLIst;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ClientModel> PopulateInquiry_ClientAllIQuotationDB(string id)
        {
            DatabaseConnectionString();
            int CountActive = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            List<ClientModel> ItemLIst = new List<ClientModel>();
            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_InquiryClientQuotation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", id);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ClientModel ItemModel = new ClientModel
                            {
                                Quotation_ControlNo = row["QUOTATION_CONTROLNO"].ToString(),
                                Quotation_ReferenceNo = row["QUOTATION_REFERENCENO"].ToString(),
                                Quotation_ClientName = row["QUOTATION_CLIENTNAME"].ToString(),
                                Quotation_Company = row["CLIENT"].ToString(),
                                Quotation_Location = row["CLIENT_LOCATION"].ToString(),
                                Quotation_EntryDate = row["ENTRYDATE"].ToString(),
                                Quotation_ProjectName = row["QUOTATION_PROJECTNAME"].ToString(),
                            };
                            ItemLIst.Add(ItemModel);

                        }
                        connection.Close();
                        return ItemLIst;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ClientModel> GetItemRelasedDB(string POC, string Location)
        {
            DatabaseConnectionString();
            List<ClientModel> listSupplierModel = new List<ClientModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_PopulateItemReleasing", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", POC);
                        command.Parameters.AddWithValue("@Location", Location);


                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();

                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ClientModel suppliermodel = new ClientModel
                            {
                                ItemName = row["CLIENT_ITEMNAME"].ToString(),
                                Qty = row["CLIENT_QTY"].ToString(),
                                Collected = row["CLIENT_RECEIVED"].ToString(),
                                Delivered = row["CLIENT_RELEASED"].ToString(),
                                ItemID = row["ITEMID"].ToString(),
                                ItemMeasure = row["CLIENT_ITEMMEASURE"].ToString(),
                            };
                            listSupplierModel.Add(suppliermodel);
                        }
                        return listSupplierModel;
                    }
                }
            }
            catch
            {
                return listSupplierModel;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<ClientModel> GetPopulatePoNumberSupplier(string POC)
        {
            DatabaseConnectionString();
            List<ClientModel> listSupplierModel = new List<ClientModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_GetPopulatePoNumberSupplier", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ClientReferenceNo", POC);


                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();

                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ClientModel suppliermodel = new ClientModel
                            {
                                PONumber = row["SUPPLIER_PONUMBER"].ToString(),
                                Supplier = row["SUPPLIER_IDNAME"].ToString(),
                                ProjectName = row["CLIENT_PROJECTNAME"].ToString(),
                                GrandTotal = row["SUPPLIER_GRANDTOTAL"].ToString(),
                                POReferenceNo = row["SUPPLIER_REFERENCENO"].ToString(),
                                TermsofPayment = row["SUPPLIER_TERMOFPAYMENT"].ToString(),

                            };
                            listSupplierModel.Add(suppliermodel);
                        }
                        return listSupplierModel;
                    }
                }
            }
            catch
            {
                return listSupplierModel;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<ClientModel> PopulateCertificateOfCompletionDB(string UserID)
        {
            DatabaseConnectionString();
            List<ClientModel> COCList = new List<ClientModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_PopulateCertificateOfCompletion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", UserID);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            ClientModel clientmodel = new ClientModel
                            {
                                COC_Client = row["CLIENT"].ToString(),
                                COC_ControlNo = row["COC_CONTROLNO"].ToString(),
                                COC_ProjectTitle = row["COC_PROJECTTITLE"].ToString(),
                                COC_EntryDate = Convert.ToDateTime(row["COC_ENTRYDATE"].ToString()),
                                COC_Resposible = row["PREPAREDBY"].ToString(),

                            };
                            COCList.Add(clientmodel);
                        }
                        return COCList;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Something wrong happened in the  module :", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public List<ClientModel> PopulateCOCinformationDB(string UserID)
        {
            DatabaseConnectionString();
            List<ClientModel> ListCOC = new List<ClientModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_PopulateAllEditCertificateOfCompletion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@User", UserID);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ClientModel cocModel = new ClientModel
                            {
                                COC_ControlNo = row["COC_CONTROLNO"].ToString(),
                                COC_ProjectTitle = row["COC_PROJECTTITLE"].ToString(),
                                COC_Client = row["COC_CLIENT"].ToString(),
                                COC_POReference = row["COC_POREFERENCE"].ToString(),
                                COC_EntryDate = Convert.ToDateTime(row["ENTRYDATE"].ToString())
                            };
                            ListCOC.Add(cocModel);
                        }
                    }
                    return ListCOC;
                }
            }
            catch
            {
                return ListCOC;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<ClientModel> PopulateInquiryCertificateOfCompletionDB(string UserID)
        {
            DatabaseConnectionString();
            List<ClientModel> listCOC = new List<ClientModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_InquiryCertificateOfCompletion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", UserID);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ClientModel clientModel = new ClientModel
                            {
                                COC_ControlNo = row["COC_CONTROLNO"].ToString(),
                                COC_Client = row["CLIENT"].ToString(),
                                COC_ProjectTitle = row["COC_PROJECTTITLE"].ToString(),
                                COC_Status = row["COC_FORMSTATUS"].ToString(),
                                EntryDate = row["COC_ENTRYDATE"].ToString(),
                                ApprovedDate = row["COC_APPROVEDDATE"].ToString(),
                                NotificationView = row["COC_NOTIFICATION"].ToString(),
                                RejectedDate = row["COC_REJECTEDDATE"].ToString(),
                            };
                            listCOC.Add(clientModel);
                        }
                        return listCOC;

                    }
                }
            }
            catch
            {
                return listCOC;

            }
            finally
            {
                connection.Close();
            }
        }

        public List<ClientModel> PopulateInquiryClientDB(string UserID)
        {
            DatabaseConnectionString();
            List<ClientModel> InquiryClientList = new List<ClientModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_InquiryClient", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", UserID);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ClientModel clientModel = new ClientModel
                            {
                                ClientLocation = row["CLIENT_LOCATION"].ToString(),
                                POC_ReferenceNo = row["CLIENT_REFERENCENO"].ToString(),
                                ClientID = row["CLIENT"].ToString(),
                                PONumber = row["CLIENT_PONUMBER"].ToString(),
                                SignProposal = row["CLIENT_QUOTATIONNO"].ToString(),
                                ProjectName = row["CLIENT_PROJECTNAME"].ToString(),
                                ProjectStatus = row["CLIENT_STATUS"].ToString(),
                                EntryDate = row["ENTRYDATE"].ToString(),
                            };
                            InquiryClientList.Add(clientModel);
                        }
                        return InquiryClientList;
                    }
                }
            }
            catch
            {
                throw;
                return InquiryClientList;
            }
            finally
            {
                connection.Close();
            }

        }

        public List<SupplierModel> PopulateAllOnGoingClientDB(string Responsible)
        {
            DatabaseConnectionString();

            SqlConnection connection = null;
            SqlCommand command = null;
            List<SupplierModel> ClientList = new List<SupplierModel>();

            using (connection = new SqlConnection(PotoSupplier))
            {
                connection.Open();
                using (command = new SqlCommand("Select_PopulateAllGoingUserClient", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@USER", Responsible);
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        SupplierModel supplierModel = new SupplierModel
                        {
                            POCReferenceNo = row["CLIENT_REFERENCENO"].ToString(),
                            PONumber = row["CLIENT_PONUMBER"].ToString(),
                            ClientStatus = row["CLIENT_STATUS"].ToString(),
                            EntryDate = row["ENTRYDATE"].ToString(),
                            Client = row["CLIENTNAME"].ToString(),
                            SignProposal = row["CLIENT_QUOTATIONNO"].ToString()
                        };
                        ClientList.Add(supplierModel);
                    }
                }
                connection.Close();
                return ClientList;
            }
        }

        public List<SupplierModel> PopulateSupplierItemListDB(string id)
        {
            DatabaseConnectionString();
            List<SupplierModel> ListSupplierModel = new List<SupplierModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_EditUserSupplierInformation_GRID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", id);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    SupplierModel supplierModel = new SupplierModel
                                    {
                                        ParticularName = row["SUPPLIER_PARTICULAR"].ToString(),
                                        itemDescription = row["ITEMDESCRIPTION"].ToString(),
                                        Qty = row["SUPPLIER_QTY"].ToString(),
                                        ItemMeasure = row["SUPPLIER_UM"].ToString(),
                                        DisplayAmount = Convert.ToDecimal(row["SUPPLIER_AMOUNT"].ToString()),
                                        DisplayTotalAmount = Convert.ToDecimal(row["SUPPLIER_TOTALAMOUNT"].ToString()),
                                        DisplayAmountVAT = Convert.ToDecimal(row["SUPPLIER_TOTALAMOUNTVAT"].ToString()),
                                        DisplayAmountWithoutVAT = Convert.ToDecimal(row["SUPPLIER_TOTALAMOUNTWITHOUTVAT"].ToString())
                                    };
                                    ListSupplierModel.Add(supplierModel);

                                }
                                connection.Close();
                                return ListSupplierModel;
                            }
                        }
                    }

                }
            }
            catch
            {
                throw;
                return ListSupplierModel;

            }
        }

        public List<SupplierModel> Select_RoutingApprovalDetails_GRIDDB(string id)
        {
            DatabaseConnectionString();
            List<SupplierModel> ListSupplierModel = new List<SupplierModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_RoutingApprovalDetails_GRID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", id);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    SupplierModel supplierModel = new SupplierModel
                                    {
                                        ParticularName = row["SUPPLIER_PARTICULAR"].ToString(),
                                        itemDescription = row["SUPPLIER_DESCRIPTION"].ToString(),
                                        Qty = row["SUPPLIER_QTY"].ToString(),
                                        ItemMeasure = row["SUPPLIER_UM"].ToString(),
                                        DisplayAmount = Convert.ToDecimal(row["SUPPLIER_AMOUNT"].ToString()),
                                        DisplayTotalAmount = Convert.ToDecimal(row["SUPPLIER_TOTALAMOUNT"].ToString()),
                                        DisplayAmountVAT = Convert.ToDecimal(row["SUPPLIER_TOTALAMOUNTVAT"].ToString()),
                                        DisplayAmountWithoutVAT = Convert.ToDecimal(row["SUPPLIER_TOTALAMOUNTWITHOUTVAT"].ToString())
                                    };
                                    ListSupplierModel.Add(supplierModel);

                                }
                                connection.Close();
                                return ListSupplierModel;
                            }
                        }
                    }

                }
            }
            catch
            {
                return ListSupplierModel;

            }
        }

        public List<SupplierModel> PopulateAllEditSupplierDB(string Responsible)
        {
            DatabaseConnectionString();
            List<SupplierModel> ListSupplierModel = new List<SupplierModel>();
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_PopulateAllEditSupplier", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@USER", Responsible);
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    SupplierModel supplierModel = new SupplierModel
                                    {
                                        POSReferenceNo = row["SUPPLIER_REFERENCENO"].ToString(),
                                        POSupplierNumber = row["SUPPLIER_PONUMBER"].ToString(),
                                        Supplier = row["SUPPLIER"].ToString(),
                                        SupplierStatus = row["SUPPLIER_STATUS"].ToString(),
                                        EntryDate = row["ENTRYDATE"].ToString(),
                                        FormStatus = row["FORMSTATUS"].ToString(),
                                    };
                                    ListSupplierModel.Add(supplierModel);
                                }
                            }
                            connection.Close();
                            return ListSupplierModel;
                        }
                    }
                }

            }
            catch
            {
                return ListSupplierModel;
            }
        }

        public List<SupplierModel> PopulateForRoutingApprovalDB(int UserID, string Status)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            List<SupplierModel> RoutingApprovalList = new List<SupplierModel>();

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_RoutingApproval", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@User", UserID);
                        command.Parameters.AddWithValue("@Status", Status);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    SupplierModel supplierModel = new SupplierModel
                                    {
                                        POSReferenceNo = row["SUPPLIER_REFERENCENO"].ToString(),
                                        POSupplierNumber = row["SUPPLIER_PONUMBER"].ToString(),
                                        Supplier = row["SUPPLIER"].ToString(),
                                        FullName = row["RESPONSIBLE"].ToString(),
                                        ApproverID = row["APPROVER"].ToString(),
                                        EndorserID = row["ENDORSER"].ToString(),
                                        SupplierStatus = row["SUPPLIER_STATUS"].ToString(),
                                        EntryDate = row["ENTRYDATE"].ToString(),
                                        ProjectName = row["SUPPLIER_PROJECTNAME"].ToString(),
                                    };
                                    RoutingApprovalList.Add(supplierModel);
                                }
                            }
                        }
                    }
                }

                return RoutingApprovalList;

            }
            catch
            {
                return RoutingApprovalList;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<SupplierModel> PopulateForPRRoutingApprovalDB(int UserID, string Status)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            List<SupplierModel> RoutingApprovalList = new List<SupplierModel>();

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_PR_RoutingApproval", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@User", UserID);
                        command.Parameters.AddWithValue("@Status", Status);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    SupplierModel supplierModel = new SupplierModel
                                    {
                                        PRNumber = row["PR_NUMBER"].ToString(),
                                        PRReferenceNo = row["PR_REFERENCENO"].ToString(),
                                        ProjectDescription = row["PR_PROJECTDESC"].ToString(),
                                        FormStatus = row["PR_FORMSTATUS"].ToString(),
                                        FullNameResponsible = row["RESPONSIBLE"].ToString(),
                                        CanvessedBy = row["CANVASSEDBY"].ToString(),
                                        EntryDate = row["PR_ENTRYDATE"].ToString(),
                                    };
                                    RoutingApprovalList.Add(supplierModel);
                                }
                            }
                        }
                    }
                }

                return RoutingApprovalList;

            }
            catch
            {
                return RoutingApprovalList;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<SupplierModel> PopulateSupplierPayablesDB()
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            List<SupplierModel> listPayables = new List<SupplierModel>();

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_PopulateSupplierInformation", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            SupplierModel suppliermodel = new SupplierModel
                            {
                                POSupplierNumber = row["SUPPLIER_PONUMBER"].ToString(),
                                Supplier = row["SUPPLIER"].ToString(),
                                DisplayGrandTotal = Convert.ToDecimal(row["SUPPLIER_GRANDTOTAL"].ToString()),
                                SupplierStatus = row["SUPPLIER_STATUS"].ToString(),
                                EntryDate = row["ENTRYDATE"].ToString(),
                                POSReferenceNo = row["SUPPLIER_REFERENCENO"].ToString(),
                                ACC_Status = row["ACCOUNTING_STATUS"].ToString(),
                                ACC_BALANCE = Convert.ToDecimal(row["BALANCE"].ToString()),
                                TermsofPayment = row["PAYMENTTERMS"].ToString(),
                                ProjectName = row["SUPPLIER_PROJECTNAME"].ToString(),
                                PRNumber = row["SUPPLIER_PRNUMBER"].ToString(),
                                PRReferenceNo = row["PR_REFERENCENO"].ToString(),




                            };
                            listPayables.Add(suppliermodel);
                        }

                        return listPayables;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<SupplierModel> PopulateInquirySupplierDB(string UserID)
        {
            DatabaseConnectionString();
            List<SupplierModel> InquirySupplierList = new List<SupplierModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_InquirySupplier", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", UserID);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            SupplierModel supplierModel = new SupplierModel
                            {
                                POSReferenceNo = row["SUPPLIER_REFERENCENO"].ToString(),
                                POSupplierNumber = row["SUPPLIER_PONUMBER"].ToString(),
                                ProjectName = row["SUPPLIER_PROJECTNAME"].ToString(),
                                Supplier = row["SUPPLIER"].ToString(),
                                EntryDate = row["ENTRYDATE"].ToString(),
                                FormStatus = row["FORMSTATUS"].ToString(),
                                ReviewedDate = row["REVIEWED_DATE"].ToString(),
                                EndorsedDate = row["ENDORSED_DATE"].ToString(),
                                ApprovedDate = row["APPROVED_DATE"].ToString(),
                                RejectedDate = row["REJECTED_DATE"].ToString(),
                                CancelledDate = row["CANCELLED_DATE"].ToString(),
                                NotificationView = row["SUPPLIER_NOTIFICATION"].ToString(),

                            };
                            InquirySupplierList.Add(supplierModel);
                        }
                        return InquirySupplierList;
                    }
                }
            }
            catch
            {
                return InquirySupplierList;
            }
            finally
            {
                connection.Close();
            }

        }

        public List<SupplierModel> PopulateInquiryPurchaseRequisitionDB(string UserID)
        {
            DatabaseConnectionString();
            List<SupplierModel> listPurchaseRequisition = new List<SupplierModel>();
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_InquiryPurchaseRequisition", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", UserID);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            SupplierModel model = new SupplierModel
                            {
                                PRNumber = row["PR_NUMBER"].ToString(),
                                PRReferenceNo = row["PR_REFERENCENO"].ToString(),
                                ProjectDescription = row["PR_PROJECTDESC"].ToString(),
                                CanvessedBy = row["CANVASSEDBY"].ToString(),
                                FullNameResponsible = row["RESPONSIBLE"].ToString(),
                                FormStatus = row["FORMSTATUS"].ToString(),
                                EntryDate = row["PR_ENTRYDATE"].ToString(),
                                ReviewedDate = row["PR_REVIEWEDDATE"].ToString(),
                                EndorsedDate = row["PR_ENDORSEDDATE"].ToString(),
                                ApprovedDate = row["PR_APPROVEDDATE"].ToString(),
                                RejectedDate = row["PR_REJECTEDDATE"].ToString(),
                                CancelledDate = row["PR_CANCELLEDDATE"].ToString(),
                                NotificationView = row["PR_NOTIFICATION"].ToString(),

                            };
                            listPurchaseRequisition.Add(model);
                        }


                        return listPurchaseRequisition;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<SupplierModel> PopulatePRTransactionDB()
        {
            DatabaseConnectionString();
            List<SupplierModel> listPurchaseRequisition = new List<SupplierModel>();
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_GetSPurchaseRequisitionTransaction", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            SupplierModel model = new SupplierModel
                            {
                                PRNumber = row["PR_NUMBER"].ToString(),
                                PONumber = row["SUPPLIER_PONUMBER"].ToString(),
                                PRReferenceNo = row["PR_REFERENCENO"].ToString(),
                                ProjectDescription = row["PR_PROJECTDESC"].ToString(),
                                CanvessedBy = row["CANVASSEDBY"].ToString(),
                                FullNameResponsible = row["RESPONSIBLE"].ToString(),
                                FormStatus = row["PR_FORMSTATUS"].ToString(),
                                EntryDate = row["PR_ENTRYDATE"].ToString(),
                                NotificationView = row["PR_NOTIFICATION"].ToString(),
                                PRStatus = row["PR_STATUS"].ToString(),

                            };
                            listPurchaseRequisition.Add(model);
                        }


                        return listPurchaseRequisition;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<SupplierModel> PopulateAllApprovedSupplierRequestDB()
        {
            DatabaseConnectionString();
            List<SupplierModel> ListDeliveryReceipt = new List<SupplierModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    using (command = new SqlCommand("Select_PopulateAllApprovedSupplierRequest", connection))
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);

                            foreach (DataRow row in dt.Rows)
                            {
                                SupplierModel supplierModel = new SupplierModel
                                {
                                    POSupplierNumber = row["SUPPLIER_PONUMBER"].ToString(),
                                    Supplier = row["SUPPLIER"].ToString(),
                                    Client = row["CLIENT"].ToString(),
                                    DisplayGrandTotal = Convert.ToDecimal(row["SUPPLIER_GRANDTOTAL"].ToString()),
                                    SupplierStatus = row["SUPPLIER_STATUS"].ToString(),
                                    EntryDate = row["ENTRYDATE"].ToString(),
                                    POCReferenceNo = row["CLIENT_REFERENCENO"].ToString(),
                                    POSReferenceNo = row["SUPPLIER_REFERENCENO"].ToString(),
                                };
                                ListDeliveryReceipt.Add(supplierModel);
                            }
                            return ListDeliveryReceipt;
                        }
                    }
                }
            }
            catch
            {
                return ListDeliveryReceipt;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<SupplierModel> PopulateAllDRApprovedSupplierRequestDB()
        {
            DatabaseConnectionString();
            List<SupplierModel> ListDeliveryReceipt = new List<SupplierModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    using (command = new SqlCommand("Select_PopulateAllDRApprovedSupplierRequest", connection))
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);

                            foreach (DataRow row in dt.Rows)
                            {
                                SupplierModel supplierModel = new SupplierModel
                                {
                                    POSupplierNumber = row["SUPPLIER_PONUMBER"].ToString(),
                                    Supplier = row["SUPPLIER"].ToString(),
                                    ProjectName = row["SUPPLIER_PROJECTNAME"].ToString(),
                                    SupplierStatus = row["SUPPLIER_STATUS"].ToString(),
                                    EntryDate = row["ENTRYDATE"].ToString(),
                                    POSReferenceNo = row["SUPPLIER_REFERENCENO"].ToString(),
                                    ItemStatus = row["ITEMSTATUS"].ToString(),
                                };
                                ListDeliveryReceipt.Add(supplierModel);
                            }
                            return ListDeliveryReceipt;
                        }
                    }
                }
            }
            catch
            {
                return ListDeliveryReceipt;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<SupplierModel> GetItemReceivedDB(string POS)
        {
            DatabaseConnectionString();
            List<SupplierModel> listSupplierModel = new List<SupplierModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_PopulateItemReceiving", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", POS);


                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();

                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            SupplierModel suppliermodel = new SupplierModel
                            {
                                ParticularName = row["SUPPLIER_PARTICULAR"].ToString(),
                                Qty = row["SUPPLIER_QTY"].ToString(),
                                Collected = row["SUPPLIER_RECEIVED"].ToString(),
                                ItemID = row["ITEMID"].ToString(),
                            };
                            listSupplierModel.Add(suppliermodel);
                        }
                        return listSupplierModel;
                    }
                }
            }
            catch
            {
                return listSupplierModel;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<SupplierModel> GetPopulatePoNumberClient(string POS)
        {
            DatabaseConnectionString();
            List<SupplierModel> listClientModel = new List<SupplierModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_GetPopulatePoNumberClient", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SupplierReferenceNo", POS);


                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();

                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            SupplierModel suppliermodel = new SupplierModel
                            {
                                PONumber = row["CLIENT_PONUMBER"].ToString(),
                                Client = row["CLIENT_IDNAME"].ToString(),
                                ProjectName = row["CLIENT_PROJECTNAME"].ToString(),
                                GrandTotal = row["CLIENT_GRANDTOTAL"].ToString(),
                                POCReferenceNo = row["CLIENT_REFERENCENO"].ToString(),
                                TermsofPayment = row["CLIENT_TERMSOFPAYMENT"].ToString(),

                            };
                            listClientModel.Add(suppliermodel);
                        }
                        return listClientModel;
                    }
                }
            }
            catch
            {
                return listClientModel;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<SupplierModel> PopulateAllEditPurchaseRequisitionDB(string id)
        {
            DatabaseConnectionString();
            List<SupplierModel> listPurchaseRequisition = new List<SupplierModel>();
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_PopulateAllEditPurchaseRequisition", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", id);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            SupplierModel model = new SupplierModel
                            {
                                PRNumber = row["PR_NUMBER"].ToString(),
                                PRReferenceNo = row["PR_REFERENCENO"].ToString(),
                                ProjectDescription = row["PR_PROJECTDESC"].ToString(),
                                FormStatus = row["PR_FORMSTATUS"].ToString(),
                                EntryDate = row["PR_ENTRYDATE"].ToString(),

                            };
                            listPurchaseRequisition.Add(model);
                        }


                        return listPurchaseRequisition;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<SupplierModel> GetPurchaseRequisition_GRIDDB(string id)
        {
            DatabaseConnectionString();
            List<SupplierModel> listModel = new List<SupplierModel>();
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_EditPurchaseRequisitionInformation_GRID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PR_REFERENCENO", id);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            SupplierModel model = new SupplierModel
                            {

                                ParticularName = row["PR_PARTICULAR"].ToString(),
                                ItemMeasure = row["PR_ITEMMEASURE"].ToString(),
                                itemDescription = row["PR_REPORTVIEW"].ToString(),
                                Qty = row["PR_QTY"].ToString(),
                                DisplayAmount_One = Convert.ToDecimal(row["PR_AMOUNTONE"].ToString()),
                                DisplayAmount_Two = Convert.ToDecimal(row["PR_AMOUNTTWO"].ToString()),
                                DisplayAmount_Three = Convert.ToDecimal(row["PR_AMOUNTTHREE"].ToString()),
                                DisplayTotalAmount_One = Convert.ToDecimal(row["PR_TOTALAMOUNTONE"].ToString()),
                                DisplayTotalAmount_Two = Convert.ToDecimal(row["PR_TOTALAMOUNTTWO"].ToString()),
                                DisplayTotalAmount_Three = Convert.ToDecimal(row["PR_TOTALAMOUNTHREE"].ToString()),
                            };
                            listModel.Add(model);
                        }
                        return listModel;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<SupplierModel> GetBillSupplierDB(string Type, string Particulars, string Reference)
        {
            DatabaseConnectionString();
            string BillAmount = "";
            List<SupplierModel> listmodel = new List<SupplierModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_SupplierPaymentTerms", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Particulars", Particulars);
                        command.Parameters.AddWithValue("@TypeofBilling", Type);
                        command.Parameters.AddWithValue("@DropDownlist", "FALSE");
                        command.Parameters.AddWithValue("@ReferenceNo", Reference);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                using (SqlDataReader sdr = command.ExecuteReader())
                                {
                                    while (sdr.Read())
                                    {
                                        SupplierModel model = new SupplierModel()
                                        {
                                            ACC_AmountPaid = sdr["TOTALBALANCE"].ToString(),
                                            ACC_LesswithTax = sdr["LESSWITHTAX"].ToString()
                                        };
                                        listmodel.Add(model);
                                    }
                                }
                            }
                        }
                    }
                }
                return listmodel;
            }
            catch
            {
                return listmodel;
            }
            finally
            {
                connection.Close();
            }

        }

        public List<AccountingModel> PopulateReceivablesInformationDB()
        {
            DatabaseConnectionString();
            List<AccountingModel> ListReceivables = new List<AccountingModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_PopulateReceivablesInformation", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            AccountingModel accountingmodel = new AccountingModel
                            {
                                ACC_ID = row["ACCID"].ToString(),
                                ACC_ReferenceNo = row["ACC_REFERENCENO"].ToString(),
                                ACC_PONumber = row["ACC_PONUMBER"].ToString(),
                                ACC_BillNo = row["ACC_BILLNO"].ToString(),
                                ACC_BillAmount = Convert.ToDecimal(row["ACC_BILLAMOUNT"].ToString()),
                                ACC_Particular = row["ACC_PARTICULAR"].ToString(),
                                ACC_BillDate = Convert.ToDateTime(row["ACC_DATEPAID"].ToString()),
                                ACC_Responsible = row["RESPONSIBLE"].ToString(),
                                ACC_GrandTotalAmount = Convert.ToDecimal(row["CLIENT_GRANDTOTAL"].ToString()),


                            };
                            ListReceivables.Add(accountingmodel);
                        }
                        return ListReceivables;

                    }
                }
            }
            catch
            {
                throw;

            }
            finally
            {
                connection.Close();
            }
        }

        public List<AccountingModel> PopulatePayablesInformationDB()
        {
            DatabaseConnectionString();
            List<AccountingModel> ListPayables = new List<AccountingModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_PopulatePayablesInformation", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            AccountingModel accountingmodel = new AccountingModel
                            {
                                ACC_ID = row["ACC_ID"].ToString(),
                                ACC_ReferenceNo = row["ACC_REFERENCENO"].ToString(),
                                ACC_PONumber = row["ACC_PONUMBER"].ToString(),
                                ACC_ATICVNO = row["ACC_ATICVNO"].ToString(),
                                ACC_AmountPaid = Convert.ToDecimal(row["ACC_AMOUNTPAID"].ToString()),
                                ACC_Particular = row["ACC_PARTICULAR"].ToString(),
                                ACC_CheckDate = Convert.ToDateTime(row["ACC_CHECKDATE"].ToString()),
                                ACC_DateCollected = Convert.ToDateTime(row["ACC_DATECOLLECTED"].ToString()),
                                ACC_InvoiceNo = row["ACC_INVOICENO"].ToString(),
                                ACC_Responsible = row["RESPONSIBLE"].ToString(),
                                ACC_GrandTotalAmount = Convert.ToDecimal(row["SUPPLIER_GRANDTOTAL"].ToString()),


                            };
                            ListPayables.Add(accountingmodel);
                        }
                        return ListPayables;

                    }
                }
            }
            catch
            {
                return ListPayables;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<AccountingModel> PopulateClientMonitoringDB()
        {
            DatabaseConnectionString();
            List<AccountingModel> ListClientMonitoring = new List<AccountingModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_PopulateClientMonitoring", connection))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    AccountingModel accountingmodel = new AccountingModel()
                                    {
                                        ACC_PONumber = row["ACC_PONUMBER"].ToString(),
                                        ACC_Client = row["CLIENT_IDNAME"].ToString(),
                                        ACC_ProjectName = row["CLIENT_PROJECTNAME"].ToString(),
                                        ACC_GrandTotalAmount = Convert.ToDecimal(row["CLIENT_GRANDTOTAL"].ToString()),
                                        ACC_GrandTotalAmountPaid = Convert.ToDecimal(row["TOTALPAID"].ToString()),
                                        ACC_LastPaymentDate = Convert.ToDateTime(row["LASTDATE"].ToString()),
                                        ACC_ReferenceNo = row["ACC_REFERENCENO"].ToString(),
                                        ACC_Status = row["ACCOUNTING_STATUS"].ToString(),
                                    };
                                    ListClientMonitoring.Add(accountingmodel);
                                }
                                return ListClientMonitoring;
                            }
                        }
                    }
                }
            }
            catch
            {
                return ListClientMonitoring;

            }
            finally
            {
                connection.Close();
            }
        }

        public List<AccountingModel> PopulateSupplierMonitoringDB()
        {
            DatabaseConnectionString();
            List<AccountingModel> ListClientMonitoring = new List<AccountingModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_PopulateSupplierMonitoring", connection))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    AccountingModel accountingmodel = new AccountingModel()
                                    {
                                        ACC_PONumber = row["ACC_PONUMBER"].ToString(),
                                        ACC_Client = row["CLIENT_IDNAME"].ToString(),
                                        ACC_Supplier = row["SUPPLIER_IDNAME"].ToString(),
                                        ACC_GrandTotalAmount = Convert.ToDecimal(row["SUPPLIER_GRANDTOTAL"].ToString()),
                                        ACC_GrandTotalAmountPaid = Convert.ToDecimal(row["TOTALPAID"].ToString()),
                                        ACC_LastPaymentDate = Convert.ToDateTime(row["LASTDATE"].ToString()),
                                        ACC_OutStandingBalance = Convert.ToDecimal(row["OUTSTANDINGBALANCE"].ToString()),
                                        ACC_ReferenceNo = row["ACC_REFERENCENO"].ToString(),
                                        ACC_Status = row["ACCOUNTING_STATUS"].ToString(),
                                        ACC_LesswithTax = Convert.ToDecimal(row["LESSWITHTAX"].ToString()),


                                    };
                                    ListClientMonitoring.Add(accountingmodel);
                                }
                                return ListClientMonitoring;
                            }
                        }
                    }
                }
            }
            catch
            {
                return ListClientMonitoring;

            }
            finally
            {
                connection.Close();
            }
        }

        public List<AccountingModel> GetClientMonitoringDB(string id)
        {
            DatabaseConnectionString();
            List<AccountingModel> ListClientMonitoring = new List<AccountingModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_GetClientMonitoring", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@REFERENCENO", id);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    AccountingModel accountingmodel = new AccountingModel()
                                    {
                                        ACC_ID = row["ACCID"].ToString(),
                                        ACC_ReferenceNo = row["ACC_REFERENCENO"].ToString(),
                                        ACC_PONumber = row["ACC_PONUMBER"].ToString(),
                                        ACC_BillNo = row["ACC_BILLNO"].ToString(),
                                        ACC_BillAmount = Convert.ToDecimal(row["ACC_BILLAMOUNT"].ToString()),
                                        ACC_Particular = row["ACC_PARTICULAR"].ToString(),
                                        ACC_BillDate = Convert.ToDateTime(row["ACC_DATEPAID"].ToString()),
                                        ACC_Responsible = row["RESPONSIBLE"].ToString(),
                                        ACC_GrandTotalAmount = Convert.ToDecimal(row["CLIENT_GRANDTOTAL"].ToString()),
                                        ACC_Remarks = row["ACC_REMARKS"].ToString(),
                                    };
                                    ListClientMonitoring.Add(accountingmodel);
                                }
                                return ListClientMonitoring;
                            }
                        }
                    }
                }
            }
            catch
            {
                return ListClientMonitoring;

            }
            finally
            {
                connection.Close();
            }
        }

        public List<AccountingModel> GetSupplierMonitoringDB(string id)
        {
            DatabaseConnectionString();
            List<AccountingModel> ListClientMonitoring = new List<AccountingModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_GetSupplierMonitoring", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@REFERENCENO", id);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    AccountingModel accountingmodel = new AccountingModel()
                                    {
                                        ACC_ID = row["ACC_ID"].ToString(),
                                        ACC_PONumber = row["ACC_PONUMBER"].ToString(),
                                        ACC_InvoiceNo = row["ACC_INVOICENO"].ToString(),
                                        ACC_AmountPaid = Convert.ToDecimal(row["ACC_AMOUNTPAID"].ToString()),
                                        ACC_CheckDate = Convert.ToDateTime(row["ACC_CHECKDATE"].ToString()),
                                        ACC_ATICVNO = row["ACC_INVOICENO"].ToString(),
                                        ACC_Responsible = row["RESPONSIBLE"].ToString(),
                                        ACC_Particular = row["ACC_PARTICULAR"].ToString(),
                                        ACC_LesswithTax = Convert.ToDecimal(row["ACC_LESSWITHTAX"].ToString()),
                                        ACC_Remarks = row["ACC_REMARKS"].ToString(),
                                    };
                                    ListClientMonitoring.Add(accountingmodel);
                                }
                                return ListClientMonitoring;
                            }
                        }
                    }
                }
            }
            catch
            {
                return ListClientMonitoring;

            }
            finally
            {
                connection.Close();
            }
        }

        public List<AccountingModel> PopulatefilterpayablesReportDB()
        {
            DatabaseConnectionString();
            List<AccountingModel> ListPayables = new List<AccountingModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    using (command = new SqlCommand("Select_PopulatePayablesReport", connection))
                    {

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            AccountingModel AccntModel = new AccountingModel
                            {
                                ACC_PONumber = row["ACC_PONUMBER"].ToString(),
                                ACC_Supplier = row["SUPPLIER_IDNAME"].ToString(),
                                ACC_ProjectName = row["CLIENT_PROJECTNAME"].ToString(),
                                ACC_Particular = row["PAYMENTTERMS"].ToString(),
                                ACC_Responsible = row["RESPONSIBLE"].ToString(),
                            };
                            ListPayables.Add(AccntModel);
                        }
                        return ListPayables;
                    }
                }
            }
            catch
            {
                return ListPayables;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<AccountingModel> PopulatefilterreceivablesReportDB()
        {
            DatabaseConnectionString();
            List<AccountingModel> ListReceivables = new List<AccountingModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    using (command = new SqlCommand("Select_PopulateReceivablesReport", connection))
                    {

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            AccountingModel AccntModel = new AccountingModel
                            {
                                ACC_ReferenceNo = row["ACC_REFERENCENO"].ToString(),
                                REC_Qoutation = row["CLIENT_QUOTATIONNO"].ToString(),
                                ACC_PONumber = row["ACC_PONUMBER"].ToString(),
                                ACC_Client = row["CLIENT_IDNAME"].ToString(),
                                ACC_Particular = row["ACC_PARTICULAR"].ToString(),
                                ACC_ProjectName = row["CLIENT_PROJECTNAME"].ToString(),

                            };
                            ListReceivables.Add(AccntModel);
                        }
                        return ListReceivables;
                    }
                }
            }
            catch
            {
                return ListReceivables;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<AccountingModel> PopulateAccountingLogs()
        {
            DatabaseConnectionString();
            List<AccountingModel> ListAccounting = new List<AccountingModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    using (command = new SqlCommand("Select_PopulateAccountingMovements", connection))
                    {

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            AccountingModel AccntModel = new AccountingModel
                            {
                                UserName = row["UserName"].ToString(),
                                Audit_Message = row["Audit_Message"].ToString(),
                                EntryDate = row["EntryDate"].ToString(),

                            };
                            ListAccounting.Add(AccntModel);
                        }
                        return ListAccounting;
                    }
                }
            }
            catch
            {
                return ListAccounting;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<UserMaintenance> PopulateSystemLogs()
        {
            DatabaseConnectionString();
            List<UserMaintenance> ListLogs = new List<UserMaintenance>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    using (command = new SqlCommand("Select_PopulateAudittrail_Function", connection))
                    {

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            UserMaintenance Model = new UserMaintenance
                            {
                                UserName = row["UserName"].ToString(),
                                Audit_Message = row["Audit_Message"].ToString(),
                                Audit_Module = row["Module"].ToString(),
                                EntryDate = row["EntryDate"].ToString(),

                            };
                            ListLogs.Add(Model);
                        }
                        return ListLogs;
                    }
                }
            }
            catch
            {
                return ListLogs;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<DRModel> PopulateReceivedLogsDB()
        {
            DatabaseConnectionString();
            List<DRModel> ListReceivedLogs = new List<DRModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(DrModule))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_ReceivedLogs", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            DRModel model = new DRModel
                            {
                                DRPONumber = row["DR_PONUMBER"].ToString(),
                                DRNumber = row["DR_NUMBER"].ToString(),
                                DRResponsible = row["DR_RESPONSIBLE"].ToString(),
                                DRDateReceived = Convert.ToDateTime(row["DATE_RECEIVED"].ToString()),
                                DRSupplier = row["DR_SUPPLIER"].ToString(),
                            };
                            ListReceivedLogs.Add(model);
                        }
                        return ListReceivedLogs;
                    }
                }
            }
            catch
            {
                return ListReceivedLogs;
            }
            finally
            {
                connection.Close();
            }

        }

        public List<DRModel> PopulateRelasedLogsDB()
        {
            DatabaseConnectionString();
            List<DRModel> ListReleasedLogs = new List<DRModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(DrModule))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_RelasedLogs", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            DRModel model = new DRModel
                            {
                                DRNumber = row["DR_NUMBER"].ToString(),
                                DRResponsible = row["DR_RESPONSIBLE"].ToString(),
                                DRDateReleased = Convert.ToDateTime(row["DATE_RELEASED"].ToString()),
                                DRClient = row["DR_CLIENT"].ToString(),
                                DRPONumber = row["DR_PONUMBER"].ToString(),
                                
                            };
                            ListReleasedLogs.Add(model);
                        }
                        return ListReleasedLogs;
                    }
                }
            }
            catch
            {
                return ListReleasedLogs;
            }
            finally
            {
                connection.Close();
            }

        }

        public List<DRModel> PopulateItemReturnedLogsDB()
        {
            DatabaseConnectionString();
            List<DRModel> ListItemReturnedLogs = new List<DRModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(DrModule))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_ItemReturnedLogs", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            DRModel model = new DRModel
                            {
                                DRReferenceNo = row["REFERENCENO"].ToString(),
                                DRNumber = row["DR_NUMBER"].ToString(),
                                Company = row["COMPANY"].ToString(),
                                DRParticular = row["PARTICULAR"].ToString(),
                                DRPONumber = row["PO_NUMBER"].ToString(),
                                DRItemReturn = row["ITEMRETURN"].ToString(),
                                DRReasonRemarks = row["REASONREMARKS"].ToString(),
                                DRDate = row["RETURNEDDATE"].ToString(),
                                ModuleType = row["MODULETYPES"].ToString(),
                                DRResponsible = row["RESPONSIBLE"].ToString(),

                            };
                            ListItemReturnedLogs.Add(model);
                        }
                        return ListItemReturnedLogs;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

        }

        public List<DRModel> PopulateDRRelasedLogsDB(string DRNumber)
        {
            DatabaseConnectionString();
            List<DRModel> ListReleasedLogs = new List<DRModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(DrModule))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_DRRelasedLogs", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@DRnumber", DRNumber);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            DRModel model = new DRModel
                            {
                                DRReferenceNo = row["DR_REFERENCENO"].ToString(),
                                DRNumber = row["DR_NUMBER"].ToString(),
                                DRItemID = row["DR_ITEMID"].ToString(),
                                DRPONumber = row["DR_PONUMBER"].ToString(),
                                DRResponsible = row["DR_RESPONSIBLE"].ToString(),
                                DRParticular = row["DR_PARTICULAR"].ToString(),
                                DRDateReleased = Convert.ToDateTime(row["DATE_RELEASED"].ToString()),
                                DRReleased = row["DR_DELIVERED"].ToString(),
                                DRClient = row["DR_CLIENT"].ToString(),
                                DRQty = row["DR_QTY"].ToString(),
                                DRRemarks = row["DR_REMARKS"].ToString(),
                            };
                            ListReleasedLogs.Add(model);
                        }
                        return ListReleasedLogs;
                    }
                }
            }
            catch
            {
                return ListReleasedLogs;
            }
            finally
            {
                connection.Close();
            }

        }

        public List<DRModel> PopulateDRReceivedLogsDB(string DRPONumber, string DRNumber)
        {
            DatabaseConnectionString();
            List<DRModel> ListReleasedLogs = new List<DRModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(DrModule))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_DRReceivedLogs", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@DRnumber", DRNumber);
                        command.Parameters.AddWithValue("@DRPONumber", DRPONumber);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            DRModel model = new DRModel
                            {
                                DRID = Convert.ToInt32(row["DR_RECEIVEID"].ToString()),
                                DRItemID = row["ITEMID"].ToString(),
                                DRNumber = row["DR_NUMBER"].ToString(),
                                DRPONumber = row["DR_PONUMBER"].ToString(),
                                DRResponsible = row["DR_RESPONSIBLE"].ToString(),
                                DRParticular = row["DR_PARTICULAR"].ToString(),
                                DRDateReceived = Convert.ToDateTime(row["DR_DATERECEIVED"].ToString()),
                                DRReceived = row["DR_COLLECTED"].ToString(),
                                DRQty = row["DR_QTY"].ToString(),
                                DRReferenceNo = row["DR_POSREFERENCENO"].ToString(),
                                DRRemarks = row["DR_REMARKS"].ToString(),
                                DRSupplier = row["DR_SUPPLIER"].ToString(),


                            };
                            ListReleasedLogs.Add(model);
                        }
                        return ListReleasedLogs;
                    }
                }
            }
            catch
            {
                return ListReleasedLogs;
            }
            finally
            {
                connection.Close();
            }

        }



        //INSERTING DATA
        public int InsertUserInformationDB(UserMaintenance UserModel)
        {
            DatabaseConnectionString();
            int result = 0;
            SqlConnection Connection = null;

            using (Connection = new SqlConnection(Security))
            {
                using (SqlCommand Command = new SqlCommand("Insert_UserInformation", Connection))
                {
                    Connection.Open();
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Parameters.AddWithValue("@FIRSTNAME", UserModel.FirstName);
                    Command.Parameters.AddWithValue("@LASTNAME", UserModel.LastName);
                    Command.Parameters.AddWithValue("@USERNAME", UserModel.UserName);
                    Command.Parameters.AddWithValue("@EMAIL", UserModel.EmailAddress);
                    Command.Parameters.AddWithValue("@POSITION", UserModel.Position);
                    Command.Parameters.AddWithValue("@LOCATION", UserModel.Location);
                    Command.Parameters.AddWithValue("@USERSTATUS", "Active");
                    Command.Parameters.AddWithValue("@EMPID", UserModel.EmployeeID);
                    Command.Parameters.AddWithValue("@USERGROUPID", Convert.ToInt32(UserModel.UserGroupID));
                    Command.Parameters.AddWithValue("@USERROLEID", Convert.ToInt32(UserModel.UserRoleID));
                    Command.Parameters.AddWithValue("@APPROVERID", Convert.ToInt32(UserModel.ApproverID));
                    Command.Parameters.AddWithValue("@ENTRYDATE", DateTime.Now.ToShortDateString());
                    Command.Parameters.AddWithValue("@SIGNATURE", UserModel.Img);




                    Command.ExecuteNonQuery();
                    result++;
                    Connection.Close();
                    return result;
                }
            }

        }

        public int InsertClientInformationDB(ClientMaster ClientModel)
        {
            DatabaseConnectionString();
            AutoGenerateNumber_CLIENTPREVIEW();
            SqlConnection connection = null;
            SqlCommand command = null;
            int result = 0;
            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    using (command = new SqlCommand("Insert_ClientInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@CLIENTID", Client_Number);
                        command.Parameters.AddWithValue("@CLIENT", ClientModel.Client);
                        command.Parameters.AddWithValue("@EMAILADDRESS", ClientModel.EmailAddress ?? "");
                        command.Parameters.AddWithValue("@CONTACTNO", ClientModel.ContactNo ?? "");
                        command.Parameters.AddWithValue("@CONTACTPERSON", ClientModel.ContactPerson);
                        command.Parameters.AddWithValue("@CLIENTSTATUS", ClientModel.ClientStatus ?? "Active");
                        command.Parameters.AddWithValue("@ENTRYDATE", DateTime.Now.ToShortDateString());
                        result++;
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                    return result;
                }
            }
            catch
            {
                return result = 0;
            }
        }

        public int InsertClientAddressinformationDB(List<ClientMaster> Details, ClientMaster ClientModel)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            int Result = 0;
            if (Details == null)
            {
                Details = new List<ClientMaster>();
            }

            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    foreach (ClientMaster item in Details)
                    {
                        using (command = new SqlCommand("Insert_ClientAddress", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@CLIENT", Client_Number ?? Convert.ToString(ClientModel.ClientID));
                            command.Parameters.AddWithValue("@CLIENT_LOCATION", item.Location);
                            command.Parameters.AddWithValue("@CLIENT_ADDRESS", item.ClientAddress);

                            Result++;
                            command.ExecuteNonQuery();
                        }
                    }
                    connection.Close();
                    return Result;
                }
            }
            catch
            {

                return Result = 0;
            }
        }

        public int InsertItemInformationDB(List<ItemMaster> value)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            int result = 0;
            if (value == null)
            {
                value = new List<ItemMaster>();
            }


            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    //Loop and insert retcords.
                    foreach (ItemMaster item in value)
                    {
                        using (command = new SqlCommand("Insert_ItemInformation", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@ITEMCODE", item.ItemCode);
                            command.Parameters.AddWithValue("@ITEMNAME", item.ItemName);
                            command.Parameters.AddWithValue("@ITEMCATEGORY", Convert.ToInt32(item.ItemCategory));
                            command.Parameters.AddWithValue("@ITEMDESCRIPTION", item.ItemDescription);
                            command.Parameters.AddWithValue("@ITEMMEASURE", item.ItemMeasure);
                            command.ExecuteNonQuery();
                            result++;
                        }
                    }

                    connection.Close();
                    return result;
                }

            }
            catch
            {
                throw;
                return result = 0;
            }
        }

        public int InsertSupplierInformationDB(SupplierMaster SupplierModel)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            int result = 0;
            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    using (command = new SqlCommand("Insert_SupplierInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@SUPPLIER", SupplierModel.Supplier.Trim());
                        command.Parameters.AddWithValue("@SUPPLIERADDRESS", SupplierModel.SupplierAddress.Trim());
                        command.Parameters.AddWithValue("@EMAILADDRESS", SupplierModel.EmailAddress ?? "");
                        command.Parameters.AddWithValue("@CONTACTNO", SupplierModel.ContactNo ?? "");
                        command.Parameters.AddWithValue("@CONTACTPERSON", SupplierModel.ContactPerson);
                        command.Parameters.AddWithValue("@LOCATION", SupplierModel.Location);
                        command.Parameters.AddWithValue("@SUPPLIERSTATUS", "Active");
                        command.Parameters.AddWithValue("@ENTRYDATE", DateTime.Now);
                        result++;
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return result;
            }
            catch
            {
                return result = 0;

            }
        }

        public int InsertUserClientInformationGridDB(List<ClientModel> ClientDetails)
        {
            DatabaseConnectionString();

            AutoGenerateReferenceNo_CLIENT();
            SqlConnection connection = null;
            SqlCommand command = null;
            int Result = 0;

            if (ClientDetails == null)
            {
                ClientDetails = new List<ClientModel>();
            }

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    foreach (ClientModel item in ClientDetails)
                    {
                        using (command = new SqlCommand("Insert_UserClientInformation_GRID", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@CLIENTID", POC_Number);
                            command.Parameters.AddWithValue("@CLIENT_REFERENCENO", "POC" + POC_ReferenceNo);
                            command.Parameters.AddWithValue("@CLIENT_ITEMNAME", item.ItemID);
                            command.Parameters.AddWithValue("@CLIENT_ITEMMEASURE", item.ItemMeasure);
                            command.Parameters.AddWithValue("@CLIENT_ITEMDESCRIPTION", item.itemDescription);
                            command.Parameters.AddWithValue("@CLIENT_QTY", item.Qty);
                            command.Parameters.AddWithValue("@CLIENT_AMOUNT", Convert.ToDecimal(item.Amount));
                            command.Parameters.AddWithValue("@CLIENT_MARKUP", Convert.ToDecimal(item.Markup));
                            command.Parameters.AddWithValue("@CLIENT_TOTALAMOUNT", Convert.ToDecimal(item.TotalAmount));
                            command.Parameters.AddWithValue("@CLIENT_RELEASED", item.Released ?? "0");
                            command.Parameters.AddWithValue("@CLIENT_RECEIVED", 0);
                            command.Parameters.AddWithValue("@ENTRYDATE", DateTime.Now.ToShortDateString());
                            command.Parameters.AddWithValue("@CLIENT_ITEMSTATUS", "ON-GOING");
                            Result++;
                            command.ExecuteNonQuery();
                        }

                    }
                    connection.Close();
                    return Result;
                }

            }
            catch
            {
                Result = 0;
                return Result;

            }
        }

        public int InsertUserClientInformationDB(ClientModel clientModel, string Responsible, List<string> PONumber, string oldReferenceNo)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            int count = 0;
            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Insert_UserClientInformation", connection))
                    {

                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@CLIENTID", POC_Number);
                        command.Parameters.AddWithValue("@CLIENT_REFERENCENO", "POC" + POC_ReferenceNo);
                        command.Parameters.AddWithValue("@CLIENT_IDNAME", clientModel.ClientID);
                        command.Parameters.AddWithValue("@CLIENT_PONUMBER", clientModel.PONumber ?? "");
                        command.Parameters.AddWithValue("@CLIENT_PODATE", clientModel.PODate ?? "");
                        command.Parameters.AddWithValue("@CLIENT_PROJECTNAME", clientModel.ProjectName);
                        command.Parameters.AddWithValue("@CLIENT_TERMSOFPAYMENT", clientModel.TermsofPayment);
                        command.Parameters.AddWithValue("@CLIENT_LOCATION", clientModel.ClientLocation ?? "");
                        command.Parameters.AddWithValue("@CLIENT_SALES", clientModel.SalesPersonnel ?? "");
                        command.Parameters.AddWithValue("@CLIENT_GRANDTOTAL", Convert.ToDecimal(clientModel.GrandTotal));
                        command.Parameters.AddWithValue("@CLIENT_DISCOUNT", Convert.ToDecimal(clientModel.Discount));
                        command.Parameters.AddWithValue("@QUOTATIONNO", clientModel.SignProposal ?? "");
                        command.Parameters.AddWithValue("@CLIENT_ADDRESS", clientModel.ClientAddress ?? "");
                        command.Parameters.AddWithValue("@ENTRYDATE", DateTime.Now);
                        command.Parameters.AddWithValue("@ENTRYYEAR", POC_Year);
                        command.Parameters.AddWithValue("@CLIENT_STATUS", "ON-GOING");
                        command.Parameters.AddWithValue("@RESPONSIBLE", Responsible);

                        command.ExecuteNonQuery();
                    }

                    connection.Close();

                    count++;

                    Delete_POTaggingDB("", oldReferenceNo ?? "POC" + POC_ReferenceNo);

                    if (PONumber != null)
                    {
                        for (int i = 0; i < PONumber.Count; i++)
                        {
                            InsertPOTaggingDB("POC" + POC_ReferenceNo, PONumber[i]);
                        }
                    }


                    return count;
                }
            }
            catch
            {
                count = 0;
                DeleteClientInformationGRIDDB();
                return count;
            }

        }

        public int InsertUserSupplierInformationDB(List<string> ListClientReference, SupplierModel supplierModel)
        {
            DatabaseConnectionString();
            AutoGeneratePONumber_SUPPLIER();

            int result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Insert_UserSupplierInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SUPPLIERID", POS_Number);
                        command.Parameters.AddWithValue("@SUPPLIER_REFERENCENO", "POS" + POS_ReferenceNo);
                        command.Parameters.AddWithValue("@SUPPLIER_PONUMBER", supplierModel.POSupplierNumber ?? PONumber);
                        command.Parameters.AddWithValue("@SUPPLIER_PODATE", supplierModel.POSupplierDate);
                        command.Parameters.AddWithValue("@SUPPLIER_IDNAME", supplierModel.SupplierID);
                        command.Parameters.AddWithValue("@SUPPLIER_ADDRESS", supplierModel.SupplierAddresss);
                        command.Parameters.AddWithValue("@SUPPLIER_DELIVERYADDRESS", supplierModel.DeliveryAddress);
                        command.Parameters.AddWithValue("@SUPPLIER_PROJECTNAME", supplierModel.ProjectName);
                        command.Parameters.AddWithValue("@SUPPLIER_TERMOFPAYMENT", supplierModel.TermsofPayment);
                        command.Parameters.AddWithValue("@SUPPLIER_APPROVERID", supplierModel.ApproverID);
                        command.Parameters.AddWithValue("@SUPPLIER_ENDORSERID", supplierModel.EndorserID ?? "0");
                        command.Parameters.AddWithValue("@SUPPLIER_SHIPPINGINS", supplierModel.Shippinginstruction);
                        command.Parameters.AddWithValue("@SUPPLIER_GRANDTOTAL", Convert.ToDecimal(supplierModel.GrandTotal));
                        command.Parameters.AddWithValue("@SUPPLIER_GRANDTOTALVAT", Convert.ToDecimal(supplierModel.GrandTotalVat));
                        command.Parameters.AddWithValue("@SUPPLIER_GRANDTOTALWITHOUTVAT", Convert.ToDecimal(supplierModel.GrandTotalWithoutVat));
                        command.Parameters.AddWithValue("@SUPPLIER_DISCOUNT", Convert.ToDecimal(supplierModel.Discount ?? "0"));
                        command.Parameters.AddWithValue("@SUPPLIER_STATUS", "ON-GOING");
                        command.Parameters.AddWithValue("@ENTRYDATE", DateTime.Now);
                        command.Parameters.AddWithValue("@ENTRYYEAR", POS_Year);
                        command.Parameters.AddWithValue("@RESPONSIBLE", supplierModel.FullName);
                        command.Parameters.AddWithValue("@TYPEOFVAT", supplierModel.TypeOfVat);
                        command.Parameters.AddWithValue("@PRNumber", supplierModel.PRNumber ?? "");
                        command.Parameters.AddWithValue("@SUPPLIER_NOTIFICATION", "VIEW");

                        command.ExecuteNonQuery();

                    }

                    connection.Close();

                    Delete_POTaggingDB(supplierModel.POSupplierNumber ?? PONumber, "");
                    if (ListClientReference != null)
                    {
                        for (int i = 0; i < ListClientReference.Count; i++)
                        {

                            InsertPOTaggingDB(ListClientReference[i], supplierModel.POSupplierNumber ?? PONumber);
                        }
                    }

                    result++;
                    return result;
                }
            }
            catch
            {
                result = 0;
                DeleteSupplierInformationGRIDDB();
                throw;
            }
            finally
            {

            }
        }

        public int InsertUserSupplierInformationGridDB(List<SupplierModel> SupplierDetails, SupplierModel supplierModel)
        {
            DatabaseConnectionString();
            AutoGenerateReferenceNo_SUPPLIER();
            SqlConnection connection = null;
            SqlCommand command = null;
            int Result = 0;
            if (SupplierDetails == null)
            {
                SupplierDetails = new List<SupplierModel>();
            }
            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    foreach (SupplierModel item in SupplierDetails)
                    {
                        using (command = new SqlCommand("Insert_UserSupplierInformation_GRID", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@SUPPLIERID", POS_Number);
                            command.Parameters.AddWithValue("@SUPPLIER_REFERENCE", "POS" + POS_ReferenceNo);
                            command.Parameters.AddWithValue("@SUPPLIER_PARTICULAR", item.ParticularName);
                            command.Parameters.AddWithValue("@SUPPLIER_ITEMDESC", item.itemDescription);
                            command.Parameters.AddWithValue("@SUPPLIER_QTY", item.Qty);
                            command.Parameters.AddWithValue("@SUPPLIER_AMOUNT", Convert.ToDecimal(item.Amount));
                            command.Parameters.AddWithValue("@SUPPLIER_UM", item.ItemMeasure);
                            command.Parameters.AddWithValue("@SUPPLIER_TOTALAMOUNT", Convert.ToDecimal(item.TotalAmount));
                            command.Parameters.AddWithValue("@SUPPLIER_TOTALAMOUNTWITHOUTVAT", Convert.ToDecimal(item.AmountWithoutVAT));
                            command.Parameters.AddWithValue("@SUPPLIER_TOTALAMOUNTVAT", Convert.ToDecimal(item.AmountVAT));
                            command.Parameters.AddWithValue("@SUPPLIER_RECEIVED", 0);
                            command.Parameters.AddWithValue("@SUPPLIER_RELEASED", 0);
                            command.Parameters.AddWithValue("@ENTRYDATE", DateTime.Now.ToShortDateString());
                            command.Parameters.AddWithValue("@SUPPLIER_ITEMSTATUS", "ON-GOING");
                            Result++;
                            command.ExecuteNonQuery();
                        }
                    }
                    connection.Close();
                    return Result;
                }
            }
            catch
            {
                throw;
                return Result = 0;

            }
        }

        public int InsertPreviewUserSupplierInformationDB(SupplierModel supplierModel)
        {
            DatabaseConnectionString();
            AutoGenerateNumber_PREVIEW();
            AutoGeneratePONumber_SUPPLIER();

            int result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Insert_PreviewInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PREVIEW_NO", Preview_No);
                        command.Parameters.AddWithValue("@SUPPLIER_PONUMBER", PONumber ?? "");
                        command.Parameters.AddWithValue("@SUPPLIER_PODATE", supplierModel.POSupplierDate);
                        command.Parameters.AddWithValue("@SUPPLIER_IDNAME", supplierModel.SupplierID);
                        command.Parameters.AddWithValue("@SUPPLIER_ADDRESS", supplierModel.SupplierAddresss);
                        command.Parameters.AddWithValue("@SUPPLIER_DELIVERYADDRESS", supplierModel.DeliveryAddress);
                        command.Parameters.AddWithValue("@SUPPLIER_PROJECTNAME", supplierModel.ProjectName);
                        command.Parameters.AddWithValue("@SUPPLIER_TERMOFPAYMENT", supplierModel.TermsofPayment);
                        command.Parameters.AddWithValue("@SUPPLIER_APPROVERID", supplierModel.ApproverID);
                        command.Parameters.AddWithValue("@SUPPLIER_ENDORSERID", supplierModel.EndorserID ?? "0");
                        command.Parameters.AddWithValue("@SUPPLIER_SHIPPINGINS", supplierModel.Shippinginstruction);
                        command.Parameters.AddWithValue("@SUPPLIER_GRANDTOTAL", Convert.ToDecimal(supplierModel.GrandTotal));
                        command.Parameters.AddWithValue("@SUPPLIER_GRANDTOTALVAT", Convert.ToDecimal(supplierModel.GrandTotalVat));
                        command.Parameters.AddWithValue("@SUPPLIER_GRANDTOTALWITHOUTVAT", Convert.ToDecimal(supplierModel.GrandTotalWithoutVat));
                        command.Parameters.AddWithValue("@SUPPLIER_DISCOUNT", Convert.ToDecimal(supplierModel.Discount ?? "0"));
                        command.Parameters.AddWithValue("@SUPPLIER_STATUS", "ON-GOING");
                        command.Parameters.AddWithValue("@ENTRYDATE", DateTime.Now);
                        command.Parameters.AddWithValue("@RESPONSIBLE", supplierModel.FullName);
                        result++;
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return result;
            }
            catch
            {
                result = 0;
                DeletePreviewSupplierInformationGRIDDB();
                throw;
            }
        }

        public int InsertPreviewUserSupplierInformationGridDB(List<SupplierModel> SupplierDetails, SupplierModel supplierModel)
        {
            DatabaseConnectionString();
            AutoGenerateNumber_PREVIEW();
            SqlConnection connection = null;
            SqlCommand command = null;
            int Result = 0;
            if (SupplierDetails == null)
            {
                SupplierDetails = new List<SupplierModel>();
            }
            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    foreach (SupplierModel item in SupplierDetails)
                    {
                        using (command = new SqlCommand("Insert_PreviewInformation_GRID", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@PREVIEW_NO", Preview_No);
                            command.Parameters.AddWithValue("@SUPPLIER_PARTICULAR", item.ParticularName);
                            command.Parameters.AddWithValue("@SUPPLIER_ITEMDESC", item.itemDescription);
                            command.Parameters.AddWithValue("@SUPPLIER_QTY", item.Qty);
                            command.Parameters.AddWithValue("@SUPPLIER_AMOUNT", Convert.ToDecimal(item.Amount));
                            command.Parameters.AddWithValue("@SUPPLIER_UM", item.ItemMeasure);
                            command.Parameters.AddWithValue("@SUPPLIER_TOTALAMOUNT", Convert.ToDecimal(item.TotalAmount));
                            command.Parameters.AddWithValue("@SUPPLIER_TOTALAMOUNTWITHOUTVAT", Convert.ToDecimal(item.AmountWithoutVAT));
                            command.Parameters.AddWithValue("@SUPPLIER_TOTALAMOUNTVAT", Convert.ToDecimal(item.AmountVAT));
                            command.Parameters.AddWithValue("@ENTRYDATE", DateTime.Now.ToShortDateString());
                            command.Parameters.AddWithValue("@SUPPLIER_ITEMSTATUS", "ON-GOING");
                            command.Parameters.AddWithValue("@RESPONSIBLE", supplierModel.FullName);
                            Result++;
                            if (item.Amount == null)
                            {
                                return Result = 0;
                            }
                            command.ExecuteNonQuery();
                        }
                    }
                    connection.Close();
                    return Result;
                }
            }
            catch
            {
                throw;
            }
        }

        public int InsertItemReceivingDB(List<SupplierModel> SupplierDetails, SupplierModel SupplierModel)
        {
            DatabaseConnectionString();
            int result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            if (SupplierDetails == null)
            {
                SupplierDetails = new List<SupplierModel>();
            }
            try
            {
                using (connection = new SqlConnection(DrModule))
                {
                    connection.Open();
                    foreach (SupplierModel Item in SupplierDetails)
                    {
                        using (command = new SqlCommand("Insert_ItemReceivedInformation", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            if (!string.IsNullOrWhiteSpace(Item.ItemCount))
                            {

                                command.Parameters.AddWithValue("@DR_POSREFERENCENO", SupplierModel.POSReferenceNo);
                                command.Parameters.AddWithValue("@DR_PONUMBER", SupplierModel.POSupplierNumber);
                                command.Parameters.AddWithValue("@ITEMID", Item.ItemID);
                                command.Parameters.AddWithValue("@DR_NUMBER", SupplierModel.DeliveryNo);
                                command.Parameters.AddWithValue("@DR_SUPPLIER", SupplierModel.Supplier);
                                command.Parameters.AddWithValue("@DR_PARTICULAR", Item.ParticularName);
                                command.Parameters.AddWithValue("@DR_QTY", Item.Qty);
                                command.Parameters.AddWithValue("@DR_COLLECTED", Item.ItemCount);
                                command.Parameters.AddWithValue("@DR_RECEIVED", Item.Collected);
                                command.Parameters.AddWithValue("@DR_DATERECEIVED", SupplierModel.DeliveryDate);
                                command.Parameters.AddWithValue("@ENTRYDATE", DateTime.Now);
                                command.Parameters.AddWithValue("@DR_REMARKS", SupplierModel.DeliveryRemarks ?? "");
                                command.Parameters.AddWithValue("@DR_RESPONSIBLE", SupplierModel.FullNameResponsible);

                                result++;
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    return result;

                }
            }
            catch
            {
                return result = 0;
            }
            finally
            {
                connection.Close();
            }
        }

        public int InsertItemReleasingDB(List<ClientModel> ClientDetails, ClientModel clientModel)
        {
            DatabaseConnectionString();
            AutoGenerateDRNumberDB();
            int result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            if (ClientDetails == null)
            {
                ClientDetails = new List<ClientModel>();
            }
            try
            {
                using (connection = new SqlConnection(DrModule))
                {
                    connection.Open();
                    foreach (ClientModel Item in ClientDetails)
                    {
                        using (command = new SqlCommand("Insert_ItemReleasedInformation", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            if (!string.IsNullOrWhiteSpace(Item.ItemCount))
                            {
                                command.Parameters.AddWithValue("@DR_PONUMBER", clientModel.PONumber ?? "");
                                command.Parameters.AddWithValue("@DR_PARTICULAR", Item.ItemName);
                                command.Parameters.AddWithValue("@DR_REFERENCENO", clientModel.POC_ReferenceNo);
                                command.Parameters.AddWithValue("@DR_QTY", Item.Qty);
                                command.Parameters.AddWithValue("@DR_UNIT", Item.ItemMeasure);
                                command.Parameters.AddWithValue("@DR_RELEASED", Item.Delivered);
                                command.Parameters.AddWithValue("@DR_DELIVERED", Item.ItemCount);
                                command.Parameters.AddWithValue("@DR_RESPONSIBLE", clientModel.FullNameResponsible);
                                command.Parameters.AddWithValue("@DR_REMARKS", clientModel.DeliveryRemarks ?? "");
                                command.Parameters.AddWithValue("@DR_NUMBER", DR_Number);
                                command.Parameters.AddWithValue("@DR_DATERELEASED", clientModel.DeliveryDate);
                                command.Parameters.AddWithValue("@DR_CLIENT", clientModel.ClientID);
                                command.Parameters.AddWithValue("@DR_CLIENTADDRESS", clientModel.ClientAddress);
                                command.Parameters.AddWithValue("@ENTRYDATE", DateTime.Now);
                                command.Parameters.AddWithValue("@DR_ITEMID", Item.ItemID);

                                result++;
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    return result;

                }
            }
            catch
            {
                Delete_DrRReleasingDB(DR_Number);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public int InsertAccountingReceivablesDB(ClientModel clientmodel)
        {
            DatabaseConnectionString();
            int result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;


            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Insert_Receivables", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ACC_REFERENCENO", clientmodel.POC_ReferenceNo);
                        command.Parameters.AddWithValue("@ACC_PONUMBER", clientmodel.PONumber ?? "");
                        command.Parameters.AddWithValue("@ACC_QUOTATION", clientmodel.SignProposal ?? "");
                        command.Parameters.AddWithValue("@ACC_OR_CRNO", clientmodel.ORCRNO ?? "");
                        command.Parameters.AddWithValue("@ACC_DATEPAID", clientmodel.ACC_DatePaid ?? "");
                        command.Parameters.AddWithValue("@BILLTYPE", clientmodel.ACC_BillType ?? "");
                        command.Parameters.AddWithValue("@ACC_BILLNO", clientmodel.ACC_BillNo);
                        command.Parameters.AddWithValue("@ACC_BILLAMOUNT", Convert.ToDecimal(clientmodel.ACC_BillAmount));
                        command.Parameters.AddWithValue("@ACC_PARTICULAR", clientmodel.ACC_Particular);
                        command.Parameters.AddWithValue("@ACC_BILLDATE", clientmodel.ACC_BillDate);
                        command.Parameters.AddWithValue("@ENTRYDATE", DateTime.Now);
                        command.Parameters.AddWithValue("@RESPONSIBLE", clientmodel.ACC_Responsible);
                        command.Parameters.AddWithValue("@ACC_TYPEOFTERMS", clientmodel.ACC_TypeOfTerms);
                        command.Parameters.AddWithValue("@ACC_ID", DBNull.Value);
                        command.Parameters.AddWithValue("@ACC_DISCOUNT", Convert.ToDecimal(clientmodel.Discount ?? "0.00"));


                        result++;
                        command.ExecuteNonQuery();
                        return result;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public int InsertAccountingPayablesDB(SupplierModel suppliermodel)
        {
            DatabaseConnectionString();
            int result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;


            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Insert_Payables", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ACC_REFERENCENO", suppliermodel.POSReferenceNo);
                        command.Parameters.AddWithValue("@ACC_PONUMBER", suppliermodel.ACC_Ponumber);
                        command.Parameters.AddWithValue("@ACC_BILLINGTYPE", suppliermodel.BillingType);
                        command.Parameters.AddWithValue("@ACC_INVOICENO", suppliermodel.ACC_InvoiceNo);
                        command.Parameters.AddWithValue("@ACC_CHECKDATE", suppliermodel.ACC_CheckDate);
                        command.Parameters.AddWithValue("@ACC_DATECOLLECTED", suppliermodel.ACC_DateCollected ?? "");
                        command.Parameters.AddWithValue("@ACC_ATICVNO", suppliermodel.ACC_ATICVNO);
                        command.Parameters.AddWithValue("@ACC_PARTICULAR", suppliermodel.ACC_Particular);
                        command.Parameters.AddWithValue("@ACC_LESSWITHTAX", Convert.ToDecimal(suppliermodel.ACC_LesswithTax));
                        command.Parameters.AddWithValue("@ACC_AMOUNTPAID", Convert.ToDecimal(suppliermodel.ACC_AmountPaidHidden));
                        command.Parameters.AddWithValue("@ENTRYDATE", DateTime.Now);
                        command.Parameters.AddWithValue("@RESPONSIBLE", suppliermodel.ACC_Responsible);
                        command.Parameters.AddWithValue("@ACC_TYPEOFTERMS", suppliermodel.TypeOfTerms);
                        command.Parameters.AddWithValue("@ACC_ID", DBNull.Value);
                        command.Parameters.AddWithValue("@ACC_DISCOUNT", Convert.ToDecimal(suppliermodel.Discount ?? "0.00"));

                        result++;
                        InsertAccountingDocumentsDB(suppliermodel.POSReferenceNo, suppliermodel.ACC_DRNO, suppliermodel.ACC_MRRNO);
                        command.ExecuteNonQuery();
                        return result;
                    }
                }
            }
            catch
            {
                throw;
                return result = 0;
            }
            finally
            {
                connection.Close();
            }
        }

        public void InsertAccountingDocumentsDB(string POS, string DR, string MRR)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    using (command = new SqlCommand("Insert_Documents", connection))
                    {
                        connection.Open();
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@ACC_ID", POS);
                        command.Parameters.AddWithValue("@ACC_DRNO", DR ?? "");
                        command.Parameters.AddWithValue("@ACC_MRRNO", MRR ?? "");
                        command.Parameters.AddWithValue("@ENTRYDATE", DateTime.Now);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public int InsertCertifcateOfCompletionDB(ClientModel clientModel, string Update)
        {
            DatabaseConnectionString();
            AutoGenerateControlNumber_COC();
            int result = 0;

            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Insert_CertificateOfCompletionInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@COC_NUMBER", CoC_Number);
                        if (Update == "0")
                        {
                            command.Parameters.AddWithValue("@COC_CONTROLNO", CoC_ControlNumber);

                        }
                        else
                        {
                            command.Parameters.AddWithValue("@COC_CONTROLNO", clientModel.COC_ControlNo);
                        }
                        command.Parameters.AddWithValue("@COC_REFERENCENO", clientModel.POC_ReferenceNo);
                        command.Parameters.AddWithValue("@COC_PROJECTDETAILS", clientModel.ProjectDetails);
                        command.Parameters.AddWithValue("@COC_PROJECTTITLE", clientModel.ProjectName);
                        command.Parameters.AddWithValue("@COC_STARTDATE", clientModel.StartDate);
                        command.Parameters.AddWithValue("@COC_COMPLETIONDATE", clientModel.CompletionDate);
                        command.Parameters.AddWithValue("@COC_POREFERENCE", clientModel.POReferenceNo ?? "");
                        command.Parameters.AddWithValue("@COC_CLIENTID", Convert.ToInt32(clientModel.ClientID));
                        command.Parameters.AddWithValue("@COC_APPROVEDBY", Convert.ToInt32(clientModel.ApproverID));
                        command.Parameters.AddWithValue("@COC_ENTRYDATE", DateTime.Now);
                        command.Parameters.AddWithValue("@COC_FORMSTATUS", "FOR APPROVAL");
                        command.Parameters.AddWithValue("@COC_ASSESSEDBY", clientModel.COC_Resposible ?? "");
                        command.Parameters.AddWithValue("@COC_CLIENTADDRESS", clientModel.ClientAddress);
                        command.Parameters.AddWithValue("@COC_CLIENTLOCATION", clientModel.ClientLocation);
                        command.Parameters.AddWithValue("@COC_PREPAREDBY", Convert.ToInt32(clientModel.FullNameResponsible));


                        result++;
                        command.ExecuteNonQuery();

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                result = 0;
                // Add useful information to the exception
                throw;

            }
            finally
            {
                connection.Close();
            }
        }

        public int InsertPreviewCertifcateOfCompletionDB(ClientModel clientModel)
        {
            DatabaseConnectionString();
            AutoGenerateNumber_COCPREVIEW();
            int result = 0;

            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Insert_PreviewCertificateOfCompletionInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PREVIEWNO", Preview_No);
                        command.Parameters.AddWithValue("@COC_PROJECTDETAILS", clientModel.ProjectDetails);
                        command.Parameters.AddWithValue("@COC_PROJECTTITLE", clientModel.ProjectName);
                        command.Parameters.AddWithValue("@COC_STARTDATE", clientModel.StartDate);
                        command.Parameters.AddWithValue("@COC_COMPLETIONDATE", clientModel.CompletionDate);
                        command.Parameters.AddWithValue("@COC_POREFERENCE", clientModel.POReferenceNo ?? "");
                        command.Parameters.AddWithValue("@COC_CLIENTID", clientModel.ClientID);
                        command.Parameters.AddWithValue("@COC_APPROVEDBY", clientModel.ApproverID);
                        command.Parameters.AddWithValue("@COC_ENTRYDATE", DateTime.Now);
                        command.Parameters.AddWithValue("@COC_FORMSTATUS", "FOR APPROVAL");
                        command.Parameters.AddWithValue("@COC_ASSESSEDBY", clientModel.COC_Resposible ?? "");


                        result++;
                        command.ExecuteNonQuery();

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                result = 0;
                // Add useful information to the exception
                throw new ApplicationException("Something wrong happened in the calculation module :", ex);

            }
            finally
            {
                connection.Close();
            }
        }

        public int Insert_ClientQuotationDB(ClientModel client)
        {
            DatabaseConnectionString();

            SqlConnection connection = null;
            SqlCommand command = null;
            int i = 0;
            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Insert_ClientQuotation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@QUOTATION_ID", Quotation_Number);
                        command.Parameters.AddWithValue("@QUOTATION_REFERENCENO", Quotation_ReferenceNo);
                        command.Parameters.AddWithValue("@QUOTATION_CONTROLNO", client.Quotation_ControlNo ?? Quotation_ControlNumber);
                        command.Parameters.AddWithValue("@QUOTATION_CLIENTNAME", client.ClientName);
                        command.Parameters.AddWithValue("@QUOTATION_COMPANY", client.ClientID);
                        command.Parameters.AddWithValue("@QUOTATION_LOCATION", client.ClientLocation);
                        command.Parameters.AddWithValue("@QUOTATION_ADDRESS", client.ClientAddress);
                        command.Parameters.AddWithValue("@QUOTATION_DISCOUNT", client.Discount ?? "0.00");
                        command.Parameters.AddWithValue("@QUOTATION_PROJECTNAME", client.ProjectName);
                        command.Parameters.AddWithValue("@QUOTATION_GRANDTOTAL", Convert.ToDecimal(client.GrandTotal));
                        command.Parameters.AddWithValue("@RESPONSIBLE", client.FullNameResponsible);
                        command.Parameters.AddWithValue("@QUOTATION_TERMS", client.ddlOthersPayment ?? "");
                        command.Parameters.AddWithValue("@QUOTATION_SUBMITTEDBY", client.Submittedby_ID ?? "");
                        command.Parameters.AddWithValue("@QUOTATION_VAT", Convert.ToDecimal(client.AmountVAT));

                        command.ExecuteNonQuery();
                        i++;
                        return i;
                    }
                }
            }
            catch
            {
                DeleteClientQuotationGRIDDB(client);
                throw;
            }
            finally
            {
                connection.Close();
            }

        }

        public int Insert_ClientQuotation_GRIDDB(List<ClientModel> clientlist, ClientModel client)
        {
            DatabaseConnectionString();

            AutoGenerateControlNumber_QUOTATION();
            SqlConnection connection = null;
            SqlCommand command = null;
            int i = 0;
            try
            {
                if (clientlist == null)
                {
                    clientlist = new List<ClientModel>();
                }

                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    foreach (ClientModel item in clientlist)
                    {
                        using (command = new SqlCommand("Insert_ClientQuotation_GRID", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@QUOTATION_REFERENCENO", Quotation_ReferenceNo);
                            command.Parameters.AddWithValue("@QUOTATION_CONTROLNO", client.Quotation_ControlNo ?? Quotation_ControlNumber);
                            command.Parameters.AddWithValue("@QUOTATION_ITEMNAME", item.ItemID);
                            command.Parameters.AddWithValue("@QUOTATION_ITEMDESC", item.itemDescription);
                            command.Parameters.AddWithValue("@QUOTATION_ITEMMEASURE", item.ItemMeasure);
                            command.Parameters.AddWithValue("@QUOTATION_QTY", item.Qty);
                            command.Parameters.AddWithValue("@QUOTATION_AMOUNT", Convert.ToDecimal(item.Amount));
                            command.Parameters.AddWithValue("@QUOTATION_MARKUP", Convert.ToDecimal(item.Markup));
                            command.Parameters.AddWithValue("@QUOTATION_TOTALAMOUNT", Convert.ToDecimal(item.TotalAmount));
                            command.ExecuteNonQuery();
                        }
                        i++;

                    }

                    return i;

                }
            }
            catch
            {
                DeleteClientQuotationGRIDDB(client);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public int Insert_PRPreviewInformationDB(SupplierModel PRModel)
        {
            DatabaseConnectionString();
            int i = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Insert_PReviewPRInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PR_ID", PR_PReviewNumber);
                        command.Parameters.AddWithValue("@PR_PROJECTDESC", PRModel.ProjectDescription);
                        command.Parameters.AddWithValue("@PR_RESPONSIBLE", PRModel.PreparedID);
                        command.Parameters.AddWithValue("@PR_DATENEEDED", PRModel.DateNeeded);
                        command.Parameters.AddWithValue("@PR_CANVASSEDBY", PRModel.CanvessedBy);
                        command.Parameters.AddWithValue("@PR_ENDORSER", PRModel.EndorserID);
                        command.Parameters.AddWithValue("@PR_APPROVER", PRModel.ApproverID);
                        command.Parameters.AddWithValue("@PR_SUPPLIERONE", PRModel.SupplierID_One ?? "0");
                        command.Parameters.AddWithValue("@PR_SUPPLIERTWO", PRModel.SupplierID_Two ?? "0");
                        command.Parameters.AddWithValue("@PR_SUPPLIERTHREE", PRModel.SupplierID_Three ?? "0");
                        command.Parameters.AddWithValue("@PR_GRANDTOTALONE", Convert.ToDecimal(PRModel.GrandTotalAmount_One));
                        command.Parameters.AddWithValue("@PR_GRANDTOTALTWO", Convert.ToDecimal(PRModel.GrandTotalAmount_Two));
                        command.Parameters.AddWithValue("@PR_GRANDTOTALTHREE", Convert.ToDecimal(PRModel.GrandTotalAmount_Three));
                        command.ExecuteNonQuery();
                        i++;
                        return i;
                    }
                }
            }
            catch (Exception ex)
            {
                DeletePreviewPRInformationGRIDDB();
                throw ex;
            }
            finally
            {
                connection.Close();

            }

        }

        public int Insert_PRPreviewInformation_GRIDDB(List<SupplierModel> PRList, SupplierModel PRModel)
        {
            DatabaseConnectionString();
            AutoGenerateNumber_PR_PREVIEW();
            int i = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            if (PRList == null)
            {
                PRList = new List<SupplierModel>();
            }

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    foreach (SupplierModel item in PRList)
                    {
                        using (command = new SqlCommand("Insert_PRPreviewInformation_GRID", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@PR_ID", PR_PReviewNumber);
                            command.Parameters.AddWithValue("@PR_PARTICULAR", item.ParticularName);
                            command.Parameters.AddWithValue("@PR_REPORTVIEW", item.itemDescription);
                            command.Parameters.AddWithValue("@PR_ITEMMEASURE", item.ItemMeasure);
                            command.Parameters.AddWithValue("@PR_QTY", item.Qty);
                            command.Parameters.AddWithValue("@PR_AMOUNTONE", Convert.ToDecimal(item.Amount_One ?? "0"));
                            command.Parameters.AddWithValue("@PR_TOTALAMOUNTONE", Convert.ToDecimal(item.TotalAmount_One));
                            command.Parameters.AddWithValue("@PR_AMOUNTTWO", Convert.ToDecimal(item.Amount_Two ?? "0"));
                            command.Parameters.AddWithValue("@PR_TOTALAMOUNTTWO", Convert.ToDecimal(item.TotalAmount_Two));
                            command.Parameters.AddWithValue("@PR_AMOUNTTHREE", Convert.ToDecimal(item.Amount_Three ?? "0"));
                            command.Parameters.AddWithValue("@PR_TOTALAMOUNTHREE", Convert.ToDecimal(item.TotalAmount_Three));
                            command.Parameters.AddWithValue("@PR_RESPONSIBLE", PRModel.PreparedID);
                            command.ExecuteNonQuery();
                            i++;
                        }
                    }
                    return i;


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }

        }

        public int Insert_PRInformationDB(SupplierModel PRModel)
        {
            DatabaseConnectionString();
            int i = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Insert_PRInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PR_ID", PR_Number);
                        command.Parameters.AddWithValue("@PR_RESPONSIBLE", PRModel.PreparedID);
                        command.Parameters.AddWithValue("@PR_NUMBER", PRModel.PRNumber ?? PR_ControlNumber);
                        command.Parameters.AddWithValue("@PR_REFERENCENO", PR_ReferenceNo);
                        command.Parameters.AddWithValue("@PR_PROJECTDESC", PRModel.ProjectDescription);
                        command.Parameters.AddWithValue("@PR_DATENEEDED", PRModel.DateNeeded);
                        command.Parameters.AddWithValue("@PR_CANVASSEDBY", PRModel.CanvessedBy);
                        command.Parameters.AddWithValue("@PR_ENDORSER", PRModel.EndorserID);
                        command.Parameters.AddWithValue("@PR_APPROVER", PRModel.ApproverID);
                        command.Parameters.AddWithValue("@PR_SUPPLIERONE", PRModel.SupplierID_One ?? "0");
                        command.Parameters.AddWithValue("@PR_SUPPLIERTWO", PRModel.SupplierID_Two ?? "0");
                        command.Parameters.AddWithValue("@PR_SUPPLIERTHREE", PRModel.SupplierID_Three ?? "0");
                        command.Parameters.AddWithValue("@PR_GRANDTOTALONE", Convert.ToDecimal(PRModel.GrandTotalAmount_One));
                        command.Parameters.AddWithValue("@PR_GRANDTOTALTWO", Convert.ToDecimal(PRModel.GrandTotalAmount_Two));
                        command.Parameters.AddWithValue("@PR_GRANDTOTALTHREE", Convert.ToDecimal(PRModel.GrandTotalAmount_Three));
                        command.ExecuteNonQuery();
                        i++;
                        return i;
                    }
                }
            }
            catch (Exception ex)
            {
                DeletePRInformationGRIDDB();
                throw ex;
            }
            finally
            {
                connection.Close();

            }

        }

        public int Insert_PRInformation_GRIDDB(List<SupplierModel> PRList, SupplierModel PRModel)
        {
            DatabaseConnectionString();
            AutoGenerateControlNumber_PRNUMBER();
            int i = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            if (PRList == null)
            {
                PRList = new List<SupplierModel>();
            }

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    foreach (SupplierModel item in PRList)
                    {
                        using (command = new SqlCommand("Insert_PRInformation_GRID", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@PR_NUMBER", PRModel.PRNumber ?? PR_ControlNumber);
                            command.Parameters.AddWithValue("@PR_REFERENCENO", PR_ReferenceNo);
                            command.Parameters.AddWithValue("@PR_PARTICULAR", item.ParticularName);
                            command.Parameters.AddWithValue("@PR_REPORTVIEW", item.itemDescription);
                            command.Parameters.AddWithValue("@PR_ITEMMEASURE", item.ItemMeasure);
                            command.Parameters.AddWithValue("@PR_QTY", item.Qty);
                            command.Parameters.AddWithValue("@PR_AMOUNTONE", Convert.ToDecimal(item.Amount_One ?? "0"));
                            command.Parameters.AddWithValue("@PR_TOTALAMOUNTONE", Convert.ToDecimal(item.TotalAmount_One));
                            command.Parameters.AddWithValue("@PR_AMOUNTTWO", Convert.ToDecimal(item.Amount_Two ?? "0"));
                            command.Parameters.AddWithValue("@PR_TOTALAMOUNTTWO", Convert.ToDecimal(item.TotalAmount_Two));
                            command.Parameters.AddWithValue("@PR_AMOUNTTHREE", Convert.ToDecimal(item.Amount_Three ?? "0"));
                            command.Parameters.AddWithValue("@PR_TOTALAMOUNTHREE", Convert.ToDecimal(item.TotalAmount_Three));
                            command.ExecuteNonQuery();
                            i++;
                        }
                    }
                    return i;


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }

        }

        public int Insert_ReleasingItemReturnDB(DRModel drmodel, string Responsible)
        {
            DatabaseConnectionString();
            int i = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(DrModule))
                {
                    connection.Open();
                    using (command = new SqlCommand("Insert_ReleasingItemReturn", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@DR_CONTROLNO", drmodel.DRNumber);
                        command.Parameters.AddWithValue("@REFERENCENO", drmodel.DRReferenceNo);
                        command.Parameters.AddWithValue("@DR_ITEMRETURN", Convert.ToInt32(drmodel.DRItemReturn));
                        command.Parameters.AddWithValue("@DR_REASONREMARKS", drmodel.DRReasonRemarks);
                        command.Parameters.AddWithValue("@ITEMNAME", drmodel.DRParticular);
                        command.Parameters.AddWithValue("@PONUMBER", drmodel.DRPONumber ?? "");
                        command.Parameters.AddWithValue("@CLIENT", drmodel.DRClient);
                        command.Parameters.AddWithValue("@RESPONSIBLE", Responsible);

                        command.ExecuteNonQuery();
                        i++;
                    }
                    return i;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public int Insert_ReceivingItemReturnDB(DRModel drmodel, string Responsible)
        {
            DatabaseConnectionString();
            int i = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(DrModule))
                {
                    connection.Open();
                    using (command = new SqlCommand("Insert_ReceivingItemReturn", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@DR_RECEIVEDID", Convert.ToInt32(drmodel.DRID));
                        command.Parameters.AddWithValue("@DR_REFERENCENO", drmodel.DRReferenceNo);
                        command.Parameters.AddWithValue("@DR_NUMBER", drmodel.DRNumber);
                        command.Parameters.AddWithValue("@DR_PONUMBER", drmodel.DRPONumber);
                        command.Parameters.AddWithValue("@DR_ITEMRETURN", Convert.ToInt32(drmodel.DRItemReturn));
                        command.Parameters.AddWithValue("@DR_REASONREMARKS", drmodel.DRReasonRemarks);
                        command.Parameters.AddWithValue("@DR_ITEMNAME", drmodel.DRParticular);
                        command.Parameters.AddWithValue("@DR_SUPPLIER", drmodel.DRSupplier);
                        command.Parameters.AddWithValue("@DR_RESPONSIBLE", Responsible);

                        command.ExecuteNonQuery();
                        i++;
                    }
                    return i;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public int UpdateDRManualReleasingDB(ClientModel model)
        {
            DatabaseConnectionString();
            int i = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(DrModule))
                {
                    connection.Open();
                    using (command = new SqlCommand("Insert_UpdateManualReleasing", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@DR_NUMBER", model.DeliveryNo);
                        command.Parameters.AddWithValue("@DR_DATE", model.DeliveryDate);

                        command.ExecuteNonQuery();
                        i++;
                    }
                    return i;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

        }

        public string Check_UpdateManualReleasing(string Item)
        {
            DatabaseConnectionString();
            string Result = "";
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(DrModule))
                {
                    connection.Open();
                    //Loop and insert retcords.

                    using (command = new SqlCommand("Check_UpdateManualReleasing", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@DR_NUMBER", Item);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();

                        sda.Fill(dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            Result = row["DR_NUMBER"].ToString();
                        }
                    }
                }
                return Result;
            }
            catch
            {
                return Result;
            }
            finally
            {
                connection.Close();
            }
        }
        


        //EDIT DATA
        public UserMaintenance EditUserInformationDB(int? id)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            UserMaintenance UserModel = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Edit_UserInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@USERID", id);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            UserModel = new UserMaintenance
                            {
                                UserID = Convert.ToInt32(row["USERID"].ToString()),
                                EmployeeID = row["EMPID"].ToString(),
                                UserName = row["USERNAME"].ToString(),
                                FirstName = row["FIRSTNAME"].ToString(),
                                LastName = row["LASTNAME"].ToString(),
                                EmailAddress = row["EMAIL"].ToString(),
                                Location = row["LOCATION"].ToString(),
                                Position = row["POSITION"].ToString(),
                                ApproverID = Convert.ToInt32(row["APPROVERID"].ToString()),
                                UserGroupID = Convert.ToInt32(row["USERGROUPID"].ToString()),
                                UserRoleID = Convert.ToInt32(row["USERROLEID"].ToString()),
                                UserStatus = row["USERSTATUS"].ToString(),
                            };
                        }
                        connection.Close();
                        return UserModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public ClientMaster EditClientInformationDB(int? id)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            ClientMaster ClientModel = null;

            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    using (command = new SqlCommand("Edit_ClientInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@CLIENTID", id);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ClientModel = new ClientMaster
                            {
                                ClientID = Convert.ToInt32(row["CLIENTID"].ToString()),
                                Client = row["CLIENT"].ToString(),
                                ContactPerson = row["CONTACTPERSON"].ToString(),
                                ContactNo = row["CONTACTNO"].ToString(),
                                EmailAddress = row["EMAILADDRESS"].ToString(),
                                ClientStatus = row["CLIENTSTATUS"].ToString()
                            };

                        }

                        connection.Close();
                        return ClientModel;

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ItemMaster EditItemInformationDB(int? id)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            ItemMaster itemMaster = null;
            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    using (command = new SqlCommand("Edit_ItemIformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@ITEMID", id);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            itemMaster = new ItemMaster
                            {
                                ItemID = Convert.ToInt32(row["ITEMID"].ToString()),
                                ItemCategory = row["ITEMCATEGORY"].ToString(),
                                ItemName = row["ITEMNAME"].ToString(),
                                ItemMeasure = row["ITEMMEASURE"].ToString(),
                                ItemDescription = row["ITEMDESCRIPTION"].ToString(),
                                ItemStatus = row["ITEMSTATUS"].ToString(),
                                ItemCode = row["ITEMCODE"].ToString(),
                                OldItemCode = row["ITEMCODE"].ToString(),
                                

                            };
                        }
                        connection.Close();
                    }
                }
                return itemMaster;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SupplierMaster EditSupplierInformationDB(int? id)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            SupplierMaster SupplierModel = null;

            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    using (command = new SqlCommand("Edit_SupplierInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SUPPLIERID", id);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            SupplierModel = new SupplierMaster
                            {
                                SupplierID = Convert.ToInt32(row["SUPPLIERID"].ToString()),
                                Supplier = row["SUPPLIER"].ToString(),
                                SupplierAddress = row["SUPPLIERADDRESS"].ToString(),
                                EmailAddress = row["EMAILADDRESS"].ToString(),
                                ContactPerson = row["CONTACTPERSON"].ToString(),
                                ContactNo = row["CONTACTNO"].ToString(),
                                SupplierStatus = row["SUPPLIERSTATUS"].ToString(),
                                Location = row["LOCATION"].ToString()

                            };
                        }
                        connection.Close();
                        return SupplierModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SupplierModel PopulateListSupplierAddressDB(string id)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            SupplierModel supplierModel = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("List_AllSupplierAddressInformation", connection))
                    {

                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SupplierID", id);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {

                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    supplierModel = new SupplierModel
                                    {
                                        SupplierAddresss = row["SupplierAddress"].ToString()
                                    };

                                }
                                connection.Close();
                                return supplierModel;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string PopulateListClientAddressDB(string id)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            ClientModel clientModel = null;
            string ClientAddress = "";

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_GetClientAddress", connection))
                    {

                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Number", id);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {

                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    ClientAddress = row["CLIENT_ADDRESS"].ToString();

                                }
                                connection.Close();
                                return ClientAddress;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ClientModel EditUserClientInformationDB(string id)
        {
            DatabaseConnectionString();
            ClientModel clientModel = null;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    using (command = new SqlCommand("Select_EditClientInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", id);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();

                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            clientModel = new ClientModel
                            {

                                POC_ReferenceNo = row["CLIENT_REFERENCENO"].ToString(),
                                PONumber = row["CLIENT_PONUMBER"].ToString(),
                                PODate = row["CLIENT_PODATE"].ToString(),
                                TermsofPayment = row["CLIENT_TERMSOFPAYMENT"].ToString(),
                                SalesPersonnel = row["CLIENT_SALES"].ToString(),
                                ClientID = row["CLIENT_IDNAME"].ToString(),
                                ClientStatus = row["CLIENT_STATUS"].ToString(),
                                DisplayGrandTotal = Convert.ToDecimal(row["CLIENT_GRANDTOTAL"].ToString()),
                                ProjectName = row["CLIENT_PROJECTNAME"].ToString(),
                                Discount = row["CLIENT_DISCOUNT"].ToString(),
                                ClientLocation = row["CLIENT_LOCATION"].ToString(),
                                SignProposal = row["CLIENT_QUOTATIONNO"].ToString(),
                                ClientAddress = row["CLIENT_ADDRESS"].ToString()
                            };

                        }
                    }
                }

                return clientModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ClientModel GetInformationReceivablesDB(string id)
        {
            DatabaseConnectionString();
            ClientModel clientModel = null;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    using (command = new SqlCommand("Select_AddReceivables", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", id);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();

                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            clientModel = new ClientModel
                            {

                                POC_ReferenceNo = row["CLIENT_REFERENCENO"].ToString(),
                                PONumber = row["CLIENT_PONUMBER"].ToString(),
                                TermsofPayment = row["CLIENT_TERMSOFPAYMENT"].ToString(),
                                SalesPersonnel = row["CLIENT_SALES"].ToString(),
                                ClientID = row["CLIENTNAME"].ToString(),
                                ClientStatus = row["CLIENT_STATUS"].ToString(),
                                DisplayGrandTotal = Convert.ToDecimal(row["CLIENT_GRANDTOTAL"].ToString()),
                                ProjectName = row["CLIENT_PROJECTNAME"].ToString(),
                                SignProposal = row["CLIENT_QUOTATIONNO"].ToString(),
                                ACC_COCNO = row["COC_CONTROLNO"].ToString(),
                            };

                        }
                    }
                }

                return clientModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ClientModel GetPOReferenceNo(string id)
        {
            DatabaseConnectionString();
            ClientModel clientModel = null;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    using (command = new SqlCommand("Select_GetPOReferenceNO", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", id);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();

                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            clientModel = new ClientModel
                            {
                                POReferenceNo = row["CLIENT_PONUMBER"].ToString(),
                                ClientID = row["CLIENT_IDNAME"].ToString(),
                                ProjectName = row["CLIENT_PROJECTNAME"].ToString(),
                                POC_ReferenceNo = row["CLIENT_REFERENCENO"].ToString(),
                                ClientLocation = row["CLIENT_LOCATION"].ToString(),
                                ClientAddress = row["CLIENT_ADDRESS"].ToString(),

                            };

                        }
                    }
                }

                return clientModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SupplierModel EditUserSupplierInformationDB(string id)
        {
            DatabaseConnectionString();
            SupplierModel supplierModel = null;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_EditUserSupplierInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", id);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    supplierModel = new SupplierModel
                                    {
                                        POSReferenceNo = row["SUPPLIER_REFERENCENO"].ToString(),
                                        SupplierAddresss = row["SUPPLIER_ADDRESS"].ToString(),
                                        Supplier = row["SUPPLIER"].ToString(),
                                        ApproverID = row["APPROVER"].ToString(),
                                        EndorserID = row["ENDORSER"].ToString(),
                                        ReviewerID = row["REVIEWER"].ToString(),
                                        DisplayGrandTotal = Convert.ToDecimal(row["SUPPLIER_GRANDTOTAL"].ToString()),
                                        DisplayGrandTotalVat = Convert.ToDecimal(row["SUPPLIER_GRANDTOTALVAT"].ToString()),
                                        DisplayGrandTotalWithoutVat = Convert.ToDecimal(row["SUPPLIER_GRANDTOTALWITHOUTVAT"].ToString()),
                                        TermsofPayment = row["SUPPLIER_TERMOFPAYMENT"].ToString(),
                                        Shippinginstruction = row["SUPPLIER_SHIPPINGINS"].ToString(),
                                        POSupplierNumber = row["SUPPLIER_PONUMBER"].ToString(),
                                        PODate = Convert.ToDateTime(row["SUPPLIER_PODATE"].ToString()),
                                        Discount = row["SUPPLIER_DISCOUNT"].ToString(),
                                        ProjectName = row["SUPPLIER_PROJECTNAME"].ToString(),
                                        FormStatus = row["FORMSTATUS"].ToString(),
                                        DeliveryAddress = row["SUPPLIER_DELIVERYADDRESS"].ToString(),
                                        FullName = row["RESPONSIBLE"].ToString(),
                                        FullNameResponsible = row["FULLNAMERESPONSIBLE"].ToString(),
                                        TypeOfVat = row["TYPEOFVAT"].ToString(),
                                        PRNumber = row["SUPPLIER_PRNUMBER"].ToString(),

                                    };
                                }
                                connection.Close();
                                return supplierModel;
                            }
                        }
                    }
                }
            }
            catch
            {
                return supplierModel;
            }
        }

        public SupplierModel GetInformationPayablesDB(string id)
        {
            DatabaseConnectionString();
            SupplierModel supplierModel = null;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_AddPayables", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", id);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    supplierModel = new SupplierModel
                                    {
                                        POSReferenceNo = row["SUPPLIER_REFERENCENO"].ToString(),
                                        SupplierAddresss = row["SUPPLIER_ADDRESS"].ToString(),
                                        Supplier = row["SUPPLIER"].ToString(),
                                        ApproverID = row["APPROVER"].ToString(),
                                        EndorserID = row["ENDORSER"].ToString(),
                                        DisplayGrandTotal = Convert.ToDecimal(row["SUPPLIER_GRANDTOTAL"].ToString()),
                                        DisplayGrandTotalVat = Convert.ToDecimal(row["SUPPLIER_GRANDTOTALVAT"].ToString()),
                                        DisplayGrandTotalWithoutVat = Convert.ToDecimal(row["SUPPLIER_GRANDTOTALWITHOUTVAT"].ToString()),
                                        TermsofPayment = row["SUPPLIER_TERMOFPAYMENT"].ToString(),
                                        Shippinginstruction = row["SUPPLIER_SHIPPINGINS"].ToString(),
                                        ACC_Ponumber = row["SUPPLIER_PONUMBER"].ToString(),
                                        PODate = Convert.ToDateTime(row["SUPPLIER_PODATE"].ToString()),
                                        FormStatus = row["FORMSTATUS"].ToString(),
                                        FullName = row["RESPONSIBLE"].ToString(),
                                        FullNameResponsible = row["FULLNAMERESPONSIBLE"].ToString(),
                                        ACC_DRNO = row["ACC_DRNO"].ToString(),
                                        ACC_MRRNO = row["ACC_MRRNO"].ToString(),
                                        Percentage = row["PERCENTAGE"].ToString(),
                                    };
                                }
                                connection.Close();
                                return supplierModel;
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
                return supplierModel;
            }
        }

        public LoginModel GetUserSessionDB(string id)
        {
            DatabaseConnectionString();
            DatabaseConnectionString();
            LoginModel loginModel = null;
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_GetUserSession", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@userID", id);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    loginModel = new LoginModel
                                    {
                                        UserID = Convert.ToInt32(row["USERID"].ToString()),
                                        EmployeeID = row["EMPID"].ToString(),
                                        UserGroup = row["USERGROUP"].ToString(),
                                        UserRoleID = row["USERROLEID"].ToString(),
                                        Department = row["DEPARTMENT"].ToString(),
                                        FullName = row["FULLNAME"].ToString(),
                                        Email = row["EMAIL"].ToString(),
                                        Status = row["USERSTATUS"].ToString(),
                                        UserRole = row["USERROLE"].ToString(),
                                        DepartmentID = row["DEPARTMENTID"].ToString(),
                                        UserGroupID = row["USERGROUPID"].ToString(),
                                        VerificationDate = row["VERIFICATIONUPDATE"].ToString(),
                                        UserName = row["USERNAME"].ToString(),
                                        Auth_SecretKey = row["AUTH_SECRETKEY"].ToString(),
                                        Auth_Status = row["AUTH_STATUS"].ToString(),
                                        Auth_QRCode = row["AUTH_QRCODE"].ToString(),
                                        Auth_ManualCode = row["AUTH_MANUALCODE"].ToString(),



                                    };

                                }
                                connection.Close();

                                return loginModel;
                            }
                        }
                    }
                }

            }
            catch
            {
                throw;
            }
        }

        public MainModel NotificationDB(string User)
        {
            DatabaseConnectionString();
            MainModel model = null;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_CountSupplierStatusForm", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@USERID", User);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            model = new MainModel
                            {

                                FORENDORSEMENT = row["FORENDORSMENT"].ToString(),
                                FORAPPROVAL = row["FORAPPROVAL"].ToString(),
                                FORREVIEW = row["FORREVIEW"].ToString(),
                                CountClosed = row["CLOSED_PROJECT"].ToString(),
                                CountOngoing = row["ONGOING_PROJECT"].ToString(),
                                CountSupplier = row["SUPPLIER"].ToString(),
                                CountClient = row["CLIENT"].ToString(),
                                CountSales = row["SALESMONTH"].ToString(),
                                CountSalesToday = row["SALESTODAY"].ToString(),
                                CountTotalEarnings = row["TOTALEARNING"].ToString(),
                                CountAverageSales = row["AVERAGESALES"].ToString(),
                                CountOrders = row["ORDERS"].ToString(),
                                APPROVED = row["APPROVED"].ToString(),
                                REJECTED = row["REJECTED"].ToString(),
                                CountReviewed = row["REVIEWED"].ToString(),
                                CountEndorsed = row["ENDORSED"].ToString(),
                                CountCOC = row["COCFORAPPROVAL"].ToString(),

                                CountPOForApproval = row["POFORAPPROVAL"].ToString(),
                                CountPOForEndorsement = row["POFORENDORSMENT"].ToString(),
                                CountPOForReview = row["POFORREVIEW"].ToString(),

                                CountPRForApproval = row["PRFORAPPROVAL"].ToString(),
                                CountPRForEndorsement = row["PRFORENDORSMENT"].ToString(),
                                CountPRForReview = row["PRFORREVIEW"].ToString(),

                                CountPR_TotalRequest = row["PR_TOTALREQUEST"].ToString(),
                                CountPR_TotalRequested = row["PR_TOTALREQUESTED"].ToString(),
                                CountPO_TotalRequested = row["PO_TOTALREQUESTED"].ToString(),
                                CountCOC_TotalRequested = row["COC_TOTALREQUESTED"].ToString(),


                            };
                        }
                    }
                    return model;

                }
            }
            catch
            {
                return model;

            }
            finally
            {
                connection.Close();
            }
        }

        public AccountingModel EditReceivablesInformationDB(string id)
        {
            DatabaseConnectionString();
            AccountingModel accountingmodel = null;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    using (command = new SqlCommand("Select_GetReceivablesInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ID", id);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);
                                foreach (DataRow row in dt.Rows)
                                {
                                    accountingmodel = new AccountingModel
                                    {

                                        ACC_ID = row["ACCID"].ToString(),
                                        ACC_BillNo = row["ACC_BILLNO"].ToString(),
                                        ACC_BillAmount = Convert.ToDecimal(row["ACC_BILLAMOUNT"].ToString()),
                                        ACC_DatePaid = row["ACC_DATEPAID"].ToString(),
                                        ORCRNO = row["ACC_OR_CRNO"].ToString(),
                                        ACC_BillType = row["BILLTYPE"].ToString(),
                                        ACC_BillDate = Convert.ToDateTime(row["ACC_DATEPAID"].ToString()),
                                        ACC_Responsible = row["RESPONSIBLE"].ToString(),
                                        ACC_Particular = row["ACC_PARTICULAR"].ToString(),
                                        ACC_Remarks = row["ACC_REMARKS"].ToString(),
                                        ACC_ReferenceNo = row["ACC_REFERENCENO"].ToString(),
                                        ACC_Client = row["CLIENTNAME"].ToString(),

                                    };
                                }
                                return accountingmodel;
                            }
                        }

                    }
                }
            }
            catch
            {
                return accountingmodel;
            }
            finally
            {
                connection.Close();
            }
        }

        public AccountingModel EditPayablesInformationDB(string id)
        {
            DatabaseConnectionString();
            AccountingModel accountingmodel = null;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    using (command = new SqlCommand("Select_GetPayablesInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ID", id);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);
                                foreach (DataRow row in dt.Rows)
                                {
                                    accountingmodel = new AccountingModel
                                    {
                                        ACC_ID = row["ACC_ID"].ToString(),


                                        ACC_InvoiceNo = row["ACC_INVOICENO"].ToString(),
                                        ACC_AmountPaid = Convert.ToDecimal(row["ACC_AMOUNTPAID"].ToString()),
                                        ACC_CheckDate = Convert.ToDateTime(row["ACC_CHECKDATE"].ToString()),
                                        ACC_DateCollected = Convert.ToDateTime(row["ACC_DATECOLLECTED"].ToString()),
                                        ACC_DRNO = row["ACC_DRNO"].ToString(),
                                        ACC_MRRNO = row["ACC_MRRNO"].ToString(),
                                        ACC_COCNO = row["ACC_COCNO"].ToString(),
                                        ACC_ATICVNO = row["ACC_ATICVNO"].ToString(),
                                        ACC_Responsible = row["RESPONSIBLE"].ToString(),
                                        ACC_Particular = row["ACC_PARTICULAR"].ToString(),
                                        ACC_LesswithTax = Convert.ToDecimal(row["ACC_LESSWITHTAX"].ToString()),
                                        ACC_Remarks = row["ACC_REMARKS"].ToString(),
                                        ACC_ReferenceNo = row["ACC_REFERENCENO"].ToString(),


                                    };
                                }
                                return accountingmodel;
                            }
                        }

                    }
                }
            }
            catch
            {
                return accountingmodel;
            }
            finally
            {
                connection.Close();
            }
        }

        public ClientModel EditRoutingCertificateOfCompletionDB(string id)
        {
            DatabaseConnectionString();
            ClientModel clientmodel = null;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_GetRoutingCertificateOfCompletion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ControlNo", id);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            clientmodel = new ClientModel
                            {
                                COC_Client = row["CLIENT"].ToString(),
                                COC_ControlNo = row["COC_CONTROLNO"].ToString(),
                                COC_ProjectTitle = row["COC_PROJECTTITLE"].ToString(),
                                ProjectDetails = row["COC_PROJECTDETAILS"].ToString(),
                                ApproverID = row["COC_APPROVEDBY"].ToString(),
                                COC_StartDate = Convert.ToDateTime(row["COC_STARTDATE"].ToString()),
                                COC_CompletionDate = Convert.ToDateTime(row["COC_COMPLETIONDATE"].ToString()),
                                POReferenceNo = row["COC_POREFERENCE"].ToString(),
                                COC_Status = row["COC_FORMSTATUS"].ToString(),
                                COC_Assessedby = row["COC_ASSESSEDBY"].ToString(),
                                COC_Resposible = row["RESPONSIBLE"].ToString(),

                            };
                        }
                        return clientmodel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Something wrong happened in the calculation module :", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public ClientModel EditCertificateOfCompletionDB(string id)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            ClientModel cocModel = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();

                    using (command = new SqlCommand("Select_GetCertificateOfCompletion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ControlNumber", id);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            cocModel = new ClientModel
                            {
                                NumberID = Convert.ToInt32(row["COC_NUMBER"].ToString()),
                                COC_ControlNo = row["COC_CONTROLNO"].ToString(),
                                ProjectName = row["COC_PROJECTTITLE"].ToString(),
                                COC_CompletionDate = Convert.ToDateTime(row["COC_COMPLETIONDATE"].ToString()),
                                COC_StartDate = Convert.ToDateTime(row["COC_STARTDATE"].ToString()),
                                ProjectDetails = row["COC_PROJECTDETAILS"].ToString(),
                                POReferenceNo = row["COC_POREFERENCE"].ToString(),
                                ApproverID = row["COC_APPROVEDBY"].ToString(),
                                ClientID = row["COC_CLIENTID"].ToString(),
                                COC_Resposible = row["COC_ASSESSEDBY"].ToString(),
                                POC_ReferenceNo = row["COC_REFERENCENO"].ToString(),
                                ClientLocation = row["COC_CLIENTLOCATION"].ToString(),
                                ClientAddress = row["COC_CLIENTADDRESS"].ToString(),


                            };
                        }
                        return cocModel;
                    }
                }
            }
            catch
            {
                return cocModel;

            }
            finally
            {
                connection.Close();
            }
        }

        public SupplierModel GetPayablesPaymentDetailsDB(string ID)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            SupplierModel model = null;

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_GetSupplierPaymentDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ACC_ID", ID);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        SqlDataReader sdr = command.ExecuteReader();
                        while (sdr.Read())
                        {
                            model = new SupplierModel()
                            {

                                BillingType = sdr["ACC_BILLINGTYPE"].ToString(),
                                DisplayACC_CheckDate = Convert.ToDateTime(sdr["ACC_CHECKDATE"].ToString()),
                                ACC_COCNO = sdr["ACC_COCNO"].ToString(),
                                ACC_DRNO = sdr["ACC_DRNO"].ToString(),
                                ACC_MRRNO = sdr["ACC_MRRNO"].ToString(),
                                ACC_ATICVNO = sdr["ACC_ATICVNO"].ToString(),
                                ACC_InvoiceNo = sdr["ACC_INVOICENO"].ToString(),
                                TypeOfTerms = sdr["ACC_TYPEOFTERMS"].ToString(),
                                ACC_Particular = sdr["ACC_PARTICULAR"].ToString(),
                                DisplayACC_LesswithTax = Convert.ToDecimal(sdr["ACC_LESSWITHTAX"].ToString()),
                                DisplayACC_AmountPaid = Convert.ToDecimal(sdr["ACC_AMOUNTPAID"].ToString()),
                                ACC_POSReference = sdr["ACC_REFERENCENO"].ToString(),
                                ACC_Ponumber = sdr["ACC_PONUMBER"].ToString(),
                                SupplierPayment = sdr["ACC_ID"].ToString(),
                                ACC_Discount = Convert.ToDecimal(sdr["DISCOUNT"].ToString()),


                            };
                        }
                        return model;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public ClientModel GetReceivablesBillingDetailsDB(string id)
        {
            DatabaseConnectionString();
            ClientModel model = null;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_GetClientBillingDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ACC_ID", id);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        SqlDataReader sdr = command.ExecuteReader();

                        while (sdr.Read())
                        {
                            model = new ClientModel()
                            {

                                PONumber = sdr["ACC_PONUMBER"].ToString(),
                                ClientBilling = sdr["ACCID"].ToString(),
                                ACC_BillNo = sdr["ACC_BILLNO"].ToString(),
                                DisplayACC_BillAmount = Convert.ToDecimal(sdr["ACC_BILLAMOUNT"].ToString()),
                                ORCRNO = sdr["ACC_OR_CRNO"].ToString(),
                                Typeofbilling = sdr["TYPEOFBILLING"].ToString(),
                                DisplayACC_BillDate = Convert.ToDateTime(sdr["ACC_BILLDATE"].ToString()),
                                ACC_Particular = sdr["ACC_PARTICULAR"].ToString(),
                                ACC_ReferenceNo = sdr["ACC_REFERENCENO"].ToString(),
                                ACC_TypeOfTerms = sdr["ACC_TYPEOFTERMS"].ToString(),
                                ACC_Discount = Convert.ToDecimal(sdr["DISCOUNT"].ToString()),
                                ACC_COCNO = sdr["COC_CONTROLNO"].ToString(),


                            };
                        }
                        return model;

                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public ClientModel GetCLientQuotationDB(string id)
        {
            DatabaseConnectionString();
            ClientModel model = null;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_EditClientQuotation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ControlNo", id);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        SqlDataReader sdr = command.ExecuteReader();

                        while (sdr.Read())
                        {
                            model = new ClientModel()
                            {

                                Quotation_ControlNo = sdr["QUOTATION_CONTROLNO"].ToString(),
                                ClientName = sdr["QUOTATION_CLIENTNAME"].ToString(),
                                ProjectName = sdr["QUOTATION_PROJECTNAME"].ToString(),
                                Discount = sdr["QUOTATION_DISCOUNT"].ToString(),
                                ClientID = sdr["QUOTATION_COMPANY"].ToString(),
                                ClientLocation = sdr["QUOTATION_LOCATION"].ToString(),
                                Quotation_ReferenceNo = sdr["QUOTATION_REFERENCENO"].ToString(),
                                ClientAddress = sdr["QUOTATION_ADDRESS"].ToString(),
                                ddlOthersPayment = sdr["QUOTATION_TERMS"].ToString(),
                                Submittedby_ID = sdr["QUOTATION_SUBMITTEDBY"].ToString(),
                                DisplayGrandTotal = Convert.ToDecimal(sdr["QUOTATION_GRANDTOTAL"].ToString()),
                                AmountVAT = sdr["QUOTATION_VAT"].ToString(),

                            };
                        }
                        return model;

                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public SupplierModel GetPurchaseRequisitionInformationDB(string id)
        {
            DatabaseConnectionString();
            SupplierModel model = null;
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_EditPurchaseRequisitionInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PR_REFERENCENO", id);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            model = new SupplierModel()
                            {
                                PRDateNeeded = Convert.ToDateTime(row["PR_DATENEEDED"].ToString()),
                                ProjectDescription = row["PR_PROJECTDESC"].ToString(),
                                PRNumber = row["PR_NUMBER"].ToString(),
                                PRReferenceNo = row["PR_REFERENCENO"].ToString(),
                                ReviewerID = row["PR_REVIEWER"].ToString(),
                                ApproverID = row["PR_APPROVER"].ToString(),
                                EndorserID = row["PR_ENDORSER"].ToString(),
                                FormStatus = row["PR_FORMSTATUS"].ToString(),
                                CanvessedBy = row["PR_CANVASSEDBY"].ToString(),
                                SupplierID_One = row["PR_SUPPLIERONE"].ToString(),
                                SupplierID_Two = row["PR_SUPPLIERTWO"].ToString(),
                                SupplierID_Three = row["PR_SUPPLIERTHREE"].ToString(),
                                FullNameResponsible = row["RESPONSIBLE"].ToString(),
                                FullName = row["PR_RESPONSIBLE"].ToString(),
                                DisplayGrandTotal_One = Convert.ToDecimal(row["PR_GRANDTOTALONE"].ToString()),
                                DisplayGrandTotal_Two = Convert.ToDecimal(row["PR_GRANDTOTALTWO"].ToString()),
                                DisplayGrandTotal_Three = Convert.ToDecimal(row["PR_GRANDTOTALTHREE"].ToString()),
                            };
                        }
                        return model;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }




        //UPDATE DATA
        public void UpdateUserInformationDB(UserMaintenance UserModel)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();

                    using (command = new SqlCommand("Update_UserInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@USERID", UserModel.UserID);
                        command.Parameters.AddWithValue("@EMPID", UserModel.EmployeeID);
                        command.Parameters.AddWithValue("@USERNAME", UserModel.UserName);
                        command.Parameters.AddWithValue("@FIRSTNAME", UserModel.FirstName);
                        command.Parameters.AddWithValue("@LASTNAME", UserModel.LastName);
                        command.Parameters.AddWithValue("@EMAIL", UserModel.EmailAddress);
                        command.Parameters.AddWithValue("@LOCATION", UserModel.Location);
                        command.Parameters.AddWithValue("@POSITION", UserModel.Position);
                        command.Parameters.AddWithValue("@USERGROUPID", UserModel.UserGroupID);
                        command.Parameters.AddWithValue("@USERROLEID", UserModel.UserRoleID);
                        command.Parameters.AddWithValue("@APPROVERID", UserModel.ApproverID);
                        command.Parameters.AddWithValue("@USERSTATUS", UserModel.UserStatus);
                        command.Parameters.AddWithValue("@UPDATEDATE", DateTime.Now.ToShortDateString());
                        if (UserModel.Img == null)
                        {
                            SqlParameter imageParameter = new SqlParameter("@SIGNATURE", SqlDbType.Image);
                            imageParameter.Value = DBNull.Value;
                            command.Parameters.Add(imageParameter);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@SIGNATURE", UserModel.Img);
                        }
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public int UpdateItemInformationDB(ItemMaster ItemModel)
        {
            DatabaseConnectionString();

            SqlConnection connection = null;
            SqlCommand command = null;
            int result = 0;
            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_ItemInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@ITEMCODE", ItemModel.ItemCode);
                        command.Parameters.AddWithValue("@ITEMID", ItemModel.ItemID);
                        command.Parameters.AddWithValue("@ITEMNAME", ItemModel.ItemName);
                        command.Parameters.AddWithValue("@ITEMCATEGORY", ItemModel.ItemCategory);
                        command.Parameters.AddWithValue("@ITEMDESCRIPTION", Convert.ToString(ItemModel.ItemDescription));
                        command.Parameters.AddWithValue("@ITEMMEASURE", ItemModel.ItemMeasure);
                        command.Parameters.AddWithValue("@ITEMSTATUS", ItemModel.ItemStatus);
                        command.Parameters.AddWithValue("@UPDATEDATE", DateTime.Now.ToShortDateString());
                        result++;
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                    return result;
                }
            }
            catch
            {
                return result = 0;
            }
        }

        public int UpdateSupplierInformationDB(SupplierMaster SupplierModel)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            int result = 0;
            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_SupplierInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SUPPLIERID", SupplierModel.SupplierID);
                        command.Parameters.AddWithValue("@SUPPLIER", SupplierModel.Supplier.Trim());
                        command.Parameters.AddWithValue("@SUPPLIERADDRESS", SupplierModel.SupplierAddress.Trim());
                        command.Parameters.AddWithValue("@EMAILADDRESS", SupplierModel.EmailAddress ?? "");
                        command.Parameters.AddWithValue("@CONTACTNO", SupplierModel.ContactNo);
                        command.Parameters.AddWithValue("@CONTACTPERSON", SupplierModel.ContactPerson);
                        command.Parameters.AddWithValue("@LOCATION", SupplierModel.Location);
                        command.Parameters.AddWithValue("@SUPPLIERSTATUS", SupplierModel.SupplierStatus);
                        command.Parameters.AddWithValue("@UPDATEDATE", DateTime.Now.ToShortDateString());
                        result++;
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
                return result;
            }
            catch
            {
                return result = 0;
            }
        }

        public int UpdateClientInformationDB(ClientMaster clientModel)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            int result = 0;
            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_ClientInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@CLIENTID", clientModel.ClientID);
                        command.Parameters.AddWithValue("@CLIENT", clientModel.Client);
                        command.Parameters.AddWithValue("@EMAILADDRESS", clientModel.EmailAddress ?? "");
                        command.Parameters.AddWithValue("@CONTACTNO", clientModel.ContactNo ?? "");
                        command.Parameters.AddWithValue("@CONTACTPERSON", clientModel.ContactPerson);
                        command.Parameters.AddWithValue("@CLIENTSTATUS", clientModel.ClientStatus ?? "Active");
                        command.Parameters.AddWithValue("@UPDATEDATE", DateTime.Now.ToShortDateString());
                        result++;
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
                return result;
            }
            catch
            {
                return result = 0;
            }
        }

        public int UpdateClientAddressinformationDB(List<ClientMaster> Details, ClientMaster ClientModel)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            int Result = 0;
            if (Details == null)
            {
                Details = new List<ClientMaster>();
            }

            try
            {
                using (connection = new SqlConnection(MasterList))
                {
                    connection.Open();
                    foreach (ClientMaster item in Details)
                    {
                        using (command = new SqlCommand("Update_ClientAddress", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@No", item.AddID);
                            command.Parameters.AddWithValue("@CLIENT", ClientModel.ClientID);
                            command.Parameters.AddWithValue("@CLIENT_LOCATION", item.Location);
                            command.Parameters.AddWithValue("@CLIENT_ADDRESS", item.ClientAddress);

                            Result++;
                            command.ExecuteNonQuery();
                        }
                    }
                    connection.Close();
                    return Result;
                }
            }
            catch
            {

                return Result = 0;
            }
        }

        public int UpdateClientTransactionStatusDB(string Status, string POCReferenceNO)
        {
            DatabaseConnectionString();
            int Result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_ClientStatusTransaction", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@POCReferenceNo", POCReferenceNO);
                        command.Parameters.AddWithValue("@Status", Status);
                        Result++;
                        command.ExecuteNonQuery();
                    }
                    return Result;
                }
            }
            catch
            {
                return Result = 0;
            }
            finally
            {
                connection.Close();
            }

        }

        public int UpdateSupplierTransactionStatusDB(string Status, string POSReferenceNO, string PONumber)
        {
            DatabaseConnectionString();
            int Result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_SupplierStatusTransaction", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@POSReferenceNo", POSReferenceNO);
                        command.Parameters.AddWithValue("@PONumber", PONumber);
                        command.Parameters.AddWithValue("@Status", Status);
                        Result++;
                        command.ExecuteNonQuery();
                    }
                    return Result;
                }
            }
            catch
            {
                return Result = 0;
            }
            finally
            {
                connection.Close();
            }

        }

        public int UpdatePurchaseRequisitionTransactionStatusDB(string Status, string PRReferenceNO)
        {
            DatabaseConnectionString();
            int Result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_PRStatusTransaction", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PRReferenceNo", PRReferenceNO);
                        command.Parameters.AddWithValue("@Status", Status);
                        Result++;
                        command.ExecuteNonQuery();
                    }
                    return Result;
                }
            }
            catch
            {
                return Result = 0;
            }
            finally
            {
                connection.Close();
            }

        }

        public int UpdateSupplierForRoutingApprovalDB(string POSReferenceNO, string OldStatus)
        {
            DatabaseConnectionString();
            int result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {

                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_ForRoutingApproval", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", POSReferenceNO);
                        command.Parameters.AddWithValue("@UpdateDate", DateTime.Now);
                        command.Parameters.AddWithValue("@OldStatus", OldStatus);

                        if (OldStatus == "FOR ENDORSEMENT")
                        {
                            command.Parameters.AddWithValue("@Status", "FOR APPROVAL");

                        }
                        else if (OldStatus == "FOR REVIEW")
                        {
                            command.Parameters.AddWithValue("@Status", "FOR ENDORSEMENT");

                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Status", "APPROVED");

                        }
                        result++;
                        command.ExecuteNonQuery();
                    }
                }

                return result;
            }
            catch
            {
                return result = 0;
            }
            finally
            {
                connection.Close();
            }
        }

        public int UpdatePR_ForRoutingApprovalDB(string POSReferenceNO, string OldStatus)
        {
            DatabaseConnectionString();
            int result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {

                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_PR_ForRoutingApproval", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", POSReferenceNO);
                        command.Parameters.AddWithValue("@UpdateDate", DateTime.Now);
                        command.Parameters.AddWithValue("@OldStatus", OldStatus);

                        if (OldStatus == "FOR ENDORSEMENT")
                        {
                            command.Parameters.AddWithValue("@Status", "FOR APPROVAL");

                        }
                        else if (OldStatus == "FOR REVIEW")
                        {
                            command.Parameters.AddWithValue("@Status", "FOR ENDORSEMENT");

                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Status", "APPROVED");

                        }
                        result++;
                        command.ExecuteNonQuery();
                    }
                }

                return result;
            }
            catch
            {
                return result = 0;
            }
            finally
            {
                connection.Close();
            }
        }

        public int UpdateSupplierRejectedDB(string POS, string RejectedBy, string RejectRemarks)
        {
            DatabaseConnectionString();
            int result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {

                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_ForRejected", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", POS);
                        command.Parameters.AddWithValue("@UpdateDate", DateTime.Now);
                        command.Parameters.AddWithValue("@RejectedBy", Convert.ToInt32(RejectedBy));
                        command.Parameters.AddWithValue("@Status", "REJECTED");
                        command.Parameters.AddWithValue("@Reject_Remark", RejectRemarks);
                        result++;
                        command.ExecuteNonQuery();
                    }
                }

                return result;
            }
            catch
            {
                return result = 0;
            }
            finally
            {
                connection.Close();
            }
        }

        public int UpdatePRRejectedDB(string PR, string RejectedBy, string RejectRemarks)
        {
            DatabaseConnectionString();
            int result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {

                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_PRForRejected", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", PR);
                        command.Parameters.AddWithValue("@UpdateDate", DateTime.Now);
                        command.Parameters.AddWithValue("@RejectedBy", Convert.ToInt32(RejectedBy));
                        command.Parameters.AddWithValue("@Status", "REJECTED");
                        command.Parameters.AddWithValue("@Reject_Remark", RejectRemarks);
                        result++;
                        command.ExecuteNonQuery();
                    }
                }

                return result;
            }
            catch
            {
                return result = 0;
            }
            finally
            {
                connection.Close();
            }
        }

        public int UpdateUserClientInformationDB(string Old_ReferenceNo)
        {
            DatabaseConnectionString();
            int result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_UserClientInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@OldReferenceNo", Old_ReferenceNo);
                        command.Parameters.AddWithValue("@NewReferenceNo", POC_ReferenceNo);
                        command.Parameters.AddWithValue("@UpdateDate", DateTime.Now);
                        result++;
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                    return result;
                }

            }
            catch
            {
                throw;
                return result = 0;
            }
        }

        public int UpdateUserSupplierInformationDB(string Old_ReferenceNo)
        {
            DatabaseConnectionString();
            int result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_UserSupplierInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@OldReferenceNo", Old_ReferenceNo);
                        command.Parameters.AddWithValue("@NewReferenceNo", POS_ReferenceNo);
                        command.Parameters.AddWithValue("@UpdateDate", DateTime.Now);
                        result++;
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                    return result;
                }
            }
            catch
            {
                return result = 0;
            }
        }

        public int UpdateItemReceivedInformationDB(List<SupplierModel> SupplierDetails, SupplierModel supplierModel)
        {
            DatabaseConnectionString();
            int result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            if (SupplierDetails == null)
            {
                SupplierDetails = new List<SupplierModel>();
            }
            try
            {
                using (connection = new SqlConnection(DrModule))
                {
                    connection.Open();
                    foreach (SupplierModel item in SupplierDetails)
                    {
                        using (command = new SqlCommand("Update_ItemReceivedInformation", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            if (!string.IsNullOrWhiteSpace(item.ItemCount))
                            {
                                command.Parameters.AddWithValue("@ItemID", item.ItemID);
                                command.Parameters.AddWithValue("@Collected", item.ItemCount);
                                result++;
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    return result;
                }
            }
            catch
            {
                return result = 0;
            }
            finally
            {
                connection.Close();
            }

        }

        public int UpdateItemReleasedInformationDB(List<ClientModel> SupplierDetails, ClientModel supplierModel)
        {
            DatabaseConnectionString();
            int result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            if (SupplierDetails == null)
            {
                SupplierDetails = new List<ClientModel>();
            }
            try
            {
                using (connection = new SqlConnection(DrModule))
                {
                    connection.Open();
                    foreach (ClientModel item in SupplierDetails)
                    {
                        using (command = new SqlCommand("Update_ItemReleasedInformation", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            if (!string.IsNullOrWhiteSpace(item.ItemCount))
                            {
                                command.Parameters.AddWithValue("@ItemID", item.ItemID);
                                command.Parameters.AddWithValue("@Collected", item.ItemCount);
                                result++;
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    return result;
                }
            }
            catch
            {
                return result = 0;
            }
            finally
            {
                connection.Close();
            }

        }

        public string UpdateEditItemRecievedDB(DRModel drmodel)
        {
            DatabaseConnectionString();
            string result = "";
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(DrModule))
                {
                    using (command = new SqlCommand("Update_EditItemReceived", connection))
                    {
                        connection.Open();
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@DR_RECEIVEID", Convert.ToInt32(drmodel.DRID));
                        command.Parameters.AddWithValue("@REFERENCENO", drmodel.DRReferenceNo);
                        command.Parameters.AddWithValue("@DR_COLLECTED", Convert.ToInt32(drmodel.DRItemReturn));
                        command.Parameters.AddWithValue("@DR_REASONREMARKS", drmodel.DRReasonRemarks);
                        command.Parameters.AddWithValue("@ITEMID", Convert.ToInt32(drmodel.DRItemID));


                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    result = row["Error1"].ToString();
                                }
                            }
                        }




                        return result;
                    }
                }
            }
            catch
            {
                return result;
            }
            finally
            {
                connection.Close();
            }
        }

        public string UpdateEditItemReleasedDB(DRModel drmodel)
        {
            DatabaseConnectionString();
            string result = "";
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(DrModule))
                {
                    using (command = new SqlCommand("Update_EditItemReleased", connection))
                    {
                        connection.Open();
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@DR_CONTROLNO", drmodel.DRNumber);
                        command.Parameters.AddWithValue("@REFERENCENO", drmodel.DRReferenceNo);
                        command.Parameters.AddWithValue("@DR_ITEMRETURN", Convert.ToInt32(drmodel.DRItemReturn));
                        command.Parameters.AddWithValue("@DR_REASONREMARKS", drmodel.DRReasonRemarks);
                        command.Parameters.AddWithValue("@ITEMID", Convert.ToInt32(drmodel.DRItemID));


                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    result = row["Error1"].ToString();
                                }
                            }
                        }




                        return result;
                    }
                }
            }
            catch
            {
                return result;
            }
            finally
            {
                connection.Close();
            }
        }

        public int UpdateEditReceivablesDB(AccountingModel accountmodel)
        {
            DatabaseConnectionString();
            int result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_ReceivablesInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@ACCID", Convert.ToInt32(accountmodel.ACC_ID));
                        command.Parameters.AddWithValue("@ACC_OR_CRNO", accountmodel.ORCRNO ?? "");
                        command.Parameters.AddWithValue("@ACC_DATEPAID", Convert.ToDateTime(accountmodel.ACC_DatePaid));
                        command.Parameters.AddWithValue("@BILLTYPE", accountmodel.ACC_BillType);
                        command.Parameters.AddWithValue("@ACC_BILLNO", accountmodel.ACC_BillNo);
                        command.Parameters.AddWithValue("@ACC_BILLAMOUNT", Convert.ToDecimal(accountmodel.REC_Amount));
                        command.Parameters.AddWithValue("@ACC_PARTICULAR", accountmodel.ACC_Particular);
                        command.Parameters.AddWithValue("@ACC_BILLDATE", Convert.ToDateTime(accountmodel.ACC_BillDate));
                        command.Parameters.AddWithValue("@ACC_REMARKS", accountmodel.ACC_Remarks);
                        command.Parameters.AddWithValue("@ACC_RESPONSIBLE", accountmodel.ACC_Responsible);
                        command.Parameters.AddWithValue("@ACC_REFERENCENO", accountmodel.ACC_ReferenceNo);


                        result++;
                        command.ExecuteNonQuery();
                        return result;
                    }
                }
            }
            catch
            {
                return result = 0;
            }
            finally
            {
                connection.Close();
            }
        }

        public int UpdateEditPayablesDB(AccountingModel accountmodel)
        {
            DatabaseConnectionString();
            int result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_PayablesInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@ACC_ID", Convert.ToInt32(accountmodel.ACC_ID));
                        command.Parameters.AddWithValue("@ACC_PARTICULAR", accountmodel.ACC_Particular);
                        command.Parameters.AddWithValue("@ACC_AMOUNTPAID", Convert.ToDecimal(accountmodel.REL_Amount));
                        command.Parameters.AddWithValue("@ACC_CHECKDATE", Convert.ToDateTime(accountmodel.ACC_CheckDate));
                        command.Parameters.AddWithValue("@ACC_DATECOLLECTED", Convert.ToDateTime(accountmodel.ACC_DateCollected));
                        command.Parameters.AddWithValue("@ACC_INVOICENO", accountmodel.ACC_InvoiceNo);
                        command.Parameters.AddWithValue("@ACC_ATICVNO", accountmodel.ACC_ATICVNO);
                        command.Parameters.AddWithValue("@ACC_REMARKS", accountmodel.ACC_Remarks);
                        command.Parameters.AddWithValue("@RESPONSIBLE", accountmodel.ACC_Responsible);
                        command.Parameters.AddWithValue("@ACC_LESSWITHTAX", Convert.ToDecimal(accountmodel.REL_Tax));
                        command.Parameters.AddWithValue("@ACC_REFERENCENO", accountmodel.ACC_ReferenceNo);

                        result++;
                        InsertAccountingDocumentsDB(accountmodel.ACC_ReferenceNo, accountmodel.ACC_DRNO, accountmodel.ACC_MRRNO);
                        command.ExecuteNonQuery();
                        return result;
                    }
                }
            }
            catch
            {
                return result = 0;
            }
            finally
            {
                connection.Close();
            }
        }

        public int UpdateCertificateOfCompletionDB(string ControlNo)
        {
            DatabaseConnectionString();
            int Result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_CertificateOfCompletion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ControlNo", ControlNo);
                        Result++;
                        command.ExecuteNonQuery();
                        return Result;
                    }
                }
            }
            catch (Exception ex)
            {
                Result = 0;
                throw new ApplicationException("Something wrong happened in the calculation module :", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public int UpdateRejectCertificateOfCompletionDB(string ControlNo, string RejectRemarks, string UserID)
        {
            DatabaseConnectionString();
            int Result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_RejectCertificateOfCompletion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ControlNo", ControlNo);
                        command.Parameters.AddWithValue("@UserID", Convert.ToInt32(UserID));
                        command.Parameters.AddWithValue("@RejectRemarks", RejectRemarks);
                        Result++;
                        command.ExecuteNonQuery();
                        return Result;
                    }
                }
            }
            catch (Exception ex)
            {
                Result = 0;
                throw new ApplicationException("Something wrong happened in the calculation module :", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public int UpdateEditCertificateOfCompletion(ClientModel clientModel)
        {
            DatabaseConnectionString();
            int result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_EditCertificateOfCompletion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ControlNo", clientModel.NumberID);
                        command.Parameters.AddWithValue("@NewControlNo", CoC_Number);
                        result++;
                        command.ExecuteNonQuery();
                        return result;
                    }
                }
            }
            catch
            {
                result = 0;
                DeleteCertificateOfCompletionDB();
                return result;

            }
            finally
            {
                connection.Close();
            }
        }

        public int UpdateSupplierPaymentDB(SupplierModel accountingmodel)
        {
            DatabaseConnectionString();
            int i = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_SupplierPayment", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ACC_DATECOLLECTED", accountingmodel.ACC_DateCollected);
                        command.Parameters.AddWithValue("@MRRNO", accountingmodel.ACC_MRRNO);
                        command.Parameters.AddWithValue("@ACC_POSREFERENCENO", accountingmodel.ACC_POSReference);
                        command.Parameters.AddWithValue("@ACC_ID", accountingmodel.SupplierPayment);
                        command.Parameters.AddWithValue("@ACC_Responsible", accountingmodel.ACC_Responsible);

                        i++;
                        command.ExecuteNonQuery();
                    }
                    return i;
                }
            }
            catch
            {
                i = 0;
                throw;
            }
            finally
            {
                connection.Close();

            }
        }

        public int UpdateClientBillingDB(ClientModel accountingmodel)
        {
            DatabaseConnectionString();
            int i = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(Accounting))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_ClientBilling", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ACC_DATECOLLECTED", accountingmodel.ACC_DatePaid);
                        command.Parameters.AddWithValue("@ACC_ORCRNO", accountingmodel.ORCRNO);
                        command.Parameters.AddWithValue("@ACC_REFERENCENO", accountingmodel.ACC_ReferenceNo);
                        command.Parameters.AddWithValue("@ACC_ID", accountingmodel.ClientBilling);
                        command.Parameters.AddWithValue("@ACC_Responsible", accountingmodel.ACC_Responsible);

                        i++;
                        command.ExecuteNonQuery();
                    }
                    return i;
                }
            }
            catch
            {
                i = 0;
                throw;
            }
            finally
            {
                connection.Close();

            }
        }

        public int UpdateClientQuotationDB(string OldQuotation_ReferenceNo)
        {
            DatabaseConnectionString();
            int i = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_ClientQuotation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@OldReferenceNo", OldQuotation_ReferenceNo);
                        command.Parameters.AddWithValue("@NewReferenceNo", Quotation_ReferenceNo);

                        i++;
                        command.ExecuteNonQuery();
                    }
                    return i;
                }
            }
            catch
            {
                i = 0;
                throw;
            }
            finally
            {
                connection.Close();

            }
        }

        public int UpdateVerificationDate(string ID)
        {
            DatabaseConnectionString();
            int i = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_VerificationDate", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", ID);

                        i++;
                        command.ExecuteNonQuery();
                    }
                    return i;
                }
            }
            catch
            {
                i = 0;
                throw;
            }
            finally
            {
                connection.Close();

            }
        }

        public int RejectPRRequestDB(string Old, string New)
        {
            DatabaseConnectionString();
            int i = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_PurchaseRequisition", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@OldReferenceNo", Old);
                        command.Parameters.AddWithValue("@NewReferenceNo", New);

                        command.ExecuteNonQuery();
                        i++;
                        return i;

                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }



        //USER SESSION DATA
        public string PopulateUserSessionDB(string username, string password)
        {
            
            DatabaseConnectionString();
               SqlConnection connection = null;
            SqlCommand command = null;
            string UserID = "";
            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_UserLogin", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserName", username);
                        command.Parameters.AddWithValue("@Password", password);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    UserID = row["USERID"].ToString();
                                }
                            }
                        }

                    }
                    connection.Close();
                    return UserID;
                }
            }
            catch
            {
                return UserID;
            }
        }

        public int UpdateNewPasswordDB(LoginModel loginModel)
        {
            DatabaseConnectionString();
            int count = 0;

            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_NewPassword", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Username", loginModel.UserName);
                        command.Parameters.AddWithValue("@NewPassword", loginModel.NewPassword);

                        count++;
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return count;
            }
            catch
            {
                count = 0;
                return count;
            }
        }

        public int ResetPasswordDB(string EmployeeID, string UserName)
        {
            DatabaseConnectionString();
            int Result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_UserResetPassword", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@EMPID", EmployeeID);
                        command.Parameters.AddWithValue("@USERNAME", UserName);
                        Result++;
                        command.ExecuteNonQuery();

                        connection.Close();
                        return Result;
                    }
                }

            }
            catch
            {
                return Result = 0;
            }
        }

        public int InsertAuditTrailDB(string UserName, string Remarks, string IP, string ComputerName)
        {
            DatabaseConnectionString();
            int Result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Insert_AuditTrail", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Audit_UserName", UserName);
                        command.Parameters.AddWithValue("@IPAddress", IP);
                        command.Parameters.AddWithValue("@ComputerName", ComputerName);
                        command.Parameters.AddWithValue("@Audit_Remarks", Remarks);
                        Result++;
                        command.ExecuteNonQuery();
                    }
                    return Result;
                }
            }
            catch
            {
                return Result = 0;

            }
            finally
            {
                connection.Close();
            }
        }

        public int InsertActionAuditTrailDB(string UserName, string Remarks, string Module)
        {
            DatabaseConnectionString();
            int Result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Insert_ActionAuditTrail", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserName", UserName);
                        command.Parameters.AddWithValue("@Audit_Message", Remarks);
                        command.Parameters.AddWithValue("@Module", Module);

                        Result++;
                        command.ExecuteNonQuery();
                    }
                    return Result;
                }
            }
            catch
            {
                return Result = 0;

            }
            finally
            {
                connection.Close();
            }
        }


        //SMTP DATABASE

        public int InsertSMTPInformationDB(SMTPModel ServerModel)
        {
            DatabaseConnectionString();
            int Result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Insert_SMTPInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SMTP_SERVER", ServerModel.SMTP_Server);
                        command.Parameters.AddWithValue("@SMTP_PORT", ServerModel.SMTP_Port);
                        command.Parameters.AddWithValue("@SMTP_STATUS", ServerModel.SMTP_Status);
                        command.Parameters.AddWithValue("@SMTP_EMAIL", ServerModel.SMTP_Email);
                        command.Parameters.AddWithValue("@SMTP_PASSWORD", ServerModel.SMTP_Password);
                        command.Parameters.AddWithValue("@SMTP_ENTRYDATE", DateTime.Now);
                        Result++;
                        command.ExecuteNonQuery();
                        connection.Close();
                        return Result;

                    }
                }
            }
            catch
            {
                return Result = 0;
            }
        }

        public int UpdateSMTPInformationDB(SMTPModel ServerModel)
        {
            DatabaseConnectionString();
            int Result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_SMTPInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SMTP_SERVER", ServerModel.SMTP_Server);
                        command.Parameters.AddWithValue("@SMTP_PORT", ServerModel.SMTP_Port);
                        command.Parameters.AddWithValue("@SMTP_STATUS", ServerModel.SMTP_Status);
                        command.Parameters.AddWithValue("@SMTP_EMAIL", ServerModel.SMTP_Email);
                        command.Parameters.AddWithValue("@SMTP_PASSWORD", ServerModel.SMTP_Password);
                        command.Parameters.AddWithValue("@SMTP_UPDATEDATE", DateTime.Now);
                        command.Parameters.AddWithValue("@SMTP_ID", ServerModel.SMTPID);
                        Result++;
                        command.ExecuteNonQuery();
                        connection.Close();
                        return Result;

                    }
                }
            }
            catch
            {
                return Result = 0;
            }
        }

        public SMTPModel PopulateSMTPInformation()
        {
            DatabaseConnectionString();
            SMTPModel ServerModel = null;
            SqlConnection Connection = null;
            SqlCommand command = null;

            try
            {
                using (Connection = new SqlConnection(Security))
                {
                    using (command = new SqlCommand("Select_PopulateSMTPInformation", Connection))
                    {
                        Connection.Open();
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    ServerModel = new SMTPModel
                                    {

                                        SMTP_Password = row["SMTP_PASSWORD"].ToString(),
                                        SMTP_ConfirmPassword = row["SMTP_PASSWORD"].ToString(),
                                        SMTP_Email = row["SMTP_EMAILADDRESS"].ToString(),
                                        SMTP_Server = row["SMTP_SERVER"].ToString(),
                                        SMTP_Port = row["SMTP_PORT"].ToString(),
                                        SMTP_Status = row["SMTP_STATUS"].ToString(),
                                        SMTPID = Convert.ToInt32(row["SMTPID"].ToString()),
                                    };

                                }
                                Connection.Close();
                                return ServerModel;
                            }
                        }
                    }
                }
            }
            catch
            {
                return ServerModel;
            }
        }

        public string EmailNotificationDB(string USERID)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            string result = "";

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_EmailNotification", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@USERID", Convert.ToInt32(USERID));

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    result = row["EMAIL"].ToString();
                                }
                            }

                            return result;

                        }

                    }
                }
            }
            catch
            {
                return result = "";
            }
            finally
            {
                connection.Close();
            }
        }

        //SUPPLIER NOTIFICATION

        public int PopulateCountSupplierNotificationDB(int id)
        {
            DatabaseConnectionString();
            int result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    using (command = new SqlCommand("Select_NewCountSupplierNotification", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@USERID", id);
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }

                        using (SqlDataReader reader = command.ExecuteReader())

                            while (reader.Read())
                            {
                                result = Convert.ToInt32(reader["TOTALNOTIFICATION"].ToString());
                            }
                    }
                    return result;
                }
            }
            catch
            {
                return result = 0;

            }
            finally
            {
                connection.Close();
            }




        }

        public List<MainModel> PopulateSystemNotifaction(int id)
        {
            DatabaseConnectionString();
            List<MainModel> ListNotification = new List<MainModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    using (command = new SqlCommand("SystemNotification", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@USERID", id);
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            MainModel supplier = new MainModel
                            {
                                NotificationReferenceNo = row["REFERENCENO"].ToString(),
                                NotificationProjectTitle = row["PROJECTNAME"].ToString(),
                                NotificationFormStatus = row["FORMSTATUS"].ToString(),
                                NotificationNumber = row["NUMBER"].ToString(),
                                NotificationDate = row["ENTRYDATE"].ToString(),
                                NotificationType = row["TYPESS"].ToString(),
                                NotificationRequestedType = row["REQUESTEDTYPESS"].ToString(),
                            };
                            ListNotification.Add(supplier);
                        }
                        return ListNotification;
                    }
                }
            }
            catch
            {
                return ListNotification;

            }
            finally
            {
                connection.Close();
            }
        }

        public List<MainModel> PopulateSystemRequestedNotifaction(int id)
        {
            DatabaseConnectionString();
            List<MainModel> ListNotification = new List<MainModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    using (command = new SqlCommand("SystemRequesterNotification", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@USERID", id);
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            MainModel supplier = new MainModel
                            {
                                NotificationReferenceNo = row["REFERENCENO"].ToString(),
                                NotificationProjectTitle = row["PROJECTNAME"].ToString(),
                                NotificationFormStatus = row["FORMSTATUS"].ToString(),
                                NotificationNumber = row["NUMBER"].ToString(),
                                NotificationDate = row["ENTRYDATE"].ToString(),
                                NotificationType = row["TYPESS"].ToString(),
                                NotificationRequestedType = row["REQUESTEDTYPESS"].ToString(),
                            };
                            ListNotification.Add(supplier);
                        }
                        return ListNotification;
                    }
                }
            }
            catch
            {
                return ListNotification;

            }
            finally
            {
                connection.Close();
            }
        }

        public List<MainModel> PopulateSupplierNotifaction(int id)
        {
            DatabaseConnectionString();
            List<MainModel> ListNotification = new List<MainModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    using (command = new SqlCommand("Select_SupplierNotification", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@USERID", id);
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            MainModel supplier = new MainModel
                            {
                                NotificationNo = row["SUPPLIER_REFERENCENO"].ToString(),
                                NotificationSupplier = row["SUPPLIER"].ToString(),
                                NotificationSupplierDate = row["ENTRYDATE"].ToString(),
                                NotificationStatus = row["FORMSTATUS"].ToString(),
                                NotificationPONumber = row["SUPPLIER_PONUMBER"].ToString(),
                                NotificationPOS = row["SUPPLIER_NOTIFICATION"].ToString(),
                            };
                            ListNotification.Add(supplier);
                        }
                        return ListNotification;
                    }
                }
            }
            catch
            {
                return ListNotification;

            }
            finally
            {
                connection.Close();
            }
        }

        public List<MainModel> PopulateRequestedSupplierNotifaction(int id)
        {
            DatabaseConnectionString();
            List<MainModel> ListNotification = new List<MainModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    using (command = new SqlCommand("Select_RequestedSupplierNotification", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@USERID", id);
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            MainModel supplier = new MainModel
                            {
                                NotificationNo = row["SUPPLIER_REFERENCENO"].ToString(),
                                NotificationSupplier = row["SUPPLIER"].ToString(),
                                NotificationSupplierDate = row["ENTRYDATE"].ToString(),
                                NotificationStatus = row["FORMSTATUS"].ToString(),
                                NotificationPONumber = row["SUPPLIER_PONUMBER"].ToString(),
                                NotificationPOS = row["SUPPLIER_NOTIFICATION"].ToString(),
                            };
                            ListNotification.Add(supplier);
                        }
                        return ListNotification;
                    }
                }
            }
            catch
            {
                return ListNotification;

            }
            finally
            {
                connection.Close();
            }
        }

        public List<MainModel> PopulateCertificateOfCompletionNotificationDB(int id)
        {
            DatabaseConnectionString();
            List<MainModel> CoCList = new List<MainModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_CertificateOfCompletionNotification", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@userID", id);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();

                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            MainModel clientmodel = new MainModel
                            {
                                NotificationControlNo = row["COC_CONTROLNO"].ToString(),
                                //NotificationProjectTitle = row["COC_PROJECTTITLE"].ToString(),
                                NotificationClient = row["CLIENT"].ToString(),
                                NotificationCoCStatus = row["COC_FORMSTATUS"].ToString(),
                                NotificationCoCDate = row["ENTRYDATE"].ToString(),
                                NotificationCOC = row["COC_NOTIFICATION"].ToString(),
                            };
                            CoCList.Add(clientmodel);

                        }
                        return CoCList;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Something wrong happened in the calculation module :", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public List<MainModel> PopulateRequestedCertificateOfCompletionNotificationDB(int id)
        {
            DatabaseConnectionString();
            List<MainModel> CoCList = new List<MainModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_RequestedCertificateOfCompletionNotification", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@userID", id);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();

                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            MainModel clientmodel = new MainModel
                            {
                                NotificationControlNo = row["COC_CONTROLNO"].ToString(),
                                NotificationProjectTitle = row["COC_PROJECTTITLE"].ToString(),
                                NotificationClient = row["CLIENT"].ToString(),
                                NotificationCoCStatus = row["COC_FORMSTATUS"].ToString(),
                                NotificationCoCDate = row["ENTRYDATE"].ToString(),
                                NotificationCOC = row["COC_NOTIFICATION"].ToString(),
                            };
                            CoCList.Add(clientmodel);

                        }
                        return CoCList;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Something wrong happened in the calculation module :", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public int UpdateViewedFormSupplier(string id)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            int result = 0;
            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_SupplierFormViewed", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", @id);
                        result++;
                        command.ExecuteNonQuery();
                        return result;
                    }
                }
            }
            catch
            {
                return result = 0;
            }
            finally
            {
                connection.Close();
            }
        }

        public int UpdateViewedFormPR(string id)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            int result = 0;
            try
            {
                using (connection = new SqlConnection(PotoSupplier))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_PRFormViewed", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", @id);
                        result++;
                        command.ExecuteNonQuery();
                        return result;
                    }
                }
            }
            catch
            {
                return result = 0;
            }
            finally
            {
                connection.Close();
            }
        }

        public int UpdateViewedFormCOC(string id)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            int result = 0;
            try
            {
                using (connection = new SqlConnection(PobyClient))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_COCFormViewed", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", @id);
                        result++;
                        command.ExecuteNonQuery();
                        return result;
                    }
                }
            }
            catch
            {
                return result = 0;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<ModuleAccess> PopulateMainPage()
        {
            DatabaseConnectionString();

            SqlConnection connection = null;
            SqlCommand command = null;
            List<ModuleAccess> listMain = new List<ModuleAccess>();
            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("TreeView_PopulateMainPage", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ModuleAccess access = new ModuleAccess()
                            {
                                MainPage = row["MainModule"].ToString(),
                                MainID = row["MainID"].ToString(),
                            };
                            listMain.Add(access);

                        }
                        return listMain;
                    }
                }

            }
            catch
            {
                return listMain;
            }
        }

        public List<ModuleAccess> PopulateSubPage()
        {
            DatabaseConnectionString();

            SqlConnection connection = null;
            SqlCommand command = null;
            List<ModuleAccess> listMain = new List<ModuleAccess>();
            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("TreeView_PopulateSubPage", connection))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ModuleAccess access = new ModuleAccess()
                            {
                                SubPage = row["SUBNAME"].ToString(),
                                SubPageID = row["SUBKEY"].ToString(),
                                SubMainID = row["MAINID"].ToString(),
                            };
                            listMain.Add(access);

                        }
                        return listMain;
                    }
                }

            }
            catch
            {
                return listMain;
            }
        }

        public int CheckAccessIfExistingDB(UserMaintenance user)
        {
            DatabaseConnectionString();
            int Result = 0;
            SqlConnection connection = null;
            SqlCommand command = null;
            var checker = "";
            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();


                    using (command = new SqlCommand("TreeView_CheckIfExistAccess", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserRoleID", user.UserRoleID);
                        command.Parameters.AddWithValue("@UserGroupID", user.DepartmentID);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();

                        sda.Fill(dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            checker = row["Counts"].ToString();
                            Result++;
                        }
                    }

                    return Result;
                }
            }
            catch
            {
                return Result = 0;
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public void DeleteModuleAccess(UserMaintenance user)
        {
            DatabaseConnectionString();
            using (SqlConnection connection = new SqlConnection(Security))
            {
                using (SqlCommand command = new SqlCommand("TreeView_DeleteModuleAccess", connection))
                {
                    connection.Open();

                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@UserRoleID", user.UserRoleID);
                    command.Parameters.AddWithValue("@UserGroupID", user.DepartmentID);

                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public int InsertModuleAccess(ModuleAccess module, UserMaintenance user)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            int v = 0;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    DeleteModuleAccess(user);
                    foreach (var item in module.AllModuleAccess)
                    {
                        using (command = new SqlCommand("TreeView_InsertModuleAccess", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@UserRoleID", user.UserRoleID);
                            command.Parameters.AddWithValue("@UserGroupID", user.DepartmentID);
                            command.Parameters.AddWithValue("@Module", item.selectedID);
                            command.ExecuteNonQuery();
                            v++;
                        }

                    }
                    connection.Close();

                    return v;

                }
            }
            catch
            {
                throw;
            }


        }

        public List<UserMaintenance> PopulateDept_UserRole()
        {
            DatabaseConnectionString();

            List<UserMaintenance> listRole = new List<UserMaintenance>();

            using (SqlConnection connection = new SqlConnection(Security))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("TreeView_PopulateDeptUserRole", connection))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        UserMaintenance user = new UserMaintenance()
                        {
                            UserRole = row["USERROLENAME"].ToString(),
                            UserRoleID = Convert.ToInt32(row["USERROLEID"].ToString()),
                            UserGroupID = Convert.ToInt32(row["USERGROUPID"].ToString()),
                            UserGroup = row["USERGROUPNAME"].ToString(),

                        };
                        listRole.Add(user);
                    }
                    connection.Close();

                    return listRole;
                }
            }
        }

        public List<ModuleAccess> PopulateEditMainPage(UserMaintenance user)
        {
            DatabaseConnectionString();

            SqlConnection connection = null;
            SqlCommand command = null;
            List<ModuleAccess> listMain = new List<ModuleAccess>();
            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("TreeView_EditMainModuleAccess", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserRoleID", user.UserRoleID);
                        command.Parameters.AddWithValue("@UserGroupID", user.DepartmentID);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ModuleAccess access = new ModuleAccess()
                            {
                                MainPage = row["MainModule"].ToString(),
                                xMainID = row["MainID"].ToString(),
                            };
                            listMain.Add(access);

                        }
                        return listMain;
                    }
                }

            }
            catch
            {
                throw;
                return listMain;
            }
        }

        public List<ModuleAccess> PopulateEditSubPage(UserMaintenance user)
        {
            DatabaseConnectionString();

            SqlConnection connection = null;
            SqlCommand command = null;
            List<ModuleAccess> listMain = new List<ModuleAccess>();
            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("TreeView_EditSubModuleAccess", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserRoleID", user.UserRoleID);
                        command.Parameters.AddWithValue("@UserGroupID", user.DepartmentID);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            ModuleAccess access = new ModuleAccess()
                            {
                                SubPage = row["SUBNAME"].ToString(),
                                xSubPageID = row["SUBKEY"].ToString(),
                                SubMainID = row["MAINID"].ToString(),
                            };
                            listMain.Add(access);

                        }
                        return listMain;
                    }
                }

            }
            catch
            {
                return listMain;
            }
        }

        public List<string> PopulateModuleAccess(string UserRoleID, string UserGroupID)
        {
            DatabaseConnectionString();
            List<string> list = new List<string>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("TreeView_SystemModuleAccess", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserRoleID", UserRoleID);
                        command.Parameters.AddWithValue("@UserGroupID", UserGroupID);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);


                        foreach (DataRow row in dt.Rows)
                        {
                            var xx = row["SUBKEY"].ToString();

                            list.Add(xx);
                        }
                        return list;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

        }

        public int CheckVerificationDate(int ID)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            int i = 0;

            try
            {
                using (connection = new SqlConnection(Security))
                {

                    connection.Open();

                    using (command = new SqlCommand("Update_VerificationCode", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", ID);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        SqlDataReader sdr = command.ExecuteReader();
                        while (sdr.Read())
                        {
                            sdr["VERIFICATIONCODE"].ToString();
                            i++;
                        }
                        return i;


                    }
                }
            }
            catch
            {
                return i = 0;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<MainModel> PopulateBackupVerificationCode(string UserID)
        {
            DatabaseConnectionString();
            List<MainModel> listCodes = new List<MainModel>();
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_Backup_VerificationCode", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", UserID);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            MainModel model = new MainModel()
                            {
                                VerificationCode = row["VVERIFICATIONCODE"].ToString(),
                                VerificationUsed = row["VUSED"].ToString(),
                            };
                            listCodes.Add(model);
                        }
                        return listCodes;

                    }
                }
            }
            catch
            {
                throw;

            }
            finally
            {
                connection.Close();
            }

        }

        public int InsertBackupVerificationCode(List<MainModel> BackUpCodes, string UserID)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            int c = 0;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    foreach (var item in BackUpCodes)
                    {
                        using (command = new SqlCommand("Insert_Backup_VerificationCode", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@UserID", UserID);
                            command.Parameters.AddWithValue("@VerificationCode", item.VerificationCode);
                            command.ExecuteNonQuery();
                        }
                    }
                    c++;
                    return c;

                }
            }
            catch
            {
                c = 0;
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public int DeleteBackupVerificationCode(string UserID)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            int c = 0;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Delete_Backup_VerificationCode", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", UserID);
                        command.ExecuteNonQuery();
                    }
                    c++;
                    return c;

                }
            }
            catch
            {
                c = 0;
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public int UpdateBackupVerificationCode(string VerificationCode, string UserID)
        {
            DatabaseConnectionString();
            SqlConnection connection = null;
            SqlCommand command = null;
            int c = 0;

            try
            {
                using (connection = new SqlConnection(Security))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_Backup_VerificationCode", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", UserID);
                        command.Parameters.AddWithValue("@VerificationCode", VerificationCode);
                        command.ExecuteNonQuery();
                    }
                    c++;
                    return c;

                }
            }
            catch
            {
                c = 0;
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public int update(string username,string Status)
        {
            DatabaseConnectionString();
            Guid guid = Guid.NewGuid();
            String uniqueUserKey = Convert.ToString(guid).Replace("-", "").Substring(0, 10);
            string AuthenticationCode = uniqueUserKey;
            int i = 0;
            Dictionary<String, String> result = new Dictionary<String, String>();
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            var setupInfo = tfa.GenerateSetupCode("ATI Procurement System", username, AuthenticationCode, 300, 300);
            string AuthenticationBarCodeImage = setupInfo.QrCodeSetupImageUrl;
            string AuthenticationManualCode = setupInfo.ManualEntryKey;

            using (SqlConnection Scon = new SqlConnection(Security))
            {
                Scon.Open();
                using (SqlCommand Scommand = new SqlCommand("Update_Authenticator", Scon))
                {
                    Scommand.CommandType = CommandType.StoredProcedure;
                    Scommand.Parameters.AddWithValue("@SecretKey", AuthenticationCode);
                    Scommand.Parameters.AddWithValue("@SecretUrl", AuthenticationBarCodeImage);
                    Scommand.Parameters.AddWithValue("@ManualCode", AuthenticationManualCode);
                    Scommand.Parameters.AddWithValue("@USERNAME", username);
                    Scommand.Parameters.AddWithValue("@Status", Status);
                    Scommand.ExecuteNonQuery();
                    i++;
                }
                Scon.Close();
                return i;
            }
        }

        public int EnableDisableAuthenticatorDB(string username, string Status)
        {
            DatabaseConnectionString();

            int i = 0;
            using (SqlConnection Scon = new SqlConnection(Security))
            {
                Scon.Open();
                using (SqlCommand Scommand = new SqlCommand("Update_EnableDisableAuthenticator", Scon))
                {
                    Scommand.CommandType = CommandType.StoredProcedure;
                    Scommand.Parameters.AddWithValue("@USERNAME", username);
                    Scommand.Parameters.AddWithValue("@Status", Status);
                    Scommand.ExecuteNonQuery();
                    i++;
                }
                Scon.Close();
                return i;
            }
        }

        public void GenerateTwoFactorAuthentication()
        {
            DatabaseConnectionString();

            using (SqlConnection Scon = new SqlConnection(Security))
            {
                Scon.Open();
                using (SqlCommand Scommand = new SqlCommand("Select *  FROM [PO_SECURITY].[dbo].[USER]", Scon))
                {
                    using (SqlDataAdapter SDA = new SqlDataAdapter(Scommand))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            SDA.Fill(DT);
                            Scon.Close();

                            if (DT.Rows.Count > 0)
                            {
                                for (int i = 0; i < DT.Rows.Count; i++)
                                {
                                    string username = DT.Rows[i]["USERNAME"].ToString();
                                    //update(username);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
