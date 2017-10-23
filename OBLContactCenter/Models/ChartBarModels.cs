using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OBLContactCenter.Models
{
    public class ChartActionTypeMemoCount
    {

        public long ActionTypeID { get; set; }
        public string ActionName { get; set; }
        public Int32 ActionMemoCount { get; set; }
    }

    public class ChartDailyRequestCount
    {

        public DateTime? requestDate { get; set; }
        public Int32 MemoCount { get; set; }
    }

    public class UserwiseTicketStatus
    {
        public string UserName { get; set; }
        public long ftr { get; set; }
        public long Open { get; set; }
        public long OnProcess { get; set; }
        public long CallBackRequired { get; set; }
        public long CallBackDone { get; set; }
        public long MailSent { get; set; }
        public long Refer { get; set; }
        public long Resolve { get; set; }
        public long CallReached { get; set; }
        public long CallUnreached { get; set; }
        public long FollowUp { get; set; }

    }

    public class ActionCount
    {
        public string TicketAction { get; set; }
        public Int32 MemoCount { get; set; }
    }
}