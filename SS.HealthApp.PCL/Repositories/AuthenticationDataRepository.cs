using SS.HealthApp.Model.UserModels;

namespace SS.HealthApp.PCL.Repositories
{
    class AuthenticationDataRepository : BaseRepository<AuthenticationData>
    {

        protected override string GetRepositoryFilename()
        {
            return "00.txt";
        }

    }
}
