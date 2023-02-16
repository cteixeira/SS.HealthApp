using Newtonsoft.Json;
using Plugin.Connectivity;
using SS.HealthApp.Model.HomeModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SS.HealthApp.PCL.Services
{

    public class HomeService : _BaseService
    {
        public async Task<List<Banner>> GetBannersAsync()
        {
            List<Banner> items = new List<Banner>();

            if (CrossConnectivity.Current.IsConnected)
            {

                using (HttpClient client = await base.GetServicesHttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync("home/banners");
                    if (response.IsSuccessStatusCode)
                    {

                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        items = JsonConvert.DeserializeObject<List<Banner>>(serviceResponse.ToString());
                        
                        Repositories.BannerRepository repository = new Repositories.BannerRepository();
                        await repository.SaveContentAsync(items);

                    }
                }

            }
            else
            {
                Repositories.BannerRepository repository = new Repositories.BannerRepository();
                items = await repository.GetContentAsync();
            }

            return items;
        }

        public async Task<List<EmergencyDelay>> GetEmergencyDelayAsync()
        {

            List<EmergencyDelay> items = new List<EmergencyDelay>();

            if (CrossConnectivity.Current.IsConnected)
            {

                using (HttpClient client = await base.GetServicesHttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync("home/EmergencyWaitTime");
                    if (response.IsSuccessStatusCode)
                    {
                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        items = JsonConvert.DeserializeObject<List<EmergencyDelay>>(serviceResponse.ToString());
                    }
                }

            }

            return items;
        }

    }
}
