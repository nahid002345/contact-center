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

[MetadataType(typeof(CCMEMOMASTERMeta))]
    
    public partial class CCMEMOMASTER
    {
        public CCMEMOMASTER()
        {
            this.CCMEMOATTACHMENTs = new HashSet<CCMEMOATTACHMENT>();
            this.CCMEMOLOGs = new HashSet<CCMEMOLOG>();
        }
    
        public long ID { get; set; }
        public long MEMOTYPE { get; set; }
        public string TICKETNO { get; set; }
        public string CUSTOMERNAME { get; set; }
        public string CUSTOMERCONTACT { get; set; }
        public Nullable<int> CUSTENTITYTYPE { get; set; }
        public string CUSTACNO { get; set; }
        public string CUSTCARDNO { get; set; }
        public string REQUESTDETAILS { get; set; }
        public long CALLCATEGORY { get; set; }
        public long CALLTYPE { get; set; }
        public long SOURCETYPE { get; set; }
        public long PRODUCTTYPE { get; set; }
        public long PRIORITY { get; set; }
        public Nullable<long> ASSIGNGROUP { get; set; }
        public Nullable<long> LASTACTIONTYPE { get; set; }
        public string LASTACTIONDETAILS { get; set; }
        public bool ISOPEN { get; set; }
        public string REMARKS { get; set; }
        public bool ISACTIVE { get; set; }
        public long CREATEDBY { get; set; }
        public System.DateTime CREATEDON { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFIEDON { get; set; }
    
        public virtual CCCALLCATAGORY CCCALLCATAGORY { get; set; }
        public virtual CCCALLTYPE CCCALLTYPE { get; set; }
        public virtual CCENUMERATION CCENUMERATION { get; set; }
        public virtual CCENUMERATION CCENUMERATION1 { get; set; }
        public virtual CCENUMERATION CCENUMERATION2 { get; set; }
        public virtual CCENUMERATION CCENUMERATION3 { get; set; }
        public virtual CCENUMERATION CCENUMERATION4 { get; set; }
        public virtual CCGROUP CCGROUP { get; set; }
        public virtual ICollection<CCMEMOATTACHMENT> CCMEMOATTACHMENTs { get; set; }
        public virtual ICollection<CCMEMOLOG> CCMEMOLOGs { get; set; }
        public virtual CCUSER CCUSER { get; set; }
    }
}
