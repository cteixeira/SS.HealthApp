﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS.HealthApp.PCL.Services;
using System;

namespace SS.HealthApp.Tests {
    [TestClass]
    public class DeclarationServiceTests {

        [TestMethod]
        public void GetItemsAsyncTest() {

            var uService = new UserService();
            var dService = new DeclarationService();

            bool validForm = uService.LoginAsync("digicustomer", "1234").Result;
            if (validForm) {
                var items = dService.GetItemsAsync().Result;
                Assert.AreNotEqual(items.Count, 0);
            }
        }

        [TestMethod]
        public void SendPresenceDeclarationAsync() {
            var uService = new UserService();
            var dService = new DeclarationService();

            bool validForm = uService.LoginAsync("digicustomer", "1234").Result;
            if (validForm) {
                var result = dService.SendPresenceDeclarationAsync("1").Result;
                Assert.AreEqual(result, true);
            }
        }

        [TestMethod]
        public void DownloadPresenceDeclarationAsync()
        {
            var uService = new UserService();
            var dService = new DeclarationService();

            bool validForm = uService.LoginAsync("digicustomer", "1234").Result;
            if (validForm)
            {
                var result = dService.DownloadPresenceDeclarationAsync("1").Result;
                Assert.AreNotEqual(result, null);
            }
        }

    }
}
