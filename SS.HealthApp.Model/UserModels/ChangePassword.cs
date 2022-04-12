using MvvmValidation;

namespace SS.HealthApp.Model.UserModels
{
    public class ChangePassword
    {

        public ChangePassword()
        {
            ConfigureValidationRules();
        }

        public string oldPassword { get; set; }

        public string newPassword { get; set; }

        public string ConfirmNewPassword { get; set; }

        #region validation
        public ValidationHelper Validator { get; private set; }

        private void ConfigureValidationRules()
        {
            Validator = new ValidationHelper();

            Validator.AddRequiredRule(() => oldPassword, "Required");

            Validator.AddRequiredRule(() => newPassword, "Required");

            Validator.AddRequiredRule(() => ConfirmNewPassword, "Required");

            Validator.AddRule(nameof(ConfirmNewPassword),
                () => {
                    return RuleResult.Assert(newPassword == ConfirmNewPassword, "PasswordMismatch");
                });

        }

        #endregion

    }
}
