using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS.HealthApp.PCL.Services;

namespace SS.HealthApp.Tests {
    [TestClass]
    public class FacilityServiceTests {

        [TestMethod]
        public void GetItemsAsyncTest() {

            var uService = new UserService();
            var fService = new FacilityService();

            bool validForm = uService.LoginAsync("digicustomer", "123456789").Result;
            if (validForm) {
                var items = fService.GetItemsAsync().Result;
                Assert.AreNotEqual(items.Count, 0);
            }
        }

        //[TestMethod]
        //public void GetTimeDistanceAsyncTest() {

        //    var uService = new UserService();
        //    var fService = new FacilityService();

        //    bool validForm = uService.LoginAsync("digicustomer", "123456789").Result;
        //    if (validForm) {
        //        var item = fService.GetTimeDistanceAsync("38.7648909,-9.3837259").Result;
        //        Assert.AreNotEqual(item, -1);
        //    }
        //}

    }
}
