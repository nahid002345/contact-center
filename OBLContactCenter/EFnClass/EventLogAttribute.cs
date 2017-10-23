using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using OBLContactCenter.EFnClass;

namespace OBLContactCenter.EFnClass
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class EventLogAttribute : ActionFilterAttribute
    {
        
        public string EventType;
        public  string customeEventName;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.ActionName;
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            if (string.IsNullOrEmpty(EventType))
                EventType = "Sys";
            var user = (CCUSER) filterContext.HttpContext.Session.Contents["User"];
            
            string IpAddress = GetIPAddress();
            DateTime browseTime = DateTime.Now;
            string eventName = (string.IsNullOrEmpty(customeEventName)) ? actionName : customeEventName;

            using (OBLCONTACTCENTEREntities db = new OBLCONTACTCENTEREntities())
            {
                CCEVENTLOG oCCEVENTLOG = new CCEVENTLOG();
                oCCEVENTLOG.UserId =(user == null) ? " Anonymous" : user.USERID;
                oCCEVENTLOG.RequestTime = browseTime;
                oCCEVENTLOG.IPAddress = IpAddress;
                oCCEVENTLOG.Controller = controllerName;
                oCCEVENTLOG.Action = actionName;
                oCCEVENTLOG.EventName = eventName;
                oCCEVENTLOG.EventType = EventType;
                db.CCEVENTLOGs.Add(oCCEVENTLOG);
                //db.SaveChanges();
            }

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
            base.OnActionExecuted(filterContext);
        }

        protected string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }
    }
}
