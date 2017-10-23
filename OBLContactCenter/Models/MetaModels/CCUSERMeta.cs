using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OBLContactCenter.Models.MetaModels
{

    public class CCUSERMeta
    {

        public long ID { get; set; }
        [Required(ErrorMessage = "Enter UserID/EmployeeID")]
        public string USERID { get; set; }
        [Required(ErrorMessage = "Enter Password")]
        public string USERPASSWORD { get; set; }
        public string EMPLOYEEID { get; set; }
        [Required(ErrorMessage = "Enter Employee/User Name")]
        public string EMPLOYEENAME { get; set; }
        [Required(ErrorMessage = "Enter Employee Role")]

        public string EMAIL { get; set; }
        [Required(ErrorMessage = "Enter User Role")]
        public long EMPLOYEEROLEID { get; set; }
        [Required(ErrorMessage = "Enter User Group")]
        public Nullable<long> GROUPID { get; set; }
        public bool ISACTIVE { get; set; }
        public long CREATEDBY { get; set; }
        public System.DateTime CREATEDON { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFIEDON { get; set; }
    }



}