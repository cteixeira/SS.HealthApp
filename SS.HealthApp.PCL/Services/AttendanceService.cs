using Newtonsoft.Json;
using Plugin.Connectivity;
using SS.HealthApp.Model.AttendanceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SS.HealthApp.PCL.Services
{
    public class AttendanceService : _BaseService
    {
        public async Task<Ticket> GetNextTicketAsync()
        {

            if (CrossConnectivity.Current.IsConnected)
            {

                using (HttpClient client = await base.GetServicesHttpClient())
                {

                    HttpResponseMessage response = await client.GetAsync("attendance/nextticket");

                    if (response.IsSuccessStatusCode)
                    {
                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        return JsonConvert.DeserializeObject<Ticket>(serviceResponse.ToString().ToLower());
                    }
                }
            }

            return null;

        }

        public async Task<CheckInResult> CheckInAsync(string appointmentID) {

            if (CrossConnectivity.Current.IsConnected)
            {

                using (HttpClient client = await base.GetServicesHttpClient())
                {

                    HttpResponseMessage response = await client.GetAsync("attendance/checkin?id=" + appointmentID);

                    if (response.IsSuccessStatusCode)
                    {
                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        return JsonConvert.DeserializeObject<CheckInResult>(serviceResponse.ToString().ToLower());
                    }
                }
            }

            return null;

        }

        public async Task<bool> RateServiceAsync(string appointmentID, int rate)
        {

            if (CrossConnectivity.Current.IsConnected)
            {

                using (HttpClient client = await base.GetServicesHttpClient())
                {

                    HttpResponseMessage response = await client.GetAsync(String.Format("attendance/rateservice?id={0}&rate={1}",appointmentID, rate));

                    if (response.IsSuccessStatusCode)
                    {
                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        return JsonConvert.DeserializeObject<bool>(serviceResponse.ToString().ToLower());
                    }
                }
            }

            return false;

        }

        public async Task<string> DownloadParkingQRCodeAsync(string appointmentID, string saveFilePath = null)
        {
            if (CrossConnectivity.Current.IsConnected)
            {

                using (HttpClient client = await base.GetServicesHttpClient())
                {

                    HttpResponseMessage response = await client.GetAsync("attendance/parkingqrcode?id=" + appointmentID);

                    if (response.IsSuccessStatusCode)
                    {
                        byte[] content = await response.Content.ReadAsByteArrayAsync();
                        return await new Repositories.ParkingQrCodeRepository().SaveContentAsync(content, string.Format("Ticket{0}.png", appointmentID.Replace("||", "_")), saveFilePath);
                    }
                }
            }

            return null;
        }

    }
}
