using System.Threading.Tasks;
using Plugin.Connectivity;
using System.Net.Http;
using SS.HealthApp.Model.ErrorModels;
using Newtonsoft.Json;
using System.Text;
using System;

namespace SS.HealthApp.PCL.Services {

    public class ErrorService : _BaseService {

        public async Task<bool> AddAsync(Exception ex) {
            if (CrossConnectivity.Current.IsConnected) {

                using (HttpClient client = await base.GetServicesHttpClient()) {
                    var package = new ErrorMessage() { Source = ex.Source, Message = ex.ToString() };
                    HttpContent postContent = new StringContent(JsonConvert.SerializeObject(package), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("error/add", postContent);

                    if (response.IsSuccessStatusCode)
                        return true;
                    else
                        return false;
                }

            }
            return false;
        }
    }
}
