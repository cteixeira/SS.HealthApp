using System.Collections.Generic;
using SS.HealthApp.ClientConnector.Interfaces;
using SS.HealthApp.Model.DeclarationModels;
using System.Linq;
using SS.HealthApp.ClientConnector.Models;
using System;

namespace SS.HealthApp.ClientConnector.SAMS
{
    public class DeclarationClientConnector : IDeclarationClientConnector
    {

        public List<PresenceDeclaration> GetPresenceDeclaration(ClientConnector.Models.AuthenticatedUser User) {

            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient()) {

                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                MySAMSApiWS.DeclaracaoPresenca[] items = serviceProxy.ObterListaDeclaracoesPresenca(User.Id);

                if (items != null) {
                    return items.Select(b => new PresenceDeclaration {
                        ID = b.IdDeclaracao,
                        Appointment = b.Acto,
                        Facility = b.UnidadeSaude,
                        Moment = b.Momento
                    }).OrderByDescending(b => b.Moment).ToList();
                }

                return null;
            }
        }

        public byte[] GetPresenceDeclarationFile(ClientConnector.Models.AuthenticatedUser User, string declarationID) {

            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient()) {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                return serviceProxy.DevolverDeclaracaoPresenca(User.Id, declarationID);

            }
        }

        public string GetPresenceDeclarationIdByAppointmentID(AuthenticatedUser User, string appointmentID) {
            //TODO: IMPLEMENTATION
            throw new NotImplementedException();
        }
    }
}
