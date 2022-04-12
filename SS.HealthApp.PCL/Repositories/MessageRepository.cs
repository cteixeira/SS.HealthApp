using System.Collections.Generic;
using SS.HealthApp.Model.MessageModels;

namespace SS.HealthApp.PCL.Repositories
{
    class MessageRepository : BaseRepository<List<Message>>
    {

        protected override string GetRepositoryFilename()
        {
            return "message.txt";
        }
        
    }
}
