using System.Web.Http;
using System.Collections.Generic;
using System;
using SS.HealthApp.Model.AccountModels;
using System.Net.Http;

namespace SS.HealthApp.Services.Controllers {

    [Authorize]
    public class AccountController : ApiController
    {
        [HttpGet]
        [Route("api/accountstatement")]
        public List<AccountStatement> GetAccountStatement()
        {
            try {
                var aService = new Core.Services.AccountService();
                return aService.GetAccountStatment();
            }
            catch (Exception ex) {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }

        [HttpGet]
        [Route("api/accountstatement/send")]
        public bool SendAccountStatment(string id)
        {
            try {
                var aService = new Core.Services.AccountService();
                return aService.SendAccountStatmentFileByEmail(id);
            }
            catch (Exception ex) {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }

        [HttpGet]
        [Route("api/accountstatement/download")]
        public HttpResponseMessage DownloadAccountStatment(string id)
        {
            try
            {
                var aService = new Core.Services.AccountService();
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK) {
                    Content = new ByteArrayContent(aService.GetAccountStatmentFile(id))
                };
            }
            catch (Exception ex) {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }

    }

}