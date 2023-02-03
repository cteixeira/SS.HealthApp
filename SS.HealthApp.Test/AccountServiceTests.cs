using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS.HealthApp.PCL.Services;

namespace SS.HealthApp.Tests {
    [TestClass]
    public class AccountServiceTests {

        [TestMethod]
        public void GetItemsAsyncTest() {

            var uService = new UserService();
            var aService = new AccountService();

            bool validForm = uService.LoginAsync("digicustomer", "123456789").Result;
            if (validForm) {
                var items = aService.GetItemsAsync().Result;
                Assert.AreNotEqual(items.Count, 0);
            }
        }

        [TestMethod]
        public void SendDocumentAsync() {
            var uService = new UserService();
            var aService = new AccountService();

            bool validForm = uService.LoginAsync("digicustomer", "123456789").Result;
            if (validForm) {
                var result = aService.SendDocumentAsync("1").Result;
                Assert.AreEqual(result, true);
            }
        }

        [TestMethod]
        public void DownloadDocumentAsync()
        {
            var uService = new UserService();
            var aService = new AccountService();

            bool validForm = uService.LoginAsync("digicustomer", "123456789").Result;
            if (validForm)
            {
                var result = aService.DownloadDocumentAsync("1").Result;
                Assert.AreNotEqual(result, null);
            }
        }
    }
}
