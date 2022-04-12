using System.Web.Http;
using System.Collections.Generic;
using System;
using SS.HealthApp.Model.MessageModels;
using System.Net.Http;
using SS.HealthApp.Model;

namespace SS.HealthApp.Services.Controllers {

    [Authorize]
    public class MessageController : ApiController {

        [HttpGet]
        [Route("api/message")]
        public List<Message> GetMessages() {
            try {
                var service = new Core.Services.MessageService();
                return service.GetMessages();
            }
            catch (Exception ex) {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }
        
        [HttpGet]
        [Route("api/message/open")]
        public List<Message> OpenMessage(string id) {
            try {
                var service = new Core.Services.MessageService();
                return service.OpenMessage(id);
            }
            catch (Exception ex) {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }

        [HttpGet]
        [Route("api/message/recipients")]
        public List<PickerItem> GetRecipients() {
            try {
                var service = new Core.Services.MessageService();
                return service.GetRecipients();
            }
            catch (Exception ex) {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }

        [HttpGet]
        [Route("api/message/subjects")]
        public List<PickerItem> GetSubjects() {
            try {
                var service = new Core.Services.MessageService();
                return service.GetSubjects();
            }
            catch (Exception ex) {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }

        [HttpPost]
        [Route("api/message/create")]
        public string Create(Message message) {
            try {
                var service = new Core.Services.MessageService();
                return service.CreateMessage(message.RecipientID, message.SubjectID, message.Detail);
            }
            catch (Exception ex) {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }

        [HttpPost]
        [Route("api/message/reply")]
        public string Reply(Message message) {
            try {
                var service = new Core.Services.MessageService();
                return service.ReplyMessage(message.ID, message.Detail);
            }
            catch (Exception ex) {
                Core.Utils.ErrorLog.Add(GetType().Name, ex.ToString());
                throw ex;
            }
        }
    }

}