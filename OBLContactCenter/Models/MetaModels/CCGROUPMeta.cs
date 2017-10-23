using OBLContactCenter.EFnClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OBLContactCenter.Models.MetaModels
{

    public class CCGROUPMeta
    {
    
        public long ID { get; set; }
        [Required(ErrorMessage = "Enter Group Name")]
        public string GROUPNAME { get; set; }
        public string EMAIL { get; set; }
        public string REMARKS { get; set; }
        public bool ISACTIVE { get; set; }
        public long CREATEDBY { get; set; }
        public System.DateTime CREATEDON { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFIEDON { get; set; }
    }



}