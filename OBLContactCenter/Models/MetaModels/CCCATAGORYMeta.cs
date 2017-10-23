using OBLContactCenter.EFnClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OBLContactCenter.Models.MetaModels
{

    public class CCCATAGORYMeta
    {
    
        public long ID { get; set; }
        [Required(ErrorMessage = "Enter Catagory Name")]
        public string NAME { get; set; }
        public string REMARKS { get; set; }
        public bool ISACTIVE { get; set; }
        public long CREATEDBY { get; set; }
        public System.DateTime CREATEDON { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFIEDON { get; set; }
    }

    public class CCCALLTYPEMeta
    {

        public long ID { get; set; }
        [Required(ErrorMessage = "Select Call Catagory")]
        public long CATAGORYID { get; set; }
        [Required(ErrorMessage = "Enter Call Type Name")]
        public string TYPENAME { get; set; }
        public string REMARKS { get; set; }
        public bool ISACTIVE { get; set; }
        public long CREATEDBY { get; set; }
        public System.DateTime CREATEDON { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFIEDON { get; set; }
    }



}