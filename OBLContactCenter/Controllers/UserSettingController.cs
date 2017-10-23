using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OBLContactCenter.EFnClass;
using System.Data;
using System.IO;
using System.Data.Entity;
namespace OBLContactCenter.Controllers
{
    [SessionExpire]
    public class UserSettingController : AppBaseController
    {
        //
        // GET: /UserSetting/
        CCUSER oCCUSER = null;
        CCENUMERATION oCCENUMERATION = null;
        //[CustomAuthorize(Roles = "Admin")]
        public ActionResult Index()
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

            var UserList = oDBContext.CCUSERs.ToList();
            return View(UserList);
        }

        [HttpGet]
        public ActionResult SearchUserList(string EmpId, string SrchName, int? RoleId, int? GroupId)
        {
            var srchUserList = oDBContext.CCUSERs.ToList();
            if (!string.IsNullOrEmpty(EmpId))
                srchUserList = srchUserList.Where(t => t.EMPLOYEEID == EmpId.Trim() || t.USERID == EmpId.Trim()).ToList();
            if (!string.IsNullOrEmpty(SrchName))
                srchUserList = srchUserList.Where(t => t.EMPLOYEENAME.ToLower().Contains(SrchName.ToLower().Trim())).ToList();
            if(RoleId.HasValue)
                srchUserList = srchUserList.Where(t => t.EMPLOYEEROLEID == RoleId).ToList();
            if (GroupId.HasValue)
                srchUserList = srchUserList.Where(t => t.GROUPID == GroupId).ToList();

            return PartialView("_UserListPartial", srchUserList);
        }

