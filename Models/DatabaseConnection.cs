using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PO_PurchasingUI.Models
{
    public class DatabaseConnection
    {


        public string var1 = HttpContext.Current.Server.MapPath("~/DatabaseConnection/ConnectionString.txt");
        public string var2 = HttpContext.Current.Server.MapPath("~/DatabaseConnection/Username.txt");
        public string var3 = HttpContext.Current.Server.MapPath("~/DatabaseConnection/Password.txt");


    }
}