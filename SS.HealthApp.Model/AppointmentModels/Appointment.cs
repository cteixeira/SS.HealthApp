using System;

namespace SS.HealthApp.Model.AppointmentModels {

    public class Appointment : _BaseModel {

        public string Description { get; set; }

        public string Facility { get; set; }

        public Enum.AppointmentStatus Status { get; set; }

        public DateTime Moment { get; set; }

        public string TicketNumber { get; set; }

        public int WaitTime { get; set; }

        public string ParkingCode { get; set; }

        public bool HasActionsAllowed {
            get { return AllowCheckIn || AllowCancel || AllowAddToCalendar || AllowParkingQRCode || AllowRateService || AllowPresenceDeclaration; }
        }

        public bool AllowCheckIn { get; set; }

        public bool AllowCancel { get; set; }

        public bool AllowAddToCalendar { get; set; }

        public bool AllowParkingQRCode { get; set; }

        public bool AllowRateService { get; set; }

        public bool AllowPresenceDeclaration { get; set; }

    }
}
