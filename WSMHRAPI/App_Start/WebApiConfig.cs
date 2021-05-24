using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WSMHRAPI
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
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            config.Formatters.Clear();
            config.Formatters.Add(new System.Net.Http.Formatting.JsonMediaTypeFormatter());


            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.Add(config.Formatters.JsonFormatter);


            config.Formatters.JsonFormatter.S‌​erializerSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                Formatting = Newtonsoft.Json.Formatting.Indented
            };

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.S‌​erializerSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                Formatting = Newtonsoft.Json.Formatting.Indented
            };

            WSM.Conn.DB.ServerName = "";
            WSM.Conn.DB.CmpID = System.Configuration.ConfigurationManager.AppSettings["CMPID"];
            WSM.Conn.DB.ServerName = System.Configuration.ConfigurationManager.AppSettings["SERVERNAME"];
            WSM.Conn.DB.UserName = System.Configuration.ConfigurationManager.AppSettings["username"];
            WSM.Conn.DB.UserPassword = WSM.Conn.DB.FuncDecryptDataServer(System.Configuration.ConfigurationManager.AppSettings["password"]);

            string dbName = null;
            int i = 0;

            foreach (string StrDBName in WSM.Conn.DB.SystemDBName)
            {

                dbName = "";

                try
                {

                    dbName = System.Configuration.ConfigurationManager.AppSettings[StrDBName];

                }
                catch { }


                WSM.Conn.DB.DBName[i] = dbName;

                i = i + 1;
            };


        }
    }
}
