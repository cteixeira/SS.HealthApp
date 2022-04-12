using System.Collections.Generic;
using SS.HealthApp.Model;
using SS.HealthApp.Model.AppointmentModels;

namespace SS.HealthApp.ClientConnector.Interfaces
{
    public interface IAppointmentClientConnector {

        List<Appointment> GetAppointments(Models.AuthenticatedUser User);

        AppointmentData GetAllData();

        List<PickerItem> GetAvailableDates(Models.AuthenticatedUser User, AppointmentBook apptBook);

        List<PickerItem> GetAvailableTime(Models.AuthenticatedUser User, AppointmentBook apptBook);

        List<PickerItem> GetPayors(Models.AuthenticatedUser User);

        bool CancelAppointment(Models.AuthenticatedUser User, string appointmentID);

        bool BookNewAppointment(Models.AuthenticatedUser User, AppointmentBook apptBook);

    }

}
