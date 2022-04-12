using System.Collections.Generic;
using SS.HealthApp.Model.MessageModels;
using SS.HealthApp.Model;

namespace SS.HealthApp.ClientConnector.Interfaces
{
    public interface IMessageClientConnector {

        List<Message> GetMessages(Models.AuthenticatedUser User);

        List<Message> OpenMessage(ClientConnector.Models.AuthenticatedUser User, string messageID);

        List<PickerItem> GetRecipients(Models.AuthenticatedUser User);

        List<PickerItem> GetSubjects(Models.AuthenticatedUser User);

        string CreateMessage(Models.AuthenticatedUser User, string recipientID, string subjectID, string message);

        string ReplyMessage(Models.AuthenticatedUser User, string messageID, string message);
    }
}
