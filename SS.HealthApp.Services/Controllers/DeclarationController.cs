using System.Web.Http;
using System;
using System.Collections.Generic;
using SS.HealthApp.Model.DeclarationModels;
using System.Net.Http;

namespace SS.HealthApp.Services.Controllers
{

    [Authorize]
    public class DeclarationController : ApiController
    {

        [HttpGet]
        [Route("api/presencedeclaration")]
        public List<PresenceDeclaration> GetPresenceDeclaration()
        {
            try
            {
                Core.Services.DeclarationService dService = new Core.Services.DeclarationService();
                return dService.GetPresenceDeclaration();
            }
            catch (Exception ex)
            {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }

        }

        [HttpGet]
        [Route("api/presencedeclaration/send")]
        public bool SendPresenceDeclaration(string Id)
        {
            try
            {
                Core.Services.DeclarationService dService = new Core.Services.DeclarationService();
                return dService.SendPresenceDeclarationFileByEmail(Id);
            }
            catch (Exception ex)
            {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }

        [HttpGet]
        [Route("api/presencedeclaration/download")]
        public HttpResponseMessage DownloadPresenceDeclaration(string id)
        {
            try
            {
                var aService = new Core.Services.DeclarationService();
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK) {
                    Content = new ByteArrayContent(aService.GetPresenceDeclarationFile(id))
                };
            }
            catch (Exception ex)
            {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }

        [HttpGet]
        [Route("api/presencedeclaration/sendbyappointmentid")]
        public bool SendPresenceDeclarationByAppointmentID(string AppointmentId)
        {
            try
            {
                Core.Services.DeclarationService dService = new Core.Services.DeclarationService();
                return dService.SendPresenceDeclarationFileByEmailByAppointmentId(AppointmentId);
            }
            catch (Exception ex)
            {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }

        [HttpGet]
        [Route("api/presencedeclaration/downloadbyappointmentid")]
        public HttpResponseMessage DownloadPresenceDeclarationByAppointmentID(string AppointmentId)
        {
            try
            {
                var aService = new Core.Services.DeclarationService();
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK) {
                    Content = new ByteArrayContent(aService.GetPresenceDeclarationFileAppointmentId(AppointmentId))
                };
            }
            catch (Exception ex)
            {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }


    }

}