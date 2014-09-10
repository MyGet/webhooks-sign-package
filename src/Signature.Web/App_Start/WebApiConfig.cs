using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using Signature.Web.Controllers;
using Signature.Web.Models;

namespace Signature.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Formatters.Add(
     new TypedJsonMediaTypeFormatter(
         typeof(WebHookEvent),
         new MediaTypeHeaderValue(
             "application/vnd.myget.webhooks.v1.preview+json")));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
