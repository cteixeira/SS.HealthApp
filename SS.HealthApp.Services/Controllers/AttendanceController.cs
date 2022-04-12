using SS.HealthApp.Model.AttendanceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SS.HealthApp.Services.Controllers
{
    [Authorize]
    public class AttendanceController : ApiController
    {

        [HttpGet]
        [Route("api/attendance/nextticket")]
        public Ticket GetNextTicket()
        {
            try
            {
                var service = new Core.Services.AttendanceService();
                return service.GetNextTicket();
            }
            catch (Exception ex)
            {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                return null;
            }
        }

        [HttpGet]
        [Route("api/attendance/checkin")]
        public CheckInResult CheckIn(string id)
        {
            try
            {
                var service = new Core.Services.AttendanceService();
                return service.CheckIn(id);
            }
            catch (Exception ex)
            {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                return new CheckInResult {
                    Sucess = false
                };
            }
        }

        [HttpGet]
        [Route("api/attendance/rateservice")]
        public bool RateService(string id, int rate)
        {
            try
            {
                var service = new Core.Services.AttendanceService();
                return service.RateService(id, rate);
            }
            catch (Exception ex)
            {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                return false;
            }
        }

        [HttpGet]
        [Route("api/attendance/parkingqrcode")]
        public HttpResponseMessage GetParkingQrCode(string id)
        {
            try
            {
                var service = new Core.Services.AttendanceService();
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK) {
                    Content = new ByteArrayContent(service.GetParkingQrCode(id))
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
