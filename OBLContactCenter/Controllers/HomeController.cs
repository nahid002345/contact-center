using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OBLContactCenter.EFnClass;
using System.IO;
using System.Data.Entity.Validation;
using OBLContactCenter.Models;
using System.Data.Entity.Core.Objects;
using System.Web.Security;
using System.Data;
using System.Linq.Expressions;
using System.Dynamic;
using System.Globalization;
namespace OBLContactCenter.Controllers
{
    [SessionExpire]
    public class HomeController : AppBaseController
    {
        CCUSER oCurrentUser = null;
        public ActionResult Index()
        {
            var User = HttpContext.User;
            var role = Roles.GetAllRoles();

            int enumDBType = 0;
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.PriorityType);

            long HightPriorityId = oDBContext.CCENUMERATIONs.FirstOrDefault(t => t.TYPE == enumDBType && t.NAME.ToLower().Contains("high")).ID;
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.ActionType);
            long openActionId = oDBContext.CCENUMERATIONs.FirstOrDefault(t => t.TYPE == enumDBType && t.NAME.ToLower().Contains("open")).ID;
            long referActionId = oDBContext.CCENUMERATIONs.FirstOrDefault(t => t.TYPE == enumDBType && t.NAME.ToLower().Contains("refer")).ID;
            //ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.TickerMsg);
            CCUSER oCCUSER = (CCUSER)Session["User"];

