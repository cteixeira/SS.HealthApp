using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Connectivity;
using SS.HealthApp.Model.UserModels;

namespace SS.HealthApp.PCL.Services
{

    public class UserService : _BaseService
    {

        public async Task<bool> LoginAsync(string Username, string Password)
        {
            try
            {
                await base.SetupServicesHttpClient(Username, Password);
            }
            catch {
                return false;
            }

            return true;
        }

        public async Task<bool> Logout() {
            try {
                await new Repositories.AuthenticationDataRepository().SaveContentAsync(new AuthenticationData());
            }
            catch {
                return false;
            }

            return true;
        }

        public async Task<bool> IsLoggedIn()
        {
            AuthenticationData authenticationData = await new Repositories.AuthenticationDataRepository().GetContentAsync();
            bool authenticationDataStored = authenticationData != null && !String.IsNullOrEmpty(authenticationData.Username) && !String.IsNullOrEmpty(authenticationData.Password);
            if (!authenticationDataStored)
            {
                return false;
            }

            //no connection to refresh the token
            if (!CrossConnectivity.Current.IsConnected)
            {
                return false;
            }

            try
            {
                //Refresh token to ensure that user pin is the same
                await base.SetupServicesHttpClient(authenticationData.Username, authenticationData.Password);
            }
            catch (Exceptions.ServicesAuthenticationException ex)
            {
                return false;
            }

            return true;
        }

        public async Task<PersonalData> GetPersonalData()
        {

            if (CrossConnectivity.Current.IsConnected)
            {

                using (HttpClient client = await base.GetServicesHttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync("user/PersonalData");
                    if (response.IsSuccessStatusCode)
                    {

                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        return JsonConvert.DeserializeObject<PersonalData>(serviceResponse.ToString());
                    }
                }
            }

            return new PersonalData();
        }

        public async Task<bool> SavePersonalData(PersonalData PersonalData)
        {
            if (CrossConnectivity.Current.IsConnected)
            {

                using (HttpClient client = await base.GetServicesHttpClient())
                {
                    HttpContent postContent = new StringContent(JsonConvert.SerializeObject(PersonalData), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("user/PersonalData/Save", postContent);

                    if (response.IsSuccessStatusCode) {
                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        return JsonConvert.DeserializeObject<bool>(serviceResponse.ToString().ToLower());
                    }
                }
            }

            return false;
        }

        public async Task<bool> ChangePassword(ChangePassword pData)
        {
            if (CrossConnectivity.Current.IsConnected) {

                using (HttpClient client = await base.GetServicesHttpClient()) {
                    HttpContent postContent = new StringContent(JsonConvert.SerializeObject(pData), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("user/PersonalData/ChangePassword", postContent);

                    if (response.IsSuccessStatusCode) {
                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        return JsonConvert.DeserializeObject<bool>(serviceResponse.ToString().ToLower());
                    }
                }
            }

            return false;
        }

    }
}
