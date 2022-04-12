using System;
using System.Collections.Generic;
using SS.HealthApp.Model.NewsModels;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Newtonsoft.Json;
using System.Net.Http;

namespace SS.HealthApp.PCL.Services
{

    public class NewsService : _BaseService<News>
    {

        public async Task<List<News>> GetItemsAsync()
        {

            Items = new List<News>();

            if (CrossConnectivity.Current.IsConnected)
            {

                using (HttpClient client = await base.GetServicesHttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync("news");
                    if (response.IsSuccessStatusCode)
                    {

                        Newtonsoft.Json.Linq.JToken serviceResponse = Newtonsoft.Json.Linq.JToken.Parse(await response.Content.ReadAsStringAsync());
                        Items = JsonConvert.DeserializeObject<List<News>>(serviceResponse.ToString());

                        Repositories.NewsRepository repository = new Repositories.NewsRepository();
                        await repository.SaveContentAsync(Items);

                    }
                }
            }
            else
            {
                Repositories.NewsRepository repository = new Repositories.NewsRepository();
                Items = await repository.GetContentAsync();
            }

            return Items;
        }

    }
}
