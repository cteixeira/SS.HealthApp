using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SS.HealthApp.Services
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter("Bearer"));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.IgnoreRoute("Token", "api/Token");

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

        }
    }
}