            Dashboard oDashboard = new Dashboard();
            oDashboard.NewsTicker = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == enumDBType).OrderByDescending(t => t.CREATEDON).ToList();
            oDashboard.USERID = oCCUSER.USERID;
            oDashboard.USERNAME = oCCUSER.EMPLOYEENAME;
            oDashboard.GROUP = oCCUSER.CCGROUP.GROUPNAME;
            oDashboard.ROLE = oCCUSER.CCROLE.ROLENAME;

            oDashboard.UrgentMemo = oDBContext.CCMEMOMASTERs.Where(t => t.PRIORITY == HightPriorityId && t.ISOPEN && t.ISACTIVE).Count();
            oDashboard.HourCrossedMemo = oDBContext.CCMEMOLOGs.Where(t => EntityFunctions.DiffHours(t.CREATEDON, DateTime.Now).Value > 6 && t.CCMEMOMASTER.CCGROUP.GROUPNAME.ToLower().Contains("check") && t.CCMEMOMASTER.CCENUMERATION.NAME.ToLower().Contains("refer") && t.CCMEMOMASTER.ISOPEN && t.ISACTIVE).Select(x => x.CCMEMOMASTER).OrderByDescending(x => x.CREATEDON).Distinct().Count();
            oDashboard.ReferMemo = oDBContext.CCMEMOMASTERs.Where(t => (t.LASTACTIONTYPE == openActionId || t.LASTACTIONTYPE == referActionId) && t.ISOPEN && t.ISACTIVE).ToList().Count();
            var OtherGroupMemo = oDBContext.CCMEMOLOGs.Where(t => !t.CCMEMOMASTER.CCGROUP.GROUPNAME.ToLower().Contains("make") && !t.CCMEMOMASTER.CCGROUP.GROUPNAME.ToLower().Contains("check") && (t.ACTIONTYPE == openActionId || t.ACTIONTYPE == referActionId) && t.CCMEMOMASTER.ISOPEN && t.CCMEMOMASTER.ISACTIVE).Select(x => x.CCMEMOMASTER).Distinct().ToList();
            oDashboard.OtherGroupMemo = OtherGroupMemo.Count;

            oDashboard.OtherGrpList = OtherGroupMemo.GroupBy(t => t.CCGROUP).Select(x => x).ToList().Select(x => new DashGroupTicket
            {
                GroupId = x.Key.ID.ToString(),
                GroupName = x.Key.GROUPNAME,
                SrchCategory = "4",
                TotalTicketNo = x.Count().ToString()
            }).ToList();

            return View(oDashboard);
        }

        [HttpGet]
        public ActionResult HeaderDetails()
        {
            UserInfo oUserInfo = null;
            if (Session["User"] != null)
            {
                CCUSER oCCUSER = (CCUSER)Session["User"];
                oUserInfo = new UserInfo();
                oUserInfo.ID = oCCUSER.ID;
                oUserInfo.USERID = oCCUSER.USERID;
                oUserInfo.USERNAME = oCCUSER.EMPLOYEENAME;
                oUserInfo.GROUP = oCCUSER.CCGROUP.GROUPNAME;
                oUserInfo.ROLE = oCCUSER.CCROLE.ROLENAME;
                oUserInfo.IMAGEPATH = oCCUSER.IMAGEPATH;
                
                oUserInfo.PendingTaskList = oDBContext.CCMEMOMASTERs.Where(t => t.ISACTIVE && t.CCMEMOLOGs.Where(x => x.ASSIGNGROUP == oCCUSER.GROUPID).OrderByDescending(p => p.ID).FirstOrDefault() != null && t.ASSIGNGROUP == oCCUSER.GROUPID && t.ISOPEN == true).OrderByDescending(t => t.ID).ToList();

                oUserInfo.PriorityTaskList = oUserInfo.PendingTaskList.Where(t => t.CREATEDBY == oCCUSER.ID).ToList();
                return PartialView("_HeaderPartial", oUserInfo);
                //DateTime.Now.Subtract(DateTime.Now).Hours
            }
            else
                return RedirectToAction("SignIn", "SignIn");
        }

        public ActionResult MenuDetails()
        {
            UserInfo oUserInfo = null;
            if (Session["User"] != null)
            {
                CCUSER oCCUSER = (CCUSER)Session["User"];
                List<CCROLEMENU> oCCMENUList = new List<CCROLEMENU>();
                oCCMENUList = oDBContext.CCROLEMENUs.Where(t => t.ROLEID == oCCUSER.EMPLOYEEROLEID && t.CCMENU.ISACTIVE == true).ToList();

                return PartialView("_UserMenuPartial", oCCMENUList);
            }
            else
                return RedirectToAction("SignIn", "SignIn");
        }

        public ActionResult TickerDetails()
        {
            Dashboard oDashboard = new Dashboard();
            if (Session["User"] != null)
            {
                int enumDBType = 0;
                enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.TickerMsg);
                oDashboard.NewsTicker = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == enumDBType && t.ISACTIVE).OrderByDescending(t => t.CREATEDON).ToList();
                return PartialView("_TickerPartial", oDashboard);
            }
            else
                return RedirectToAction("SignIn", "SignIn");
        }

        [HttpGet]
        public JsonResult GetStatusCount()
        {
            var List = oDBContext.CCMEMOMASTERs.Where(c => c.ISACTIVE).GroupBy(t => t.CCENUMERATION).Select(x => new ChartActionTypeMemoCount
            {
                ActionMemoCount = x.Count(),
                ActionTypeID = x.Key.ID,
                ActionName = x.Key.NAME

            });
            return Json(List, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetStatusCountByDate(string fromDate, string toDate)
        {
            var frDate = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var toDt = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            toDt = toDt.AddDays(1); //1 Day is added because given toDt is upto DATE 00.00AM which will not take toDt Data
            var List = oDBContext.CCMEMOMASTERs.Where(c => c.ISACTIVE && (c.CREATEDON >= frDate && c.CREATEDON <= toDt)).GroupBy(t => t.CCENUMERATION).Select(x => new ChartActionTypeMemoCount
            {
                ActionMemoCount = x.Count(),
                ActionTypeID = x.Key.ID,
                ActionName = x.Key.NAME

            });
            return Json(List, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDailyRequestCount()
        {
            var List = oDBContext.CCMEMOMASTERs.Where(x => x.ISACTIVE).GroupBy(t => EntityFunctions.TruncateTime(t.CREATEDON)).Select(x => new ChartDailyRequestCount
            {
                requestDate = x.Key.Value,
                MemoCount = x.Count()
            });

            return Json(List.AsEnumerable().OrderBy(x => x.requestDate).Select(r => new
            {
                date = r.requestDate.GetValueOrDefault().ToString("dd.MM.yyyy"),
                count = r.MemoCount
            }), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetDailyRequestCountByDate(string fromDate, string toDate)
        {
            var frDate = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var toDt = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            toDt = toDt.AddDays(1); //1 Day is added because given toDt is upto DATE 00.00AM which will not take toDt Data
            var List = oDBContext.CCMEMOMASTERs.Where(x => x.ISACTIVE && (x.CREATEDON >= frDate && x.CREATEDON <= toDt)).GroupBy(t => EntityFunctions.TruncateTime(t.CREATEDON)).Select(x => new ChartDailyRequestCount
            {
                requestDate = x.Key.Value,
                MemoCount = x.Count()
            });

            return Json(List.AsEnumerable().OrderBy(x => x.requestDate).Select(r => new
            {
                date = r.requestDate.GetValueOrDefault().ToString("dd.MM.yyyy"),
                count = r.MemoCount
            }), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetUserwiseTicketStatus()
        {
            var List = oDBContext.CCMEMOLOGs.Where(t => t.ISACTIVE).GroupBy(p => new { p.CCUSER.EMPLOYEENAME }).Select(x => new UserwiseTicketStatus
            {
                UserName = x.Key.EMPLOYEENAME,
                ftr = x.Where(g => g.CCENUMERATION.NAME.ToLower().Contains("ftr")).Count(),
                Open = x.Where(g => g.CCENUMERATION.NAME.ToLower().Contains("open")).Count(),
                OnProcess = x.Where(g => g.CCENUMERATION.NAME.ToLower().Contains("on process")).Count(),
                CallBackRequired = x.Where(g => g.CCENUMERATION.NAME.ToLower().Contains("call back required")).Count(),
                CallBackDone = x.Where(g => g.CCENUMERATION.NAME.ToLower().Contains("call back done")).Count(),
                MailSent = x.Where(g => g.CCENUMERATION.NAME.ToLower().Contains("mail sent")).Count(),
                Refer = x.Where(g => g.CCENUMERATION.NAME.ToLower().Contains("refer")).Count(),
                Resolve = x.Where(g => g.CCENUMERATION.NAME.ToLower().Contains("resolve")).Count(),
                CallReached = x.Where(g => g.CCENUMERATION.NAME.ToLower().Contains("call reached")).Count(),
                CallUnreached = x.Where(g => g.CCENUMERATION.NAME.ToLower().Contains("call unreached")).Count(),
                FollowUp = x.Where(g => g.CCENUMERATION.NAME.ToLower().Contains("FollowUp")).Count()
            });

            var check = List.ToList();
            return Json(List, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetUserwiseTicketStatusByDate(string fromDate, string toDate)
        {

            var frDate = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var toDt = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            toDt = toDt.AddDays(1); //1 Day is added because given toDt is upto DATE 00.00AM which will not take toDt Data

            var List = oDBContext.CCMEMOLOGs.Where(t => t.ISACTIVE && (t.CREATEDON >= frDate && t.CREATEDON <= toDt) ).GroupBy(p => new { p.CCUSER.EMPLOYEENAME }).Select(x => new UserwiseTicketStatus
            {
                UserName = x.Key.EMPLOYEENAME,
                ftr = x.Where(g => g.CCENUMERATION.NAME.ToLower().Contains("ftr")).Count(),
                Open = x.Where(g => g.CCENUMERATION.NAME.ToLower().Contains("open")).Count(),
                OnProcess = x.Where(g => g.CCENUMERATION.NAME.ToLower().Contains("on process")).Count(),
                CallBackRequired = x.Where(g => g.CCENUMERATION.NAME.ToLower().Contains("call back required")).Count(),
                CallBackDone = x.Where(g => g.CCENUMERATION.NAME.ToLower().Contains("call back done")).Count(),
                MailSent = x.Where(g => g.CCENUMERATION.NAME.ToLower().Contains("mail sent")).Count(),
                Refer = x.Where(g => g.CCENUMERATION.NAME.ToLower().Contains("refer")).Count(),
                Resolve = x.Where(g => g.CCENUMERATION.NAME.ToLower().Contains("resolve")).Count(),
                CallReached = x.Where(g => g.CCENUMERATION.NAME.ToLower().Contains("call reached")).Count(),
                CallUnreached = x.Where(g => g.CCENUMERATION.NAME.ToLower().Contains("call unreached")).Count(),
                FollowUp = x.Where(g => g.CCENUMERATION.NAME.ToLower().Contains("FollowUp")).Count()
            });

            var check = List.ToList();
            return Json(List, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SearchList(string searchCat, int? grpId)
        {
            oCurrentUser = (CCUSER)Session["User"];
            var srchTaskList = new TaskList();
            int enumDBType = 0;
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.PriorityType);



            if (searchCat == "1")
            {
                srchTaskList.TASKTYPE = 1;
                srchTaskList.TASKTYPENAME = "High Priority Request List";
                long HightPriorityId = oDBContext.CCENUMERATIONs.FirstOrDefault(t => t.TYPE == enumDBType && t.NAME.ToLower().Contains("high")).ID;
                srchTaskList.CCMEMOLIST = oDBContext.CCMEMOMASTERs.Where(t => t.PRIORITY == HightPriorityId && t.ISOPEN && t.ISACTIVE == true).OrderByDescending(x => x.CREATEDON).ToList();
            }
            else if (searchCat == "2")//when any case is referred to checker 6hours ago
            {
                srchTaskList.TASKTYPE = 2;
                srchTaskList.TASKTYPENAME = "6 Hours Crossed Request";
                srchTaskList.CCMEMOLIST = oDBContext.CCMEMOLOGs.Where(t => EntityFunctions.DiffHours(t.CREATEDON, DateTime.Now).Value > 6 && t.CCMEMOMASTER.CCGROUP.GROUPNAME.ToLower().Contains("check") && t.CCMEMOMASTER.CCENUMERATION.NAME.ToLower().Contains("refer") && t.CCMEMOMASTER.ISOPEN && t.CCMEMOMASTER.ISACTIVE).Select(x => x.CCMEMOMASTER).OrderByDescending(x => x.CREATEDON).Distinct().ToList();
            }
            else if (searchCat == "3")
            {
                srchTaskList.TASKTYPE = 3;
                srchTaskList.TASKTYPENAME = "Total Refer Request List";

                enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.ActionType);
                long openActionId = oDBContext.CCENUMERATIONs.FirstOrDefault(t => t.TYPE == enumDBType && t.NAME.ToLower().Contains("open")).ID;
                long referActionId = oDBContext.CCENUMERATIONs.FirstOrDefault(t => t.TYPE == enumDBType && t.NAME.ToLower().Contains("refer")).ID;
                srchTaskList.CCMEMOLIST = oDBContext.CCMEMOMASTERs.Where(t => (t.LASTACTIONTYPE == openActionId || t.LASTACTIONTYPE == referActionId) && t.ISOPEN && t.ISACTIVE).OrderByDescending(x => x.CREATEDON).ToList();
            }
            else if (searchCat == "4")
            {
                srchTaskList.TASKTYPE = 4;
                srchTaskList.TASKTYPENAME = "Group Pending Request List";

                enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.ActionType);
                long openActionId = oDBContext.CCENUMERATIONs.FirstOrDefault(t => t.TYPE == enumDBType && t.NAME.ToLower().Contains("open")).ID;
                long referActionId = oDBContext.CCENUMERATIONs.FirstOrDefault(t => t.TYPE == enumDBType && t.NAME.ToLower().Contains("refer")).ID;
                srchTaskList.CCMEMOLIST = oDBContext.CCMEMOLOGs.Where(t => !t.CCMEMOMASTER.CCGROUP.GROUPNAME.ToLower().Contains("make") && !t.CCMEMOMASTER.CCGROUP.GROUPNAME.ToLower().Contains("check") && (t.ACTIONTYPE == openActionId || t.ACTIONTYPE == referActionId) && t.CCMEMOMASTER.ISOPEN && t.CCMEMOMASTER.ISACTIVE).Select(x => x.CCMEMOMASTER).Distinct().OrderByDescending(x => x.CREATEDON).ToList();

                if (grpId.HasValue)
                {
                    srchTaskList.TASKTYPE = 4;

                    srchTaskList.CCMEMOLIST = srchTaskList.CCMEMOLIST.Where(t => t.ASSIGNGROUP == grpId).ToList();
                    srchTaskList.TASKTYPENAME = srchTaskList.CCMEMOLIST[0].CCGROUP.GROUPNAME + " Pending Request List";
                }
            }
            return PartialView("_requestListPartial", srchTaskList);
        }


        [HttpGet]
        public ActionResult SrchTicket(string srchText)
        {
            
            var srchTaskList = new TaskList();
            int enumDBType = 0;
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.PriorityType);

                srchTaskList.TASKTYPE = 5;
                srchTaskList.TASKTYPENAME = "Search CRM List";

                srchTaskList.CCMEMOLIST = oDBContext.CCMEMOMASTERs.Where(t => t.TICKETNO.Contains(srchText) || t.CUSTOMERNAME.Contains(srchText) || t.CUSTOMERCONTACT.Contains(srchText) || t.CUSTACNO.Contains(srchText) || t.CUSTCARDNO.Contains(srchText)).OrderByDescending(x => x.CREATEDON).ToList();

            return PartialView("_requestListPartial", srchTaskList);
        }
    }
}
