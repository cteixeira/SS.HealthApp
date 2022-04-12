using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SS.HealthApp.Services
{
    public class Startup
    {

        public void Configuration(IAppBuilder app)
        {

            //Configure NLog Layout Renders 
            global::NLog.Config.ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("company-id", typeof(SS.HealthApp.Services.NLog.CompanyLayoutRender));
            global::NLog.Config.ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("user-id", typeof(SS.HealthApp.Services.NLog.UserIdLayoutRender));
            global::NLog.Config.ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("user-username", typeof(SS.HealthApp.Services.NLog.UserUsernameLayoutRender));

            //configure user authentication
            AuthenticationConfig.ConfigureOAuth(app);

            //Configure web api
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);

            //configure the connectors to each client
            SS.HealthApp.Core.ClientConnectorManager.ConfigureClientConnectors();

        }

    }
}