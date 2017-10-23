using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OBLContactCenter.Models
{
    public class LogIn
    {

        public long ID { get; set; }
        [Required(ErrorMessage = "Enter UserID/EmployeeID")]
        public string USERID { get; set; }

        [Required(ErrorMessage = "Enter Password")]
        [DataType(DataType.Password)]
        public string USERPASSWORD { get; set; }
    }
}