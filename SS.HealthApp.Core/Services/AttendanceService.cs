using SS.HealthApp.ClientConnector.Interfaces;
using SS.HealthApp.Model.AttendanceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.HealthApp.Core.Services
{
    public class AttendanceService
    {
        public Ticket GetNextTicket()
        {

            var attendanceService = ClientConnector.ClientConnectorManager.Instance.Create
                <IAttendanceClientConnector>(CoreContext.CurrentUser.CompanyId);

            //get all active tickets
            return attendanceService.GetCurrentTickets(CoreContext.CurrentUser.ConvertToClientConnectorModel()).FirstOrDefault();

        }

        public CheckInResult CheckIn(string AppointmentId)
        {
            var attendanceService = ClientConnector.ClientConnectorManager.Instance.Create
                <IAttendanceClientConnector>(CoreContext.CurrentUser.CompanyId);

            return attendanceService.CheckIn(CoreContext.CurrentUser.ConvertToClientConnectorModel(), AppointmentId);

        }

        public bool RateService(string AppointmentId, int Rate)
        {
            var attendanceService = ClientConnector.ClientConnectorManager.Instance.Create
               <IAttendanceClientConnector>(CoreContext.CurrentUser.CompanyId);

            return attendanceService.RateService(CoreContext.CurrentUser.ConvertToClientConnectorModel(), AppointmentId, Rate);
        }

        public byte[] GetParkingQrCode(string AppointmentId)
        {
            var attendanceService = ClientConnector.ClientConnectorManager.Instance.Create
               <IAttendanceClientConnector>(CoreContext.CurrentUser.CompanyId);

            return attendanceService.GetParkingQrCode(CoreContext.CurrentUser.ConvertToClientConnectorModel(), AppointmentId);
        }


    }
}
