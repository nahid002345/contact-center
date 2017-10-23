using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OBLContactCenter.EFnClass;
using System.Data;
using OBLContactCenter.Models;
using System.Data.Entity;
namespace OBLContactCenter.Controllers
{
    [SessionExpire]
    public class CallCatTypeSettingController : AppBaseController
    {

        CCCALLCATAGORY oCCCALLCATAGORY = null;
        CCCALLTYPE oCCCALLTYPE = null;

        public ActionResult Index()
        {
            CALLCATAGORYTYPE CallCatTypeList = new CALLCATAGORYTYPE();

            CallCatTypeList.CallCatagoryList = oDBContext.CCCALLCATAGORies.ToList();
            CallCatTypeList.CallTypeList = oDBContext.CCCALLTYPEs.ToList();
            CallCatTypeList.SelectedList = 1;

            ViewBag.CallCatagoryList = oDBContext.CCCALLCATAGORies.Where(t => t.ISACTIVE).Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.NAME.ToString()
            });

            return View(CallCatTypeList);
        }

        [HttpGet]
        public ActionResult SearchTypeList(int? catagoryId, int? page)
        {
            //oCurrentUser = (CCUSER)Session["User"];
            var list = oDBContext.CCCALLTYPEs.Where(t => t.ISACTIVE ).ToList();
            if (!string.IsNullOrEmpty(catagoryId.ToString()))
                list = list.Where(t => t.CATAGORYID == catagoryId).ToList();

            return PartialView("_TypeListPartial", list);
        }

        [HttpGet]
        public ActionResult CreateCategory()
        {
            oCCCALLCATAGORY = new CCCALLCATAGORY();
            oCCCALLCATAGORY.ISACTIVE = true;

            return PartialView("_EditCatagoryPartial", oCCCALLCATAGORY);
        }

        [HttpGet]
        public ActionResult EditCategory(int id)
        {

            oCCCALLCATAGORY = oDBContext.CCCALLCATAGORies.FirstOrDefault(t => t.ID == id);

            return PartialView("_EditCatagoryPartial", oCCCALLCATAGORY);
        }

        [HttpPost]
        public ActionResult InsertEditCategory(CCCALLCATAGORY oCCCALLCATAGORY, string btnName)
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
                            oCCCALLCATAGORY.ISACTIVE = true;
                            oCCCALLCATAGORY.CREATEDBY = oCurrentUser.ID;
                            oCCCALLCATAGORY.CREATEDON = DateTime.Now;

                            oDBContext.CCCALLCATAGORies.Add(oCCCALLCATAGORY);
                            oDBContext.SaveChanges();
                            TempData["SuccessMsg"] = "Data Saved Successfully.";
                            break;
                        case "update":
                            oCCCALLCATAGORY.MODIFIEDBY = oCurrentUser.ID;
                            oCCCALLCATAGORY.MODIFIEDON = DateTime.Now;
                            oDBContext.Entry(oCCCALLCATAGORY).State = EntityState.Modified;
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

        [HttpGet]
        public ActionResult CreateType()
        {
            oCCCALLTYPE = new CCCALLTYPE();
            oCCCALLTYPE.ISACTIVE = true;

            ViewBag.CatList = oDBContext.CCCALLCATAGORies.Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.NAME.ToString()
            });


            return PartialView("_EditTypePartial", oCCCALLTYPE);
        }

        [HttpGet]
        public ActionResult EditType(int id)
        {
            ViewBag.CatList = oDBContext.CCCALLCATAGORies.Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.NAME.ToString()
            });

            oCCCALLTYPE = oDBContext.CCCALLTYPEs.FirstOrDefault(t => t.ID == id);

            return PartialView("_EditTypePartial", oCCCALLTYPE);
        }

        [HttpPost]
        public ActionResult InsertEditType(CCCALLTYPE oCCCALLTYPE, string btnName)
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
                            oCCCALLTYPE.ISACTIVE = true;
                            oCCCALLTYPE.CREATEDBY = oCurrentUser.ID;
                            oCCCALLTYPE.CREATEDON = DateTime.Now;

                            oDBContext.CCCALLTYPEs.Add(oCCCALLTYPE);
                            oDBContext.SaveChanges();
                            TempData["SuccessMsg"] = "Data Saved Successfully.";
                            break;
                        case "update":
                            oCCCALLTYPE.MODIFIEDBY = oCurrentUser.ID;
                            oCCCALLTYPE.MODIFIEDON = DateTime.Now;
                            oDBContext.Entry(oCCCALLTYPE).State = EntityState.Modified;
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
