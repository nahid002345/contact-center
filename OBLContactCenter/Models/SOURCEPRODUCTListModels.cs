using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using OBLContactCenter.EFnClass;

namespace OBLContactCenter.Models
{
    public class SOURCEPRODUCTList
    {
        public List<CCENUMERATION> SourceList { get; set; }
        public List<CCENUMERATION> ProductList { get; set; }
        public int SelectedList { get; set; }
    }
}