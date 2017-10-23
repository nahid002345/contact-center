using OBLContactCenter.EFnClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OBLContactCenter.Models.MetaModels
{

    public class CCROLEMeta
    {
    
        public long ID { get; set; }
        [Required(ErrorMessage = "Enter Role Name")]
        public string ROLENAME { get; set; }
        public bool ISADMIN { get; set; }
        public bool ISACTIVE { get; set; }
        public long CREATEDBY { get; set; }
        public System.DateTime CREATEDON { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFIEDON { get; set; }
    }



}