using System.Web.Http;
using System.Collections.Generic;
using System;
using SS.HealthApp.Model.NewsModels;

namespace SS.HealthApp.Services.Controllers
{

    [Authorize]
    public class NewsController : ApiController
    {

        [HttpGet]
        [Route("api/News")]
        public List<News> GetNews()
        {
            try
            {
                Core.Services.NewsService nService = new Core.Services.NewsService();
                return nService.GetNews();
            }
            catch (Exception ex)
            {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }

        }

    }
}