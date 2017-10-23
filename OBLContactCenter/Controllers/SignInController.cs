using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OBLContactCenter.EFnClass;
using OBLContactCenter.Models;
using System.Web.Security;

namespace OBLContactCenter.Controllers
{
    public class SignInController : AppBaseController
    {
        //
        // GET: /SignIn/
        CCUSER oCCUSER = null;

        [AllowAnonymous]
        [EventLog(EventType="Sys",customeEventName="Login")]
        public ActionResult Index()
        {
            
            
            Session.Clear();
            return View();
        }

        [AllowAnonymous]
        public ActionResult SignIn(LogIn objSignIn)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(objSignIn.USERID, objSignIn.USERPASSWORD))
                {
                    oCCUSER = oDBContext.CCUSERs.FirstOrDefault(t => t.USERID == objSignIn.USERID && t.USERPASSWORD == objSignIn.USERPASSWORD);
                    Session["User"] = oCCUSER;
                    Session["UserRole"] = oCCUSER.CCROLE.ROLENAME;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["SignMsg"] = "Invalid User ID / Password";
                    return View("Index");
                }
                //oCCUSER = oDBContext.CCUSERs.FirstOrDefault(t => t.USERID == objSignIn.USERID && t.USERPASSWORD == objSignIn.USERPASSWORD);
                //if (oCCUSER != null)
                //{
                //    Session["User"] = oCCUSER;
                //    Session["UserRole"] = oCCUSER.CCROLE.ROLENAME;
                //    Roles.AddUserToRole(oCCUSER.USERID, oCCUSER.CCROLE.ROLENAME);
                //    return RedirectToAction("Index", "Home");
                //}
                //else
                //{
                //    TempData["SignMsg"] = "Invalid User ID / Password";
                //    return View("Index");
                //}
            }

            return View("Index");
        }
        public ActionResult LogOut(string Msg)
        {
            //TempData["SignMsg"] = RouteData.Values["Msg"];
            
            Session.Clear();
            return View("Index");
        }

        public ActionResult NotAuthorized()
        {
            TempData["SignMsg"] = "Not Authorized. Please Sign in again...";
            Session.Clear();
            return View("Index");
        }

    }
}
