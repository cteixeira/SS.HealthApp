using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS.HealthApp.PCL.Services;

namespace SS.HealthApp.Tests {
    [TestClass]
    public class UserServiceTests {

        [TestMethod]
        public void GetPersonalDataTest() {

            var uService = new UserService();

            bool validForm = uService.LoginAsync("digicustomer", "123456789").Result;
            if (validForm) {
                var item = uService.GetPersonalData().Result;
                Assert.AreNotEqual(item.Name, string.Empty);
            }
        }

        [TestMethod]
        public void SavePersonalDataTest() {

            var uService = new UserService();

            bool validForm = uService.LoginAsync("digicustomer", "123456789").Result;
            if (validForm) {
                var res = uService.SavePersonalData(new Model.UserModels.PersonalData() {
                    Email = "teste@sbsi.pt",
                    Mobile = "932111000",
                    PhoneNumber = "222222222"
                }).Result;
                Assert.AreEqual(res, true);
            }
        }

        [TestMethod]
        public void ChangePasswordTest() {

            var uService = new UserService();

            bool validForm = uService.LoginAsync("digicustomer", "123456789").Result;
            if (validForm) {
                var res = uService.ChangePassword(new Model.UserModels.ChangePassword() { oldPassword = "123456789", newPassword = "123456789" } ).Result;
                Assert.AreNotEqual(res, string.Empty);
            }
        }
    }
}
