using MvvmValidation;
using Newtonsoft.Json;
using System;

namespace SS.HealthApp.Model.AppointmentModels
{

    public class AppointmentBook : _BaseModel
    {

        public Enum.AppointmentType Type { get; set; }
        public PickerItem Doctor { get; set; }
        public PickerItem Specialty { get; set; }
        public PickerItem Service { get; set; }
        public PickerItem Facility { get; set; }
        public PickerItem Payor { get; set; }
        public DateTime Moment { get; set; }
        public string SlotID { get; set; }

        public AppointmentBook()
        {
            ConfigureValidationRules();
        }

        public AppointmentBook(Enum.AppointmentType type, PickerItem doctor, PickerItem specialty, PickerItem service, PickerItem facility, PickerItem payor, DateTime moment) : this()
        {
            Type = type;
            Doctor = doctor;
            Specialty = specialty;
            Service = service;
            Facility = facility;
            Payor = payor;
            Moment = moment;
        }

        #region validation

        [JsonIgnore]
        public ValidationHelper Validator { get; private set; }

        private void ConfigureValidationRules()
        {
            Validator = new ValidationHelper();

            Validator.AddRule(() => Doctor, () => {
                if (Type == Enum.AppointmentType.C && (Doctor == null || String.IsNullOrEmpty(Doctor.ID)))
                {
                    return RuleResult.Invalid("Required");
                }
                return RuleResult.Valid();
            });
            Validator.AddRequiredRule(() => Specialty, "Required");
            Validator.AddRequiredRule(() => Service, "Required");
            Validator.AddRequiredRule(() => Facility, "Required");
            Validator.AddRequiredRule(() => Payor, "Required");
            Validator.AddRequiredRule(() => SlotID, "Required");
        }

        #endregion

    }
}
