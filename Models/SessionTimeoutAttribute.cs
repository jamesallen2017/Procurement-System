using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PO_PurchasingUI.Models
{
    [AttributeUsage(AttributeTargets.Class |
   AttributeTargets.Method, Inherited = true, AllowMultiple = true)]

	public class SessionTimeoutAttribute : ActionFilterAttribute
	{
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            // check if session is supported  
            if (ctx.Session != null)
            {
                // check if a new session id was generated  
                if (ctx.Session["UserID"] == null || ctx.Session.IsNewSession)
                {
                    //Check is Ajax request  
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.HttpContext.Response.ClearContent();
                        filterContext.HttpContext.Items["AjaxPermissionDenied"] = true;
                    }
                    // check if a new session id was generated  
                    else
                    {
                        filterContext.Result = new RedirectResult("~/Login/Logout");
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }

	}
}