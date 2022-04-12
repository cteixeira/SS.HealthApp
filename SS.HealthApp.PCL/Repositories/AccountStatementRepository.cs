using System.Collections.Generic;
using SS.HealthApp.Model.AccountModels;
using System.Threading.Tasks;
using PCLStorage;
using System.IO;
using System;

namespace SS.HealthApp.PCL.Repositories
{
    class AccountStatementRepository : BaseRepository<List<AccountStatement>>
    {

        protected override string GetRepositoryFilename()
        {
            return "account.txt";
        }
        
    }
}
