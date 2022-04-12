using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS.HealthApp.PCL.Services;

namespace SS.HealthApp.Tests {
    [TestClass]
    public class AccountServiceTests {

        [TestMethod]
        public void GetItemsAsyncTest() {

            var uService = new UserService();
            var aService = new AccountService();

            bool validForm = uService.LoginAsync("51110202", "6673").Result;
            if (validForm) {
                var items = aService.GetItemsAsync().Result;
                Assert.AreNotEqual(items.Count, 0);
            }
        }

        [TestMethod]
        public void SendDocumentAsync() {
            var uService = new UserService();
            var aService = new AccountService();

            bool validForm = uService.LoginAsync("51110202", "6673").Result;
            if (validForm) {
                var result = aService.SendDocumentAsync("2014NCS90614P 00000000011").Result;
                Assert.AreEqual(result, true);
            }
        }

        [TestMethod]
        public void DownloadDocumentAsync()
        {
            var uService = new UserService();
            var aService = new AccountService();

            bool validForm = uService.LoginAsync("51110202", "6673").Result;
            if (validForm)
            {
                var result = aService.DownloadDocumentAsync("2014NCS90614P 00000000011").Result;
                Assert.AreEqual(result, true);
            }
        }
    }
}
