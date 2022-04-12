using SS.HealthApp.ClientConnector.Interfaces;
using System.Configuration;

namespace SS.HealthApp.Core
{
    public class ClientConnectorManager
    {

        public static void ConfigureClientConnectors()
        {

            //local connector
            ConfigureClientConnectorLocal(ClientConnector.ClientConnectorManager.Instance);

            //sams connector
            ConfigureClientConnectorSAMS(ClientConnector.ClientConnectorManager.Instance);

            //TODO: other connectors

        }

        private static void ConfigureClientConnectorLocal(ClientConnector.ClientConnectorManager manager)
        {

            string clientId = ConfigurationManager.AppSettings["LocalClientId"];

            //Account Service
            manager.Register<IAccountClientConnector>(clientId, delegate {
                return new ClientConnector.Local.AccountClientConnector();
            });

            //Appointment Service
            manager.Register<IAppointmentClientConnector>(clientId, delegate {
                return new ClientConnector.Local.AppointmentClientConnector();
            });

            //Declaration Service
            manager.Register<IDeclarationClientConnector>(clientId, delegate {
                return new ClientConnector.Local.DeclarationClientConnector();
            });

            //Facility Service
            manager.Register<IFacilityClientConnector>(clientId, delegate {
                return new ClientConnector.Local.FacilityClientConnector();
            });

            //Home Service
            manager.Register<IHomeClientConnector>(clientId, delegate {
                return new ClientConnector.Local.HomeClientConnector();
            });

            //Login Service
            manager.Register<ILoginClientConnector>(clientId, delegate
            {
                return new ClientConnector.Local.LoginClientConnector();
            });

            //News Service
            manager.Register<INewsClientConnector>(clientId, delegate
            {
                return new ClientConnector.Local.NewsClientConnector();
            });

            //User Service
            manager.Register<IUserClientConnector>(clientId, delegate {
                return new ClientConnector.Local.UserClientConnector();
            });

            //Message Service
            manager.Register<IMessageClientConnector>(clientId, delegate {
                return new ClientConnector.Local.MessageClientConnector();
            });

            //Attendance Service
            manager.Register<IAttendanceClientConnector>(clientId, delegate {
                return new ClientConnector.Local.AttendanceClientConnector();
            });

        }

        private static void ConfigureClientConnectorSAMS(ClientConnector.ClientConnectorManager manager)
        {

            string clientId = ConfigurationManager.AppSettings["SAMSClientId"];

            //Account Service
            manager.Register<IAccountClientConnector>(clientId, delegate {
                return new ClientConnector.SAMS.AccountClientConnector();
            });

            //Appointment Service
            manager.Register<IAppointmentClientConnector>(clientId, delegate {
                return new ClientConnector.SAMS.AppointmentClientConnector();
            });

            //Declaration Service
            manager.Register<IDeclarationClientConnector>(clientId, delegate {
                return new ClientConnector.SAMS.DeclarationClientConnector();
            });

            //Facility Service
            manager.Register<IFacilityClientConnector>(clientId, delegate {
                return new ClientConnector.SAMS.FacilityClientConnector();
            });

            //Home Service
            manager.Register<IHomeClientConnector>(clientId, delegate {
                return new ClientConnector.SAMS.HomeClientConnector();
            });

            //Login Service
            manager.Register<ILoginClientConnector>(clientId, delegate {
                return new ClientConnector.SAMS.LoginClientConnector();
            });

            //News Service
            manager.Register<INewsClientConnector>(clientId, delegate {
                return new ClientConnector.SAMS.NewsClientConnector();
            });

            //User Service
            manager.Register<IUserClientConnector>(clientId, delegate {
                return new ClientConnector.SAMS.UserClientConnector();
            });

            //Message Service
            manager.Register<IMessageClientConnector>(clientId, delegate {
                return new ClientConnector.SAMS.MessageClientConnector();
            });

            //Attendance Service
            manager.Register<IAttendanceClientConnector>(clientId, delegate {
                return new ClientConnector.SAMS.AttendanceClientConnector();
            });

        }

    }
}
