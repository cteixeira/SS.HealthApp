using System.Collections.Generic;
using SS.HealthApp.ClientConnector.Interfaces;
using SS.HealthApp.Model.MessageModels;
using System.Linq;
using System.Globalization;
using SS.HealthApp.Model;
using SS.HealthApp.ClientConnector.Models;

namespace SS.HealthApp.ClientConnector.SAMS
{
    public class MessageClientConnector : IMessageClientConnector
    {

        public List<Message> GetMessages(AuthenticatedUser User)
        {
            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient()) {

                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                MySAMSApiWS.Mensagem[] msgs = serviceProxy.ObterListaMensagens(User.Id);

                if (msgs != null) {
                    return msgs.Select(m => new Message {
                        ID = m.MensagemID.ToString(),
                        Name = m.MensagemRecebida ? m.NomeRemetente : m.NomeDestinatario,
                        Subject = m.Assunto,
                        Moment = m.DataHora,
                        Read = m.Visto,
                        Received = m.MensagemRecebida
                    }).OrderByDescending(m => m.Moment).ToList();
                }

                return null;
            }
        }

        public List<Message> OpenMessage(AuthenticatedUser User, string messageID) {
            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient()) {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                MySAMSApiWS.Mensagem[] msgs = serviceProxy.AbrirMensagem(User.Id, long.Parse(messageID));

                var ti = CultureInfo.CurrentCulture.TextInfo;

                if (msgs != null) {
                    return msgs.Select(m => new Message {
                        ID = m.MensagemID.ToString(),
                        Name = m.MensagemRecebida ? m.NomeRemetente : ti.ToTitleCase(ti.ToLower(m.NomeRemetente)), //All the names are by default in Upper Case
                        Subject = m.Assunto,
                        Detail = m.TextoMensagem,
                        Moment = m.DataHora,
                        Read = m.Visto,
                        Received = m.MensagemRecebida,
                        ConversationID = m.ConversacaoID
                    }).OrderByDescending(m => m.Moment).ToList();
                }

                return null;
            }
        }

        public List<PickerItem> GetRecipients(AuthenticatedUser User) {

            var recipients = new List<PickerItem>();

            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient()) {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                MySAMSApiWS.Utilizador[] items = serviceProxy.ObterListaDestinatarios();

                if (items != null) {
                    foreach (var item in items) {
                        recipients.Add(new PickerItem(item.UtilizadorID.ToString(), item.Nome));
                    }
                }
            }

            return recipients;
        }

        public List<PickerItem> GetSubjects(AuthenticatedUser User) {

            var subjects = new List<PickerItem>();

            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient()) {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                MySAMSApiWS.MensagemAssunto[] items = serviceProxy.ObterListaAssuntos();
                if (items != null) {
                    foreach (var item in items) {
                        subjects.Add(new PickerItem(item.MensagemAssuntoID.ToString(), item.DesignacaoAssunto));
                    }
                }
            }

            return subjects;
        }

        public string CreateMessage(AuthenticatedUser User, string recipientID, string subjectID, string message) {
            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient()) {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;
                return serviceProxy.CriarMensagem(User.Id, long.Parse(recipientID), long.Parse(subjectID), message, null).ToString();
            }
        }

        public string ReplyMessage(AuthenticatedUser User, string messageID, string message) {
            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient()) {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;
                return serviceProxy.ResponderMensagem(User.Id, long.Parse(messageID), message, null).ToString();
            }
        }
    }
}
