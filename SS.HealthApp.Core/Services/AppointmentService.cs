using System;
using System.Collections.Generic;
using SS.HealthApp.ClientConnector.Interfaces;
using SS.HealthApp.Model;
using SS.HealthApp.Model.AppointmentModels;
using System.Configuration;
using System.Linq;
using SS.HealthApp.Model.AttendanceModels;

namespace SS.HealthApp.Core.Services {
    public class AppointmentService {

        public List<Appointment> GetAppointments() {

            Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null);

            var appointmentService = ClientConnector.ClientConnectorManager.Instance.Create
                <IAppointmentClientConnector>(CoreContext.CurrentUser.CompanyId);

            List<Appointment> userAppointments = appointmentService
                                                        .GetAppointments(CoreContext.CurrentUser.ConvertToClientConnectorModel())
                                                        .OrderByDescending(d => d.Moment)
                                                        .ToList();

            //check if any need to get tickets to associate with appointment
            if (userAppointments.Any(a => a.Status == Model.Enum.AppointmentStatus.Arrived || a.Status == Model.Enum.AppointmentStatus.Called)) {

                var attendanceService = ClientConnector.ClientConnectorManager.Instance.Create
                                            <IAttendanceClientConnector>(CoreContext.CurrentUser.CompanyId);

                //get all active tickets
                List<Ticket> tickets = attendanceService.GetCurrentTickets(CoreContext.CurrentUser.ConvertToClientConnectorModel());

                foreach (Ticket ticket in tickets) {

                    Appointment appt = userAppointments.Where(a => a.ID == ticket.AppointmentId).FirstOrDefault();

                    if(appt != null){
                        appt.TicketNumber = ticket.Number; //associate ticket with respective appointement
                        appt.Facility = ticket.Local; //change the facility to appointment local to display to user
                    }
                }
            }

            return userAppointments;
                
        }

        public AppointmentData GetAllData() {

            Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null);

            string cacheKey = GetCacheKey();

            //check cache
            AppointmentData apptData = CacheManager<AppointmentData>.Get(cacheKey);
            if (apptData != null)
                return apptData;

            var service = ClientConnector.ClientConnectorManager.Instance.Create
                <IAppointmentClientConnector>(CoreContext.CurrentUser.CompanyId);
            apptData = service.GetAllData();

            if (apptData != null) {
                apptData.Doctors = apptData.Doctors.OrderBy(d => d.Title).ToList();
                apptData.Specialties = apptData.Specialties.OrderBy(d => d.Title).ToList();
                apptData.Services = apptData.Services.OrderBy(d => d.Title).ToList();
                apptData.Facilities = apptData.Facilities.OrderBy(d => d.Title).ToList();

                //save on cache
                CacheManager<AppointmentData>.Save(apptData, cacheKey, GetCacheExpiration());
            }

            return apptData;

        }

        public List<PickerItem> GetAvailableDates(AppointmentBook apptBook) {

            Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null);

            var service = ClientConnector.ClientConnectorManager.Instance.Create
                <IAppointmentClientConnector>(CoreContext.CurrentUser.CompanyId);
            return service.GetAvailableDates(CoreContext.CurrentUser.ConvertToClientConnectorModel(), apptBook);
        }

        public List<PickerItem> GetAvailableTime(AppointmentBook apptBook) {
            var service = ClientConnector.ClientConnectorManager.Instance.Create
                <IAppointmentClientConnector>(CoreContext.CurrentUser.CompanyId);
            return service.GetAvailableTime(CoreContext.CurrentUser.ConvertToClientConnectorModel(), apptBook);
        }

        public List<PickerItem> GetPayors() {
            var service = ClientConnector.ClientConnectorManager.Instance.Create
                <IAppointmentClientConnector>(CoreContext.CurrentUser.CompanyId);
            return service.GetPayors(CoreContext.CurrentUser.ConvertToClientConnectorModel());
        }

        public bool CancelAppointment(string appointmentID) {

            Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null);

            var service = ClientConnector.ClientConnectorManager.Instance.Create
                <IAppointmentClientConnector>(CoreContext.CurrentUser.CompanyId);
            return service.CancelAppointment(CoreContext.CurrentUser.ConvertToClientConnectorModel(), appointmentID);
        }

        public bool BookNewAppointment(AppointmentBook apptBook) {

            Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null);

            var service = ClientConnector.ClientConnectorManager.Instance.Create
                <IAppointmentClientConnector>(CoreContext.CurrentUser.CompanyId);
            return service.BookNewAppointment(CoreContext.CurrentUser.ConvertToClientConnectorModel(), apptBook);
        }

        private string GetCacheKey() {
            return string.Format("Appointment_Data_Company_{0}", CoreContext.CurrentUser.CompanyId);
        }

        private DateTimeOffset GetCacheExpiration() {
            return DateTimeOffset.Now.AddMinutes(Int32.Parse(ConfigurationManager.AppSettings["AppointmentDataCacheExpirationMinutes"]));
        }
    }
}
