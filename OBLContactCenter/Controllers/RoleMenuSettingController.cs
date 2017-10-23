using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OBLContactCenter.EFnClass;
using System.Data;
using OBLContactCenter.Models;
namespace OBLContactCenter.Controllers
{
    [SessionExpire]
    public class RoleMenuSettingController : AppBaseController
    {
        
        CCROLEMENU oCCROLEMENU = null;
        
        public ActionResult Index()
        {
            ViewBag.RoleList = oDBContext.CCROLEs.Where(t => t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.ROLENAME.ToString()
            });

            ViewBag.MenuList = oDBContext.CCMENUs.Where(t => t.ISACTIVE && t.ISPARENT == true).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.NAME.ToString()
            });

            return View();
        }


        [HttpGet]
        public ActionResult RoleMenu()
        {
            
            ViewBag.RoleList = oDBContext.CCROLEs.Where(t=>t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.ROLENAME.ToString()
            });


            return PartialView("_RoleMenuPartial", oCCROLEMENU);
        }

        [HttpGet]
        public ActionResult GetRoleWiseMenuList(int id,int? MenuId)
        {
            var roleMenuItemTemp = oDBContext.CCMENUs.Where(t => t.ISACTIVE);
            if (MenuId.HasValue)
                roleMenuItemTemp =oDBContext.CCMENUs.Where(t => t.ISACTIVE && t.PARENTID == MenuId);
            else
                roleMenuItemTemp =oDBContext.CCMENUs.Where(t => t.ISACTIVE && t.ISPARENT == true);   
            

            var roleMenuItemList = roleMenuItemTemp.Select(t => new RoleMenu
            {
                ID=t.ID,
                MenuName=t.NAME,
                IsParent=t.ISPARENT,
                RoleId=id,
                RoleName = t.CCROLEMENUs.FirstOrDefault(x => x.ROLEID == id).CCROLE.ROLENAME,
                IsAssigned = t.CCROLEMENUs.FirstOrDefault(x => x.ROLEID == id).ISACTIVE
            }).ToList();
            return PartialView("_RMenuAssignListPartial", roleMenuItemList);
        }


        public ActionResult SaveRoleMenuList(List<RoleMenu> oRoleMenuList, FormCollection frmCollection)
        {
            try
            {
                oCurrentUser = (CCUSER)Session["User"];
                var roleId = frmCollection["RoleId"].ToList()[0].ToString();
                //var menuId = frmCollection["MenuId"].ToList()[0].ToString();
                oDBContext.Database.ExecuteSqlCommand("Delete from CCROLEMENU where ROLEID = {0}", roleId);
                oDBContext.SaveChanges();

                var selectedRoleMenuId = frmCollection.GetValues("chkSelect").ToList();
                foreach (string oMenuId in selectedRoleMenuId)
                {
                    CCROLEMENU oCCROLEMENU = new CCROLEMENU();
                    oCCROLEMENU.ROLEID = Convert.ToInt32(roleId);
                    oCCROLEMENU.MENUID = Convert.ToInt32(oMenuId);
                    oCCROLEMENU.ISACTIVE = true;
                    oCCROLEMENU.CREATEDBY = oCurrentUser.USERID;
                    oCCROLEMENU.CREATEDON = DateTime.Now;
                    oDBContext.CCROLEMENUs.Add(oCCROLEMENU);
                    oDBContext.SaveChanges();
                }
                TempData["SuccessMsg"] = "Data Saved Successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = "Data is not saved due to " + ExceptionMsg(ex);
            }
            return RedirectToAction("Index");
        }
        
    }
}
