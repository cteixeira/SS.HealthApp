using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SS.HealthApp.Core.GoogleAPI {

	public class RestService : IRestService
	{

        public const string KEY = "AIzaSyA2HXGvTRVeoIHzy6D65bjY1cXnVGcw4WI";
        public const string GOOGLE_API_URL = "https://maps.googleapis.com/maps/api/distancematrix/json?origins={0}&destinations={1}&key={2}";

        HttpClient clientGoogle;

		public string distance { get; private set; }

		public RestService()
		{
			clientGoogle = new HttpClient();
			clientGoogle.MaxResponseContentBufferSize = 256000;
		}

		public async Task<int> GetDurationAsync(string origin, string destination)
		{
			var uri = new Uri(string.Format(GOOGLE_API_URL, origin, destination, KEY));
			var response = await clientGoogle.GetAsync(uri);
			if (response.IsSuccessStatusCode) {
				var content = await response.Content.ReadAsStringAsync();
				DistanceResponse distance = JsonConvert.DeserializeObject<DistanceResponse>(content.ToString());
                if(distance.Rows.Length > 0 && distance.Rows[0].Elements.Length > 0 && distance.Rows[0].Elements[0].Duration != null)
				    return distance.Rows[0].Elements[0].Duration.Value;
			}
			return -1;
		}

		public async Task<int> GetDistanceAsync(string origin, string destination)
		{
			var uri = new Uri(string.Format(GOOGLE_API_URL, origin, destination, KEY));
			var response = await clientGoogle.GetAsync(uri);
			if (response.IsSuccessStatusCode) {
				var content = await response.Content.ReadAsStringAsync();
				DistanceResponse distance = JsonConvert.DeserializeObject<DistanceResponse>(content.ToString());
                if (distance.Rows.Length > 0 && distance.Rows[0].Elements.Length > 0 && distance.Rows[0].Elements[0].Distance != null)
                    return distance.Rows[0].Elements[0].Distance.Value;
            }
			return -1;
		}



	}
}
