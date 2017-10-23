using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OBLContactCenter.EFnClass;
using OBLContactCenter.Models;

namespace OBLContactCenter.Controllers
{
    [SessionExpire]
    public class ReportController : Controller
    {
        
        public ActionResult Index()
        {
            ReportViewModel oReportViewModel = new ReportViewModel();
            oReportViewModel.ReportName = "Contact Center Detail Report";
            oReportViewModel.ReportPath = "/RptOBLContactCenter/RptDetailReport";
            return View(oReportViewModel);
        }

        public ActionResult SummaryReport()
        {
            ReportViewModel oReportViewModel = new ReportViewModel();
            oReportViewModel.ReportName = "Contact Center Summary Report";
            oReportViewModel.ReportPath = "/RptOBLContactCenter/RptInformationSummary";
            return View("Index",oReportViewModel);
        }

    }
}
