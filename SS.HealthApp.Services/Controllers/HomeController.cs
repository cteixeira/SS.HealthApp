using System.Web.Http;
using System.Collections.Generic;
using System;
using SS.HealthApp.Model.HomeModels;

namespace SS.HealthApp.Services.Controllers
{

    [Authorize]
    public class HomeController : ApiController
    {

        [HttpGet]
        [Route("api/home/banners")]
        public List<Banner> GetBanners()
        {
            try
            {
                SS.HealthApp.Core.Services.HomeService hService = new Core.Services.HomeService();
                return hService.GetBanners();
            }
            catch (Exception ex)
            {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                return null;
            }
        }

        [HttpGet]
        [Route("api/home/EmergencyWaitTime")]
        public List<EmergencyDelay> GetEmergencyWaitTime()
        {
            try
            {
                Core.Services.HomeService hService = new Core.Services.HomeService();
                return hService.GetEmergencyWaitTime();
            }
            catch (Exception ex)
            {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }

    }
}