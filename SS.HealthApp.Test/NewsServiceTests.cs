using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS.HealthApp.PCL.Services;

namespace SS.HealthApp.Tests {
    [TestClass]
    public class NewServiceTests {

        [TestMethod]
        public void GetItemsAsyncTest() {

            var uService = new UserService();
            var nService = new NewsService();

            bool validForm = uService.LoginAsync("51110202", "6673").Result;
            if (validForm) {
                var items = nService.GetItemsAsync().Result;
                Assert.AreNotEqual(items.Count, 0);
            }
        }
        
    }
}
