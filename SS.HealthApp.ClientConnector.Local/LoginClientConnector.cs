using System.Linq;
using SS.HealthApp.ClientConnector.Interfaces;

namespace SS.HealthApp.ClientConnector.Local
{
    public class LoginClientConnector : ILoginClientConnector
    {

        public ClientConnector.Models.AuthenticatedUser Login(Model.UserModels.Login LoginData)
        {
            Models.User user = UserClientConnector.dataSource.FirstOrDefault(u => u.Username.ToLower().Trim() == LoginData.Username.ToLower().Trim() && u.Password == LoginData.Password);
            if(user != null)
            {
                return new ClientConnector.Models.AuthenticatedUser {
                    Id = user.Id,
                    Username = user.Username
                };
            }
            return null;
        }
     
    }
}
