using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.HealthApp.Model.AttendanceModels
{
    public class Ticket
    {

        public string Number { get; set; }

        public string Local { get; set; }

        public int WaitTimePrevision { get; set; }

        public string AppointmentId { get; set; }

        public DateTime Moment { get; set; }

    }
}
