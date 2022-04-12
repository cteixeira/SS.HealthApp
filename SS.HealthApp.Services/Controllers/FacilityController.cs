using System.Web.Http;
using System.Collections.Generic;
using System;
using SS.HealthApp.Model.FacilityModels;
using System.Threading.Tasks;

namespace SS.HealthApp.Services.Controllers
{

    [Authorize]
    public class FacilityController : ApiController
    {

        [HttpGet]
        [Route("api/Facilities")]
        public List<Facility> GetFacilities()
        {
            try
            {
                Core.Services.FacilityService fService = new Core.Services.FacilityService();
                return fService.GetFacilities();
            }
            catch (Exception ex)
            {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }

        [HttpGet]
        [Route("api/Facilities/timedistance")]
        public async Task<int> GetTimeDistance(string origin, string destination) {
            try {
                var fService = new Core.Services.FacilityService();
                return await fService.GetTimeDistance(origin, destination);
            }
            catch (Exception ex) {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }

    }
}