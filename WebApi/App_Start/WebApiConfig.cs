using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace sas_Futura
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}/{idestado}/{estado}",
                defaults: new { id = RouteParameter.Optional, idestado = RouteParameter.Optional, estado = RouteParameter.Optional }
            );


            //config.Routes.MapHttpRoute(
            //    name: "DeviceUsersApi",
            //    routeTemplate: "api/{controller}/{id}/{param2}",
            //    defaults: new { id = RouteParameter.Optional, param2 = RouteParameter.Optional }
            //);

            //var json = config.Formatters.JsonFormatter;
            //json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            //config.Formatters.Remove(config.Formatters.XmlFormatter);

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();


        }
    }
}
