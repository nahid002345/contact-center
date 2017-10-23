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
    
    public class RoleActionSettingController : AppBaseController
    {
        
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Index()
        {
            int enumDBType = 0;
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.FormType);
            ViewBag.RoleList = oDBContext.CCROLEs.Where(t => t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.ROLENAME.ToString()
            });

            ViewBag.MemoTypeList = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == enumDBType && t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.NAME.ToString()
            });

            return View();
        }


        
        [HttpGet]
        public ActionResult GetRoleWiseActionList(int RoleId, int MTypeId)
        {
            int enumDBType =0;
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.FormType);
            var MTypeList = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == enumDBType && t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.NAME.ToString()
            }).ToList();
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.ActionType);
            var roleActionItemList = oDBContext.CCENUMERATIONs.Where(t => t.ISACTIVE && t.TYPE == enumDBType).Select(t => new RoleAction
            {
                ActionID=t.ID,
                ActionName=t.NAME,
                RoleId = RoleId,
                RoleName = t.CCACTIONMAPs.FirstOrDefault(x => x.ROLEID == RoleId && x.MEMOTYPEID ==MTypeId).CCROLE.ROLENAME,
                IsAssigned = t.CCACTIONMAPs.FirstOrDefault(x => x.ROLEID == RoleId && x.MEMOTYPEID == MTypeId).ISACTIVE,
                MemoTypeID = MTypeId

            }).ToList();
            return PartialView("_RolewiseActionListPartial", roleActionItemList);
        }


        public ActionResult SaveRoleActionList(FormCollection frmCollection)
        {
            try
            {
                oCurrentUser = (CCUSER)Session["User"];
                var roleId = frmCollection["RoleId"].ToList()[0].ToString();
                var MemoTypeId = frmCollection["MemoTypeID"].ToList()[0].ToString();

                oDBContext.Database.ExecuteSqlCommand("Delete from CCACTIONMAP where ROLEID = {0} AND MEMOTYPEID = {1} ", roleId, MemoTypeId);
                oDBContext.SaveChanges();

                var selectedRoleMenuId = frmCollection.GetValues("chkSelect").ToList();
                foreach (string oActionId in selectedRoleMenuId)
                {
                    CCACTIONMAP oCCACTIONMAP = new CCACTIONMAP();
                    oCCACTIONMAP.ROLEID = Convert.ToInt32(roleId);
                    oCCACTIONMAP.MEMOTYPEID = Convert.ToInt32(MemoTypeId);
                    oCCACTIONMAP.ACTIONID = Convert.ToInt32(oActionId);
                    oCCACTIONMAP.ISACTIVE = true;
                    oCCACTIONMAP.CREATEDBY = oCurrentUser.USERID;
                    oCCACTIONMAP.CREATEDON = DateTime.Now;
                    oDBContext.CCACTIONMAPs.Add(oCCACTIONMAP);
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
