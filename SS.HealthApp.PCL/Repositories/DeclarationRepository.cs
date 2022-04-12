using PCLStorage;
using System.Threading.Tasks;
using System.Collections.Generic;
using SS.HealthApp.Model.DeclarationModels;
using System.IO;
using System;

namespace SS.HealthApp.PCL.Repositories
{
    class DeclarationRepository : BaseRepository<List<PresenceDeclaration>>
    {
        protected override string GetRepositoryFilename()
        {
            return "declaration.txt";
        }

    }
}
