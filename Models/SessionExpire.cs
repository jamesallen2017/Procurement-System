using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PO_PurchasingUI.Models
{
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public class SessionExpireAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			HttpContext context = HttpContext.Current;
			if (context.Session != null)
			{
				if (context.Session.IsNewSession == true)
				{
					string sessionCookie = context.Request.Headers["Cookie"];

					if ((sessionCookie != null) && (sessionCookie.IndexOf("ASP.NET_SessionId_My") >= 0))
					{
						// FormsAuthentication.SignOut();  
						string redirectTo = "~/Login/UserLogin";
						if (!string.IsNullOrEmpty(context.Request.RawUrl))
						{
							//filterContext.HttpContext.Response.Redirect(FormsAuthentication.LoginUrl);

							redirectTo = string.Format("~/Login/UserLogin?ReturnUrl={0}", HttpUtility.UrlEncode(context.Request.RawUrl));
							filterContext.Result = new RedirectResult(redirectTo);
							return;
						}

					}
				}
			}

			base.OnActionExecuting(filterContext);
		}
	}
}