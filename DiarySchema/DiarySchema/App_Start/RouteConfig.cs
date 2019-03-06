using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DiarySchema
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(name: "Default2", url: "{controller}/{action}/{year}/{semesterNumber}/{group}/{subject}",
                 defaults: new {  }
            );
            routes.MapRoute(name: "Default3", url: "{controller}/{action}/{group}/{subject}/{year}/{semesterNumber}",
                 defaults: new { year = UrlParameter.Optional, semesterNumber = UrlParameter.Optional }
            );//не проверено
            routes.MapRoute(name: "Default4", url: "{controller}/{action}/{groupId}/{subjectId}/{semesterId}",
            defaults: new { }
            );
            routes.MapRoute(name: "Default", url: "{controller}/{action}",
                 defaults: new { controller = "Home", action = "Index" }
            );
            routes.MapRoute(name: "Default5", url: "{controller}/{action}/{year}/{semesterNumber}",
                 defaults: new { controller = "Home", action = "Index" }
            );
            routes.MapRoute(name: "Default1", url: "{controller}/{action}/{id}",
                 defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}
