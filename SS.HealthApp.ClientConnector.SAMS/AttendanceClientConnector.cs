using System;
using System.Collections.Generic;
using SS.HealthApp.ClientConnector.Interfaces;
using SS.HealthApp.ClientConnector.Models;
using SS.HealthApp.Model.AttendanceModels;

namespace SS.HealthApp.ClientConnector.SAMS
{
    public class AttendanceClientConnector : IAttendanceClientConnector {

        public List<Ticket> GetCurrentTickets(AuthenticatedUser User) {
            var tickets = new List<Ticket>();

            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient())
            {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                MySAMSApiWS.Senha[] items = serviceProxy.ObterSenhas(User.Id);

                if (items != null)
                {
                    foreach(var item in items)
                    {
                        tickets.Add(new Ticket()
                        {
                            AppointmentId = item.RowID,
                            Local = item.SalaEspera,
                            Number = item.Numero.ToString(),
                            Moment = item.Momento
                        });
                    }
                }
            }
            return tickets;
        }

        public CheckInResult CheckIn(AuthenticatedUser User, string AppointmentId) {

            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient())
            {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                int ret = serviceProxy.CheckIn(User.Id, AppointmentId);
                
                if (ret < 0)
                {
                    return new CheckInResult
                    {
                        Sucess = false,
                        Message = "Por favor dirija-se a um balcão."
                    };
                }
                else
                {
                    return new CheckInResult
                    {
                        Sucess = true,
                        Message = string.Empty, 
                        Code = ret
                    };
                }
            }
        }

        public bool RateService(AuthenticatedUser User, string AppointmentId, int Rate) {
            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient())
            {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                return serviceProxy.AvaliarAtendimento(User.Id, AppointmentId, Rate);

            }
        }

        public byte[] GetParkingQrCode(AuthenticatedUser User, string AppointmentId) {

            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient())
            {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                return serviceProxy.ObterQRCode(User.Id, AppointmentId);
            }
        }
    }
}
