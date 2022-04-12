using System.Collections.Generic;
using SS.HealthApp.ClientConnector.Interfaces;
using SS.HealthApp.Model;
using SS.HealthApp.Model.AppointmentModels;
using SS.HealthApp.ClientConnector.Models;
using System.Linq;
using System;
using SS.HealthApp.ClientConnector.Local.Utils;

namespace SS.HealthApp.ClientConnector.Local
{
    public class AppointmentClientConnector : IAppointmentClientConnector
    {

        public static Dictionary<string, List<Appointment>> apptListDataSource = FillApptListDataSource();

        public List<Appointment> GetAppointments(AuthenticatedUser User)
        {
            return apptListDataSource[User.Id].OrderByDescending(n => n.Moment).ToList();
        }

        public AppointmentData GetAllData()
        {
            var ApptData = new AppointmentData();

            ApptData.Doctors = new List<PickerItem>() {
                        new PickerItem("1", "Dr. Peter Sellers"),
                        new PickerItem("2", "Dr. John Ford"),
                        new PickerItem("3", "Dr. Samantha Morris")
                    };

            ApptData.Specialties = new List<PickerItem>() {
                        new PickerItem("1", "Cardiology"),
                        new PickerItem("2", "Stomatology"),
                        new PickerItem("3", "Orthopedics"),
                        new PickerItem("4", "Pneumology")
                    };

            ApptData.Services = new List<ServiceItem>() {
                        new ServiceItem("1", "Orthopedics Treatment", Model.Enum.AppointmentType.C),
                        new ServiceItem("2", "Stomatology Consultation", Model.Enum.AppointmentType.C),
                        new ServiceItem("3", "Pneumology Consultation", Model.Enum.AppointmentType.C),
                        new ServiceItem("4", "Cardiology Exam", Model.Enum.AppointmentType.E),
                        new ServiceItem("5", "Pneumology Exam", Model.Enum.AppointmentType.E)

                    };

            ApptData.Facilities = new List<PickerItem>() {
                        new PickerItem("1", "All Saints Hospital"),
                        new PickerItem("2", "Surgical Hospital"),
                        new PickerItem("3", "Main Clinic")
                    };

            ApptData.Relations = new List<AppointmentData.Relation>() {
                        new AppointmentData.Relation(ApptData.Doctors[0], ApptData.Specialties[0], ApptData.Services[3], ApptData.Facilities[0]),
                        new AppointmentData.Relation(ApptData.Doctors[1], ApptData.Specialties[1], ApptData.Services[1], ApptData.Facilities[0]),
                        new AppointmentData.Relation(ApptData.Doctors[1], ApptData.Specialties[1], ApptData.Services[1], ApptData.Facilities[1]),
                        new AppointmentData.Relation(ApptData.Doctors[2], ApptData.Specialties[2], ApptData.Services[0], ApptData.Facilities[2])
                    };

            return ApptData;
        }

        public List<PickerItem> GetAvailableDates(AuthenticatedUser User, AppointmentBook apptBook)
        {
            var availableDates = new List<PickerItem>();
            //the title part will be filled in the client accordingly with date local settings
            availableDates.Add(new PickerItem(new DateTime(2017, 5, 19).ToString("yyyy-MM-dd"), string.Empty));
            availableDates.Add(new PickerItem(new DateTime(2017, 5, 20).ToString("yyyy-MM-dd"), string.Empty));
            availableDates.Add(new PickerItem(new DateTime(2017, 5, 21).ToString("yyyy-MM-dd"), string.Empty));
            return availableDates;
        }

        public List<PickerItem> GetAvailableTime(AuthenticatedUser User, AppointmentBook apptBook)
        {
            var availableTime = new List<PickerItem>();
            availableTime.Add(new PickerItem("1", "14:00"));
            availableTime.Add(new PickerItem("2", "14:30"));
            availableTime.Add(new PickerItem("3", "15:00"));
            availableTime.Add(new PickerItem("4", "15:30"));
            availableTime.Add(new PickerItem("5", "16:00"));
            return availableTime;
        }

        public List<PickerItem> GetPayors(AuthenticatedUser User)
        {
            var payors = new List<PickerItem>();
            payors.Add(new PickerItem("1", "Private"));
            payors.Add(new PickerItem("2", "Insurance Payor A"));
            return payors;
        }

        public bool CancelAppointment(AuthenticatedUser User, string appointmentID)
        {
            apptListDataSource[User.Id].Remove(apptListDataSource[User.Id].Find(a => a.ID == appointmentID));
            return true;
        }

        public bool BookNewAppointment(AuthenticatedUser User, AppointmentBook apptBook)
        {
            apptListDataSource[User.Id].Add(new Appointment() {
                ID = new Random().Next(1000, int.MaxValue).ToString(),
                Facility = apptBook.Facility.Title,
                Description = apptBook.Service.Title,
                Status = Model.Enum.AppointmentStatus.Booked,
                Moment = apptBook.Moment,
                AllowAddToCalendar = true,
                AllowCancel = true
            });
            return true;
        }

