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
    
    public class ActionAsgGroupSettingController : AppBaseController
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


        [HttpPost]
        public JsonResult GetMemoTypeWiseAction(int MTypeId, int? RoleId)
        {
            //oCurrentUser = (CCUSER)Session["User"];
            int enumDBType = 0;

            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.ActionType);
            var actionList = oDBContext.CCACTIONMAPs.Where(p => p.ROLEID == RoleId && p.MEMOTYPEID == MTypeId && p.ISACTIVE == true).Select(x => x.CCENUMERATION).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.NAME.ToString()
            });
            return Json(actionList);

        }


        [HttpGet]
        public ActionResult GetRoleMTypeActionWiseGroupList(int RoleId, int MTypeId, int? ActionId)
        {
            int enumDBType =0;
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.ActionType);
            var roleActionGroupItemList = oDBContext.CCGROUPs.Where(t => t.ISACTIVE ).Select(t => new RoleActionGroup
            {
                ActionID=ActionId.Value,
                RoleId = RoleId,
                MemoTypeID=MTypeId,
                GroupId=t.ID,
                GroupName=t.GROUPNAME,
                IsAssigned = t.CCACTIONGROUPMAPs.FirstOrDefault(x => x.ROLEID == RoleId && x.MEMOTYPEID == MTypeId && x.ACTIONID == ActionId.Value).ISACTIVE
            }).ToList();
            return PartialView("_ActwiseAsgnGroupPartial", roleActionGroupItemList);
        }


        public ActionResult SaveActionGroupList(FormCollection frmCollection)
        {
            try
            {
                oCurrentUser = (CCUSER)Session["User"];
                var roleId = new List<int>(frmCollection["RoleId"].Split(',').Select(s => int.Parse(s)))[0];
                var MemoTypeId = new List<int>(frmCollection["MemoTypeID"].Split(',').Select(s => int.Parse(s)))[0]; 
                var actionId = new List<int>(frmCollection["ActionID"].Split(',').Select(s => int.Parse(s)))[0]; 

                oDBContext.Database.ExecuteSqlCommand("Delete from CCACTIONGROUPMAP where ROLEID = {0} AND MEMOTYPEID = {1} AND ACTIONID = {2} ", roleId, MemoTypeId,actionId);
                oDBContext.SaveChanges();

                var selectedGroupId = frmCollection["chkSelect"].Split(',').Select(s => int.Parse(s)).ToList();
                foreach (int oGroupId in selectedGroupId)
                {
                    CCACTIONGROUPMAP oCCACTIONGROUPMAP = new CCACTIONGROUPMAP();
                    oCCACTIONGROUPMAP.ROLEID = Convert.ToInt32(roleId);
                    oCCACTIONGROUPMAP.MEMOTYPEID = Convert.ToInt32(MemoTypeId);
                    oCCACTIONGROUPMAP.ACTIONID = Convert.ToInt32(actionId);
                    oCCACTIONGROUPMAP.ASGNGROUPID = oGroupId;
                    oCCACTIONGROUPMAP.ISACTIVE = true;
                    oCCACTIONGROUPMAP.CREATEDBY = oCurrentUser.USERID;
                    oCCACTIONGROUPMAP.CREATEDON = DateTime.Now;
                    oDBContext.CCACTIONGROUPMAPs.Add(oCCACTIONGROUPMAP);
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
