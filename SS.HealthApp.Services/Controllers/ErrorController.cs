using System.Web.Http;
using SS.HealthApp.Model.ErrorModels;

namespace SS.HealthApp.Services.Controllers {

    [Authorize]
    public class ErrorController : ApiController
    {
        [HttpPost]
        [Route("api/error/add")]
        public void Add(ErrorMessage error)
        {
            try {
                Core.Utils.ErrorLog.Add(error.Source, error.Message);
            }
            catch  {}
        }
        
    }

}