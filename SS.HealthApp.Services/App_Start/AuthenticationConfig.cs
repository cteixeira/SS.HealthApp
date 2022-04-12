using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using NLog;
using Owin;
using System;
using System.Threading.Tasks;

namespace SS.HealthApp.Services
{

    public class AuthenticationConfig
    {

        public static void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                //#if DEBUG
                //production environment should only works with https
                AllowInsecureHttp = true,
                //#endif

                TokenEndpointPath = new PathString("/api/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(60),
                Provider = new AuthorizationProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

        }

    }

    class AuthorizationProvider : OAuthAuthorizationServerProvider
    {

        string companyId;

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {

            try
            {
                string clientId, clientSecret;
                context.TryGetBasicCredentials(out clientId, out clientSecret);

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    context.SetError("invalid_clientapp", "client app is not valid");
                    context.Rejected();
                    return;
                }

                var wcSec = new SimpleSolutions.wcSecure.Decifra();
                string clientExpectedPassword = wcSec.Decifrar(System.Configuration.ConfigurationManager.AppSettings[clientId + "ClientSecret"]);
                if (clientExpectedPassword != clientSecret)
                {
                    context.SetError("invalid_clientapp_authentication", "client app authentication is not valid");
                    context.Rejected();
                    return;
                }

                companyId = clientId;

                if (string.IsNullOrEmpty(companyId))
                {
                    context.SetError("invalid_companyid", "companyid parameter is not valid");
                    context.Rejected();
                    return;
                }



                context.Validated();
            }
            catch (Exception ex)
            {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
            }

        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            try
            {
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

                SS.HealthApp.Core.Services.UserService uService = new Core.Services.UserService();

                var loginData = new Model.UserModels.Login
                {
                    Username = context.UserName,
                    Password = context.Password,
                    CompanyId = companyId
                };
                Core.Models.AuthenticatedUser authenticatedUser = uService.LoginUser(loginData);

                if (authenticatedUser == null)
                {
                    context.SetError("invalid_user_credentials", "The user name or password is incorrect.");
                    return;
                }

                context.Validated(authenticatedUser);
            }
            catch (Exception ex)
            {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
            }

        }

    }

}