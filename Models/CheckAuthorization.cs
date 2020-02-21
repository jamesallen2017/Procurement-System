using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PO_PurchasingUI.Models
{
	public class CheckAuthorization : AuthorizeAttribute
	{
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var test = HttpContext.Current.Session["UserID"];
            if (test == null || !HttpContext.Current.Request.IsAuthenticated)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.ClearContent();
                    filterContext.HttpContext.Items["AjaxPermissionDenied"] = true;
                }
                else
                {
                    filterContext.Result = new RedirectResult(System.Web.Security.FormsAuthentication.LoginUrl + "?ReturnURL=" +
                         filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.RawUrl));
                }
            }
            else
            {
                //Code HERE for page level authorization  

            }
        }

        //public override void OnAuthorization(AuthorizationContext filterContext)
        //{
        //    string actionName = filterContext.ActionDescriptor.ActionName;
        //    string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

        //    if (filterContext != null)
        //    {
        //        HttpSessionStateBase objHttpSessionStateBase = filterContext.HttpContext.Session;
        //        var userSession = objHttpSessionStateBase["UserName"];
        //        if (((userSession == null) && (!objHttpSessionStateBase.IsNewSession)) || (objHttpSessionStateBase.IsNewSession))
        //        {
        //            objHttpSessionStateBase.RemoveAll();
        //            objHttpSessionStateBase.Clear();
        //            objHttpSessionStateBase.Abandon();

        //            if (filterContext.HttpContext.Request.IsAjaxRequest())
        //            {
        //                filterContext.HttpContext.Response.StatusCode = 403;
        //                filterContext.Result = new JsonResult { Data = "LogOut" };
        //            }
        //            else
        //            {
        //                filterContext.Result = new RedirectResult("~/Login/UserLogin");
        //            }

        //        }
        //        else
        //        {
        //            if (!HttpContext.Current.Request.IsAuthenticated)
        //            {
        //                filterContext.Result = new RedirectResult(FormsAuthentication.LoginUrl + "?ReturnURL=" +
        //                 filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.RawUrl));
        //            }
        //            else
        //            {
        //                base.OnAuthorization(filterContext);
        //            }
        //        }
        //    }
        //}
	}
}