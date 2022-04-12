using SS.HealthApp.ClientConnector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.HealthApp.ClientConnector.Interfaces
{
    public interface IAttendanceClientConnector
    {
        List<Model.AttendanceModels.Ticket> GetCurrentTickets(AuthenticatedUser User);

        Model.AttendanceModels.CheckInResult CheckIn(AuthenticatedUser User, string AppointmentId);

        bool RateService(AuthenticatedUser User, string AppointmentId, int Rate);

        byte[] GetParkingQrCode(AuthenticatedUser User, string AppointmentId);

    }
}
