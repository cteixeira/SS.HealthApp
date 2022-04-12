using SS.HealthApp.Model.AppointmentModels;

namespace SS.HealthApp.PCL.Repositories
{
    class AppointmentDataRepository : BaseRepository<AppointmentData>
    {
        protected override string GetRepositoryFilename()
        {
            return "appointment_data.txt";
        }

    }
}
