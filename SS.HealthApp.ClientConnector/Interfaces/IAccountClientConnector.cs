using System.Collections.Generic;
using SS.HealthApp.Model.AccountModels;

namespace SS.HealthApp.ClientConnector.Interfaces
{
    public interface IAccountClientConnector
    {

        List<AccountStatement> GetAccountStatement(Models.AuthenticatedUser User);

        byte[] GetAccountStatementFile(ClientConnector.Models.AuthenticatedUser User, string documentID);
    }
}
