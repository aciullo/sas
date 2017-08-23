using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using sas_Futura.Providers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
[assembly: OwinStartup(typeof(sas_Futura.Startup))]
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace sas_Futura
{

    public class Startup : System.Web.HttpApplication
    {
        public static String IdEmpresa;
        public static string API_KEY;
        public static string FuturaConn;
        public void Configuration(IAppBuilder app)
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ConfigureOAuth(app);
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
            IdEmpresa = Convert.ToString(ConfigurationManager.AppSettings["IdEmpresa"]);
            API_KEY = Convert.ToString(ConfigurationManager.AppSettings["Api_key"]);
            FuturaConn= Convert.ToString(ConfigurationManager.ConnectionStrings["FuturaConnection"]);

            log4net.Config.XmlConfigurator.Configure();


        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                 AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                //AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(2),
                Provider = new SimpleAuthorizationServerProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

        }
    }
}