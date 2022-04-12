using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS.HealthApp.PCL.Services;
using SS.HealthApp.Model;
using SS.HealthApp.Model.AppointmentModels;

namespace SS.HealthApp.Tests
{
    [TestClass]
    public class AttendanceServiceTests
    {

        [TestMethod]
        public void GetCurrentTicketsTest()
        {

            var uService = new UserService();
            var aService = new AttendanceService();

            bool validForm = uService.LoginAsync("51110202", "6673").Result;
            if (validForm)
            {
                var ticket = aService.GetNextTicketAsync().Result;
                Assert.AreNotEqual(ticket.Number, -1);
            }
        }

        [TestMethod]
        public void CheckInTest()
        {
            var uService = new UserService();
            var aService = new AttendanceService();

            bool validForm = uService.LoginAsync("51110202", "6673").Result;
            if (validForm)
            {
                var result = aService.CheckInAsync("4905||1796||1").Result;
                Assert.AreEqual(result.Sucess, true);
            }
        }

        [TestMethod]
        public void RateServiceTest()
        {

            var uService = new UserService();
            var aService = new AttendanceService();

            bool validForm = uService.LoginAsync("51110202", "6673").Result;
            if (validForm)
            {
                var res = aService.RateServiceAsync("1427||5017||1", 6).Result;
                Assert.AreEqual(res, true);
            }
        }

        [TestMethod]
        public void GetParkingQrCodeTest()
        {

            var uService = new UserService();
            var aService = new AttendanceService();

            bool validForm = uService.LoginAsync("51110202", "6673").Result;
            if (validForm)
            {
                var code = aService.DownloadParkingQRCodeAsync("1427||5017||1", "C:\\").Result;

                Assert.AreNotEqual(code, null);
            }
        }
    }
}