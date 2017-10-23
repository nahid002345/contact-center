using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OBLContactCenter.EFnClass
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class SessionExpireAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;


            if (ctx.Session["User"] == null)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult { Data = "_Logon_" };
                }
                else
                {
                    
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary {
                        { "Controller", "SignIn" },
                        { "Action", "LogOut" }
                });
                    //filterContext.ActionParameters["Msg"] = "Session is Expired. Please Sign in again...";
                    filterContext.Controller.TempData.Add("SignMsg", "Session is Expired. Please Sign in again...");
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
