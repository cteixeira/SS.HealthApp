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

        public NewsService()
        {
            Repository = new Repositories.NewsRepository();
            RequestUri = "news";
        }

    }
}
