using System.Collections.Generic;
using SS.HealthApp.Model.NewsModels;

namespace SS.HealthApp.PCL.Repositories
{
    class NewsRepository : BaseRepository<List<News>>
    {

        protected override string GetRepositoryFilename()
        {
            return "news.txt";
        }

    }
}
