using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS.HealthApp.PCL.Services;

namespace SS.HealthApp.Tests {
    [TestClass]
    public class MessageServiceTests {

        [TestMethod]
        public void GetItemsAsyncTest() {

            var uService = new UserService();
            var mService = new MessageService();

            bool validForm = uService.LoginAsync("digicustomer", "1234").Result;
            if (validForm) {
                var items = mService.GetItemsAsync().Result;
                Assert.AreNotEqual(items.Count, 0);
            }
        }

        [TestMethod]
        public void OpenItemAsync()
        {
            var uService = new UserService();
            var mService = new MessageService();

            bool validForm = uService.LoginAsync("digicustomer", "1234").Result;
            if (validForm)
            {
                var items = mService.OpenItemAsync("1").Result;
                Assert.AreNotEqual(items.Count, 0);
            }
        }

        [TestMethod]
        public void GetRecipientsAsync() {
            var uService = new UserService();
            var mService = new MessageService();

            bool validForm = uService.LoginAsync("digicustomer", "1234").Result;
            if (validForm) {
                var items = mService.GetRecipientsAsync().Result;
                Assert.AreNotEqual(items.Count, 0);
            }
        }

        [TestMethod]
        public void GetSubjectssync() {
            var uService = new UserService();
            var mService = new MessageService();

            bool validForm = uService.LoginAsync("digicustomer", "1234").Result;
            if (validForm) {
                var items = mService.GetSubjectsAsync().Result;
                Assert.AreNotEqual(items.Count, 0);
            }
        }

        [TestMethod]
        public void CreateMessageAsync() {
            var uService = new UserService();
            var mService = new MessageService();

            bool validForm = uService.LoginAsync("digicustomer", "1234").Result;
            if (validForm) {
                var items = mService.CreateMessageAsync("1", "2", "Vamos lá testar isto a sério \nFavor não responder.").Result;
                Assert.AreNotEqual(items, new long[]{0, -1});
            }
        }

        [TestMethod]
        public void ReplyMessageAsync() {
            var uService = new UserService();
            var mService = new MessageService();

            bool validForm = uService.LoginAsync("digicustomer", "1234").Result;
            if (validForm) {
                var items = mService.ReplyMessageAsync("1", "Vamos lá testar isto a sério \nFavor não responder.").Result;
                Assert.AreNotEqual(items, new long[] { 0, -1 });
            }
        }

    }
}
