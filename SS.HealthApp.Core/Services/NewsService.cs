using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using SS.HealthApp.Model.NewsModels;
using SS.HealthApp.ClientConnector.Interfaces;

namespace SS.HealthApp.Core.Services {
    public class NewsService {

        public List<News> GetNews()
        {

            Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null);

            string cacheKey = GetCacheKey();

            //check cache
            List<News> news = CacheManager<List<News>>.Get(cacheKey);
            if (news != null)
                return news;

            INewsClientConnector service = ClientConnector.ClientConnectorManager.Instance.Create<INewsClientConnector>(CoreContext.CurrentUser.CompanyId);
            news = service.GetNews();

            //save on cache
            if (news != null) {
                news = news.OrderByDescending(d => d.Date).ToList();
                CacheManager<List<News>>.Save(news, cacheKey, GetCacheExpiration());
            }

            return news;
        }

        private string GetCacheKey()
        {
            return string.Format("News_Company_{0}", CoreContext.CurrentUser.CompanyId);
        }

        private DateTimeOffset GetCacheExpiration()
        {
            return DateTimeOffset.Now.AddMinutes(Int32.Parse(ConfigurationManager.AppSettings["NewsCacheExpirationMinutes"]));
        }

    }
}
