using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OBLContactCenter.EFnClass;
using System.Data;
using System.Data.Entity.Validation;
using System.IO;
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

            var createList = oDBContext.CCMEMOMASTERs.Where(t => t.ISACTIVE && t.CREATEDBY == oCurrentUser.ID).ToList();
            Session["List"] = createList;

            return View(createList);
        }

        [HttpGet]
        public ActionResult SearchList(string ticktNo, string acCard, int? catagoryId,  int? actionId, int? statusId)
        {
            oCurrentUser = (CCUSER)Session["User"];
            var list = oDBContext.CCMEMOMASTERs.Where(t => t.ISACTIVE && t.CREATEDBY == oCurrentUser.ID).ToList();
            if (!string.IsNullOrEmpty(ticktNo))
                list = list.Where(t => t.TICKETNO == ticktNo.Trim()).ToList();
            if (!string.IsNullOrEmpty(acCard))
                list = list.Where(t =>(t.CUSTCARDNO != null && t.CUSTCARDNO.Contains(acCard)) ||(t.CUSTACNO != null && t.CUSTACNO.Contains(acCard))).ToList();
            if (!string.IsNullOrEmpty(catagoryId.ToString()))
                list = list.Where(t => t.CALLCATEGORY == catagoryId).ToList();
            if (!string.IsNullOrEmpty(actionId.ToString()))
                list = list.Where(t => t.LASTACTIONTYPE == actionId).ToList();
            return PartialView("_createListPartial", list);
        }

        public ActionResult CreateMemo()
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
            ViewBag.ActionList = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == enumDBType && t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
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

            return PartialView("_createFrmPartial", oCCMEMOMASTER);
        }

        public ActionResult EditMemo(int id)
        {
            oCurrentUser = (CCUSER)Session["User"];
            int enumDBType = 0;
            CCMEMOMASTER oCCMEMOMASTER = new CCMEMOMASTER();
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.FormType);
            long InboundType = oDBContext.CCENUMERATIONs.FirstOrDefault(x => x.TYPE == enumDBType && x.NAME.ToLower().Contains("inbound") && x.ISACTIVE == true).ID;

            oCCMEMOMASTER.TICKETNO = DateTime.Now.ToString("yyMMddhhmmssfff");
            oCCMEMOMASTER.MEMOTYPE = InboundType;
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.StatusType);
            ViewBag.StatusList = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == enumDBType && t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.VALUE.ToString(),
                Text = x.NAME.ToString(),
                Selected=true
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
            if (oCurrentUser.CCROLE.ROLENAME.ToLower().Contains("maker"))
                enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.InboundActionTypeMaker);
            else if (oCurrentUser.CCROLE.ROLENAME.ToLower().Contains("check"))
                enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.InboundActionTypeChecker);
            else
                enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.InboundActionTypeOther);
            ViewBag.ActionList = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == enumDBType && t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
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

            

            if (id == OutboundId)
            {
                enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.OutboundActionType);
                var actionList = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == enumDBType && t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
                {
                    Value = x.ID.ToString(),
                    Text = x.NAME.ToString()
                });
                return Json(actionList);
            }
            
            else
            {
                if(oCurrentUser.CCROLE.ROLENAME.ToLower().Contains("maker"))
                    enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.InboundActionTypeMaker);
                else if (oCurrentUser.CCROLE.ROLENAME.ToLower().Contains("check"))
                    enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.InboundActionTypeChecker);
                else
                    enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.InboundActionTypeOther);
                var actionList = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == enumDBType && t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
                {
                    Value = x.ID.ToString(),
                    Text = x.NAME.ToString()
                });
                return Json(actionList);
            }


        }

        [HttpPost]
        public JsonResult GetGroupByAction(int id)
        {

            oCurrentUser = (CCUSER)Session["User"];
            int enumDBType = 0;
            //enumDBType =Convert.ToInt32(Enumaretion.DBEnumType.InboundActionType);
            long ReferId = oDBContext.CCENUMERATIONs.FirstOrDefault(x => x.NAME.ToLower().Contains("refer") && x.ISACTIVE == true).ID;
            long ReleaseId = oDBContext.CCENUMERATIONs.FirstOrDefault(x => x.NAME.ToLower().Contains("release") && x.ISACTIVE == true).ID;
            if (id == ReferId)
            {
                var groupList = oDBContext.CCGROUPs.Where(t => t.ID != oCurrentUser.GROUPID).Select(x => x).ToList().Select(x => new SelectListItem
                {
                    Value = x.ID.ToString(),
                    Text = x.GROUPNAME.ToString()
                });
                return Json(groupList);
            }
            else if (id == ReleaseId)
            {
                var groupList = oDBContext.CCGROUPs.Where(t => t.GROUPNAME.ToLower().Contains("checker")).Select(x => x).ToList().Select(x => new SelectListItem
                {
                    Value = x.ID.ToString(),
                    Text = x.GROUPNAME.ToString()
                });
                return Json(groupList);
            }
            else
            {
                var groupList = oDBContext.CCGROUPs.Where(t => t.ID == oCurrentUser.GROUPID).Select(x => x).ToList().Select(x => new SelectListItem
                {
                    Value = x.ID.ToString(),
                    Text = x.GROUPNAME.ToString(),
                    Selected = true
                });
                return Json(groupList);
            }


        }

        public FileResult DownloadFiles(string id)
        {
            Int32 FileId = Convert.ToInt32(id);
            var fileATTACHMENT = oDBContext.CCMEMOATTACHMENTs.FirstOrDefault(t => t.ID == FileId);
            byte[] fileBytes = System.IO.File.ReadAllBytes(fileATTACHMENT.FILEPATH);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileATTACHMENT.FILENAME);
        }
    }
}
