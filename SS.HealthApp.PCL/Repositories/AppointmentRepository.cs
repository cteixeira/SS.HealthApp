using System.Collections.Generic;
using SS.HealthApp.Model.AppointmentModels;

namespace SS.HealthApp.PCL.Repositories
{
    class AppointmentRepository : BaseRepository<List<Appointment>>
    {
        protected override string GetRepositoryFilename()
        {
            return "appointment.txt";
        }

    }
}
