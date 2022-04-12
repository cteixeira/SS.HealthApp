using System.Collections.Generic;
using SS.HealthApp.Model.NewsModels;

namespace SS.HealthApp.ClientConnector.Interfaces
{
    public interface INewsClientConnector
    {

        List<News> GetNews();

    }
}
