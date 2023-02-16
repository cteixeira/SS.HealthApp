using System.Threading.Tasks;
using System.Collections.Generic;
using SS.HealthApp.Model.DeclarationModels;
using Plugin.Connectivity;
using System.Net.Http;
using Newtonsoft.Json;

namespace SS.HealthApp.PCL.Services {

    public class DeclarationService : _BaseService<PresenceDeclaration> {

        public DeclarationService()
        {
            Repository = new Repositories.DeclarationRepository();
            RequestUri = "presencedeclaration";
        }

        public async Task<bool> SendPresenceDeclarationAsync(string id) {

            if (CrossConnectivity.Current.IsConnected) {

                using (HttpClient client = await base.GetServicesHttpClient()) {

                    HttpResponseMessage response = await client.GetAsync("presencedeclaration/send?id=" + id);

                    if (response.IsSuccessStatusCode) {
                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        return JsonConvert.DeserializeObject<bool>(serviceResponse.ToString().ToLower());
                    }
                }
            }

            return false;
        }

        public async Task<string> DownloadPresenceDeclarationAsync(string id, string saveFilePath = null)
        {

            if (CrossConnectivity.Current.IsConnected)
            {

                using (HttpClient client = await base.GetServicesHttpClient())
                {

                    HttpResponseMessage response = await client.GetAsync("presencedeclaration/download?id=" + id);

                    if (response.IsSuccessStatusCode) {
                        byte[] content = await response.Content.ReadAsByteArrayAsync();
                        return await new Repositories.DeclarationRepository().SaveContentAsync(content, string.Format("{0}.pdf", id), saveFilePath);
                    }
                }
            }

            return null;
        }

        public async Task<bool> SendPresenceDeclarationByAppointmentIdAsync(string appointmentId)
        {

            if (CrossConnectivity.Current.IsConnected)
            {

                using (HttpClient client = await base.GetServicesHttpClient())
                {

                    HttpResponseMessage response = await client.GetAsync("presencedeclaration/sendbyappointmentid?AppointmentId=" + appointmentId);

                    if (response.IsSuccessStatusCode)
                    {
                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        return JsonConvert.DeserializeObject<bool>(serviceResponse.ToString().ToLower());
                    }
                }
            }

            return false;
        }

        public async Task<string> DownloadPresenceDeclarationByAppointmentIdAsync(string appointmentId, string saveFilePath = null)
        {

            if (CrossConnectivity.Current.IsConnected)
            {

                using (HttpClient client = await base.GetServicesHttpClient())
                {

                    HttpResponseMessage response = await client.GetAsync("presencedeclaration/downloadbyappointmentid?AppointmentId=" + appointmentId);

                    if (response.IsSuccessStatusCode)
                    {
                        byte[] content = await response.Content.ReadAsByteArrayAsync();
                        return await new Repositories.DeclarationRepository().SaveContentAsync(content, string.Format("appt{0}.pdf", appointmentId), saveFilePath);
                    }
                }
            }

            return null;
        }

    }
}
