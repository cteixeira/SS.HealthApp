using System.Collections.Generic;
using SS.HealthApp.ClientConnector.Interfaces;
using SS.HealthApp.ClientConnector.Models;
using SS.HealthApp.Model.MessageModels;
using SS.HealthApp.Model;
using System.Linq;
using System;

namespace SS.HealthApp.ClientConnector.Local
{
    public class MessageClientConnector : IMessageClientConnector
    {

        private static Dictionary<string, List<Message>> msgListDataSource = FillMsgListDataSource();
        private static Dictionary<string, List<Message>> msgThreadDataSource = FillMsgThreadDataSource();

        public List<Message> GetMessages(AuthenticatedUser User){
            return msgListDataSource[User.Id].OrderByDescending(n => n.Moment).ToList();
        }

        public List<Message> OpenMessage(AuthenticatedUser User, string messageID) {
            return msgThreadDataSource[string.Format("{0}_{1}", User.Id, messageID)].OrderByDescending(n => n.Moment).ToList();
        }

        public List<PickerItem> GetRecipients(AuthenticatedUser User) {
            var recipients = new List<PickerItem>();
            recipients.Add(new PickerItem("1", "Contact Center"));
            recipients.Add(new PickerItem("2", "Dr. Peter Sellers"));
            return recipients;
        }

        public List<PickerItem> GetSubjects(AuthenticatedUser User) {
            var subjects = new List<PickerItem>();
            subjects.Add(new PickerItem("1", "Online help"));
            subjects.Add(new PickerItem("2", "Customer Complaint"));
            subjects.Add(new PickerItem("3", "Talk to a Doctor"));
            return subjects;
        }

        public string CreateMessage(AuthenticatedUser User, string recipientID, string subjectID, string message) {

            int id = new Random().Next(1000, int.MaxValue);

            msgListDataSource[User.Id].Add(new Message() {
                ID = id.ToString(),
                Name = GetRecipients(User).Find(r => r.ID == recipientID).Title,
                Subject = GetSubjects(User).Find(r => r.ID == subjectID).Title,
                Moment = DateTime.Now,
                Read = false
            });

            msgThreadDataSource[string.Format("{0}_{1}", User.Id, id)] = new List<Message>();
            ReplyMessage(User, id.ToString(), message);

            return id.ToString();
        }

        public string ReplyMessage(AuthenticatedUser User, string messageID, string message) {
            msgThreadDataSource[string.Format("{0}_{1}", User.Id, messageID)].Add(new Message() {
                Received = false,
                Detail = message,
                Moment = DateTime.Now
            });
            return new Random().Next(1000, int.MaxValue).ToString();
        }

        private static Dictionary<string, List<Message>> FillMsgListDataSource() {

            var dSource = new Dictionary<string, List<Message>>();

            foreach (var user in UserClientConnector.dataSource) {
                dSource[user.Id] = new List<Message> {
                    new Message() { ID = "1", Name = "Contact Center", Subject = "Online help", Moment = new System.DateTime(2016, 12, 28, 11, 30, 0), Read = false },
                    new Message() { ID = "2", Name = "Dr. Peter Sellers", Subject = "Talk to a Doctor", Moment = new System.DateTime(2016, 8, 20, 11, 30, 0), Read = true },
                    new Message() { ID = "3", Name = "Contact Center", Subject = "Customer Complaint", Moment = new System.DateTime(2016, 5, 15, 11, 30, 0), Read = true }
                };
            }

            return dSource;

        }

        private static Dictionary<string, List<Message>> FillMsgThreadDataSource()
        {
            var dSource = new Dictionary<string, List<Message>>();

            foreach (var user in UserClientConnector.dataSource) {

                foreach (var msg in msgListDataSource[user.Id]) {

                    dSource[string.Format("{0}_{1}", user.Id, msg.ID)]=  new List<Message> {
                        new Message() { Received = true, Moment = new System.DateTime(2016, 12, 28, 11, 30, 0), Detail = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat." },
                        new Message() { Received = false, Moment = new System.DateTime(2016, 12, 28, 11, 15, 0), Detail = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua." },
                        new Message() { Received = true, Moment = new System.DateTime(2016, 12, 28, 09, 17, 0), Detail = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco " },
                        new Message() { Received = false, Moment = new System.DateTime(2016, 12, 28, 09, 02, 0), Detail = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."}
                    };
                }
            }

            return dSource;
        }

    }
}
