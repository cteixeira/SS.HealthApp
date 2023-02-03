using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS.HealthApp.PCL.Services;
using SS.HealthApp.Model;
using SS.HealthApp.Model.AppointmentModels;

namespace SS.HealthApp.Tests
{
    [TestClass]
    public class AppointmentServiceTests
    {

        [TestMethod]
        public void GetItemsAsyncTest()
        {

            var uService = new UserService();
            var aService = new AppointmentService();

            bool validForm = uService.LoginAsync("digicustomer", "123456789").Result;
            if (validForm)
            {
                var items = aService.GetItemsAsync().Result;
                Assert.AreNotEqual(items.Count, 0);
            }
        }

        [TestMethod]
        public void CancelAppointmentTest()
        {
            var uService = new UserService();
            var aService = new AppointmentService();

            bool validForm = uService.LoginAsync("digicustomer", "123456789").Result;
            if (validForm)
            {
                var result = aService.CancelAppointmentAsync("1").Result;
                Assert.AreEqual(result, true);
            }
        }

        [TestMethod]
        public void GetAllDataAsyncTest()
        {

            var uService = new UserService();
            var aService = new AppointmentService();

            bool validForm = uService.LoginAsync("digicustomer", "123456789").Result;
            if (validForm)
            {
                var item = aService.GetAllDataAsync().Result;
                Assert.AreNotEqual(item.Doctors.Count, 0);
                Assert.AreNotEqual(item.Services.Count, 0);
                Assert.AreNotEqual(item.Specialties.Count, 0);
                Assert.AreNotEqual(item.Services.Count, 0);
                Assert.AreNotEqual(item.Relations.Count, 0);
            }
        }

        [TestMethod]
        public void GetFilteredDataTest()
        {

            var uService = new UserService();
            var aService = new AppointmentService();

            bool validForm = uService.LoginAsync("digicustomer", "123456789").Result;
            if (validForm)
            {
                var aptData = aService.GetAllDataAsync().Result;

                AppointmentBook aptBook = new AppointmentBook();
                aptBook.Doctor = aptData.Relations[0].Doctor;

                aptData = aService.GetFilteredData(aptBook);
                Assert.AreEqual(aptData.Doctors.Count, 0);
            }
        }

        [TestMethod]
        public void GetAvailableDatesAsync()
        {

            var uService = new UserService();
            var aService = new AppointmentService();

            AppointmentBook aptBook = new AppointmentBook();
            aptBook.Doctor = new PickerItem("1226", "Teresa McGuire");
            aptBook.Specialty = new PickerItem("44", "Clínica Geral");
            aptBook.Service = new PickerItem("217748||1", "Consulta de Clínica Geral | Médico Assistente");
            aptBook.Facility = new PickerItem("2", "SAMS Centro Clínico (Lx)");
            aptBook.Type = Model.Enum.AppointmentType.C;
            aptBook.Moment = Convert.ToDateTime("2017-06-01");

            bool validForm = uService.LoginAsync("digicustomer", "123456789").Result;
            if (validForm)
            {
                var items = aService.GetAvailableDatesAsync(aptBook).Result;

                //have items?
                Assert.AreNotEqual(items.Count, 0);

                //all item have diferent dates?
                Assert.AreEqual(items.Count, items.Select(i => i.ID).Distinct().Count());
                Assert.AreEqual(items.Count, items.Select(i => i.Title).Distinct().Count());
            }
        }

        [TestMethod]
        public void GetAvailableTimeAsync()
        {

            var uService = new UserService();
            var aService = new AppointmentService();

            AppointmentBook aptBook = new AppointmentBook();
            aptBook.Doctor = new PickerItem("1226", "Teresa McGuire");
            aptBook.Specialty = new PickerItem("44", "Clínica Geral");
            aptBook.Service = new PickerItem("217748||1", "Consulta de Clínica Geral | Médico Assistente");
            aptBook.Facility = new PickerItem("2", "SAMS Centro Clínico (Lx)");
            aptBook.Type = Model.Enum.AppointmentType.C;
            aptBook.Moment = Convert.ToDateTime("2017-07-06");

            bool validForm = uService.LoginAsync("digicustomer", "123456789").Result;
            if (validForm)
            {
                var items = aService.GetAvailableTimeAsync(aptBook).Result;

                //have items?
                Assert.AreNotEqual(items.Count, 0);

                //all item have diferent times?
                Assert.AreEqual(items.Count, items.Select(i => i.ID).Distinct().Count());
                Assert.AreEqual(items.Count, items.Select(i => i.Title).Distinct().Count());

            }
        }

        [TestMethod]
        public void BookNewAppointmentAsync()
        {
            var uService = new UserService();
            var aService = new AppointmentService();
            AppointmentBook apptBook = new AppointmentBook(0, new PickerItem("1", "..."), new PickerItem("1", "..."),
                new PickerItem("1", "..."), new PickerItem("1", "..."), new PickerItem("1", "..."), DateTime.Now.Date);

            bool validForm = uService.LoginAsync("digicustomer", "123456789").Result;
            if (validForm)
            {
                var result = aService.BookNewAppointmentAsync(apptBook).Result;
                Assert.AreEqual(result, true);
            }
        }

        [TestMethod]
        public void GetAvailableDatesNoDoctorSelectedExamAsync()
        {

            var uService = new UserService();
            var aService = new AppointmentService();

            AppointmentBook aptBook = new AppointmentBook();
            aptBook.Doctor = null;
            aptBook.Specialty = new PickerItem("103", "Imagiologia | Ecografia");
            aptBook.Service = new PickerItem("113794||1", "Ecografia abdominal");
            aptBook.Facility = new PickerItem("2", "SAMS Centro Clínico (Lx)");
            aptBook.Type = Model.Enum.AppointmentType.E;
            aptBook.Moment = Convert.ToDateTime("2017-06-01");

            bool validForm = uService.LoginAsync("digicustomer", "123456789").Result;
            if (validForm)
            {
                var items = aService.GetAvailableDatesAsync(aptBook).Result;

                //have items?
                Assert.AreNotEqual(items.Count, 0);

                //all item have diferent dates?
                Assert.AreEqual(items.Count, items.Select(i => i.ID).Distinct().Count());
                Assert.AreEqual(items.Count, items.Select(i => i.Title).Distinct().Count());
            }
        }

        [TestMethod]
        public void GetAvailableTimeNoDoctorSelectedAsync()
        {

            var uService = new UserService();
            var aService = new AppointmentService();

            AppointmentBook aptBook = new AppointmentBook();
            aptBook.Doctor = null;
            aptBook.Specialty = new PickerItem("103", "Imagiologia | Ecografia");
            aptBook.Service = new PickerItem("113794||1", "Ecografia abdominal");
            aptBook.Facility = new PickerItem("2", "SAMS Centro Clínico (Lx)");
            aptBook.Type = Model.Enum.AppointmentType.E;
            aptBook.Moment = Convert.ToDateTime("2017-06-01");

            bool validForm = uService.LoginAsync("digicustomer", "123456789").Result;
            if (validForm)
            {
                var items = aService.GetAvailableTimeAsync(aptBook).Result;

                //have items?
                Assert.AreNotEqual(items.Count, 0);

                //all item have diferent times?
                Assert.AreEqual(items.Count, items.Select(i => i.ID).Distinct().Count());
                Assert.AreEqual(items.Count, items.Select(i => i.Title).Distinct().Count());

            }

        }
    }
}