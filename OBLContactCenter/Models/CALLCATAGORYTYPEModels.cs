using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using OBLContactCenter.EFnClass;

namespace OBLContactCenter.Models
{
    public class CALLCATAGORYTYPE
    {
        public List<CCCALLCATAGORY> CallCatagoryList { get; set; }
        public List<CCCALLTYPE> CallTypeList { get; set; }
        public int SelectedList { get; set; }
    }
}