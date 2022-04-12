using System;
using System.Web.Http;
using SS.HealthApp.Model.UserModels;

namespace SS.HealthApp.Services.Controllers
{

    [Authorize]
    public class UserController : ApiController
    {

        [HttpGet]
        [Route("api/user/PersonalData")]
        public PersonalData GetPersonalData()
        {
            try
            {
                Core.Services.UserService uService = new Core.Services.UserService();
                return uService.GetPersonalData();
            }
            catch (Exception ex)
            {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }

        }

        [HttpPost]
        [Route("api/user/PersonalData/Save")]
        public bool SavePersonalData(PersonalData PersonalData)
        {
            try
            {
                Core.Services.UserService uService = new Core.Services.UserService();
                return uService.SavePersonalData(PersonalData);
            }
            catch (Exception ex)
            {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }

        [HttpPost]
        [Route("api/user/PersonalData/ChangePassword")]
        public bool ChangePassword(ChangePassword pData)
        {
            try
            {
                Core.Services.UserService uService = new Core.Services.UserService();
                return uService.ChangePassword(pData);
            }
            catch (Exception ex)
            {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }

    }

}