using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using SS.HealthApp.Model.FacilityModels;
using Plugin.Connectivity;
using System.Net.Http;
using Plugin.Geolocator;

namespace SS.HealthApp.PCL.Services
{

    public class FacilityService : _BaseService<Facility>
    {
        public FacilityService()
        {
            Repository = new Repositories.FacilityRepository();
            RequestUri = "facilities";
        }

        public async Task<string> GetOriginCoordinates() {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 10; //100 is new default
            var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);
            string origin = string.Format("{0},{1}", position.Latitude.ToString().Replace(',', '.'), position.Longitude.ToString().Replace(',', '.'));
            return origin;
        }

        public async Task<int> GetTimeDistanceAsync(string destination) {

            try {
                if (CrossConnectivity.Current.IsConnected) {

                    string origin = await GetOriginCoordinates();

                    using (HttpClient client = await base.GetServicesHttpClient()) {

                        HttpResponseMessage response = await client.GetAsync(string.Format("facilities/timedistance?origin={0}&destination={1}", origin, destination));

                        if (response.IsSuccessStatusCode) {
                            Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                            return JsonConvert.DeserializeObject<int>(serviceResponse.ToString().ToLower());
                        }
                    }
                }
            }
            catch { }

            return -1;
        }
    }
}
