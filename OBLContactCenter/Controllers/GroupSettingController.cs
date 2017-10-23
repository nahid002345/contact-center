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
    public class GroupSettingController : AppBaseController
    {
        
        CCGROUP oCCGROUP = null;
        public ActionResult Index()
        {
            var GroupList = oDBContext.CCGROUPs.ToList();
            return View(GroupList);
        }

        [HttpGet]
        public ActionResult CreateGroup()
        {
            oCCGROUP = new CCGROUP();
            oCCGROUP.ISACTIVE = true;

            ViewBag.GroupList = oDBContext.CCGROUPs.Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.GROUPNAME.ToString()
            });
            ViewBag.RoleList = oDBContext.CCROLEs.Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.ROLENAME.ToString()
            });



            return PartialView("_EditGroupPartial", oCCGROUP);
        }

        [HttpGet]
        public ActionResult EditGroup(int id)
        {
            ViewBag.GroupList = oDBContext.CCGROUPs.Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.GROUPNAME.ToString()
            });
            ViewBag.RoleList = oDBContext.CCROLEs.Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.ROLENAME.ToString()
            });

            oCCGROUP = oDBContext.CCGROUPs.FirstOrDefault(t => t.ID == id);

            return PartialView("_EditGroupPartial", oCCGROUP);
        }

        [HttpPost]
        public ActionResult InsertEditGroup(CCGROUP oCCGROUP, string btnName)
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
                            oCCGROUP.ISACTIVE = true;
                            oCCGROUP.CREATEDBY = oCurrentUser.ID;
                            oCCGROUP.CREATEDON = DateTime.Now;

                            oDBContext.CCGROUPs.Add(oCCGROUP);
                            oDBContext.SaveChanges();
                            TempData["SuccessMsg"] = "Data Saved Successfully.";
                            break;
                        case "update":
                            oCCGROUP.MODIFIEDBY = oCurrentUser.ID;
                            oCCGROUP.MODIFIEDON = DateTime.Now;
                            oDBContext.Entry(oCCGROUP).State = EntityState.Modified;
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
