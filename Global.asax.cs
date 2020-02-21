using Microsoft.AspNet.SignalR;
using PO_PurchasingUI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;


namespace PO_PurchasingUI
{
	public class MvcApplication : System.Web.HttpApplication
	{

  



		protected void Application_Start(object sender, EventArgs e)
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;
			AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
			MvcHandler.DisableMvcResponseHeader = true;
        }

		protected void Application_PreSendRequestHeaders()
		{
			Response.Headers.Remove("Server");
			Response.Headers.Remove("X-AspNet-Version");
		}

		
		protected void Application_End()
		{
            
		}

		protected void Application_EndRequest()
		{
			if (Context.Items["AjaxPermissionDenied"] is bool)
			{
				Context.Response.StatusCode = 401;
				Context.Response.End();
			}
		}


        private void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();

            if (ex is HttpAntiForgeryException)
            {
                Response.Clear();
                Server.ClearError(); //make sure you log the exception first
                Response.Redirect("/Login/LoginAuthentication/", true);
            }
        }

    }
}
