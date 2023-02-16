using System.Threading.Tasks;
using System.Collections.Generic;
using Plugin.Connectivity;
using System.Net.Http;
using SS.HealthApp.Model.AccountModels;
using Newtonsoft.Json;
using System.Text;

namespace SS.HealthApp.PCL.Services {

    public class AccountService : _BaseService<AccountStatement> {

        public AccountService()
        {
            Repository = new Repositories.AccountStatementRepository();
            RequestUri = "accountstatement";
        }

        public async Task<bool> SendDocumentAsync(string id) {

            if (CrossConnectivity.Current.IsConnected) {

                using (HttpClient client = await base.GetServicesHttpClient()) {

                    
                    HttpResponseMessage response = await client.GetAsync(string.Format("accountstatement/send?id={0}", id));

                    if (response.IsSuccessStatusCode) {
                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        return JsonConvert.DeserializeObject<bool>(serviceResponse.ToString().ToLower());
                    }
                }
            }

            return false;
        }

        public async Task<string> DownloadDocumentAsync(string id, string saveFilePath = null) {

            if (CrossConnectivity.Current.IsConnected) {

                using (HttpClient client = await base.GetServicesHttpClient()) {

                    HttpResponseMessage response = await client.GetAsync("accountstatement/download?id=" + id);

                    if (response.IsSuccessStatusCode) {
                        byte[] content = await response.Content.ReadAsByteArrayAsync();
                        return await new Repositories.AccountStatementRepository().SaveContentAsync(content, string.Format("{0}.pdf", id), saveFilePath);
                    }
                }
            }

            return null;
        }
    }
}
