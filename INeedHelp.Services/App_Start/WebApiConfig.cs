using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace INeedHelp.Services
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();

            config.Routes.MapHttpRoute(
       name: "RequestsApi",
       routeTemplate: "api/requests/{action}/{id}",
       defaults: new { controller = "requests", id = RouteParameter.Optional }
   );
            
            config.Routes.MapHttpRoute(
                name: "RequestsWidhIdApi",
                routeTemplate: "api/requests/{requestId}/{action}/{id}",
                defaults: new { controller = "requests",  
                    id = RouteParameter.Optional,
                                requestId = RouteParameter.Optional
                });



            config.Routes.MapHttpRoute(
                   name: "FriendsApi",
                   routeTemplate: "api/friends/{action}/{id}",
                   defaults: new { controller = "friends", id = RouteParameter.Optional }
               );

            config.Routes.MapHttpRoute(
                   name: "UserApi",
                   routeTemplate: "api/users/{action}/{id}",
                   defaults: new { controller = "users", id = RouteParameter.Optional }
               );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            config.EnableSystemDiagnosticsTracing();
        }
    }
}
