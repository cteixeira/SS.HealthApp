using SS.HealthApp.Model.UserModels;

namespace SS.HealthApp.ClientConnector.Interfaces
{
    public interface IUserClientConnector
    {

        PersonalData GetPersonalData(Models.AuthenticatedUser User);

        bool SavePersonalData(ClientConnector.Models.AuthenticatedUser User, PersonalData PersonalData);

        bool ChangePassword(ClientConnector.Models.AuthenticatedUser User, ChangePassword pData);

    }
}
