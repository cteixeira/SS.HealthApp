using System.Web.Http;
using System;
using System.Collections.Generic;
using SS.HealthApp.Model;
using SS.HealthApp.Model.AppointmentModels;

namespace SS.HealthApp.Services.Controllers {

    [Authorize]
    public class AppointmentController: ApiController
    {

        [HttpGet]
        [Route("api/appointment/list")]
        public List<Appointment> GetAppointments() {
            try {
                var service = new Core.Services.AppointmentService();
                return service.GetAppointments();
            }
            catch (Exception ex) {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                return null;
            }
        }

        [HttpGet]
        [Route("api/appointment/data")]
        public AppointmentData GetAllData() {
            try {
                var service = new Core.Services.AppointmentService();
                return service.GetAllData();
            }
            catch (Exception ex) {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }

        [HttpPost]
        [Route("api/appointment/availdates")]
        public List<PickerItem> GetAvailableDates(AppointmentBook apptBook) {
            try {
                var service = new Core.Services.AppointmentService();
                return service.GetAvailableDates(apptBook);
            }
            catch (Exception ex) {
                //Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                //TODO: remove the following line after the Null Reference exception issue be resolved
                Core.Utils.ErrorLog.Add(GetType().Name, String.Concat(ex.ToString(), " || ", Newtonsoft.Json.JsonConvert.SerializeObject(apptBook)));
                throw ex;
            }
        }

        [HttpPost]
        [Route("api/appointment/availtime")]
        public List<PickerItem> GetAvailableTime(AppointmentBook apptBook) {
            try {
                var service = new Core.Services.AppointmentService();
                return service.GetAvailableTime(apptBook);
            }
            catch (Exception ex) {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }

        [HttpGet]
        [Route("api/appointment/payors")]
        public List<PickerItem> GetPayors() {
            try {
                var service = new Core.Services.AppointmentService();
                return service.GetPayors();
            }
            catch (Exception ex) {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }

        [HttpGet]
        [Route("api/appointment/cancel")]
        public bool CancelAppointment(string id) {
            try {
                var aService = new Core.Services.AppointmentService();
                return aService.CancelAppointment(id);
            }
            catch (Exception ex) {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }

        [HttpPost]
        [Route("api/appointment/book")]
        public bool BookNewAppointment(AppointmentBook apptBook) {
            try {
                var service = new Core.Services.AppointmentService();
                return service.BookNewAppointment(apptBook);
            }
            catch (Exception ex) {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }
    }

}