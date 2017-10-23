using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OBLContactCenter.EFnClass;
using System.Data;
using System.Data.Entity.Validation;
using System.IO;
using OBLContactCenter.Models;
using System.Transactions;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Globalization;
namespace OBLContactCenter.Controllers
{
    public class CRMTaskController : AppBaseController
    {

        [SessionExpire]
        public ActionResult Index(string sortOrder, string searchString, int? page)
        {
            ResetIndexPage();
            oCurrentUser = (CCUSER)Session["User"];
            int enumDBType = 0;

            ViewBag.AssignGroupList = oDBContext.CCGROUPs.Where(t => t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.GROUPNAME.ToString()
            });

            ViewBag.CallCatagoryList = oDBContext.CCCALLCATAGORies.Where(t => t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.NAME.ToString()
            });
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.ActionType);
            ViewBag.ActionList = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == enumDBType && t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.NAME.ToString()
            });

            var taskList = new TaskList();
            var list = oDBContext.CCMEMOMASTERs.Where(t => t.ISACTIVE && t.CCMEMOLOGs.Where(x => x.ASSIGNGROUP == oCurrentUser.GROUPID).FirstOrDefault() != null &&
                (t.CCUSER.GROUPID != oCurrentUser.GROUPID || t.CCMEMOLOGs.Where(x => x.ASSIGNGROUP == oCurrentUser.GROUPID).Count() > 0));


            switch (sortOrder)
            {
                default:
                    list = list.OrderBy(s => s.ID);
                    break;
            }
            var totalList = new List<CCMEMOMASTER>();
            if (list != null)
            {
                var pendingList = list.Where(t => t.ASSIGNGROUP == oCurrentUser.GROUPID && t.ISOPEN).OrderByDescending(x => x.ID).ToList();
                var nonPendingList = list.Except(list.Where(t => t.ASSIGNGROUP == oCurrentUser.GROUPID && t.ISOPEN)).OrderByDescending(t => t.ID).ToList();

                totalList.AddRange(pendingList);
                totalList.AddRange(nonPendingList);
            }
            taskList.CCMEMOLIST = totalList.ToList();
            //taskList.CCMEMOLIST = list
            taskList.USERID = oCurrentUser.ID;
            taskList.USERGROUPID = oCurrentUser.GROUPID.Value;

            Session["List"] = taskList;

            return View(taskList);
        }

        private void ResetIndexPage()
        {
            oCurrentUser = (CCUSER)Session["User"];
            string brId = Request.Browser.Id.ToString();
            RemoveOverTimeOccupiedToken();
            RemoveOccupiedTokenByUIdBrowserId(oCurrentUser.USERID, brId);

        }

        [HttpGet]
        public ActionResult SearchList(string ticktNo, string acCard, int? catagoryId, int? groupId, int? actionId, int? statusId, string from, string to, string custName, string contact, int? callTypeId)
        {
            oCurrentUser = (CCUSER)Session["User"];
            var srchTaskList = new TaskList();
            if (Session["List"] != null)
            {
                var fullList = (TaskList)Session["List"];
                var list = fullList.CCMEMOLIST;
                if (!string.IsNullOrEmpty(ticktNo))
                    list = list.Where(t => t.TICKETNO == ticktNo.Trim()).ToList();
                if (!string.IsNullOrEmpty(acCard))
                    list = list.Where(t => (t.CUSTCARDNO != null && t.CUSTCARDNO.Contains(acCard)) || (t.CUSTACNO != null && t.CUSTACNO.Contains(acCard))).ToList();
                if (!string.IsNullOrEmpty(catagoryId.ToString()))
                    list = list.Where(t => t.CALLCATEGORY == catagoryId).ToList();
                if (!string.IsNullOrEmpty(groupId.ToString()))
                    list = list.Where(t => t.ASSIGNGROUP == groupId).ToList();
                if (!string.IsNullOrEmpty(actionId.ToString()))
                    list = list.Where(t => t.LASTACTIONTYPE == actionId).ToList();
                if (!string.IsNullOrEmpty(from))
                {
                    var fromDate = DateTime.ParseExact(from, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    list = list.Where(t => t.CREATEDON >= fromDate).ToList();
                }
                if (!string.IsNullOrEmpty(to))
                {
                    var toDate = DateTime.ParseExact(to, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    list = list.Where(t => t.CREATEDON <= toDate.AddHours(23).AddMinutes(59).AddSeconds(59)).ToList();
                }
                if (!string.IsNullOrEmpty(custName.Trim()))
                {
                    list = list.Where(t => t.CUSTOMERNAME.Contains(custName)).ToList();
                }
                if (!string.IsNullOrEmpty(contact.Trim()))
                {
                    list = list.Where(t => t.CUSTOMERCONTACT != null && t.CUSTOMERCONTACT.Contains(contact)).ToList();
                }
                if (!string.IsNullOrEmpty(callTypeId.ToString()))
                {
                    list = list.Where(t => t.CALLTYPE == callTypeId.Value).ToList();
                }

                srchTaskList.CCMEMOLIST = list.ToList();
                srchTaskList.USERID = oCurrentUser.ID;
                srchTaskList.USERGROUPID = oCurrentUser.GROUPID.Value;

            }
            return PartialView("_taskListPartial", srchTaskList);
        }


        public ActionResult EditTaskMemo(int id)
        {
            RemoveOverTimeOccupiedToken();
            oCurrentUser = (CCUSER)Session["User"];
            int enumDBType = 0;
            CCMEMOMASTER oCCMEMOMASTER = null;
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead }))
            {
                oCCMEMOMASTER = oDBContext.CCMEMOMASTERs.FirstOrDefault(t => t.ID == id);
                oCCMEMOMASTER.LASTACTIONDETAILS = string.Empty;
                oCCMEMOMASTER.LASTACTIONTYPE = null;

            }
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.StatusType);

            var StatusList = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == enumDBType && t.ISACTIVE).ToList();

            if (!oCurrentUser.CCROLE.ROLENAME.ToLower().Contains("admin") && !oCurrentUser.CCROLE.ROLENAME.ToLower().Contains("maker") && !oCurrentUser.CCROLE.ROLENAME.ToLower().Contains("checker"))
            {
                StatusList.RemoveAt(StatusList.FindIndex(t => t.NAME.ToLower() == "close"));
            }

            ViewBag.StatusList = StatusList.Select(x => x).ToList().Select(x => new SelectListItem
                                {
                                    Value = x.VALUE.ToString(),
                                    Text = x.NAME.ToString()
                                });;

            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.ActionType);
            ViewBag.ActionList = oDBContext.CCACTIONMAPs.Where(p => p.ROLEID == oCurrentUser.EMPLOYEEROLEID && p.MEMOTYPEID == oCCMEMOMASTER.MEMOTYPE && p.ISACTIVE == true).Select(x => x.CCENUMERATION).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.NAME.ToString()
            });
            oCCMEMOMASTER.ASSIGNGROUP = oCurrentUser.GROUPID;
            ViewBag.AssignGroupList = oDBContext.CCGROUPs.Where(t => t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.GROUPNAME.ToString(),
                Selected = true,
            });
            TaskInfo oTaskInfo = new TaskInfo();
            oTaskInfo.CCMEMO = oCCMEMOMASTER;
            if (IsTokenBusy(oCCMEMOMASTER.ID.ToString()))
            {
                oTaskInfo.IsTaskBusy = true;
                oTaskInfo.TaskOccupiedBy = oTaskTakenList.Find(t => t.TokenId == oCCMEMOMASTER.ID.ToString());
            }
            else
            {
                oTaskInfo.IsTaskBusy = false;
                AddOccupiedTask(oCurrentUser, oCCMEMOMASTER.ID.ToString());
            }

            return PartialView("_taskFrmPartial", oTaskInfo);
        }

        [HttpPost]
        public ActionResult SubmitTaskMemo(TaskInfo objMemoBind, List<HttpPostedFileBase> ctrlFile)
        {
            try
            {

                oCurrentUser = (CCUSER)Session["User"];
                if (ModelState.IsValid && CheckOccupiedTokenByUIdTokenId(oCurrentUser.USERID, objMemoBind.CCMEMO.ID.ToString()))
                {

                    objMemoBind.CCMEMO.ISACTIVE = true;
                    objMemoBind.CCMEMO.MODIFIEDBY = oCurrentUser.ID;
                    objMemoBind.CCMEMO.MODIFIEDON = DateTime.Now;
                    oDBContext.Entry(objMemoBind.CCMEMO).State = EntityState.Modified;
                    oDBContext.SaveChanges();

                    CCMEMOLOG oCCMEMOLOG = new CCMEMOLOG();
                    oCCMEMOLOG.MEMOMASTERID = objMemoBind.CCMEMO.ID;
                    oCCMEMOLOG.ACTEDBY = oCurrentUser.ID;
                    oCCMEMOLOG.ACTIONTYPE = objMemoBind.CCMEMO.LASTACTIONTYPE.Value;
                    oCCMEMOLOG.ACTIONDETAILS = objMemoBind.CCMEMO.LASTACTIONDETAILS;
                    oCCMEMOLOG.ASSIGNGROUP = objMemoBind.CCMEMO.ASSIGNGROUP.Value;

                    oCCMEMOLOG.ISACTIVE = true;
                    oCCMEMOLOG.CREATEDBY = oCurrentUser.ID;
                    oCCMEMOLOG.CREATEDON = DateTime.Now;

                    oDBContext.CCMEMOLOGs.Add(oCCMEMOLOG);
                    oDBContext.SaveChanges();

                    //var uploadFiles = Request.Files;

                    if (ctrlFile != null && ctrlFile.Count > 0)
                    {
                        foreach (HttpPostedFileBase oFile in ctrlFile)
                        {
                            if (oFile != null && oFile.ContentLength > 0)
                            {
                                var fileName = Path.GetFileName(oFile.FileName);
                                var path = Path.Combine(Server.MapPath("~/UploadDoc/"), objMemoBind.CCMEMO.TICKETNO + "_" + fileName);
                                //Console.Write(path);
                                oFile.SaveAs(path);

                                CCMEMOATTACHMENT oCCMEMOATTACHMENT = new CCMEMOATTACHMENT();
                                oCCMEMOATTACHMENT.MEMOMASTERID = objMemoBind.CCMEMO.ID;
                                oCCMEMOATTACHMENT.FILENAME = objMemoBind.CCMEMO.TICKETNO + "_" + fileName;
                                oCCMEMOATTACHMENT.FILEPATH = path;

                                oCCMEMOATTACHMENT.CREATEDBY = oCurrentUser.ID;
                                oCCMEMOATTACHMENT.CREATEDON = DateTime.Now;

                                oDBContext.CCMEMOATTACHMENTs.Add(oCCMEMOATTACHMENT);
                                oDBContext.SaveChanges();
                            }
                        }
                    }
                    oDBContext.SaveChanges();
                    RemoveOccupiedTokenByUIdTokenId(oCurrentUser.USERID, objMemoBind.CCMEMO.ID.ToString());
                    TempData["SuccessMsg"] = "Data Updated Successfully.";

                }
                else
                {
                    if (!ModelState.IsValid)
                    {
                        var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                        TempData["ErrorMsg"] = "Data is not Saved or Updated due to " + errors.ToString();
                    }
                    else if (!CheckOccupiedTokenByUIdTokenId(oCurrentUser.USERID, objMemoBind.CCMEMO.ID.ToString()))
                    {
                        TempData["ErrorMsg"] = "Task Occupied Time is finished already!!!";
                    }
                }

            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                if (oTaskTakenList != null)
                    oTaskTakenList.RemoveAll(t => t.UserId == oCurrentUser.USERID);
            }

            return RedirectToAction("Index");
        }



        [HttpPost]
        public JsonResult GetCallType(int id)
        {
            var CallType = oDBContext.CCCALLTYPEs.Where(t => t.CATAGORYID == id && t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.TYPENAME.ToString()
            });

            return Json(CallType);
        }

        [HttpPost]
        public JsonResult GetGroupByAction(int ActionId, int? MemoTypeId)
        {

            oCurrentUser = (CCUSER)Session["User"];
            int enumDBType = 0;
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.ActionType);
            long referActionId = oDBContext.CCENUMERATIONs.FirstOrDefault(t => t.TYPE == enumDBType && t.NAME.ToLower().Contains("refer")).ID;
            var groupList = oDBContext.CCACTIONGROUPMAPs.Where(t => t.ACTIONID == ActionId && t.MEMOTYPEID == MemoTypeId.Value && t.ROLEID == oCurrentUser.EMPLOYEEROLEID).Select(x => x).ToList();
            if (groupList != null && groupList.Count > 0)
            {
                var actionGroupList = groupList.Select(x => new SelectListItem
                {
                    Value = x.CCGROUP.ID.ToString(),
                    Text = x.CCGROUP.GROUPNAME.ToString(),
                    Selected = true
                });
                return Json(actionGroupList);
            }
            else
            {
                var actGroupList = new List<SelectListItem>();
                var actionGroup = new SelectListItem();
                actionGroup.Value = oCurrentUser.CCGROUP.ID.ToString();
                actionGroup.Text = oCurrentUser.CCGROUP.GROUPNAME.ToString();
                actionGroup.Selected = true;
                actGroupList.Add(actionGroup);
                return Json(actGroupList);
            }
        }

        [HttpPost]
        public JsonResult GetStatusByAction(int id)
        {
            bool memoStatus = true;
            int enumDBType = 0;
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.ActionType);
            var actionListToClose = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == enumDBType && (t.NAME.ToLower().Contains("ftr") || t.NAME.ToLower().Contains("resolve"))).ToList();
            if (actionListToClose.Find(t => t.ID == id) != null)
                memoStatus = false;
            return Json(memoStatus);
        }
        public FileResult DownloadFiles(string id)
        {
            Int32 FileId = Convert.ToInt32(id);
            var fileATTACHMENT = oDBContext.CCMEMOATTACHMENTs.FirstOrDefault(t => t.ID == FileId);
            byte[] fileBytes = System.IO.File.ReadAllBytes(fileATTACHMENT.FILEPATH);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileATTACHMENT.FILENAME);
        }

        [HttpPost]
        public void CloseModalTaskEnd()
        {
            oCurrentUser = (CCUSER)Session["User"];
            RemoveOccupiedTokenByUId(oCurrentUser.USERID);
        }
    }
}
