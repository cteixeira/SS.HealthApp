namespace SS.HealthApp.ClientConnector.Interfaces
{
    public interface ILoginClientConnector
    {
        Models.AuthenticatedUser Login(Model.UserModels.Login LoginData);
    }
}
