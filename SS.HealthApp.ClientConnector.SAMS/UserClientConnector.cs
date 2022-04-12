using SS.HealthApp.ClientConnector.Interfaces;
using System;
using SS.HealthApp.ClientConnector.Models;
using SS.HealthApp.Model.UserModels;
using System.Globalization;
using System.Threading;

namespace SS.HealthApp.ClientConnector.SAMS
{
    public class UserClientConnector : IUserClientConnector
    {

        public PersonalData GetPersonalData(AuthenticatedUser User)
        {
            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient())
            {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                MySAMSApiWS.Utilizador userData = serviceProxy.ObterDadosPessoais(User.Id, User.Properties["pin"]);

                if (userData != null) {

                    

                    PersonalData ret = new PersonalData {
                        Name = userData.Nome,
                        Email = userData.Email,
                        Mobile = userData.Telemovel,
                        PhoneNumber = userData.Telefone,
                        TaxNumber = userData.NIF,
                        Address = String.Format("{0} \n{1} {2} \n{3} \n{4}", userData.Morada, userData.CodigoPostal, userData.Localidade, userData.Concelho, userData.Distrito)
                    };

                    if (ret.Name != null)
                    {
                        ret.Name = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(ret.Name.ToLower());
                    }

                    if (ret.Address != null)
                    {
                        ret.Address = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(ret.Address.ToLower());
                    }

                    return ret;
                }

                return null;
            }
        }

        public bool SavePersonalData(AuthenticatedUser User, PersonalData PersonalData)
        {
            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient())
            {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                //TODO: send name, emai and address
                return serviceProxy.AtualizarDadosPessoais(User.Id, User.Properties["pin"], PersonalData.PhoneNumber, PersonalData.Mobile, PersonalData.Email);

                //TODO: how to notify the error

            }
        }

        public bool ChangePassword(AuthenticatedUser User, ChangePassword pData)
        {
            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient()) {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                string errorMessage = string.Empty;
                var userID = User.Id;
                return serviceProxy.AlterarPin(out errorMessage, userID, pData.oldPassword, pData.newPassword);
            }
        }

        
    }
}
