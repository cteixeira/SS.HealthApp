using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS.HealthApp.PCL.Services;

namespace SS.HealthApp.Tests {
    [TestClass]
    public class NewServiceTests {

        [TestMethod]
        public void GetItemsAsyncTest() {

            var uService = new UserService();
            var nService = new NewsService();

            bool validForm = uService.LoginAsync("digicustomer", "1234").Result;
            if (validForm) {
                var items = nService.GetItemsAsync().Result;
                Assert.AreNotEqual(items.Count, 0);
            }
        }
        
    }
}
