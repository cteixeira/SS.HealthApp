using System.Collections.Generic;
using SS.HealthApp.Model.FacilityModels;

namespace SS.HealthApp.PCL.Repositories
{
    class FacilityRepository : BaseRepository<List<Facility>>
    {

        protected override string GetRepositoryFilename()
        {
            return "facilities.txt";
        }

    }
}
