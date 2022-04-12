using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using SS.HealthApp.Model.HomeModels;
using SS.HealthApp.ClientConnector.Interfaces;

namespace SS.HealthApp.Core.Services {
    public class HomeService {

        public List<Banner> GetBanners() {

            string cacheKey = GetCacheKey();

            //check cache
            List<Banner> banners = CacheManager<List<Banner>>.Get(cacheKey);
            if (banners != null)
                return banners;

            IHomeClientConnector service = 
                ClientConnector.ClientConnectorManager.Instance.Create<IHomeClientConnector>(CoreContext.CurrentUser.CompanyId);

            banners = service.GetBanners();

            //save on cache
            if (banners != null)
                CacheManager<List<Banner>>.Save(banners, cacheKey, GetCacheExpiration());
            
            return banners;

        }

        public List<EmergencyDelay> GetEmergencyWaitTime()
        {
            IHomeClientConnector service = 
                ClientConnector.ClientConnectorManager.Instance.Create<IHomeClientConnector>(CoreContext.CurrentUser.CompanyId);
            return service.GetEmergencyWaitTime();
        }

        private string GetCacheKey()
        {
            return string.Format("Banners_Company_{0}", CoreContext.CurrentUser.CompanyId);
        }

        private DateTimeOffset GetCacheExpiration()
        {
            return DateTimeOffset.Now.AddMinutes(Int32.Parse(ConfigurationManager.AppSettings["BannersCacheExpirationMinutes"]));
        }

    }
}
