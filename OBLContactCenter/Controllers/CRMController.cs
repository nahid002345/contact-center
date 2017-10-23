using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OBLContactCenter.EFnClass;
using System.Data;
using System.Data.Entity.Validation;
using System.IO;
using System.Globalization;
namespace OBLContactCenter.Controllers
{
    [SessionExpire]
    public class CRMController : AppBaseController
    {


        public ActionResult Index()
        {
            oCurrentUser = (CCUSER)Session["User"];
            int enumDBType = 0;

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

            var createList = oDBContext.CCMEMOMASTERs.Where(t => t.ISACTIVE && t.CREATEDBY == oCurrentUser.ID ).OrderByDescending(t => t.ID).ToList();
            Session["List"] = createList;

            return View(createList);
        }

        [HttpGet]
        public ActionResult SearchList(string ticktNo, string acCard, int? catagoryId, int? groupId, int? actionId, int? statusId, string from, string to, string custName, string contact, int? callTypeId)
        {
            oCurrentUser = (CCUSER)Session["User"];
            var list = oDBContext.CCMEMOMASTERs.Where(t => t.ISACTIVE && t.CREATEDBY == oCurrentUser.ID).ToList();
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
                list = list.Where(t => t.CUSTOMERCONTACT.Contains(contact)).ToList();
            }
            if (!string.IsNullOrEmpty(callTypeId.ToString()))
            {
                list = list.Where(t => t.CALLTYPE == callTypeId.Value).ToList();
            }
            return PartialView("_createListPartial", list);
        }

        public ActionResult CreateMemo()
        {
            oCurrentUser = (CCUSER)Session["User"];
            int enumDBType = 0;
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.FormType);
            long InboundType = oDBContext.CCENUMERATIONs.FirstOrDefault(x => x.TYPE == enumDBType && x.NAME.ToLower().Contains("inbound") && x.ISACTIVE == true).ID;

            CCMEMOMASTER oCCMEMOMASTER = new CCMEMOMASTER();

