﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS.HealthApp.PCL.Services;

namespace SS.HealthApp.Tests {
    [TestClass]
    public class HomeServiceTests {

        [TestMethod]
        public void GetBannersAsyncTest() {

            var uService = new UserService();
            var hService = new HomeService();

            bool validForm = uService.LoginAsync("digicustomer", "1234").Result;
            if (validForm) {
                var items = hService.GetBannersAsync().Result;
                Assert.AreNotEqual(items.Count, 0);
            }
        }

        [TestMethod]
        public void GetEmergencyDelayAsyncTest() {

            var uService = new UserService();
            var hService = new HomeService();

            bool validForm = uService.LoginAsync("digicustomer", "1234").Result;
            if (validForm) {
                var item = hService.GetEmergencyDelayAsync().Result;
                Assert.AreNotEqual(item.Count, 0);
            }
        }
        
    }
}
