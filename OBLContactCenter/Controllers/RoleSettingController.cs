using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OBLContactCenter.EFnClass;
using System.Data;
using System.Data.Entity;
namespace OBLContactCenter.Controllers
{
    [SessionExpire]
    public class RoleSettingController : AppBaseController
    {
        
        CCROLE oCCROLE = null;
        public ActionResult Index()
        {
            var RoleList = oDBContext.CCROLEs.ToList();
            return View(RoleList);
        }

        [HttpGet]
        public ActionResult CreateEditRole(Int32 id)
        {
            
            ViewBag.RoleList = oDBContext.CCROLEs.Where(t=>t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.ROLENAME.ToString()
            });

            oCCROLE = oDBContext.CCROLEs.FirstOrDefault(t => t.ID == id);
            if (oCCROLE == null)
            {
                oCCROLE = new CCROLE();
                oCCROLE.ISACTIVE = true;
            }

            return PartialView("_CreateEditRolePartial", oCCROLE);
        }

        [HttpPost]
        public ActionResult InsertEditRole(CCROLE oCCROLE, string btnName)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            try
            {
                if (ModelState.IsValid)
                {
                    oCurrentUser = (CCUSER)Session["User"];
                    switch (btnName)
                    {
                        case "create":
                            oCCROLE.ISACTIVE = true;
                            oCCROLE.CREATEDBY = oCurrentUser.ID;
                            oCCROLE.CREATEDON = DateTime.Now;

                            oDBContext.CCROLEs.Add(oCCROLE);
                            oDBContext.SaveChanges();
                            TempData["SuccessMsg"] = "Data Saved Successfully.";
                            break;
                        case "update":
                            oCCROLE.MODIFIEDBY = oCurrentUser.ID;
                            oCCROLE.MODIFIEDON = DateTime.Now;
                            oDBContext.Entry(oCCROLE).State = EntityState.Modified;
                            oDBContext.SaveChanges();
                            TempData["SuccessMsg"] = "Data Updated Successfully.";
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = "Data is not saved due to " + ExceptionMsg(ex);
            }
            return RedirectToAction("Index");
        }

        
    }
}
