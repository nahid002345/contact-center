using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ExpressiveAnnotations.Attributes;

namespace OBLContactCenter.Models.MetaModels
{

    public class CCMEMOMASTERMeta
    {

        public long ID { get; set; }
        [Required(ErrorMessage = "Select Memo Type")]
        public long MEMOTYPE { get; set; }        
        public string TICKETNO { get; set; }
        [Required(ErrorMessage = "Enter Customer Name")]
        [DataType(DataType.Text)]
        public string CUSTOMERNAME { get; set; }
        [RegularExpression("([0-9]+)", ErrorMessage = "Only Numbers are allowed ")]
        public string CUSTOMERCONTACT { get; set; }
        public Nullable<int> CUSTENTITYTYPE { get; set; }
        [RequiredIf("CUSTCARDNO == null", ErrorMessage = "Enter AC No. / Card No.")]
        [RegularExpression("([0-9]+)",ErrorMessage="Only Numbers are allowed ")]
        public string CUSTACNO { get; set; }
        [RequiredIf("CUSTACNO == null", ErrorMessage = "Enter AC No. / Card No.")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Only Numbers are allowed ")]
        public string CUSTCARDNO { get; set; }
        [Required(ErrorMessage = "Enter Customer Request Detail")]
        [StringLength(1000)]
        public string REQUESTDETAILS { get; set; }
        [Required(ErrorMessage = "Select Call Catagory")]
        public long CALLCATEGORY { get; set; }
        [Required(ErrorMessage = "Select Call Type")]
        public long CALLTYPE { get; set; }
        [Required(ErrorMessage = "Select Source")]
        public long SOURCETYPE { get; set; }

        [Required(ErrorMessage = "Select Product")]
        public long PRODUCTTYPE { get; set; }
        [Required(ErrorMessage = "Select Priority")]
        public long PRIORITY { get; set; }
        [Required(ErrorMessage = "Select Group")]
        public Nullable<long> ASSIGNGROUP { get; set; }
        [Required(ErrorMessage = "Select Action Type")]
        public Nullable<long> LASTACTIONTYPE { get; set; }
        [StringLength(1000)]
        public string LASTACTIONDETAILS { get; set; }
        public bool ISOPEN { get; set; }
        public string REMARKS { get; set; }
        public bool ISACTIVE { get; set; }
        public long CREATEDBY { get; set; }
        public System.DateTime CREATEDON { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFIEDON { get; set; }

    }

}