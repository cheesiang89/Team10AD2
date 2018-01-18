﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using Team10AD_Web;

namespace Team10AD_Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AuthConfig.RegisterOpenAuth();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            //Stores employee id to pass to detail page
            Session["employeeid"] = null;
            //Stores department id to pass to detail page
            Session["departmentid"] = "";
            //Stores requisition id to pass to requisition detail page
            Session["requisitiondetail"] = "";
        }
    }
}
