using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SS.HealthApp.ClientConnector.Interfaces;
using SS.HealthApp.Model.AttendanceModels;
using SS.HealthApp.ClientConnector.Models;
using SS.HealthApp.Model.AppointmentModels;

namespace SS.HealthApp.ClientConnector.Local
{
    public class AttendanceClientConnector : IAttendanceClientConnector
    {

        private static Dictionary<string, List<Ticket>> ticketsDataSource = FillTicketsDataSource();

        public List<Ticket> GetCurrentTickets(AuthenticatedUser User)
        {
            return ticketsDataSource[User.Id].OrderBy(t => t.Moment).ToList();
        }

        public CheckInResult CheckIn(AuthenticatedUser User, string AppointmentId)
        {
            Appointment appt = AppointmentClientConnector.apptListDataSource[User.Id].Where(apptment => apptment.ID == AppointmentId).FirstOrDefault();
            if (appt == null || !appt.AllowCheckIn)
            {
                return new CheckInResult {
                    Sucess = false,
                    Message = ""
                };
            }

            appt.Status = Model.Enum.AppointmentStatus.Arrived;
            AppointmentClientConnector.SetActionByAppointmentStatus(appt);

            var lastTicket = ticketsDataSource[User.Id].Select(t => int.Parse(t.Number)).OrderByDescending(number => number).FirstOrDefault();

            ticketsDataSource[User.Id].Add(new Ticket {
                Number = "888", //(++lastTicket).ToString(),
                Local = appt.Description + " Floor",
                WaitTimePrevision = 15,
                AppointmentId = appt.ID,
                Moment = appt.Moment
            });

            return new CheckInResult {
                Sucess = true,
                Message = appt.Description + " Floor"
            };

        }

        public bool RateService(AuthenticatedUser User, string AppointmentId, int Rate)
        {

            Appointment appt = AppointmentClientConnector.apptListDataSource[User.Id].Where(apptment => apptment.ID == AppointmentId).FirstOrDefault();
            if (appt == null || !appt.AllowRateService)
            {
                return false;
            }

            //do nothing - simulate some work
            System.Threading.Thread.Sleep(500);
            return true;
        }

        public byte[] GetParkingQrCode(AuthenticatedUser User, string AppointmentId)
        {

            Appointment appt = AppointmentClientConnector.apptListDataSource[User.Id].Where(apptment => apptment.ID == AppointmentId).FirstOrDefault();
            if (appt == null || !appt.AllowParkingQRCode)
            {
                throw new Exception("Qr code not available");
            }

            return System.IO.File.ReadAllBytes(string.Concat(Settings.ResourcesDir, "qrcode.png"));
        }

        private static Dictionary<string, List<Ticket>> FillTicketsDataSource()
        {
            var dSource = new Dictionary<string, List<Ticket>>();

            foreach (var user in UserClientConnector.dataSource)
            {
                dSource[user.Id] = new List<Ticket>();
                //initiate the ticket collection with one ticket foreach appointment with "Arrived" or "Called" status
                var appts = AppointmentClientConnector
                                .apptListDataSource[user.Id]
                                .Where(a => a.Status == Model.Enum.AppointmentStatus.Arrived || a.Status == Model.Enum.AppointmentStatus.Called);


                foreach (Appointment appt in appts)
                {
                    dSource[user.Id].Add(new Ticket {
                        Number = "888",
                        Local = appt.Description + " Floor",
                        WaitTimePrevision = 15,
                        AppointmentId = appt.ID,
                        Moment = appt.Moment
                    });
                }

            }

            return dSource;
        }

    }
}
