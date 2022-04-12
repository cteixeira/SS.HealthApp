using System.Collections.Generic;
using SS.HealthApp.ClientConnector.Interfaces;
using SS.HealthApp.Model.AccountModels;
using System.Linq;

namespace SS.HealthApp.ClientConnector.SAMS
{
    public class AccountClientConnector : IAccountClientConnector
    {

        public List<AccountStatement> GetAccountStatement(ClientConnector.Models.AuthenticatedUser User)
        {
            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient()) {

                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                MySAMSApiWS.DocumentoFinanceiro[] docs = serviceProxy.ObterListaDocumentos(User.Id);

                if (docs != null) {
                    return docs.Select(b => new AccountStatement {
                        ID = b.IdDocumento,
                        Description = b.Numero,
                        Facility = b.UnidadeSaude,
                        Date = b.Data,
                        Value = b.Valor,
                        Payed = b.Pago
                    }).OrderByDescending(b => b.Date).ToList();
                }

                return null;
            }
        }

        public byte[] GetAccountStatementFile(ClientConnector.Models.AuthenticatedUser User, string documentID) {
            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient()) {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                return serviceProxy.DevolverDocumento(User.Id, documentID);
            }
        }
    }
}
