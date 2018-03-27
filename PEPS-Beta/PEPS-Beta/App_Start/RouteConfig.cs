using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PEPS_Beta
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Index2",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index2", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Pricer",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Pricer", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "VoirIndicesParam",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "VoirIndicesParam", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "IndiceLigne",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "IndiceLigne  ", id = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "EstimerParam",
            url: "{controller}/{action}/{id}",
            defaults: new { controller = "Home", action = "EstimerParam", id = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "UpdatePortefeuille",
            url: "{controller}/{action}/{id}",
            defaults: new { controller = "Home", action = "UpdatePortefeuille", id = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "VoirCurrPort",
            url: "{controller}/{action}/{id}",
            defaults: new { controller = "Home", action = "VoirCurrPort", id = UrlParameter.Optional }
          );

            
        }
    }
}
