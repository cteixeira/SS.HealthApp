using SS.HealthApp.ClientConnector.Interfaces;
namespace SS.HealthApp.ClientConnector.SAMS
{
    public class LoginClientConnector : ILoginClientConnector
    {
        public ClientConnector.Models.AuthenticatedUser Login(Model.UserModels.Login LoginData)
        {
            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient())
            {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;
                bool validUser = serviceProxy.Login(LoginData.Username, LoginData.Password);

                if (validUser)
                {
                    ClientConnector.Models.AuthenticatedUser authUser = new ClientConnector.Models.AuthenticatedUser {
                        Id = LoginData.Username,
                        Username = LoginData.Username
                    };
                    authUser.Properties.Add("pin", LoginData.Password);
                    return authUser;
                }

            }
            return null;
        }
    }
}
