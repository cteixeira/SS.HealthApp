using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS.HealthApp.PCL.Services;

namespace SS.HealthApp.Tests {
    [TestClass]
    public class ErrorServiceTests {

        [TestMethod]
        public void AddErrorAsync() {
            var uService = new UserService();
            var eService = new ErrorService();

            bool validForm = uService.LoginAsync("51110202", "6673").Result;
            if (validForm) {
                var result = eService.AddAsync(new System.Exception("error testing")).Result;
                Assert.AreEqual(result, true);
            }
        }
    }
}
