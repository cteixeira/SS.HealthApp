using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS.HealthApp.PCL.Services;

namespace SS.HealthApp.Tests {
    [TestClass]
    public class ErrorServiceTests {

        [TestMethod]
        public void AddErrorAsync() {
            var uService = new UserService();
            var eService = new ErrorService();

            bool validForm = uService.LoginAsync("digicustomer", "123456789").Result;
            if (validForm) {
                var result = eService.AddAsync(new System.Exception("error testing")).Result;
                Assert.AreEqual(result, true);
            }
        }
    }
}
