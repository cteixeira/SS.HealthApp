using System;
using System.Collections.Generic;
using System.Configuration;
using SS.HealthApp.Model.FacilityModels;
using SS.HealthApp.ClientConnector.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace SS.HealthApp.Core.Services {

    public class FacilityService {

        public List<Facility> GetFacilities()
        {

            Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null);

            string cacheKey = GetCacheKey();

            //check cache
            List<Facility> facilities = CacheManager<List<Facility>>.Get(cacheKey);
            if(facilities != null)
                return facilities;

            IFacilityClientConnector service = 
                ClientConnector.ClientConnectorManager.Instance.Create<IFacilityClientConnector>(CoreContext.CurrentUser.CompanyId);
            facilities = service.GetFacilities();

            //save on cache
            if (facilities != null) 
                CacheManager<List<Facility>>.Save(facilities, cacheKey, GetCacheExpiration());

            return facilities;
        }

        public async Task<int> GetTimeDistance(string origin, string destination) {

            Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null);

            return await new GoogleAPI.RestService().GetDurationAsync(origin, destination);
        }

        private string GetCacheKey() {
            return string.Format("Facilities_Company_{0}", CoreContext.CurrentUser.CompanyId);
        }

        private DateTimeOffset GetCacheExpiration() {
            return DateTimeOffset.Now.AddMinutes(Int32.Parse(ConfigurationManager.AppSettings["FacilitiesCacheExpirationMinutes"]));
        }

    }
}
