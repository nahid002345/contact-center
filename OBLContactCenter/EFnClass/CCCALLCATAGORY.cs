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

[MetadataType(typeof(CCCATAGORYMeta))]
    
    public partial class CCCALLCATAGORY
    {
        public CCCALLCATAGORY()
        {
            this.CCCALLTYPEs = new HashSet<CCCALLTYPE>();
            this.CCMEMOMASTERs = new HashSet<CCMEMOMASTER>();
        }
    
        public long ID { get; set; }
        public string NAME { get; set; }
        public string REMARKS { get; set; }
        public bool ISACTIVE { get; set; }
        public long CREATEDBY { get; set; }
        public System.DateTime CREATEDON { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFIEDON { get; set; }
    
        public virtual ICollection<CCCALLTYPE> CCCALLTYPEs { get; set; }
        public virtual ICollection<CCMEMOMASTER> CCMEMOMASTERs { get; set; }
    }
}