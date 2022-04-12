using System.Collections.Generic;
using SS.HealthApp.Model.HomeModels;

namespace SS.HealthApp.PCL.Repositories
{
    class BannerRepository : BaseRepository<List<Banner>>
    {

        protected override string GetRepositoryFilename()
        {
            return "banners.txt";
        }

    }
}