            oCCMEMOMASTER.TICKETNO = DateTime.Now.ToString("yyMMddhhmmssfff");

            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.StatusType);
            ViewBag.StatusList = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == enumDBType && t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.VALUE.ToString(),
                Text = x.NAME.ToString()
            });

            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.FormType);
            ViewBag.MemoTypeList = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == enumDBType && t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.NAME.ToString()
            });
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.PriorityType);
            ViewBag.PriorityList = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == enumDBType && t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.NAME.ToString()
            });

            ViewBag.CallCatagoryList = oDBContext.CCCALLCATAGORies.Where(t => t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.NAME.ToString()
            });

            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.SourceType);
            ViewBag.SourceList = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == enumDBType && t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.NAME.ToString()
            });
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.ProductType);
            ViewBag.ProductList = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == enumDBType && t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.NAME.ToString()
            });
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.ActionType);
            //ViewBag.ActionList = oDBContext.CCENUMERATIONs.Where(t => t.CCACTIONMAPs.Where(p => p.ROLEID == oCurrentUser.EMPLOYEEROLEID && p.MEMOTYPEID == InboundType) != null && t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            ViewBag.ActionList = oDBContext.CCACTIONMAPs.Where(p => p.ROLEID == oCurrentUser.EMPLOYEEROLEID && p.MEMOTYPEID == InboundType && p.ISACTIVE == true).Select(x => x.CCENUMERATION).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.NAME.ToString()
            });
            oCCMEMOMASTER.ASSIGNGROUP = oCurrentUser.GROUPID;
            ViewBag.AssignGroupList = oDBContext.CCGROUPs.Where(t =>t.ID == oCurrentUser.GROUPID && t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.GROUPNAME.ToString(),
                Selected = true,
            });
            oCCMEMOMASTER.ISOPEN = true;
            return PartialView("_createFrmPartial", oCCMEMOMASTER);
        }

        public ActionResult EditMemo(int id)
        {
            oCurrentUser = (CCUSER)Session["User"];
            int enumDBType = 0;
            CCMEMOMASTER oCCMEMOMASTER = new CCMEMOMASTER();

            oCCMEMOMASTER.TICKETNO = DateTime.Now.ToString("yyMMddhhmmssfff");
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.StatusType);
            ViewBag.StatusList = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == enumDBType && t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.VALUE.ToString(),
                Text = x.NAME.ToString()
            });

            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.FormType);
            ViewBag.MemoTypeList = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == enumDBType && t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.NAME.ToString()
            });
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.PriorityType);
            ViewBag.PriorityList = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == enumDBType && t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.NAME.ToString()
            });

            ViewBag.CallCatagoryList = oDBContext.CCCALLCATAGORies.Where(t => t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.NAME.ToString()
            });

            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.SourceType);
            ViewBag.SourceList = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == enumDBType && t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.NAME.ToString()
            });
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.ProductType);
            ViewBag.ProductList = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == enumDBType && t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.NAME.ToString()
            });
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
            return PartialView("_createFrmPartial", oCCMEMOMASTER);
        }

        [HttpPost]
        public ActionResult SubmitMemo(CCMEMOMASTER objMemoBind, List<HttpPostedFileBase> ctrlFile)
        {
            try
            {
                oCurrentUser = (CCUSER)Session["User"];
                if (ModelState.IsValid)
                {

                    objMemoBind.ISACTIVE = true;
                    objMemoBind.CREATEDBY = oCurrentUser.ID;
                    objMemoBind.CREATEDON = DateTime.Now;
                    oDBContext.CCMEMOMASTERs.Add(objMemoBind);
                    oDBContext.SaveChanges();

                    CCMEMOLOG oCCMEMOLOG = new CCMEMOLOG();
                    oCCMEMOLOG.MEMOMASTERID = objMemoBind.ID;
                    oCCMEMOLOG.ACTEDBY = oCurrentUser.ID;
                    oCCMEMOLOG.ACTIONTYPE = objMemoBind.LASTACTIONTYPE.Value;
                    oCCMEMOLOG.ACTIONDETAILS = objMemoBind.LASTACTIONDETAILS;
                    oCCMEMOLOG.ASSIGNGROUP = objMemoBind.ASSIGNGROUP.Value;

                    oCCMEMOLOG.ISACTIVE = true;
                    oCCMEMOLOG.CREATEDBY = oCurrentUser.ID;
                    oCCMEMOLOG.CREATEDON = DateTime.Now;

                    oDBContext.CCMEMOLOGs.Add(oCCMEMOLOG);
                    oDBContext.SaveChanges();

                    var uploadFiles = Request.Files;

                    if (ctrlFile != null && ctrlFile.Count > 0)
                    {
                        foreach (HttpPostedFileBase oFile in ctrlFile)
                        {
                            if (oFile != null && oFile.ContentLength > 0)
                            {
                                var fileName = Path.GetFileName(oFile.FileName);
                                var path = Path.Combine(Server.MapPath("~/UploadDoc/"), objMemoBind.TICKETNO + "_" + fileName);
                                oFile.SaveAs(path);

                                CCMEMOATTACHMENT oCCMEMOATTACHMENT = new CCMEMOATTACHMENT();
                                oCCMEMOATTACHMENT.MEMOMASTERID = objMemoBind.ID;
                                oCCMEMOATTACHMENT.FILENAME = objMemoBind.TICKETNO + "_" + fileName;
                                oCCMEMOATTACHMENT.FILEPATH = path;

                                oCCMEMOATTACHMENT.CREATEDBY = oCurrentUser.ID;
                                oCCMEMOATTACHMENT.CREATEDON = DateTime.Now;

                                oDBContext.CCMEMOATTACHMENTs.Add(oCCMEMOATTACHMENT);
                                oDBContext.SaveChanges();
                            }
                        }
                    }
                    //oDBContext.SaveChanges();
                    TempData["SuccessMsg"] = "Data Saved Successfully.";
                }
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);
                TempData["ErrorMsg"] = "Data is not saved due to " + ExceptionMsg(ex);
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult ViewRequest(string requestId)
        {
            Int32 dataId = Convert.ToInt32(requestId);
            var requestView = oDBContext.CCMEMOMASTERs.FirstOrDefault(t => t.ID == dataId);

            return PartialView("_vwFrmPartial", requestView);
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
        public JsonResult GetActionByMemoType(int id)
        {

            oCurrentUser = (CCUSER)Session["User"];
            int enumDBType = 0;
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.FormType);
            long InboundType = oDBContext.CCENUMERATIONs.FirstOrDefault(x => x.TYPE == enumDBType && x.NAME.ToLower().Contains("inbound") && x.ISACTIVE == true).ID;
            long OutboundId = oDBContext.CCENUMERATIONs.FirstOrDefault(x => x.TYPE == enumDBType && x.NAME.ToLower().Contains("outbound") && x.ISACTIVE == true).ID;

            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.ActionType);
            var actionList = oDBContext.CCACTIONMAPs.Where(p => p.ROLEID == oCurrentUser.EMPLOYEEROLEID && p.MEMOTYPEID == id && p.ISACTIVE == true).Select(x => x.CCENUMERATION).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.NAME.ToString()
            });
            return Json(actionList);

            //if (id == OutboundId)
            //{
            //    enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.ActionType);
            //    var actionList = oDBContext.CCACTIONMAPs.Where(p => p.ROLEID == oCurrentUser.EMPLOYEEROLEID && p.MEMOTYPEID == id && p.ISACTIVE == true).Select(x => x.CCENUMERATION).ToList().Select(x => new SelectListItem
            //    {
            //        Value = x.ID.ToString(),
            //        Text = x.NAME.ToString()
            //    });
            //    return Json(actionList);
            //}

            //else
            //{
            //    enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.ActionType);
            //    var actionList = oDBContext.CCACTIONMAPs.Where(p => p.ROLEID == oCurrentUser.EMPLOYEEROLEID && p.MEMOTYPEID == id && p.ISACTIVE == true).Select(x => x.CCENUMERATION).ToList().Select(x => new SelectListItem
            //    {
            //        Value = x.ID.ToString(),
            //        Text = x.NAME.ToString()
            //    });
            //    return Json(actionList);
            //}


        }

        //[HttpPost]
        //public JsonResult GetGroupByAction(int id)
        //{

        //    oCurrentUser = (CCUSER)Session["User"];
        //    int enumDBType = 0;
        //    enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.ActionType);
        //    long referActionId = oDBContext.CCENUMERATIONs.FirstOrDefault(t => t.TYPE == enumDBType && t.NAME.ToLower().Contains("refer")).ID;
        //    if (id == referActionId)
        //    {
        //        var groupList = oDBContext.CCGROUPs.Where(t => t.ID != oCurrentUser.GROUPID).Select(x => x).ToList().Select(x => new SelectListItem
        //        {
        //            Value = x.ID.ToString(),
        //            Text = x.GROUPNAME.ToString()
        //        });
        //        return Json(groupList);
        //    }
        //    else if (id == referActionId)
        //    {
        //        var groupList = oDBContext.CCGROUPs.Where(t => t.GROUPNAME.ToLower().Contains("checker")).Select(x => x).ToList().Select(x => new SelectListItem
        //        {
        //            Value = x.ID.ToString(),
        //            Text = x.GROUPNAME.ToString()
        //        });
        //        return Json(groupList);
        //    }
        //    else
        //    {
        //        var groupList = oDBContext.CCGROUPs.Where(t => t.ID == oCurrentUser.GROUPID).Select(x => x).ToList().Select(x => new SelectListItem
        //        {
        //            Value = x.ID.ToString(),
        //            Text = x.GROUPNAME.ToString(),
        //            Selected = true
        //        });
        //        return Json(groupList);
        //    }


        //}

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
                var actionGroup= new SelectListItem();
                actionGroup.Value= oCurrentUser.CCGROUP.ID.ToString();
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

        [HttpGet]
        public ActionResult GetCustomerReqHistoryList(string acNo, string cardNo)
        {
            var custReqhis = oDBContext.CCMEMOMASTERs.Where(t => t.ISACTIVE && (t.CUSTCARDNO == cardNo || t.CUSTACNO == acNo)).ToList();
            return PartialView("_customerPreRequestListPartial", custReqhis);
        }

        [HttpPost]
        public JsonResult GetCustomerReqHistoryCount(string acNo, string cardNo)
        {
            var custReqhis = oDBContext.CCMEMOMASTERs.Where(t => t.ISACTIVE && (t.CUSTCARDNO == cardNo || t.CUSTACNO == acNo)).ToList().Count;
            return Json(custReqhis);
        }
    }
}
