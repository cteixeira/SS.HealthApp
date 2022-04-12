using System.Threading.Tasks;
using System.Collections.Generic;
using SS.HealthApp.Model.AppointmentModels;
using SS.HealthApp.Model;
using Plugin.Connectivity;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Linq;

namespace SS.HealthApp.PCL.Services {

    public class AppointmentService : _BaseService<Appointment> {

        private static AppointmentData ApptData;
        public static AppointmentBook ApptBook = new AppointmentBook();

        public async Task<List<Appointment>> GetItemsAsync() {
            
            Items = new List<Appointment>();

            if (CrossConnectivity.Current.IsConnected) {

                using (HttpClient client = await base.GetServicesHttpClient()) {
                    HttpResponseMessage response = await client.GetAsync("appointment/list");
                    if (response.IsSuccessStatusCode) {

                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        Items = JsonConvert.DeserializeObject<List<Appointment>>(serviceResponse.ToString());

                        var repository = new Repositories.AppointmentRepository();
                        await repository.SaveContentAsync(Items);
                    }
                }
            }
            else {
                var repository = new Repositories.AppointmentRepository();
                Items = await repository.GetContentAsync();
            }

            return Items;
        }
        
        public async Task<AppointmentData> GetAllDataAsync() {

            //this avoids calling the webservice every time because the service can take a long time
            if (ApptData != null && ApptData.Relations != null && ApptData.Relations.Count > 0)
                return ApptData;

            ApptData = new AppointmentData();

            if (CrossConnectivity.Current.IsConnected) {

                using (HttpClient client = await base.GetServicesHttpClient()) {
                    HttpResponseMessage response = await client.GetAsync("appointment/data");
                    if (response.IsSuccessStatusCode) {

                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        await Task.Run(() => {
                            ApptData = JsonConvert.DeserializeObject<AppointmentData>(serviceResponse.ToString());
                        });

                        var repository = new Repositories.AppointmentDataRepository();
                        await repository.SaveContentAsync(ApptData);
                    }
                }
            }
            else {
                var repository = new Repositories.AppointmentDataRepository();
                ApptData = await repository.GetContentAsync();
            }

            return ApptData;
        }

        public AppointmentData GetFilteredData(AppointmentBook aptBook) {

            var data = new AppointmentData();

            var query = ApptData.Relations.AsQueryable();

            query = query.Where(r => r.Service != null && r.Service.Type.ToString() == aptBook.Type.ToString());

            if (aptBook.Doctor != null)
                query = query.Where(r => r.Doctor != null && r.Doctor.ID == aptBook.Doctor.ID);

            if (aptBook.Specialty != null)
                query = query.Where(r => r.Specialty != null && r.Specialty.ID == aptBook.Specialty.ID);

            if (aptBook.Service != null)
                query = query.Where(r => r.Service != null && r.Service.ID == aptBook.Service.ID);

            if (aptBook.Facility != null)
                query = query.Where(r => r.Facility != null && r.Facility.ID == aptBook.Facility.ID);

            data.Relations = query.ToList();

            data.Doctors = data.Relations
                .Where(r => r.Doctor != null)
                .Select(r => r.Doctor)
                .GroupBy(r => r.ID)
                .Select(grp => grp.First())
                .OrderBy(r => r.Title)
                .ToList();

            data.Specialties = data.Relations
                .Where(r => r.Specialty != null)
                .Select(r => r.Specialty)
                .GroupBy(r => r.ID)
                .Select(grp => grp.First())
                .OrderBy(r => r.Title)
                .ToList();

            data.Services = data.Relations
                .Where(r => r.Service != null)
                .Select(r => r.Service)
                .GroupBy(r => r.ID)
                .Select(grp => grp.First())
                .OrderBy(r => r.Title)
                .ToList();

            data.Facilities = data.Relations
                .Where(r => r.Facility != null)
                .Select(r => r.Facility)
                .GroupBy(r => r.ID)
                .Select(grp => grp.First())
                .OrderBy(r => r.Title)
                .ToList();

            return data;
        }

        public async Task<List<PickerItem>> GetAvailableDatesAsync(AppointmentBook apptBook) {

            List<PickerItem> availableDates = new List<PickerItem>();

            if (CrossConnectivity.Current.IsConnected) {

                using (HttpClient client = await base.GetServicesHttpClient()) {

                    HttpContent postContent = new StringContent(JsonConvert.SerializeObject(apptBook), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("appointment/availdates", postContent);

                    if (response.IsSuccessStatusCode) {
                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        availableDates = JsonConvert.DeserializeObject<List<PickerItem>>(serviceResponse.ToString());

                        foreach (var item in availableDates) 
                            item.Title = System.Convert.ToDateTime(item.ID).ToString("dd/MMM/yyyy");
                    }
                }
            }

            return availableDates;
        }

        public async Task<List<PickerItem>> GetAvailableTimeAsync(AppointmentBook apptBook) {

            List<PickerItem> availableTime = new List<PickerItem>();

            if (CrossConnectivity.Current.IsConnected) {

                using (HttpClient client = await base.GetServicesHttpClient()) {

                    HttpContent postContent = new StringContent(JsonConvert.SerializeObject(apptBook), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("appointment/availtime", postContent);

                    if (response.IsSuccessStatusCode) {

                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        availableTime = JsonConvert.DeserializeObject<List<PickerItem>>(serviceResponse.ToString());

                    }
                }
            }

            return availableTime;

        }

        public async Task<List<PickerItem>> GetPayorsAsync() {

            List<PickerItem> payors = new List<PickerItem>();

            if (CrossConnectivity.Current.IsConnected) {

                using (HttpClient client = await base.GetServicesHttpClient()) {

                    HttpResponseMessage response = await client.GetAsync("appointment/payors");

                    if (response.IsSuccessStatusCode) {
                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        payors = JsonConvert.DeserializeObject<List<PickerItem>>(serviceResponse.ToString());
                    }
                }
            }

            return payors;

        }

        public async Task<bool> CancelAppointmentAsync(string appointmentID) {

            if (CrossConnectivity.Current.IsConnected) {

                using (HttpClient client = await base.GetServicesHttpClient()) {

                    HttpResponseMessage response = await client.GetAsync("appointment/cancel?id=" + appointmentID);

                    if (response.IsSuccessStatusCode) {
                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        bool result = JsonConvert.DeserializeObject<bool>(serviceResponse.ToString().ToLower());
                        if (result)
                        {
                            Model.AppointmentModels.Appointment canceledAppointment = Items.FirstOrDefault(ap => ap.ID == appointmentID);
                            if(canceledAppointment != null)
                            {
                                canceledAppointment.Status = Enum.AppointmentStatus.Closed;
                                var repository = new Repositories.AppointmentRepository();
                                await repository.SaveContentAsync(Items);
                            }
                        }
                        return result;
                    }
                }
            }

            return false;
        }

        public async Task<bool> BookNewAppointmentAsync(AppointmentBook apptBook) {

            if (CrossConnectivity.Current.IsConnected) {

                using (HttpClient client = await base.GetServicesHttpClient()) {

                    HttpContent postContent = new StringContent(JsonConvert.SerializeObject(apptBook), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("appointment/book", postContent);

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