        private static Dictionary<string, List<Appointment>> FillApptListDataSource()
        {

            var dSource = new Dictionary<string, List<Appointment>>();

            foreach (var user in UserClientConnector.dataSource)
            {
                dSource[user.Id] = new List<Appointment> {
                    new Appointment() {
                        ID = "1",
                        Facility = "All Saints Hospital",
                        Description = "Orthopedics Treatment",
                        Status = Model.Enum.AppointmentStatus.Booked,
                        Moment = DateTime.Now.AddDays(30).DateTimeRoundUp5MinutesInterval()
                    },
                    new Appointment() {
                        ID = "2",
                        Facility = "Main Clinic",
                        Description = "Stomatology Consultation",
                        Status = Model.Enum.AppointmentStatus.Booked,
                        Moment = DateTime.Now.DateTimeRoundUp5MinutesInterval()
                    },
                    new Appointment() {
                        ID = "3",
                        Facility = "All Saints Hospital",
                        Description = "Orthopedics Treatment",
                        Status = Model.Enum.AppointmentStatus.Booked,
                        Moment = DateTime.Now.DateTimeRoundUp5MinutesInterval()
                    },
                    new Appointment() {
                        ID = "4",
                        Facility = "All Saints Hospital",
                        Description = "Pneumology Consultation",
                        Status = Model.Enum.AppointmentStatus.Booked,
                        Moment = DateTime.Now.DateTimeRoundUp5MinutesInterval()
                    },
                    new Appointment() {
                        ID = "5",
                        Facility = "Main Clinic",
                        Description = "Cardiology Exam",
                        Status = Model.Enum.AppointmentStatus.Closed,
                        Moment = DateTime.Now.DateTimeRoundUp5MinutesInterval()
                    },
                    new Appointment() {
                        ID = "6",
                        Facility = "Main Clinic",
                        Description = "Cardiology Exam",
                        Status = Model.Enum.AppointmentStatus.Closed,
                        Moment = DateTime.Now.AddDays(-10).DateTimeRoundUp5MinutesInterval()
                    },
                    new Appointment() {
                        ID = "7",
                        Facility = "All Saints Hospital",
                        Description = "Cardiology Consultation",
                        Status = Model.Enum.AppointmentStatus.Closed,
                        Moment = DateTime.Now.AddDays(-30).DateTimeRoundUp5MinutesInterval()
                    },
                    new Appointment() {
                        ID = "8",
                        Facility = "Main Clinic",
                        Description = "Cardiology Exam",
                        Status = Model.Enum.AppointmentStatus.Closed,
                        Moment = DateTime.Now.AddDays(-40).DateTimeRoundUp5MinutesInterval()
                    },
                    new Appointment() {
                        ID = "9",
                        Facility = "Main Clinic",
                        Description = "Cardiology Consultation",
                        Status = Model.Enum.AppointmentStatus.Closed,
                        Moment = DateTime.Now.AddDays(-45).DateTimeRoundUp5MinutesInterval()
                    },new Appointment() {
                        ID = "10",
                        Facility = "Main Clinic",
                        Description = "Cardiology Exam",
                        Status = Model.Enum.AppointmentStatus.Closed,
                        Moment = DateTime.Now.AddDays(-60).DateTimeRoundUp5MinutesInterval()
                    },
                    new Appointment() {
                        ID = "11",
                        Facility = "Main Clinic",
                        Description = "Blood analysis",
                        Status = Model.Enum.AppointmentStatus.Closed,
                        Moment = DateTime.Now.AddDays(-90).DateTimeRoundUp5MinutesInterval()
                    },
                    new Appointment() {
                        ID = "12",
                        Facility = "Main Clinic",
                        Description = "Pneumology Exam",
                        Status = Model.Enum.AppointmentStatus.Closed,
                        Moment = DateTime.Now.AddDays(-95).DateTimeRoundUp5MinutesInterval()
                    }
                };
            }

            foreach (var item in dSource)
            {
                foreach (var appt in item.Value)
                {
                    SetActionByAppointmentStatus(appt);
                }
            }

            return dSource;

        }

        public static void SetActionByAppointmentStatus(Appointment appt)
        {

            appt.AllowCheckIn = false;
            appt.AllowCancel = false;
            appt.AllowAddToCalendar = false;
            appt.AllowRateService = false;
            appt.AllowParkingQRCode = false;
            appt.AllowPresenceDeclaration = false;

            switch (appt.Status)
            {
                case Model.Enum.AppointmentStatus.Arrived:
                case Model.Enum.AppointmentStatus.Called:
                    appt.AllowParkingQRCode = true;
                    break;
                case Model.Enum.AppointmentStatus.Booked:
                    if (appt.Moment.Date == DateTime.Today) {
                        appt.AllowCheckIn = true;
                        appt.AllowCancel = true;
                    }
                    else {
                        appt.AllowAddToCalendar = true;
                        appt.AllowCancel = true;
                    }
                    break;
                case Model.Enum.AppointmentStatus.Closed:
                    if (appt.Moment.Date == DateTime.Today) {
                        appt.AllowParkingQRCode = true;
                        appt.AllowPresenceDeclaration = true;
                        appt.AllowRateService = true;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
