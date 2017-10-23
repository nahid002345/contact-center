using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OBLContactCenter.EFnClass
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class TaskTakenAttribute : ActionFilterAttribute
    {
        public string UserId { get; set; }
        public string TokenType { get; set; }
        public string TokenId { get; set; }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
            base.OnActionExecuted(filterContext);
        }
    }
}