        public ActionResult Ticker()
        {
            int enumDBType = 0;
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.TickerMsg);
            var tickerList = oDBContext.CCENUMERATIONs.Where(t=>t.TYPE == enumDBType).ToList();
            return View("Ticker",tickerList);
        }

        [HttpGet]
        public ActionResult CreateUser()
        {
            oCCUSER = new CCUSER();
            oCCUSER.ISACTIVE = true;

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



            return PartialView("_EditUserPartial", oCCUSER);
        }

        [HttpGet]
        public ActionResult EditUser(int id)
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

            oCCUSER = oDBContext.CCUSERs.FirstOrDefault(t => t.ID == id);

            return PartialView("_EditUserPartial", oCCUSER);
        }

        [HttpPost]
        public ActionResult InsertEditUser(CCUSER oCCUser, HttpPostedFileBase ctrlImage, string btnName)
        {
            string UserImagePath = string.Empty;
            //ctrlImage = Request.Files[0];
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            try
            {
                if (ModelState.IsValid)
                {
                    oCurrentUser = (CCUSER)Session["User"];
                    if (ctrlImage != null)
                    {
                        var fileName = Path.GetFileName(ctrlImage.FileName);
                        
                    }
                    switch (btnName)
                    {
                        case "create":
                            if (ctrlImage != null)
                            {
                                var path = oCCUser.USERID+"."+Path.GetExtension(ctrlImage.FileName);
                                UserImagePath = path;
                                oCCUser.IMAGEPATH = UserImagePath;
                            }
                            oCCUser.ISACTIVE = true;
                            oCCUser.CREATEDBY = oCurrentUser.ID;
                            oCCUser.CREATEDON = DateTime.Now;
                            

                            oDBContext.CCUSERs.Add(oCCUser);
                            oDBContext.SaveChanges();
                            if (ctrlImage != null)
                            {
                                ctrlImage.SaveAs(Path.Combine(Server.MapPath("~/Content/UserImage/"), UserImagePath));
                            }

                            TempData["SuccessMsg"] = "Data Saved Successfully.";
                            break;
                        case "update":
                            if (ctrlImage != null)
                            {
                                var path = oCCUser.USERID + "." + Path.GetExtension(ctrlImage.FileName);
                                UserImagePath = path;
                                oCCUser.IMAGEPATH = UserImagePath;
                            }

                            oCCUser.MODIFIEDBY = oCurrentUser.ID;
                            oCCUser.MODIFIEDON = DateTime.Now;
                            
                            oDBContext.Entry(oCCUser).State = EntityState.Modified;
                            oDBContext.SaveChanges();
                            if (ctrlImage != null)
                            {
                                ctrlImage.SaveAs(Path.Combine(Server.MapPath("~/Content/UserImage/"), UserImagePath));
                            }
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

        public ActionResult Create(CCUSER oCCUser)
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

            oCurrentUser = (CCUSER)Session["User"];
            oCCUser.ISACTIVE = true;
            oCCUser.CREATEDBY = oCurrentUser.ID;
            oCCUser.CREATEDON = DateTime.Now;
            if (ModelState.IsValid)
            {
            try
            {
                oDBContext.CCUSERs.Add(oCCUser);
                oDBContext.SaveChanges();

                TempData["SuccessMsg"] = "Data Saved Successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = "Data is not saved due to " + ExceptionMsg(ex);
            }
            }
            else
            {
                TempData["ErrorMsg"] = "Data is not complete or in valid format.Please Check...";
            }
            return View("Create", oCCUser);
        }

        public ActionResult Profile(string Length)
        {
            Int32 uId = Convert.ToInt32(Length);
            var UserProfile=oDBContext.CCUSERs.FirstOrDefault(t=>t.ID == uId);

            return View("UserProfile", UserProfile);
        }


        [HttpGet]
        public ActionResult CreateTicker()
        {
            int enumDBType = 0;
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.TickerMsg);
            oCCENUMERATION = new CCENUMERATION();
            oCCENUMERATION.TYPE = enumDBType;
            oCCENUMERATION.ISACTIVE = true;


            return PartialView("_EditTickerPartial", oCCENUMERATION);
        }

        [HttpGet]
        public ActionResult EditTicker(int id)
        {
            int enumDBType = 0;
            enumDBType = Convert.ToInt32(Enumaretion.DBEnumType.TickerMsg);
            oCCENUMERATION = oDBContext.CCENUMERATIONs.FirstOrDefault(t =>  t.ID == id && t.TYPE == enumDBType);

            return PartialView("_EditTickerPartial", oCCENUMERATION);
        }

        [HttpPost]
        public ActionResult InsertEditTicker(CCENUMERATION oCCENUMERATION, string btnName)
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
                            oCCENUMERATION.ISACTIVE = true;
                            oCCENUMERATION.CREATEDBY = oCurrentUser.EMPLOYEEID;
                            oCCENUMERATION.CREATEDON = DateTime.Now;
                            oDBContext.CCENUMERATIONs.Add(oCCENUMERATION);
                            oDBContext.SaveChanges();
                            TempData["SuccessMsg"] = "Data Saved Successfully.";
                            break;
                        case "update":
                            oCCENUMERATION.CREATEDBY = oCurrentUser.EMPLOYEEID;
                            oCCENUMERATION.CREATEDON = DateTime.Now;
                            oDBContext.Entry(oCCENUMERATION).State = EntityState.Modified;
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
            return RedirectToAction("Ticker");
        }

        public ActionResult ChangePasswordProfile(string Id)
        {
            Int32 uId = Convert.ToInt32(Id);
            var UserProfile = oDBContext.CCUSERs.FirstOrDefault(t => t.ID == uId);

            return View("ChangePasswordProfile", UserProfile);
        }

        [HttpPost]
        public ActionResult ChangePassword(CCUSER oCCUser, string btnName)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            try
            {
                if (ModelState.IsValid)
                {
                    oCurrentUser = (CCUSER)Session["User"];
                    oCCUser.MODIFIEDBY = oCurrentUser.ID;
                    oCCUser.MODIFIEDON = DateTime.Now;

                    oDBContext.Entry(oCCUser).State = EntityState.Modified;
                    oDBContext.SaveChanges();
                    TempData["SuccessMsg"] = "Data Updated Successfully.";

                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = "Data is not saved due to " + ExceptionMsg(ex);
            }
            return Redirect("../UserSetting/ChangePasswordProfile/"+ oCCUser.ID.ToString());
        }
    }
}
