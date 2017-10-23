using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using OBLContactCenter.EFnClass;

namespace OBLContactCenter.Models
{
    public class ReportViewModel
    {
        public string ReportName { get; set; }
        public string ReportPath { get; set; }

    }
}