using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Client;
using SS.HealthApp.Model.UserModels;

namespace SS.HealthApp.PCL.Services
{

    public abstract class _BaseService
    {
        #region fields

        private AuthenticationData authenticationData = null;

        #endregion

        #region SS.HealthApp.Services connection

        protected async Task SetupServicesHttpClient(string Username, string Password)
        {

            await GetTokenFromOAuthServer(Username, Password);
            //at this stage the authentication data and token are saved in filesystem

        }

        protected async Task<HttpClient> GetServicesHttpClient()
        {
            string token = await GetValidToken();

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpClient.BaseAddress = new Uri(Settings.ServicesBaseUrl);
            return httpClient;

        }

        private async Task<string> GetValidToken()
        {
            //check if token is in memory
            if (authenticationData == null || String.IsNullOrEmpty(authenticationData.AccessToken))
            {
                //There is no token on memory. Get Token From Database
                authenticationData = await new Repositories.AuthenticationDataRepository().GetContentAsync();
            }

            //check if token is expired
            if (String.IsNullOrEmpty(authenticationData.AccessToken) || authenticationData.TokenIsEspired())
            {
                //no access token on the database or the token is expired
                authenticationData = await GetTokenFromOAuthServer(authenticationData.Username, authenticationData.Password);
            }

            return authenticationData.AccessToken;
        }

        private async Task<AuthenticationData> GetTokenFromOAuthServer(string Username, string Password)
        {

            OAuth2Client oAuth2Client = new OAuth2Client(new Uri(Settings.TokenEndpointUrl), Settings.CompanyID, Settings.CompanySecret);
            TokenResponse tokenResponse = null;
            tokenResponse = await oAuth2Client.RequestResourceOwnerPasswordAsync(Username, Password);

            if (tokenResponse.IsError)
            {
                if (tokenResponse.Error.ToLower() == "invalid_clientapp" ||
                    tokenResponse.Error.ToLower() == "invalid_clientapp_authentication"
                    || tokenResponse.Error.ToLower() == "invalid_companyid")
                {
                    throw new Exceptions.AppAuthenticationException(tokenResponse.Error);
                }
                if (tokenResponse.Error.ToLower() == "invalid_user_credentials")
                {
                    throw new Exceptions.UserAuthenticationException(tokenResponse.Error);
                }

                throw new Exception(String.Format("An error occured obtaining token: {0}", tokenResponse.Error));
            }

            var authData = new AuthenticationData
            {
                Username = Username,
                Password = Password,
                AccessToken = tokenResponse.AccessToken,
                TokenIssuedDate = DateTime.Now,
                TokenExpiresIn = TimeSpan.FromSeconds(tokenResponse.ExpiresIn)
            };

            //save authentication data to filesystem
            await new Repositories.AuthenticationDataRepository().SaveContentAsync(authData);

            //return authentication data with valid acess token
            return authData;

        }

        #endregion

    }

    public abstract class _BaseService<T> : _BaseService where T : SS.HealthApp.Model._BaseModel {

        #region properties

        protected static List<T> Items { get; set; }

        #endregion

        public T GetItem(string ID)
        {
            return Items != null ? Items.Find(n => n.ID == ID) : null;
        }

    }
}
