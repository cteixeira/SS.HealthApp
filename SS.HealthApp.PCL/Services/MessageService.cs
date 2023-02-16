using System.Threading.Tasks;
using System.Collections.Generic;
using Plugin.Connectivity;
using System.Net.Http;
using SS.HealthApp.Model.MessageModels;
using Newtonsoft.Json;
using SS.HealthApp.Model;
using System.Text;

namespace SS.HealthApp.PCL.Services {

    public class MessageService : _BaseService<Message> {

        public MessageService()
        {
            Repository = new Repositories.MessageRepository();
            RequestUri = "message";
        }

        public async Task<List<Message>> OpenItemAsync(string id) {

            if (CrossConnectivity.Current.IsConnected) {

                using (HttpClient client = await base.GetServicesHttpClient()) {

                    HttpResponseMessage response = await client.GetAsync("message/open?id=" + id);

                    if (response.IsSuccessStatusCode) {
                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        return JsonConvert.DeserializeObject<List<Message>>(serviceResponse.ToString());
                    }
                }
            }

            return null;
        }

        public async Task<List<PickerItem>> GetSubjectsAsync() {
            List<PickerItem> subjects = new List<PickerItem>();

            if (CrossConnectivity.Current.IsConnected) {

                using (HttpClient client = await base.GetServicesHttpClient()) {

                    HttpResponseMessage response = await client.GetAsync("message/subjects");

                    if (response.IsSuccessStatusCode) {
                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        subjects = JsonConvert.DeserializeObject<List<PickerItem>>(serviceResponse.ToString());
                    }
                }
            }
            return subjects;
        }

        public async Task<List<PickerItem>> GetRecipientsAsync() {
            List<PickerItem> recipients = new List<PickerItem>();

            if (CrossConnectivity.Current.IsConnected) {

                using (HttpClient client = await base.GetServicesHttpClient()) {

                    HttpResponseMessage response = await client.GetAsync("message/recipients");

                    if (response.IsSuccessStatusCode) {
                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        recipients = JsonConvert.DeserializeObject<List<PickerItem>>(serviceResponse.ToString());
                    }
                }
            }
            return recipients;
        }

        public async Task<string> CreateMessageAsync(string recipientID, string subjectID, string message) {

            if (CrossConnectivity.Current.IsConnected) {

                using (HttpClient client = await base.GetServicesHttpClient()) {

                    var package = new Message() { RecipientID = recipientID, SubjectID = subjectID, Detail = message };

                    HttpContent postContent = new StringContent(JsonConvert.SerializeObject(package), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("message/create", postContent);

                    if (response.IsSuccessStatusCode) {
                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        return JsonConvert.DeserializeObject<string>(serviceResponse.ToString().ToLower());
                    }
                }
            }

            return null;
        }

        public async Task<string> ReplyMessageAsync(string messageID, string message) {

            if (CrossConnectivity.Current.IsConnected) {

                using (HttpClient client = await base.GetServicesHttpClient()) {

                    var package = new Message() { ID = messageID, Detail = message };

                    HttpContent postContent = new StringContent(JsonConvert.SerializeObject(package), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("message/reply", postContent);

                    if (response.IsSuccessStatusCode) {
                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        return JsonConvert.DeserializeObject<string>(serviceResponse.ToString().ToLower());
                    }
                }
            }

            return null;
        }

    }
}
