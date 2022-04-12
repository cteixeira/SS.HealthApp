using System;
using System.Collections.Generic;
using SS.HealthApp.ClientConnector.Interfaces;
using SS.HealthApp.ClientConnector.Models;
using SS.HealthApp.Model.DeclarationModels;
using System.Linq;

namespace SS.HealthApp.ClientConnector.Local
{
    public class DeclarationClientConnector : IDeclarationClientConnector
    {
        public List<PresenceDeclaration> GetPresenceDeclaration(ClientConnector.Models.AuthenticatedUser User)
        {
            return new List<PresenceDeclaration>
            {
                new PresenceDeclaration {
                    ID = "1",
                    Facility = "All Saints Hospital",
                    Appointment = "Stomatology Consultation",
                    Moment = new System.DateTime(2016, 5, 19, 11, 30, 0)
                },
                new PresenceDeclaration() {
                    ID = "2",
                    Facility = "Main Clinic",
                    Appointment = "Cardiology Exam",
                    Moment = new System.DateTime(2016, 4, 19, 11, 30, 0)
                    },
                new PresenceDeclaration() {
                    ID = "3",
                    Facility = "Surgical Hospital",
                    Appointment = "Dermatologist Consultation",
                    Moment = new System.DateTime(2015, 8, 5, 16, 45, 0)
                },
                new PresenceDeclaration() {
                    ID = "4",
                    Facility = "Main Clinic",
                    Appointment = "Cardiology Consultation",
                    Moment = new System.DateTime(2011, 8, 8, 12, 40, 0)
                }
            };
        }

        public byte[] GetPresenceDeclarationFile(AuthenticatedUser User, string declarationID)
        {
            return System.IO.File.ReadAllBytes(string.Concat(Settings.ResourcesDir, "testdoc.pdf"));
        }

        public string GetPresenceDeclarationIdByAppointmentID(AuthenticatedUser User, string appointmentID)
        {
            //dummy return the the first declaratio ID
            return GetPresenceDeclaration(User).First().ID;
        }
    }
}
