using System.Collections.Generic;

namespace SS.HealthApp.Model.AppointmentModels {

    public class AppointmentData {

        public List<PickerItem> Doctors { get; set; }

        public List<PickerItem> Specialties { get; set; }

        public List<ServiceItem> Services { get; set; }

        public List<PickerItem> Facilities { get; set; }

        public List<Relation> Relations { get; set; }

        public AppointmentData() {
            Doctors = new List<PickerItem>();
            Specialties = new List<PickerItem>();
            Services = new List<ServiceItem>();
            Facilities = new List<PickerItem>();
            Relations = new List<Relation>();
        }

        public class Relation {

            public PickerItem Doctor { get; set; }
            public PickerItem Specialty { get; set; }
            public ServiceItem Service { get; set; }
            public PickerItem Facility { get; set; }

            public Relation(PickerItem doctor, PickerItem specialty, ServiceItem service, PickerItem facility) {
                this.Doctor = doctor;
                this.Specialty = specialty;
                this.Service = service;
                this.Facility = facility;
            }
        }
    }
}
