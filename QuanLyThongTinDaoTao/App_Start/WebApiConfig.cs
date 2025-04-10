using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace QuanLyThongTinDaoTao
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Xoá formatter XML, chỉ dùng JSON
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Cấu hình JSON formatter
            var jsonFormatter = config.Formatters.JsonFormatter;
            jsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            jsonFormatter.SerializerSettings.Formatting = Formatting.Indented;

            // Cấu hình Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
