using System;
using System.Collections.Generic;
using System.Text;

namespace Enumaretion
{

    
    public enum Month
    {
        Select = 0,
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }

    public enum DBEnumType
    {
        FormType=1,
        FormEntityType=2,
        SourceType=3,
        ProductType=4,
        StatusType=5,
        ActionType=6,
        PriorityType=7,
        TickerMsg=8

    }

    public enum DashBoardRequestType
    {
        HighPriority=1,
        HoursCrossed=2,
        Refer=3

    }
   
}
