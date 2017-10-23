﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace OBLContactCenter.Hubs
{
    public class NotificationHub : Hub
    {
        private static string conString = ConfigurationManager.ConnectionStrings["OBLCONTACTCENTEREntities"].ToString();
        public void Hello()
        {
            Clients.All.hello();
        }

        [HubMethodName("updateNotification")]
        public static void UpdateNotification()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.All.updateNotification();
        }
    }
}