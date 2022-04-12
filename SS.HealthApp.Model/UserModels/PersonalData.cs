using MvvmValidation;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace SS.HealthApp.Model.UserModels {
    public class PersonalData : _BaseModel
    {

        public PersonalData()
        {
            ConfigureValidationRules();
        }

        public string Name { get; set; }

        public string Email { get; set; }

        public string TaxNumber { get; set; }

        public string Mobile { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        #region validation

        [JsonIgnore]
        public ValidationHelper Validator { get; private set; }

        private void ConfigureValidationRules()
        {
            Validator = new ValidationHelper();

            //Validator.AddRequiredRule(() => Name, "Required");

            Validator.AddRequiredRule(() => Email, "Required");

            Validator.AddRule(nameof(Email),
                () => {
                    const string regexPattern =
                        @"^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$";
                    return RuleResult.Assert(Regex.IsMatch(Email, regexPattern),
                        "Email must by a valid email address");
                });

            //Validator.AddRequiredRule(() => TaxNumber, "Required");

            Validator.AddRequiredRule(() => Mobile, "Required");

            Validator.AddRequiredRule(() => PhoneNumber, "Required");

            //Validator.AddRequiredRule(() => Address, "Required");
        }

        #endregion

    }
}
