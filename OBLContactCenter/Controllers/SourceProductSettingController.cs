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
    public class SourceProductSettingController : AppBaseController
    {

        CCENUMERATION oCCENUMERATION = null;
        Int32 SourceEnumType =Convert.ToInt32(Enumaretion.DBEnumType.SourceType);
        Int32 ProductEnumType = Convert.ToInt32(Enumaretion.DBEnumType.ProductType);

        public ActionResult Index()
        {
            SOURCEPRODUCTList SourceProductList = new SOURCEPRODUCTList();

            SourceProductList.SourceList = oDBContext.CCENUMERATIONs.Where(t=>t.TYPE == SourceEnumType).ToList();
            SourceProductList.ProductList = oDBContext.CCENUMERATIONs.Where(t => t.TYPE == ProductEnumType).ToList();
            SourceProductList.SelectedList = 1;

            return View(SourceProductList);
        }

        [HttpGet]
        public ActionResult CreateSource()
        {
            oCCENUMERATION = new CCENUMERATION();
            oCCENUMERATION.TYPE = SourceEnumType;
            oCCENUMERATION.ISACTIVE = true;

            return PartialView("_EditSourcePartial", oCCENUMERATION);
        }

        [HttpGet]
        public ActionResult EditSource(int id)
        {

            oCCENUMERATION = oDBContext.CCENUMERATIONs.FirstOrDefault(t => t.ID == id);

            return PartialView("_EditSourcePartial", oCCENUMERATION);
        }

        [HttpPost]
        public ActionResult InsertEditSource(CCENUMERATION oCCENUMERATION, string btnName)
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

                            oDBContext.CCENUMERATIONs.Add(oCCENUMERATION);
                            oDBContext.SaveChanges();
                            TempData["SuccessMsg"] = "Data Saved Successfully.";
                            break;
                        case "update":
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
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult CreateProduct()
        {
            oCCENUMERATION = new CCENUMERATION();
            oCCENUMERATION.TYPE = ProductEnumType;
            oCCENUMERATION.ISACTIVE = true;

            return PartialView("_EditProductPartial", oCCENUMERATION);
        }

        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            oCCENUMERATION = oDBContext.CCENUMERATIONs.FirstOrDefault(t => t.ID == id);

            return PartialView("_EditProductPartial", oCCENUMERATION);
        }

        [HttpPost]
        public ActionResult InsertEditProduct(CCENUMERATION oCCENUMERATION, string btnName)
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
                            oDBContext.CCENUMERATIONs.Add(oCCENUMERATION);
                            oDBContext.SaveChanges();
                            TempData["SuccessMsg"] = "Data Saved Successfully.";
                            break;
                        case "update":
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
            return RedirectToAction("Index");
        }


    }
}
