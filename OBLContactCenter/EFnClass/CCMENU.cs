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
    using System;
    using System.Collections.Generic;
    
    public partial class CCMENU
    {
        public CCMENU()
        {
            this.CCMENU1 = new HashSet<CCMENU>();
            this.CCROLEMENUs = new HashSet<CCROLEMENU>();
        }
    
        public long ID { get; set; }
        public string NAME { get; set; }
        public string ACTION { get; set; }
        public string CONTROLLER { get; set; }
        public Nullable<bool> ISPARENT { get; set; }
        public Nullable<long> PARENTID { get; set; }
        public Nullable<int> ORDERNO { get; set; }
        public string ICONTEXT { get; set; }
        public bool ISACTIVE { get; set; }
        public System.DateTime CREATEDON { get; set; }
        public string CREATEDBY { get; set; }
    
        public virtual ICollection<CCMENU> CCMENU1 { get; set; }
        public virtual CCMENU CCMENU2 { get; set; }
        public virtual ICollection<CCROLEMENU> CCROLEMENUs { get; set; }
    }
}