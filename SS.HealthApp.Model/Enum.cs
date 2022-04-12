using System;

namespace SS.HealthApp.Model {
    public class Enum {
        public enum AppointmentStatus {
            Arrived,
            Called,
            [Obsolete]
            BookedToday,
            Booked,            
            Closed
        }

        public enum AppointmentType {
            C,
            E,
            O
        }
    }
}
