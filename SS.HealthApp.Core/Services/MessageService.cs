using System.Collections.Generic;
using System.Linq;
using SS.HealthApp.Model.MessageModels;
using SS.HealthApp.ClientConnector.Interfaces;
using SS.HealthApp.Model;

namespace SS.HealthApp.Core.Services {
    public class MessageService {

        public List<Message> GetMessages() {

            Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null);

            var service = ClientConnector.ClientConnectorManager.Instance.Create
                <IMessageClientConnector>(CoreContext.CurrentUser.CompanyId);
            return
                service.GetMessages(CoreContext.CurrentUser.ConvertToClientConnectorModel())
                    .OrderByDescending(m => m.Moment)
                    .ToList();
        }

        public List<Message> OpenMessage(string messageID) {

            Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null);

            var service = ClientConnector.ClientConnectorManager.Instance.Create
                <IMessageClientConnector>(CoreContext.CurrentUser.CompanyId);
            return
                service.OpenMessage(CoreContext.CurrentUser.ConvertToClientConnectorModel(), messageID)
                    .OrderByDescending(m => m.Moment)
                    .ToList();
        }

        public List<PickerItem> GetRecipients() {
            var service = ClientConnector.ClientConnectorManager.Instance.Create
                <IMessageClientConnector>(CoreContext.CurrentUser.CompanyId);
            return service.GetRecipients(CoreContext.CurrentUser.ConvertToClientConnectorModel());
        }

        public List<PickerItem> GetSubjects() {
            var service = ClientConnector.ClientConnectorManager.Instance.Create
                <IMessageClientConnector>(CoreContext.CurrentUser.CompanyId);
            return service.GetSubjects(CoreContext.CurrentUser.ConvertToClientConnectorModel());
        }

        public string CreateMessage(string recipientID, string subjectID, string message) {

            Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null);

            var service = ClientConnector.ClientConnectorManager.Instance.Create
                <IMessageClientConnector>(CoreContext.CurrentUser.CompanyId);
            return service.CreateMessage(CoreContext.CurrentUser.ConvertToClientConnectorModel(), recipientID, subjectID, message);
        }

        public string ReplyMessage(string messageID, string message) {

            Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null);

            var service = ClientConnector.ClientConnectorManager.Instance.Create
                <IMessageClientConnector>(CoreContext.CurrentUser.CompanyId);
            return service.ReplyMessage(CoreContext.CurrentUser.ConvertToClientConnectorModel(), messageID, message);
        }
    }
}
