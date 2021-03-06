//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OBLContactCenter.EFnClass
{
using OBLContactCenter.Models.MetaModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

[MetadataType(typeof(CCUSERMeta))]
    
    public partial class CCUSER
    {
        public CCUSER()
        {
            this.CCMEMOATTACHMENTs = new HashSet<CCMEMOATTACHMENT>();
            this.CCMEMOLOGs = new HashSet<CCMEMOLOG>();
            this.CCMEMOMASTERs = new HashSet<CCMEMOMASTER>();
        }
    
        public long ID { get; set; }
        public string USERID { get; set; }
        public string USERPASSWORD { get; set; }
        public string EMPLOYEEID { get; set; }
        public string EMPLOYEENAME { get; set; }
        public string EMAIL { get; set; }
        public long EMPLOYEEROLEID { get; set; }
        public Nullable<long> GROUPID { get; set; }
        public string IMAGEPATH { get; set; }
        public bool ISACTIVE { get; set; }
        public long CREATEDBY { get; set; }
        public System.DateTime CREATEDON { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFIEDON { get; set; }
    
        public virtual CCGROUP CCGROUP { get; set; }
        public virtual ICollection<CCMEMOATTACHMENT> CCMEMOATTACHMENTs { get; set; }
        public virtual ICollection<CCMEMOLOG> CCMEMOLOGs { get; set; }
        public virtual ICollection<CCMEMOMASTER> CCMEMOMASTERs { get; set; }
        public virtual CCROLE CCROLE { get; set; }
    }
}
