using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using OBLContactCenter.EFnClass;

namespace OBLContactCenter.EFnClass
{
    public static class MenuActiveExtensions
    {
        public static MvcHtmlString MenuItem(this HtmlHelper htmlHelper,
                                             string text, string action,
                                             string controller,
                                             string iconClass = null)
        {
            var li = new TagBuilder("li");
            var routeData = htmlHelper.ViewContext.RouteData;
            var currentAction = routeData.GetRequiredString("action");
            var currentController = routeData.GetRequiredString("controller");
            if (string.Equals(currentAction,
                              action,
                              StringComparison.OrdinalIgnoreCase) &&
                string.Equals(currentController,
                              controller,
                              StringComparison.OrdinalIgnoreCase))
            {
                li.AddCssClass("active");
            }
            var basePath = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            var i = new TagBuilder("i");
            i.AddCssClass(iconClass);

            //if (routeValues != null)
            //{
            //    li.InnerHtml = (htmlAttributes != null)
            //        ? htmlHelper.ActionLink(text,
            //                                action,
            //                                controller,
            //                                routeValues,
            //                                htmlAttributes).ToHtmlString()
            //        : htmlHelper.ActionLink(text,
            //                                action,
            //                                controller,
            //                                routeValues).ToHtmlString();
            //}
            //else
            //{
            //    li.InnerHtml = htmlHelper.ActionLink(text,
            //                                         action,
            //                                         controller).ToHtmlString();
            //}

            li.InnerHtml = htmlHelper.Raw(string.Format("<a href=\"{0}/{1}/{2}\"><i class=\"{4}\"></i>{3}</a>", basePath, controller, action, text, iconClass)).ToString();

            return MvcHtmlString.Create(li.ToString());
        }

        public static MvcHtmlString ParentMenuItem(this HtmlHelper htmlHelper, string LinkText, List<CCMENU> SubMenuList, string iconClass = null)
        {
            var parentli = new TagBuilder("li");
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            var routeData = urlHelper.RequestContext.RouteData;
            //var currentAction = routeData.GetRequiredString("action");
            //var currentController = routeData.GetRequiredString("controller");
            var currentAction = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
            var currentController = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();

            var request = HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            //if (appUrl != "/") appUrl += "/";

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            
            var basePath = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            if (SubMenuList.Exists(t => t.ACTION.Equals(currentAction, StringComparison.OrdinalIgnoreCase)) &&
                SubMenuList.Exists(t => t.CONTROLLER.Equals(currentController, StringComparison.OrdinalIgnoreCase)))
            {
                parentli.AddCssClass("active");
                parentli.InnerHtml = htmlHelper.Raw(string.Format("<a href=\"javascript:;\"><i class=\"{2}\"></i><span class=\"title\">{1}</span> <span class=\"selected\"></span> <span class=\"arrow open\"></span></a>", basePath, LinkText, iconClass)).ToString();
            }
            else
            {
                parentli.InnerHtml = htmlHelper.Raw(string.Format("<a href=\"javascript:;\"><i class=\"{2}\"></i><span class=\"title\">{1}</span></span> <span class=\"arrow\"></span></a>", basePath, LinkText, iconClass)).ToString();
            }
            var ul = new TagBuilder("ul");
            ul.AddCssClass("sub-menu");
            foreach (var item in SubMenuList)
            {
                var li = new TagBuilder("li");
                if (string.Equals(currentAction,
                              item.ACTION,
                              StringComparison.OrdinalIgnoreCase) &&
                string.Equals(currentController,
                              item.CONTROLLER,
                              StringComparison.OrdinalIgnoreCase))
                {
                    li.InnerHtml = htmlHelper.Raw(string.Format("<a href=\"{0}/{1}/{2}\"><i class=\"{4}\"></i>{3}<span class=\"selected\"></span></a>", baseUrl, item.CONTROLLER, item.ACTION, item.NAME, item.ICONTEXT)).ToString();
                    li.AddCssClass("active");
                }
                else
                {
                    li.InnerHtml = htmlHelper.Raw(string.Format("<a href=\"{0}/{1}/{2}\"><i class=\"{4}\"></i>{3}</a>", baseUrl, item.CONTROLLER, item.ACTION, item.NAME, item.ICONTEXT)).ToString();
                }

                ul.InnerHtml += li.ToString(TagRenderMode.Normal);
            }
            parentli.InnerHtml += ul.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(parentli.ToString());
        }
    }
}